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


        /// <summary>
        /// Initializes the MapController class.
        /// </summary>
        /// <param name="logger">The logger used for logging information and errors.</param>
        /// <param name="kommuneInfoApiService">The service used to retrieve municipality info from Kartverkets KommuneInfoApi </param>       
        public MapController(ILogger<MapController> logger, IKommuneInfoApiService kommuneInfoApiService)
        {
            _logger = logger;
            _KommuneInfoApiService = kommuneInfoApiService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Retrieves municipality information from Kartverkets KommuneInfoApi based on the provided geographical coordinates.
        /// </summary>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <returns>A JSON object containing municipalityname, municipalitynumber, countyname and countynumber.</returns>
        /// <remarks>
        /// This method uses an external service (_KommuneInfoApiService) to fetch the information.
        /// The method is asynchronous.
        /// </remarks>
        [HttpGet("GetKommuneInfo")]
        public async Task<IActionResult> GetKommuneInfo(double latitude, double longitude)
        {
            var kommuneInfo = await _KommuneInfoApiService.GetKommuneInfoAsync(latitude, longitude);

            // Returns kommuneinfo as JSON
            return Json(kommuneInfo);

        }
    }
}
