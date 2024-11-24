using MySqlConnector;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Data
{
    public class Case
    {
        [Key]
        [Required]
        public int CaseNo { get; set; }
        [Required]
        public string LocationInfo { get; set; }
        public string? Description { get; set; }
        public string? CommentCaseWorker { get; set; }
        [Required]
        public DateOnly? Date { get; set; }
        [Required]
        public int User_UserID { get; set; }
        [Required]
        public int IssueNo { get; set; }
        public byte[]? Images { get; set; }
        public int KommuneNo { get; set; }
        [Required]
        public int FylkesNo { get; set; }
        [Required]
        public int StatusNo { get; set; }
        [ForeignKey("StatusNo")]
        public Status Status { get; set; }
        [ForeignKey("FylkesNo")]
        public FylkesInfo FylkesInfo { get; set; }
        [ForeignKey("KommuneNo")]
        public KommuneInfo KommuneInfo { get; set; }
        [ForeignKey("IssueNo")]
        public Issue Issue {  get; set; }


    }
}
