using AngleSharp.Dom;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Brukernavn er påkrevd")]
        [StringLength(50, ErrorMessage = "Brukernavnet må være mellom {2} og {1} tegn.", MinimumLength = 2)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [StringLength(100, ErrorMessage = "E-postadressen kan ikke være lengre enn {1} tegn.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, ErrorMessage = "Passordet må være mellom {2} og {1} tegn.", MinimumLength = 6)]
        public string Password { get; set; }
        //explains the relation between caseworker and user, 1:1
        public int? CaseWorkerUser { get; set; }
        public CaseWorker CaseWorker { get; set; }
        //explains the relation between User and Cases, 1:n
        public ICollection<Case> Cases { get; set; }
    }
}
