using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kartverket.Controllers;
using Kartverket.Data;
using Kartverket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kartverket.Test
{
    public class CaseControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CaseController _controller;
        private readonly Case _caseTestValue;
        private readonly Issue _issueTestValue;
        private readonly KommuneInfo _kommuneInfoTestValue;
        private readonly FylkesInfo _fylkesInfoTestValue;
        private readonly Status _statusTestValue;
        private readonly User _userTestValue;
        private readonly CaseWorker _caseWorkerTestValue;
        private readonly KartverketEmployee _employeeTestValue;

        public CaseControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Initialize test data
            _caseTestValue = new Case {
                CaseNo = 1,
                LocationInfo = "{'type':'Point','coordinates':[10.0, 60.0]}",
                Description = "a description",
                Date = DateOnly.FromDateTime(DateTime.Now),
                User_UserID = 100000,
                IssueNo = 1,
                Images = null,
                KommuneNo = 0312,
                FylkesNo = 03,
                CommentCaseWorker = "halla"
            };
            _issueTestValue = new Issue {
                issueNo = 1,
                IssueType = "Vei/Sti"
            };
            _kommuneInfoTestValue = new KommuneInfo {
                KommuneInfoID = 0312,
                KommuneName = "Oslo"  };
            _fylkesInfoTestValue = new FylkesInfo {
                FylkesNameID = 03,
                FylkesName = "Oslo"
            };
            _statusTestValue = new Status {
                StatusNo = 1,
                StatusName = "Sendt" 
            };
            _userTestValue = new User { 
                UserID = 100000,
                UserName = "Erik",
                Mail = "Erik@gmail.com",
                Password = "test123",
                CaseWorkerUser = null
            };
            _caseWorkerTestValue = new CaseWorker {
                CaseWorkerID = 1,
                KartverketEmployee_EmployeeID = 1,
                Password = "Passord1"
            };
            _employeeTestValue = new KartverketEmployee { 
                EmployeeID = 1,
                PhoneNo = 12345678,
                Mail = "Jens@Kartverket.no",
                Title = "Geolog",
                Wage = 100000,
                Firstname = "Jens",
                Lastname = "Stol"
            };
            // Before calling AreaChangeOverview, check if data is present
            var casesCount = _context.Case.Count();
            var issuesCount = _context.Issues.Count();
            var kommuneInfosCount = _context.KommuneInfo.Count();
            var fylkesInfosCount = _context.FylkesInfo.Count();
            var statusCount = _context.Status.Count();

            Console.WriteLine($"Cases: {casesCount}, Issues: {issuesCount}, KommuneInfos: {kommuneInfosCount}, FylkesInfos: {fylkesInfosCount}, Status: {statusCount}");


            // Add test data to the context
            _context.Case.Add(_caseTestValue);
            _context.Issues.Add(_issueTestValue);
            _context.Status.Add(_statusTestValue);
            _context.KommuneInfo.Add(_kommuneInfoTestValue);
            _context.FylkesInfo.Add(_fylkesInfoTestValue);
            _context.Users.Add(_userTestValue);
            _context.CaseWorkers.Add(_caseWorkerTestValue);
            _context.KartverketEmployee.Add(_employeeTestValue);
            _context.SaveChanges();

            _controller = new CaseController(_context);
            
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Clear the database after each test
        }


        /// <summary>
        /// må bytte ut AreaChangeOverview_ReturnsViewResultWithCorrectModel med en test av HasProfileCaseOverview()
        /// </summary>
        //[Fact]
        //public void AreaChangeOverview_ReturnsViewResultWithCorrectModel()
        //{
        //    // Arrange
        //    _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        //    _controller.TempData["ErrorMessage"] = "Test Error Message";
        //    _controller.TempData["OpprettetSaksnr"] = "123456";
        //
        //    // Act
        //    var result = _controller.AreaChangeOverview();
        //
        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<AreaChangeOverviewModel>(viewResult.Model);
        //
        //    Assert.NotNull(model.Cases);
        //    Assert.NotNull(model.Issues);
        //    Assert.NotNull(model.KommuneInfos);
        //    Assert.NotNull(model.FylkesInfos);
        //    Assert.NotNull(model.Status);
        //
        //    Assert.NotEmpty(model.Cases);
        //    Assert.NotEmpty(model.Issues);
        //    Assert.NotEmpty(model.KommuneInfos);
        //    Assert.NotEmpty(model.FylkesInfos);
        //    Assert.NotEmpty(model.Status);
        //
        //    Assert.Equal("Test Error Message", _controller.ViewBag.ErrorMessage);
        //    Assert.Equal("123456", _controller.ViewBag.ViewModel);
        //    Assert.Null(_controller.TempData["ErrorMessage"]);
        //    Dispose();
        //}

        [Fact]
        public void OverviewCaseworker_ReturnsViewResultWithCorrectModel()
        {
            // Act
            var result = _controller.OverviewCaseworker();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<OverviewCaseworkerModel>(viewResult.Model);

            Assert.NotNull(model.Cases);
            Assert.NotNull(model.Users);
            Assert.NotNull(model.CaseWorkers);
            Assert.NotNull(model.Employees);

            Assert.NotEmpty(model.Cases);
            Assert.NotEmpty(model.Users);
            Assert.NotEmpty(model.CaseWorkers);
            Assert.NotEmpty(model.Employees);

            Assert.Contains(model.Cases, c => c == _caseTestValue);
            Assert.Contains(model.Users, u => u == _userTestValue);
            Assert.Contains(model.CaseWorkers, cw => cw == _caseWorkerTestValue);
            Assert.Contains(model.Employees, e => e == _employeeTestValue);
            Dispose();
        }

        
    }
}
