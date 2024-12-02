using Kartverket.Models.ModelsDB;

namespace Kartverket.Models.ViewModels
{
    public class MultipleCasesModel
    {
        public List<Case> Cases { get; set; }
        public List<Issue> AllIssues { get; set; }

    }
}
