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
    }
}