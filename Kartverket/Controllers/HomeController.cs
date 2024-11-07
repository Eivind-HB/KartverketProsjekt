using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static List<AreaChange> areaChanges = new List<AreaChange>();
        private static List<UserData> UserDataChanges = new List<UserData>();
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

        
                
    }
}
