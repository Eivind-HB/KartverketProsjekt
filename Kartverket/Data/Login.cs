using System.ComponentModel.DataAnnotations;
namespace Kartverket.Data

{
    public class Login
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Passord er påkrevet")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mail er påkrevet")]
        [EmailAddress(ErrorMessage = "Ugyldig mail")]
        public string Mail { get; set; }
    }
}