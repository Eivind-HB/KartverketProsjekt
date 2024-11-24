using System.ComponentModel.DataAnnotations;
namespace Kartverket.Models

{
    public class PasswordUpdateModel
    {
        [Required(ErrorMessage = "Nåværende passord er påkrevet")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nytt passord er påkrevet")]
        [StringLength(100, ErrorMessage = "Det nye passordet må være minst 6 karakterer langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Det nye passordet og det gjenntatte pasordet er ikke det samme.")]
        public string ConfirmPassword { get; set; }
    }
}