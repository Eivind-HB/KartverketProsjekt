using System;
using System.IO;
using System.Linq;
using System.Globalization;
using Xunit;
using CsvHelper;
using Kartverket.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kartverket.Test
{
    public class CsvTests
    {

        [Fact]
        public void CaseWorkerTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "CaseWorker.csv");

            // Act
            var records = ReadCsvFile<CaseWorkerTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(5, records.Count()); // Checking that the CSV file has 1 record
            Assert.Equal(1, records.First().CaseWorkerID);
            Assert.Equal(1, records.First().KartverketEmployee_EmployeeID);
        }

        [Fact]
        public void FylkeInfoTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "fylkeinfo.csv");

            // Act
            var records = ReadCsvFile<FylkeInfoTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(16, records.Count()); // Checking that the CSV file has 15 records
            Assert.Equal(03, records.First().FylkesNameID);
            Assert.Equal("Oslo", records.First().FylkesName);
        }

        [Fact]
        public void KommuneInfoTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "kommuneinfo.csv");

            // Act
            var records = ReadCsvFile<KommmuneInfoTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(358, records.Count()); // Checking that the CSV file has 357 records
            Assert.Equal(5636, records.First().KommuneInfoID);
            Assert.Equal("Unjárga - Nesseby", records.First().KommuneName);
        }

        [Fact]
        public void StatusTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "Status.csv");

            // Act
            var records = ReadCsvFile<StatusTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(5, records.Count()); // Checking that the CSV file has 5 records
            Assert.Equal(1, records.First().StatusNo);
            Assert.Equal("Sendt", records.First().StatusName);
        }

        [Fact]
        public void KartverketEmployeeTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "KartverketEmployee.csv");

            // Act
            var records = ReadCsvFile<KartverketEmployeeTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(21, records.Count()); // Checking that the CSV file has 21 records
            Assert.Equal(1, records.First().EmployeeID);
            Assert.Equal(00000000, records.First().PhoneNo);
            Assert.Equal("admin@kartverket.no", records.First().Mail);
            Assert.Equal("Admin", records.First().Title);
            Assert.Equal(0, records.First().Wage);
            Assert.Equal("Admin", records.First().Firstname);
            Assert.Equal("Adminsen", records.First().Lastname);

        }

        [Fact]
        public void IssueTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "issue.csv");

            // Act
            var records = ReadCsvFile<IssueTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Equal(4, records.Count()); // Checking that the CSV file has 4 records
            Assert.Equal(1, records.First().issueNo);
            Assert.Equal("Adresse/Tomt", records.First().IssueType);

        }

        public void UserTestCsvFileReading()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string csvFilePath = Path.Combine(projectDirectory, "Kartverket", "Data", "User.csv");

            // Act
            var records = ReadCsvFile<UserTestRecord>(csvFilePath);

            // Assert
            Assert.NotNull(records);
            Assert.Single(records); // Checking that the CSV file has 4 records
            Assert.Equal(404, records.First().UserID);
            Assert.Equal("NoUserProfile", records.First().UserName);

        }



        private IEnumerable<T> ReadCsvFile<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }
    }

    //Class that reflects the csv file information the test method is fetching from
    public class CaseWorkerTestRecord
    {
        public int CaseWorkerID { get; set; }
        public int KartverketEmployee_EmployeeID { get; set; }
        // public string Password { get; set; } -- chose not to have this due to privacy and necessity
    }
    //Class that reflects the csv file information the test method is fetching from
    public class FylkeInfoTestRecord
    {
        public int FylkesNameID { get; set; }
        public string FylkesName { get; set; }
    }
    //Class that reflects the csv file information the test method is fetching from
    public class KommmuneInfoTestRecord
    {
        public int KommuneInfoID { get; set; }
        public string KommuneName { get; set; }
    }
    //Class that reflects the csv file information the test method is fetching from
    public class StatusTestRecord
    {
        public int StatusNo { get; set; }
        public string StatusName { get; set; }
    }
    //Class that reflects the csv file information the test method is fetching from
    public class KartverketEmployeeTestRecord
    {
        public int EmployeeID { get; set; }
        public int PhoneNo { get; set; }
        public string Mail { get; set; }
        public string Title { get; set; }
        public int Wage { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
    //Class that reflects the csv file information the test method is fetching from
    public class IssueTestRecord
    {
        public int issueNo { get; set; }
        public string IssueType { get; set; }
    }

    public class UserTestRecord
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}