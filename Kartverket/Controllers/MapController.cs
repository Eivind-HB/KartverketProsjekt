using Kartverket.Models;
using Kartverket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Kartverket.Controllers
     
{
    [Route("api/[controller]")]
    public class MapController : Controller
    {
        private static List<PositionModel> positions = new List<PositionModel>();

        private readonly IKommuneInfoApiService _KommuneInfoApiService;
        private readonly ILogger<MapController> _logger;


        public MapController(ILogger<MapController> logger, IKommuneInfoApiService kommuneInfoApiService)
        {
            _logger = logger;
            _KommuneInfoApiService = kommuneInfoApiService;
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

        //Fetching of kommuneinfo
        [HttpGet("GetKommuneInfo")]
        public async Task<IActionResult> GetKommuneInfo(double latitude, double longitude)
        {
            var kommuneInfo = await _KommuneInfoApiService.GetKommuneInfoAsync(latitude, longitude);

            // Returns kommuneinfo as JSON
            return Json(kommuneInfo);

        }
    }
}
