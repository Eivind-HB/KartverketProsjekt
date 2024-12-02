using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Kartverket.Models.ModelsDB
{
    public class CaseWorker
    {
        [Key]
        public int CaseWorkerID { get; set; }
        public int KartverketEmployee_EmployeeID { get; set; }
        public string Password { get; set; }

        public bool MustChangePassword { get; set; } = false;
        [ForeignKey("KartverketEmployee_EmployeeID")]
        public KartverketEmployee KartverketEmployee { get; set; }
        public User User { get; set; }
        public ICollection<CaseWorkerAssignment> CaseWorkerAssignments { get; set; }
    }
}
