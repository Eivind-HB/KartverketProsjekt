using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace Kartverket.Models.ModelsDB
{
    public class KartverketEmployee
    {
        [Key]
        public int EmployeeID { get; set; }
        public int PhoneNo { get; set; }
        public string Mail { get; set; }
        public string Title { get; set; }
        public int Wage { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public ICollection<CaseWorker> CaseWorkers { get; set; }

    }
}
