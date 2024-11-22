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

            var controller = new HomeController(mockLogger, dbContext);
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

            var controller = new HomeController(mockLogger, dbContext);
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

            var controller = new HomeController(mockLogger, dbContext);
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
            var controller = new HomeController(mockLogger, dbContext);

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
            var controller = new HomeController(mockLogger, mockDbContext);

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
            var controller = new HomeController(mockLogger, dbContext);

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
            var controller = new HomeController(mockLogger, dbContext);

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
            var controller = new HomeController(mockLogger, dbContext);

            // Act
            var result = controller.RegisteredCaseOverview();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterAreaChange_ValidData_ReturnsRedirectToAction()
        {
            // Arrange
            var mockLogger = Substitute.For<ILogger<HomeController>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            var controller = new HomeController(mockLogger, dbContext);

            // Mocking User Identity
            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Name, "testuser")
    };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            // Set the User property of the controller
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            var areaModel = new AreaChange
            {
                GeoJson = "{\"type\":\"Point\",\"coordinates\":[102.0, 0.5]}",
                Description = "Test description",
                IssueType = "1",
                Kommunenavn = "Oslo",
                Kommunenummer = "0301",
                Fylkesnavn = "Oslo",
                Fylkesnummer = "03"
            };

            var userModel = new UserData
            {
                UserName = "testuser"
            };

            // Create a mock IFormFile
            var fileMock = Substitute.For<IFormFile>();
            fileMock.Length.Returns(1);
            fileMock.FileName.Returns("testimage.png");

            using (var stream = new MemoryStream())
            {
                await fileMock.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position for reading
                fileMock.OpenReadStream().Returns(stream);
            }

            // Act
            var result = await controller.RegisterAreaChange(areaModel, userModel, fileMock) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AreaChangeOverview", result.ActionName);

            // Verify that the data was saved to the database
            Assert.Single(dbContext.Case); // Ensure one case was added
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
            var controller = new HomeController(mockLogger, dbContext);

            // Create an AreaChange model with valid GeoJson but invalid Kommune name
            var areaModel = new AreaChange
            {
                GeoJson = "{\"type\":\"Point\",\"coordinates\":[102.0, 0.5]}", // Valid GeoJson
                Description = "Test description",
                IssueType = "1",
                Kommunenavn = null // Invalid Kommune name
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
            Assert.Equal("Kommune er ikke regisrert! Prøv å trykk en ekstra gang på kartet etter du har markert det", result.ViewData["ErrorMessage"]); // Check for specific error message
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
            var controller = new HomeController(mockLogger, dbContext);

            // Create an AreaChange model with invalid GeoJson (empty string)
            var areaModel = new AreaChange
            {
                GeoJson = "", // Invalid GeoJson (empty string)
                Description = "Test description",
                IssueType = "1",
                Kommunenavn = "Oslo" // Ensure this is valid to focus on GeoJson validation
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