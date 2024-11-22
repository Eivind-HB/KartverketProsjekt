﻿using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Kartverket.Data
{
    public class CaseWorker
    {
        [Key]
        public int CaseWorkerID { get; set; }
        public int KartverketEmployee_EmployeeID { get; set; }
        public string Password { get; set; }

        public bool MustChangePassword { get; set; } = false;
    }
}
