namespace Kartverket.Models
{
    public class OverviewCaseworkerModel
    {
        public List<Kartverket.Data.Case> Cases { get; set; }
        public List<Kartverket.Data.Issue> AllIssues { get; set; }
        public List<Kartverket.Data.Status> AllStatus { get; set; }
        public List<Kartverket.Data.User> Users { get; set; }
        public List<Kartverket.Data.CaseWorker> CaseWorkers { get; set; }
        public List<Kartverket.Data.KartverketEmployee> Employees { get; set; }
        public List<Kartverket.Data.CaseWorkerAssignment> CaseWorkerAssignment { get; set; }

    }
}
