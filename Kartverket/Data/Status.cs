using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class Status
    {
        [Key]
        public int StatusNo { get; set; }
        public string? StatusName { get; set; }
    }
}
