using Kartverket.Controllers;
using Kartverket.Data;
using Kartverket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using System.Threading.Tasks;

namespace Kartverket.Test
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_SetsViewBagIsLoggedInCorrectly()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var mockTempDataDictionaryFactory = Substitute.For<ITempDataDictionaryFactory>();
            var mockTempDataDictionary = Substitute.For<ITempDataDictionary>();
            mockTempDataDictionaryFactory.GetTempData(Arg.Any<HttpContext>()).Returns(mockTempDataDictionary);

            // Create a mock for IHttpClientFactory
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>();

            // Pass the mockHttpClientFactory to the HomeController
            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);
            controller.TempData = mockTempDataDictionary;

            var httpContext = Substitute.For<HttpContext>();
            var user = Substitute.For<ClaimsPrincipal>();
            var identity = Substitute.For<IIdentity>();

            identity.IsAuthenticated.Returns(true);
            user.Identity.Returns(identity);
            httpContext.User.Returns(user);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True((bool)result.ViewData["IsLoggedIn"]);
        }

        [Fact]
        public void Index_SetsViewBagIsLoggedInToFalseWhenNotAuthenticated()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var mockTempDataDictionaryFactory = Substitute.For<ITempDataDictionaryFactory>();
            var mockTempDataDictionary = Substitute.For<ITempDataDictionary>();
            mockTempDataDictionaryFactory.GetTempData(Arg.Any<HttpContext>()).Returns(mockTempDataDictionary);

            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);
            controller.TempData = mockTempDataDictionary;

            var httpContext = Substitute.For<HttpContext>();
            var user = Substitute.For<ClaimsPrincipal>();
            var identity = Substitute.For<IIdentity>();

            identity.IsAuthenticated.Returns(false);
            user.Identity.Returns(identity);
            httpContext.User.Returns(user);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result.ViewData["IsLoggedIn"]);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var mockTempDataDictionaryFactory = Substitute.For<ITempDataDictionaryFactory>();
            var mockTempDataDictionary = Substitute.For<ITempDataDictionary>();
            mockTempDataDictionaryFactory.GetTempData(Arg.Any<HttpContext>()).Returns(mockTempDataDictionary);

            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);
            controller.TempData = mockTempDataDictionary;

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void OmOss_ReturnsRedirectResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Act
            var result = controller.OmOss();

            // Assert
            Assert.IsType<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.Equal("https://www.kartverket.no/om-kartverket", redirectResult.Url);
        }

        [Fact]
        public void KontaktOss_ReturnsRedirectResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockDbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, mockDbContext, mockHttpClientFactory);

            // Act
            var result = controller.KontaktOss();

            // Assert
            Assert.IsType<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.Equal("https://www.kartverket.no/om-kartverket/kontakt-oss", redirectResult.Url);
        }

        [Fact]
        public void RoadCorrection_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Act
            var result = controller.RoadCorrection();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisterAreaChange_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Act
            var result = controller.RegisterAreaChange();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisteredCaseOverview_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Act
            var result = controller.RegisteredCaseOverview();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterAreaChange_ValidData_ReturnsRedirectToAction()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            // Mock DataSeeder to avoid file access
            var mockDataSeeder = new Mock<DataSeeder>();
            mockDataSeeder.Setup(ds => ds.SeedData(It.IsAny<ModelBuilder>())).Verifiable();

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger.Object, context, mockHttpClientFactory);

            // Set up user authentication
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var areaModel = new AreaChange
            {
                GeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0, 60.0]}",
                Description = "Test description",
                IssueType = 1,
                Kommunenummer = 0301,
                Fylkesnummer = 03
            };

            var userModel = new UserData { UserName = "testuser" };

            // Mock file upload
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            // Act
            var result = await controller.RegisterAreaChange(areaModel, userModel, fileMock.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("HasProfileCaseOverview", redirectResult.ActionName);
            Assert.Equal("Case", redirectResult.ControllerName);

            // Verify that a new case was added to the database
            var addedCase = await context.Case.FirstOrDefaultAsync();
            Assert.NotNull(addedCase);
            Assert.Equal(areaModel.Description, addedCase.Description);
            Assert.Equal(areaModel.GeoJson, addedCase.LocationInfo);
            // Add more assertions as needed
        }

        [Fact]
        public async Task RegisterAreaChange_InvalidKommuneName_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Create an AreaChange model with valid GeoJson but invalid Kommune name
            var areaModel = new AreaChange
            {
                GeoJson = "{\"type\":\"Point\",\"coordinates\":[102.0, 0.5]}", // Valid GeoJson
                Description = "Test description",
                IssueType = 1,
                Kommunenummer = null // Invalid Kommune name
            };

            var userModel = new UserData
            {
                UserName = "testuser"
            };

            // Act
            var result = await controller.RegisterAreaChange(areaModel, userModel, null) as ViewResult;

            // Assert
            Assert.NotNull(result); // Ensure that result is not null
            Assert.Equal("RoadCorrection", result.ViewName); // Check that it returns the correct view
            Assert.Equal("Kommune er ikke registrert! Prøv å trykk en ekstra gang på kartet etter du har markert det", result.ViewData["ErrorMessage"]); // Check for specific error message
        }

        [Fact]
        public async Task RegisterAreaChange_InvalidGeoJson_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            var mockHttpClientFactory = Substitute.For<IHttpClientFactory>(); // Mock IHttpClientFactory

            var controller = new HomeController(mockLogger, dbContext, mockHttpClientFactory);

            // Create an AreaChange model with invalid GeoJson (empty string)
            var areaModel = new AreaChange
            {
                GeoJson = "", // Invalid GeoJson (empty string)
                Description = "Test description",
                IssueType = 1,
            };

            var userModel = new UserData
            {
                UserName = "testuser"
            };

            // Act
            var result = await controller.RegisterAreaChange(areaModel, userModel, null) as ViewResult;

            // Assert
            Assert.NotNull(result); // Ensure that result is not null
            Assert.Equal("RoadCorrection", result.ViewName); // Check that it returns the correct view
            Assert.Equal("Kartet må være markert!", result.ViewData["ErrorMessage"]); // Check for specific error message
        }
    }
}