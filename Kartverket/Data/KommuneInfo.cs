using MySqlConnector;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Data
{
    public class KommuneInfo
    {
        [Key]
        public int KommuneInfoID { get; set; }
        public string KommuneName { get; set; }
    }
}
