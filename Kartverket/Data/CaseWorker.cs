using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class CaseWorker
    {
        [Key]
        public int CaseWorkerID { get; set; }
        public int KartverketEmployee_EmployeeID { get; set; }
    }
}
