using Kartverket.Controllers;
using Kartverket.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace Kartverket.Tests
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
    }
}