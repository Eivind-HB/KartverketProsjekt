namespace Kartverket.Models
{
    public class SingleCaseModel
    {
        public Kartverket.Data.Case Case { get; set; }
        public List<Kartverket.Data.Issue> Issues { get; set; }
        public List<Kartverket.Data.KommuneInfo> KommuneInfos { get; set; }
        public List<Kartverket.Data.FylkesInfo> FylkesInfos { get; set; }
        public List<Kartverket.Data.Status> Status { get; set; }


    }
}
