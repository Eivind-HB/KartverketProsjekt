using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;

public static class DataSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        SeedKommuneInfo(modelBuilder);
        SeedFylkesInfo(modelBuilder);
        SeedIssue(modelBuilder);
    }

    private static void SeedKommuneInfo(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "kommuneinfo.csv");
        SeedFromCsv<KommuneInfo>(modelBuilder, path, new KommuneInfoMap());
    }

    private static void SeedFylkesInfo(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "fylkeinfo.csv");
        SeedFromCsv<FylkesInfo>(modelBuilder, path, new FylkesInfoMap());
    }

    private static void SeedIssue(ModelBuilder modelBuilder)
    {
        string path = Path.Combine("Data", "issue.csv");
        SeedFromCsv<Issue>(modelBuilder, path, new IssueMap());
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