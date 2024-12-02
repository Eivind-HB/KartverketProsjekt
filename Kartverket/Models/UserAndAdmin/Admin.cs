using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Models.UserAndAdmin
{
    public class Admin
    {
        [Key]
        public int CaseWorkerID { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [StringLength(100, ErrorMessage = "E-postadressen kan ikke være lengre enn {1} tegn.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, ErrorMessage = "Passordet må være mellom {2} og {1} tegn.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
