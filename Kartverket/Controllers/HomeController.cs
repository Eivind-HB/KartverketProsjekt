using Ganss.Xss;
using Kartverket.Data;
using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;


        //In-memory lagring av lister
        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<PositionModel> positions = new List<PositionModel>();
        private static readonly List<User> Usersinfo = new List<User>();


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
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

            string address = "Address not found"; // Default address
            try
            {
                var coordinates = ExtractCoordinates(areaModel.GeoJson);
                if (coordinates != null)
                {
                    address = await FetchAddressFromApiAsync(coordinates.Value.Lat, coordinates.Value.Lon);
                }
                else
                {
                    address = "Adresse ikke funnet";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching address: {ex.Message}");
            }

            //creates the information that will actually be fed into mariaDB
            var newCase = new Case
            {
                CaseNo = CaseNoNumber,
                LocationInfo = areaModel.GeoJson,
                Address = address, // Save the address in the database
                Description = areaModel.Description,
                Date = DateOnly.FromDateTime(DateTime.Now),
                User_UserID = userId,
                IssueNo = (int)issueNo,
                Images = areaModel.ImageData,
                KommuneNo = (int)areaModel.Kommunenummer,
                FylkesNo = (int)areaModel.Fylkesnummer,
                StatusNo = 1 // Default status: "Sendt"
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
                .FirstOrDefaultAsync(c => c.CaseNo == newCase.CaseNo); // Get the specific case
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

        private async Task<string> FetchAddressFromApiAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "KartverketProject (your.email@kartverket.com)");

            // Ensure culture-invariant coordinate formatting
            var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}";
            Console.WriteLine($"Request URL: {url}");

            try
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Code: {response.StatusCode}");
                Console.WriteLine($"Response Content: {content}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);
                        return jsonResponse?.display_name?.ToString() ?? "Address not found";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                        return "Address parsing failed";
                    }
                }

                Console.WriteLine($"API error: {response.StatusCode}");
                return "Address not available";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return "Failed to fetch address";
            }
        }



        private (double Lat, double Lon)? ExtractCoordinates(string geoJson)
        {
            try
            {
                var geoJsonObj = JsonConvert.DeserializeObject<dynamic>(geoJson);
                var coordinates = geoJsonObj?.geometry?.coordinates;
                if (coordinates != null && coordinates.Count >= 2)
                {
                    // Ensure culture-invariant parsing
                    double latitude = Convert.ToDouble(coordinates[1], CultureInfo.InvariantCulture);
                    double longitude = Convert.ToDouble(coordinates[0], CultureInfo.InvariantCulture);
                    return (latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error extracting coordinates: {ex.Message}");
            }
            return null;
        }


    }
}