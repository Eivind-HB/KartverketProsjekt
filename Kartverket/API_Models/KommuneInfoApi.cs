using System.Text.Json.Serialization;

namespace Kartverket.API_Models
{
   
    public class KommuneInfo
    {
        [JsonPropertyName("fylkesnavn")]
        public string? fylkesnavn { get; set; }

        [JsonPropertyName("fylkesnummer")]
        public string? fylkesnummer { get; set; }

        [JsonPropertyName("kommunenavn")]
        public string? kommunenavn { get; set; }

        [JsonPropertyName("kommunenummer")]
        public string? kommunenummer { get; set; }
       
    }

}
