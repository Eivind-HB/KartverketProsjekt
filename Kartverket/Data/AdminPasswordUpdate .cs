using System.ComponentModel.DataAnnotations;
namespace Kartverket.Data

{
    public class AdminPasswordUpdate
    {
        public int CaseWorkerID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPasswordAdmin { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPasswordAdmin", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPasswordAdmin { get; set; }
    }
}