using System.ComponentModel.DataAnnotations;
namespace Kartverket.Data

{
public class UserUpdate
{
    [Key]

    public int UserID { get; set; }

    [Required(ErrorMessage = "Brukernavn er påkrevet")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Mail er påkrevet")]
    [EmailAddress(ErrorMessage = "Ugyldig mail")]
    public string Mail { get; set; }
}
}