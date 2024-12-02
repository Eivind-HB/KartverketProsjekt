using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Kartverket.Models.ModelsDB
{
    [PrimaryKey(nameof(CaseNo), nameof(CaseWorkerID))]
    public class CaseWorkerAssignment
    {

        //[Key]

        public int CaseNo { get; set; }
        public int CaseWorkerID { get; set; }
        public decimal PaidHours { get; set; }

        [ForeignKey("CaseNo")]
        public Case Case { get; set; }
        [ForeignKey("CaseWorkerID")]
        public CaseWorker CaseWorkers { get; set; }
    }
}
