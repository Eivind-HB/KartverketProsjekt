using Kartverket.Models;
using Kartverket.Services;
using Kartverket.API_Models;
using Kartverket.Data;

using MySqlConnector;

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Ganss.Xss;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoApiService _KommuneInfoApiService;
        //EF
        private readonly ApplicationDbContext _context;
        //Hashing av passord
        private readonly PasswordHasher<User> _passwordHasher;

        //In-memory lagring av lister
        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<LogInData> LogInInfo = new List<LogInData>();
        private static List<PositionModel> positions = new List<PositionModel>();
        private static readonly List<User> Usersinfo = new List<User>();


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
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

        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        public IActionResult RegisteredCaseOverview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAreaChange(AreaChange areaModel, UserData userModel, IFormFile ImageUpload)
        {
            var sanitizer = new HtmlSanitizer();
            areaModel.GeoJson = sanitizer.Sanitize(areaModel.GeoJson);
            areaModel.Description = sanitizer.Sanitize(areaModel.Description);
            areaModel.Kommunenavn = sanitizer.Sanitize(areaModel.Kommunenavn);
            areaModel.Fylkesnavn = sanitizer.Sanitize(areaModel.Fylkesnavn);
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
                    ViewData["ErrorMessage"]="Bare filtyper JPG, JPEG, PNG under 5MB er tillatt";
                    return View("RoadCorrection",areaModel);
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

            //Checks that the description in areamodel is not null/empty
            if (string.IsNullOrEmpty(areaModel.Kommunenavn))
            {
                // Store error message in ViewBag
                ViewBag.ErrorMessage = "Kommune er ikke regisrert! Prøv å trykk en ekstra gang på kartet etter du har markert det";
                return View("RoadCorrection"); // Return to the same view with the model
            }

            var newChange = new AreaChange
            {
                IssueId = Guid.NewGuid().ToString(),
                GeoJson = areaModel.GeoJson,
                Description = areaModel.Description,
                IssueType = areaModel.IssueType,
                IssueDate = DateTime.Now,
                ImageData = areaModel.ImageData,

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


            //init av noen variabler som jeg selv har brukt, skal nok endres
            var geoJson = areaModel.GeoJson;
            var description = areaModel.Description;
            var fylkesNo = Int32.Parse(areaModel.Fylkesnummer);
            var kommuneNo = Int32.Parse(areaModel.Kommunenummer);
            var issueNo = Int32.Parse(areaModel.IssueType);


            //Niri EF
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
            //MySqlGeometry mySqlGeometry = MySqlGeometry.FromWKB(wkb);   funker ikke

            //random id int nummer
            Random rnd = new Random();
            int CaseNoNumber = rnd.Next(100000, 999999);


            //TempData["OpprettetSaksnr"] = CaseNoNumber;
            //ViewBag.ViewModel = TempData["OpprettetSaksnr"];
            //TempData.Keep("OpprettetSaksnr");

            //var newlycreated = true;

            //random id nummer, placeholder
            var userId = rnd.Next(1, 10);
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                userId = 0;
                if (userIdClaim != null)
                {
                    userId = int.Parse(userIdClaim.Value);
                }
            }

            var dateNow = DateOnly.FromDateTime(DateTime.Now);


            var newGeoChange = new Case
            {
                CaseNo = CaseNoNumber,
                LocationInfo = geoJson,
                Description = description,
                Date = dateNow,
                //CaseWorker_CaseWorkerID = 1,
                User_UserID = userId, 
                Issue_IssueNr = issueNo,
                Images = areaModel.ImageData,
                KommuneNo = kommuneNo,
                FylkesNo = fylkesNo,
                StatusNo = 1

            };

            // Save to the database
            _context.Case.Add(newGeoChange);
            _context.SaveChanges();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AreaChangeOverview", "Case");
            }
            return View("RegisteredCaseOverview", newGeoChange);
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }


        //Fetching of KommuneInfo
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