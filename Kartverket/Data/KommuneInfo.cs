using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class KommuneInfo
    {
        [Key]
        public int KommuneInfoID { get; set; }
        public string? KommuneName { get; set; }
    }
}
