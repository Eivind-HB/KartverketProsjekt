﻿using Kartverket.API_Models;
using Microsoft.Extensions.Options;
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
        public async Task<KommuneInfo> GetKommuneInfoAsync(string kommuneNr)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}/kommuner/{kommuneNr}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"KommuneInfo Response: {json}");
                var kommuneInfo = JsonSerializer.Deserialize<KommuneInfo>(json);
                return kommuneInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching KommuneInfo for {kommuneNr}: {ex.Message}");
                return null;
            }

        }
    }
}
