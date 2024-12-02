using Kartverket.Models.ModelsDB;

namespace Kartverket.Models.ViewModels
{
    public class OverviewCaseworkerModel
    {
        public List<Case> Cases { get; set; }
        public List<Issue> AllIssues { get; set; }
        public List<Status> AllStatus { get; set; }
        public List<User> Users { get; set; }
        public List<CaseWorker> CaseWorkers { get; set; }
        public List<KartverketEmployee> Employees { get; set; }
        public List<CaseWorkerAssignment> CaseWorkerAssignment { get; set; }

    }
}
