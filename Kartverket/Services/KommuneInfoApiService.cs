using Kartverket.API_Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace Kartverket.Services
{
    public class KommuneInfoApiService : IKommuneInfoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KommuneInfoApiService> _logger;
        private readonly ApiSettings _apiSettings;

        public KommuneInfoApiService(HttpClient httpClient, ILogger<KommuneInfoApiService> logger, IOptions<ApiSettings> apisettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apisettings.Value;
        }


        public async Task<KommuneInfo> GetKommuneInfoAsync(double latitude, double longitude)
        {
            string formattedLatitude = latitude.ToString(CultureInfo.InvariantCulture);
            string formattedLongitude = longitude.ToString(CultureInfo.InvariantCulture);

            try
            {
                // Koordsys 4326 is WGS84 which is the coordinate system used by leaflet
                int koordsys = 4326;

                var response = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}/punkt?nord={formattedLatitude}&ost={formattedLongitude}&koordsys={koordsys}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"KommuneInfo Response: {json}");
                var kommuneInfo = JsonSerializer.Deserialize<KommuneInfo>(json);

                return kommuneInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching KommuneInfo for coordinates ({latitude}, {longitude}): {ex.Message}");
                return null;
            }

        }

    }
}
