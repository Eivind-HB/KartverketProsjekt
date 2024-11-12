using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class CaseWorkerOverview
    {
        [Key]
        public int CaseWorkerId { get; set; }
        public int CaseWorkerList_Case_CaseNo { get; set; }
        public string? PaidHours { get; set; }
    }
}
