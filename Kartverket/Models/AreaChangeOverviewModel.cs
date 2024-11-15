namespace Kartverket.Models
{
    public class AreaChangeOverviewModel
    {
        public List<Kartverket.Data.Case> Cases { get; set; }
        public List<Kartverket.Data.Issue> Issues { get; set; }
        public List<Kartverket.Data.KommuneInfo> KommuneInfos { get; set; }
        public List<Kartverket.Data.FylkesInfo> FylkesInfos { get; set; }


    }
}
