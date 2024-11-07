namespace Kartverket.Models
{
    public class AreaChange
    { 
        
        public string? IssueId { get; set; }
        public string? IssueType { get; set; }
        public DateTime IssueDate { get; set; }
        public string? GeoJson { get; set; }
        public string? Description { get; set; }
        public string? Kommunenavn { get; set; }
        public string? Kommunenummer { get; set; }
        public string? Fylkesnavn { get; set; }
        public string? Fylkesnummer { get; set; }
        public string? Username { get; set; }
        public string? ImagePath { get; set; }

    }
}