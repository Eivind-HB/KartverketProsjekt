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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.EntityFrameworkCore;
using Microsoft.AspNetCore.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoApiService _KommuneInfoApiService;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UserData> _passwordHasher;

        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<LogInData> LogInInfo = new List<LogInData>();
        private static List<PositionModel> positions = new List<PositionModel>();
        private static readonly List<User> Usersinfo = new List<User>();


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = new PasswordHasher<UserData>();
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
        public async Task<IActionResult> LogInForm(LogInData model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by username and password
                var user = await _context.UserData.FirstOrDefaultAsync(u =>
                u.UserName == model.Brukernavn);
                if (user != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user,
                        user.Password, model.Passord);
                    if (result == PasswordVerificationResult.Success)
                    {
                        HttpContext.Session.SetString("UserId", user.UserId);
                        return RedirectToAction("Index");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            return View(model);
        }
                


        [HttpGet]
        public async Task<IActionResult> UDOverview()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("RegistrationForm");
            }
            var userData = await _context.Users.FindAsync(userId);
            return View(userData);
        }

        [HttpPost]
        public async Task<IActionResult> UDOverview(UserData userData)
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
                    Password = _passwordHasher.HashPassword(null, userData.Password)
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // sign in the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.UserId),
                    new Claim(ClaimTypes.Name, newUser.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("UDOverview");
            }
            return View("RegistrationForm", userData);
        }

        private async Task<UserData?> GetUserData()
        {
            var uderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return await _context.Users.FindAsync(userId);
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
