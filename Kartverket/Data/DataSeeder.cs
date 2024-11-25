using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;

public class DataSeeder
{
    public virtual void SeedData(ModelBuilder modelBuilder)
    {
        SeedKommuneInfo(modelBuilder);
        SeedFylkesInfo(modelBuilder);
        SeedIssue(modelBuilder);
        SeedStatus(modelBuilder);
        SeedEmployees(modelBuilder);
        SeedCaseWorker(modelBuilder);
        SeedUser(modelBuilder);
    }

    //finds link and starts SeedFromCsv()
    private static void SeedKommuneInfo(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "kommuneinfo.csv");
        SeedFromCsv<KommuneInfo>(modelBuilder, path, new KommuneInfoMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedFylkesInfo(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "fylkeinfo.csv");
        SeedFromCsv<FylkesInfo>(modelBuilder, path, new FylkesInfoMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedIssue(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "issue.csv");
        SeedFromCsv<Issue>(modelBuilder, path, new IssueMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedStatus(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "Status.csv");
        SeedFromCsv<Status>(modelBuilder, path, new StatusMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedEmployees(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "KartverketEmployee.csv");
        SeedFromCsv<KartverketEmployee>(modelBuilder, path, new KartverketEmployeeMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedCaseWorker(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "CaseWorker.csv");
        SeedFromCsv<CaseWorker>(modelBuilder, path, new CaseWorkerMap());
    }

    //finds link and starts SeedFromCsv()
    private static void SeedUser(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "User.csv");
        SeedFromCsv<User>(modelBuilder, path, new UserMap());
    }

    private static void SeedFromCsv<T>(ModelBuilder modelBuilder, string path, ClassMap<T> classMap) where T : class
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap(classMap);
            var records = csv.GetRecords<T>().ToList();

            modelBuilder.Entity<T>().HasData(records);
        }
    }
}
/// <summary>
/// The ClassMaps sets and explains the information that is gathered by dataseeder
/// </summary>
public class KommuneInfoMap : ClassMap<KommuneInfo>
{
    public KommuneInfoMap()
    {
        Map(m => m.KommuneName).Name("KommuneName");
        Map(m => m.KommuneInfoID).Name("KommuneInfoID");
    }
}

public class FylkesInfoMap : ClassMap<FylkesInfo>
{
    public FylkesInfoMap()
    {
        Map(m => m.FylkesName).Name("FylkesName");
        Map(m => m.FylkesNameID).Name("FylkesNameID");
    }
}

public class IssueMap : ClassMap<Issue>
{
    public IssueMap()
    {
        Map(m => m.IssueType).Name("IssueType");
        Map(m => m.issueNo).Name("issueNo");
    }
}

public class StatusMap : ClassMap<Status>
{
    public StatusMap()
    {
        Map(m => m.StatusName).Name("StatusName");
        Map(m => m.StatusNo).Name("StatusNo");
    }
}

public class KartverketEmployeeMap : ClassMap<KartverketEmployee>
{
    public KartverketEmployeeMap()
    {
        Map(m => m.EmployeeID).Name("EmployeeID");
        Map(m => m.PhoneNo).Name("PhoneNo");
        Map(m => m.Mail).Name("Mail");
        Map(m => m.Title).Name("Title");
        Map(m => m.Wage).Name("Wage");
        Map(m => m.Firstname).Name("Firstname");
        Map(m => m.Lastname).Name("Lastname");
    }

}

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Map(m => m.UserID).Name("UserID");
        Map(m => m.UserName).Name("UserName");
        Map(m => m.Mail).Name("Mail");
        Map(m => m.Password).Name("Password");
    }

}

public class CaseWorkerMap : ClassMap<CaseWorker>
{
    public CaseWorkerMap()
    {
        Map(m => m.CaseWorkerID).Name("CaseWorkerID");
        Map(m => m.KartverketEmployee_EmployeeID).Name("KartverketEmployee_EmployeeID");
        Map(m => m.Password).Name("Password");
    }
}