using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
        private static List<LogInData> LogInInfo = new List<LogInData>();
        private static List<PositionModel> positions = new List<PositionModel>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
        public IActionResult LogInForm(LogInData model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by username and password
                var user = UserDataChanges.FirstOrDefault(u =>
                    u.UserName == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // User found, set the UserId in the session
                    HttpContext.Session.SetString("UserId", user.UserId);
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // If we got this far, something failed; redisplay form
            return View(model);
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
                var newUser = new UserData
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = userData.UserName,
                    Email = userData.Email,
                    HomeMunicipality = userData.HomeMunicipality,
                    Password = userData.Password
                };

                UserDataChanges.Add(newUser);

                // Set the UserId in the session
                HttpContext.Session.SetString("UserId", newUser.UserId);

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
        public IActionResult RegisterAreaChange(AreaChange areaModel, UserData userModel)
        {
            var newChange = new AreaChange
            {
                IssueId = Guid.NewGuid().ToString(),
                GeoJson = areaModel.GeoJson,
                Description = areaModel.Description,
                IssueType = areaModel.IssueType,
                IssueDate = DateTime.Now,

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
            string? userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return UserDataChanges.FirstOrDefault(u => u.UserId == userId);
        }

    }
}
