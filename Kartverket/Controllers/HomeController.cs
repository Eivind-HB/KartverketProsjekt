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

        //private static List<PositionModel> positions = new List<PositionModel>();

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

        public IActionResult LogInForm()
        {
            return View();
        }

        [HttpGet]
        public ViewResult RegistrationForm()
        {
            return View();
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


        [HttpPost]
        public async Task<IActionResult> KommuneInfoApi(string kommuneNr)
        {
            if (string.IsNullOrEmpty(kommuneNr))
            {
                ViewData["Error"] = "Venligst legg inn et gyldig Kommunenummer. Det skal være 4 siffer.";
                return View("Index");
            }


            var kommuneInfo = await _KommuneInfoApiService.GetKommuneInfoAsync(kommuneNr);
            if (kommuneInfo != null)
            {
                var viewModel = new KommuneInfoViewModel
                {
                    Kommunenavn = kommuneInfo.Kommunenavn,
                    Kommunenummer = kommuneInfo.Kommunenummer,
                    Fylkesnavn = kommuneInfo.Fylkesnavn,
                    SamiskForvaltningsomrade = kommuneInfo.SamiskForvaltningsomrade
                };
                return View("Index", viewModel);
            }
            else
            {
                ViewData["Error"] = $"Ingen resultater for dette nummeret: '{kommuneNr}'.";
                return View("Index");
            }

        }
    }
}
