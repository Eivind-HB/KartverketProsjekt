using System.ComponentModel.DataAnnotations;
namespace Kartverket.Models.Logins

{
    public class LoginAdmin
    {
        [Key]
        public int CaseWorkerID { get; set; }

        [Required(ErrorMessage = "Mail er påkrevet")]
        [EmailAddress(ErrorMessage = "Ugyldig mail")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Passord er påkrevet")]
        public string Password { get; set; }

    }
}