using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoApiService _KommuneInfoApiService;
              
        private static List<AreaChange> changes = new List<AreaChange>();
        private static List<PositionModel> positions = new List<PositionModel>();

        public HomeController(ILogger<HomeController> logger, IKommuneInfoApiService kommuneInfoApiService)
        {
            _logger = logger;
            _KommuneInfoApiService = kommuneInfoApiService;
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
        public ViewResult RegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult RegistrationForm(UserData userData)
        {
            return View("UDOverview", userData);
        }

        [HttpGet]
        public IActionResult CorrectMap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CorrectMap(PositionModel model)
        {
            if (ModelState.IsValid)
            {
                positions.Add(model);

                return View("CorrectionOverview", positions);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            var newChange = new AreaChange
            {
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson,
                Description = description
            };

            changes.Add(newChange);

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
            return View(changes);
        }
                
        
        [HttpGet]
        public async Task<IActionResult> GetKommuneInfo(double latitude, double longitude)
        {
            var kommuneInfo = await _KommuneInfoApiService.GetKommuneInfoAsync(latitude, longitude);

            // Returner kommuneinfo som JSON
            return Json(kommuneInfo);

        }

    }
}
