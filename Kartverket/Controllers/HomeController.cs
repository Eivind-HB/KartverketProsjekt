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
using Kartverket.API_Models;
using System.Data;


namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoApiService _KommuneInfoApiService;
        private readonly ApplicationDbContext _context;

        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<LogInData> LogInInfo = new List<LogInData>();
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<User> Usersinfo = new List<User>();


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

        [HttpGet]
        public IActionResult LogInForm()
        {
            return View(new LogInData());
        }

        [HttpPost]
        public IActionResult LogInForm(User model1, LogInData model2)
        {
            if (ModelState.IsValid)
            {
                // Find the user by username and password
                var user = Usersinfo.FirstOrDefault(u =>
                    u.UserName == model1.UserName && u.Password == model1.Password);
                Console.Write(user);
                Console.Write(user.UserName, model1.UserName);

                if (user != null)
                {
                    // User found, set the UserId in the session
                    HttpContext.Session.SetInt32("UserId", model1.UserID);
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // If we got this far, something failed; redisplay form
            return View(model2);
        }


        [HttpGet]
        public IActionResult UDOverview()
        {
            var userData = GetUserData();
            if (userData == null)
            {
                return RedirectToAction("RegistrationForm");
            }
            return View(userData);
        }

        [HttpPost]
        public IActionResult UDOverview(UserData userData)
        {
            if (ModelState.IsValid)
            {
                Random rnd = new Random();
                //random id nummer -- gamle string id : UserId = Guid.NewGuid().ToString(),
                var userID = rnd.Next(100000, 999999);
                var newUser = new UserData
                {
                    UserId = userID,
                    UserName = userData.UserName,
                    Email = userData.Email,
                    HomeMunicipality = userData.HomeMunicipality,
                    Password = userData.Password
                };

                UserDataChanges.Add(newUser);

                // Set the UserId in the session
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                HttpContext.Session.SetString("Password", newUser.Password);
                HttpContext.Session.SetString("Mail", newUser.Email);
                HttpContext.Session.SetString("UserName", newUser.UserName);

                return RedirectToAction("UDOverview");
            }

            // If ModelState is not valid, return to the form
            return View("RegistrationForm", userData);
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
        //public IActionResult RegisterAreaChange(string geoJson, string description, int UserID, AreaChange areaModel, UserData userModel, IFormFile ImageUpload)
        
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


            //init av noen variabler som jeg selv har brukt, skal nok endres
            var geoJson = areaModel.GeoJson;
            var description = areaModel.Description;
            var fylkesNo = Int32.Parse(areaModel.Fylkesnummer);
            var kommuneNo = Int32.Parse(areaModel.Kommunenummer);


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
            int CaseNoNumber = rnd.Next(100000, 999999);

            //random id nummer, placeholder
            var userID = rnd.Next(100000, 999999);

            var dateNow = DateOnly.FromDateTime(DateTime.Now);


            var newGeoChange = new Case
            {
                CaseNo = CaseNoNumber,
                LocationInfo = geoJson,
                Description = description,
                Date = dateNow,
                //CaseWorker_CaseWorkerID = 1,
                User_UserID = userID, 
                Issue_IssueNr = 1,
                KommuneNo = kommuneNo,
                FylkesNo = fylkesNo

            };

            // Save to the database
            _context.Case.Add(newGeoChange);
            _context.SaveChanges();

            return RedirectToAction("AreaChangeOverview");
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

        private UserData? GetUserData()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            string? username = HttpContext.Session.GetString("UserName");
            string? password = HttpContext.Session.GetString("Password");
            string? mail = HttpContext.Session.GetString("Mail");


            var newUser = new User
            {
                UserID = userId,
                UserName = username,
                Mail = mail,
                Password = password

            };
            // Save to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return UserDataChanges.FirstOrDefault(u => u.UserId == userId);
        }

    }
}
