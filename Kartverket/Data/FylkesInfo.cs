using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class FylkesInfo
    {
        [Key]
        public int FylkesNameID { get; set; }
        public string? FylkesName { get; set; }
    }
}
