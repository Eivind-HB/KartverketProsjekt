namespace Kartverket.Models
{
    public class AreaChange
    {

        public int? IssueType { get; set; }
        public string? GeoJson { get; set; }
        public string? Description { get; set; }
        public int? Kommunenummer { get; set; }
        public int? Fylkesnummer { get; set; }
        public byte[]? ImageData { get; set; }

    }
}