namespace Kartverket.Models
{
    public class OverviewCaseworkerModel
    {
        public List<Kartverket.Data.Case> Cases { get; set; }
        public List<Kartverket.Data.Issue> Issues { get; set; }
        public List<Kartverket.Data.KommuneInfo> KommuneInfos { get; set; }
        public List<Kartverket.Data.FylkesInfo> FylkesInfos { get; set; }
        public List<Kartverket.Data.User> Users { get; set; }
        public List<Kartverket.Data.CaseWorker> CaseWorkers { get; set; }
        public List<Kartverket.Data.KartverketEmployee> Employees { get; set; }


    }
}
