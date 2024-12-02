using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Models.ModelsDB
{
    public class Issue
    {
        [Key]
        public int issueNo { get; set; }
        public string? IssueType { get; set; }
        public ICollection<Case> Cases { get; set; }
    }
}
