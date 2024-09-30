using System.ComponentModel.DataAnnotations;


namespace Kartverket.Models
{
    public class MapCorrection
    {
        [Required]
        public string ProblemDescription { get; set; }
        
        [Required]
        public string Longitude { get; set; }
        
        [Required]
        public string Latitude { get; set; }



    }
}
