using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class CaseWorkerList
    {
        public string AmountWorkers { get; set; }
        [Key]
        public int Case_CaseNo { get; set; }
        ///[HasKey]
        public int CaseWorkerOverview_CaseWorkerID { get; set; }
    }
}
