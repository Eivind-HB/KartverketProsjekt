using System.ComponentModel.DataAnnotations;

namespace Kartverket.Models
{
    public class LogInData
    {
        [Required]
        public string? Brukernavn { get; set; }
        [Required]
        public string? Passord { get; set; }
    }
}