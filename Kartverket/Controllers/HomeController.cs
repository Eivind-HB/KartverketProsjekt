using Ganss.Xss;
using Kartverket.Data;
using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Security.Claims;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private readonly ApplicationDbContext _context;
        

        //In-memory lagring av lister
        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<PositionModel> positions = new List<PositionModel>();
        private static readonly List<User> Usersinfo = new List<User>();


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;            
        }

        public IActionResult Index()
        {
            ViewBag.IsLoggedIn = User.Identity.IsAuthenticated;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        /// <summary>
        /// Redirects user to Kartverkets about us page. That page is made by Kartverket, not by group 13!
        /// </summary>
        /// <returns>Redirects to "Om Oss" at Kartverkets site</returns>
        public IActionResult OmOss()
        {
            return Redirect("https://www.kartverket.no/om-kartverket");
        }


        /// <summary>
        /// Redirects user to Kartverkets contact us page. That page is made by Kartverket, not by group 13!
        /// </summary>
        /// <returns>Redirects to "Kontakt oss" at Kartverkets site</returns>
        public IActionResult KontaktOss()
        {
            return Redirect("https://www.kartverket.no/om-kartverket/kontakt-oss");
        }

        public IActionResult RoadCorrection()
        {
            return View();
        }

        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        public IActionResult RegisteredCaseOverview()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAreaChange(AreaChange areaModel, UserData userModel, IFormFile ImageUpload)
        {
            //Sanitizes GeoJson, Username and Kommuneinfo
            var sanitizer = new HtmlSanitizer();
            areaModel.GeoJson = sanitizer.Sanitize(areaModel.GeoJson);
            areaModel.Description = sanitizer.Sanitize(areaModel.Description);
            userModel.UserName = sanitizer.Sanitize(userModel.UserName);

            //This section handles the image upload to the database
            //Checks if the image is uploaded and if it is, it checks if the file is of the correct type and size
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                var allowedExtension = new[] { ".jpg", ".jpeg", ".png" };
                const long maxFileSize = 5 * 1024 * 1024;
                var fileExtension = Path.GetExtension(ImageUpload.FileName).ToLower();

                //Checks if the file is of the correct type, if not it returns an error message
                if (!allowedExtension.Contains(fileExtension))
                {
                    ViewData["ErrorMessage"] = "Bare filtyper JPG, JPEG, PNG under 5MB er tillatt";
                    return View("RoadCorrection", areaModel);
                }

                //Checks if the file is of the correct size, if not it returns an error message
                if (ImageUpload.Length > maxFileSize)
                {
                    ModelState.AddModelError("ImageUpload", "Filen kan ikke være større enn 5MB");
                }

                //Image is converted to byte array and stored in the database
                using (var memoryStream = new MemoryStream())
                {
                    await ImageUpload.CopyToAsync(memoryStream);
                    areaModel.ImageData = memoryStream.ToArray();
                }
            }


            //Checks that the geojson in areamodel is not null/empty
            if (string.IsNullOrEmpty(areaModel.GeoJson))
            {
                // Store error message in ViewBag
                ViewBag.ErrorMessage = "Kartet må være markert!";
                return View("RoadCorrection"); // Return to the same view with the model
            }

            //Checks that the kommuneinfo(name here) in areamodel is not null/empty
            var issueNo = areaModel.IssueType;
            if (areaModel.Kommunenummer == null && issueNo != 3)
            {
                // Store error message in ViewBag
                ViewBag.ErrorMessage = "Kommune er ikke registrert! Prøv å trykk en ekstra gang på kartet etter du har markert det";
                return View("RoadCorrection"); // Return to the same view with the model
            }
            if (issueNo == 3) //if the issuetype is 'Sjø', thus without kommune/fylkes info from API call
            {
                //set the numbers to 100 and 100100, which is sjø uten kommune/fylke in DB
                areaModel.Kommunenummer = 100100;
                areaModel.Fylkesnummer = 100;
            }

            var userChange = new UserData
            {
                UserName = userModel.UserName,
            };

            UserDataChanges.Add(userChange);


            Random rnd = new Random();
            
            //init of variables gathered from areaModel which are to be sent to mariaDB
            var geoJson = areaModel.GeoJson;
            var description = areaModel.Description;
            var fylkesNo = areaModel.Fylkesnummer;
            var kommuneNo = areaModel.Kommunenummer;
            var dateNow = DateOnly.FromDateTime(DateTime.Now);
            int CaseNoNumber = rnd.Next(100000, 999999);//random id int nummer for casenumer(ID and PK)
            //create variables describing the logged in state
            bool loggedIn = User.Identity.IsAuthenticated;//if the user is logged inn
            bool admin = User.IsInRole("Admin");//id the user is logged inn as an admin

            if (description.Length > 1000) //Max 1000 characters in DB
            {
                // Store error message in ViewBag
                ViewBag.ErrorMessage = "Beskrivelse kan maks være 1000 tegn!";
                return View("RoadCorrection"); // Return to the same view with the model
            }

            Geometry geometry;
            try
            {
                var reader = new GeoJsonReader();
                geometry = reader.Read<Geometry>(geoJson);
            }
            catch (Exception ex)
            {
                // Log or handle parsing error
                Console.WriteLine($"GeoJson parsing error: {ex.Message}");
                return BadRequest("Invalid GeoJson format.");
            }

            //random id nummer for users without login
            var userId = rnd.Next(100000, 999999);
            if (loggedIn)
            {
                if (admin)
                {
                    var caseWorkerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (caseWorkerIdClaim != null)
                    {
                        //userId is set to the admins (caseworkers) userid
                        userId = int.Parse(caseWorkerIdClaim.Value);
                    }
                }
                else //if the user isnt an admin...
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    userId = 0; //had to create the variable outside of the loop, code got angry at me;(
                    if (userIdClaim != null)
                    {
                        //... userId is set to the logged in users userid
                        userId = int.Parse(userIdClaim.Value);
                    }
                }
            }

            //creates the information that will actually be fed into mariaDB
            var newCase = new Case
            {
                CaseNo = CaseNoNumber,
                LocationInfo = geoJson,
                Description = description,
                Date = dateNow,
                User_UserID = userId, 
                IssueNo = (int)issueNo, //it says it 'may be null', but its already been checked earlier in the code
                Images = areaModel.ImageData,
                KommuneNo = (int)kommuneNo, //  ^^^^
                FylkesNo = (int)fylkesNo, //    ^^^^
                StatusNo = 1 //default statusnumber, "Sendt"

            };

            // Save to the database
            _context.Case.Add(newCase);
            _context.SaveChanges();
            if (loggedIn)
            {
                return RedirectToAction("HasProfileCaseOverview", "Case");
            }

            // Retrieve a single case with related data
            var singleCase = await _context.Case
                .Include(c => c.KommuneInfo)  // Include KommuneInfo
                .Include(c => c.Status)        // Include Status
                .Include(c => c.FylkesInfo)    // Include FylkesInfo
                .Include(c => c.Issue)         // Include Issue
                .FirstOrDefaultAsync(c => c.CaseNo == newGeoChange.CaseNo); // Get the specific case
                                                                            // Create the view model for a single case
            var viewModel = new Kartverket.Models.SingleCaseModel
            {
                Case = singleCase // Assign the retrieved case directly
            };
            return View("RegisteredCaseOverview", viewModel);
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }
    }
}