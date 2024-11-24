using MySqlConnector;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class CaseWorkerAssignment
    {

        [Key]
        public int CaseNo { get; set; }
        public int CaseWorkerID { get; set; }
        public decimal PaidHours { get; set; }
    }
}
