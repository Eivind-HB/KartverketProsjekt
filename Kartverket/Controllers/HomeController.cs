using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Kartverket.Data;
using MySqlConnector;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

using static System.Net.WebRequestMethods;


namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoApiService _KommuneInfoApiService;
        private readonly ApplicationDbContext _context;

        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<PositionModel> positions = new List<PositionModel>();


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OmOss()
        {
            return Redirect("https://www.kartverket.no/om-kartverket");
        }

        public IActionResult KontaktOss()
        {
            return Redirect("https://www.kartverket.no/om-kartverket/kontakt-oss");
        }

        public IActionResult RoadCorrection()
        {
            return View();
        }

        public IActionResult TemporaryChanges()
        {
            return View();
        }

        public IActionResult LogInForm()
        {
            return View();
        }

        [HttpGet]
        public ViewResult RegistrationForm()
        {
            return View();
        }

        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description, int UserID, AreaChange areaModel, UserData userModel, IFormFile ImageUpload)
        {
            string imagePath = null;
            
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data.");
                }

                if (geoJson == null)
                {
                    return BadRequest(geoJson);
                }
        
        public IActionResult RegisterAreaChange(AreaChange areaModel, UserData userModel, IFormFile ImageUpload)
        {
            string imagePath = null;

        if (ImageUpload != null && ImageUpload.Length > 0)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var filePath = Path.Combine(uploads, ImageUpload.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageUpload.CopyTo(fileStream);
            }

            imagePath = $"/wwwroot/images/{ImageUpload.FileName}";
        }

            var newChange = new AreaChange
            {
                IssueId = Guid.NewGuid().ToString(),
                GeoJson = areaModel.GeoJson,
                Description = areaModel.Description,
                IssueType = areaModel.IssueType,
                IssueDate = DateTime.Now,
                ImagePath = imagePath,

                Kommunenavn = areaModel.Kommunenavn,
                Kommunenummer = areaModel.Kommunenummer,
                Fylkesnavn = areaModel.Fylkesnavn,
                Fylkesnummer = areaModel.Fylkesnummer,
            };

            var userChange = new UserData
            {
                UserName = userModel.UserName,
            };

            areaChanges.Add(newChange);
            UserDataChanges.Add(userChange);

                //Niri EF faenskap
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data.");
                }

                if (geoJson == null)
                {
                    return BadRequest(geoJson);
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
                
                //Create MySqlGeometry from WKB
                //MySqlGeometry mySqlGeometry = MySqlGeometry.FromWKB(wkb);   funker ikke :)

                //random id int nummer
                Random rnd = new Random();
                int CaseNoNumber = rnd.Next(999999);



                var newGeoChange = new Case
                {
                    CaseNo = CaseNoNumber,
                    LocationInfo = geoJson,
                    Description = description,
                    Date = DateTime.Now,
                    //CaseWorker_CaseWorkerID = 1,
                    User_UserID = UserID, 
                    Issue_IssueNr = 1

                };

                // Save to the database
                _context.Case.Add(newGeoChange);
                _context.SaveChanges();

                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }

        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            var changes_db = _context.Case.ToList();
            return View(changes_db);
        }


        [HttpPost]
        public async Task<IActionResult> KommuneInfoApi(string kommuneNr)
        {
            if (string.IsNullOrEmpty(kommuneNr))
            {
                ViewData["Error"] = "Venligst legg inn et gyldig Kommunenummer. Det skal v�re 4 siffer.";
                return View("Index");
            }

            var areaChangesList = areaChanges; 
            var userDataList = UserDataChanges;

            var model = new ChangeOverviewModel
            {
                AreaChanges = areaChangesList,
                Users = userDataList
            };

            return View(model);
        }

        
                
    }
}
