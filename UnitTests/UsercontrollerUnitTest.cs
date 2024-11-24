using Kartverket.Controllers;
using Kartverket.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Kartverket.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task UDOverview_UserAuthenticated_ReturnsViewWithUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserID = userId,
                UserName = "testuser",
                Mail = "testuser@example.com",
                Password = "hashedpassword"
            };

            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.UDOverview();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<User>(viewResult.Model);
            Assert.Equal(userId, model.UserID);
            Assert.Equal("testuser", model.UserName);
            Assert.Equal("testuser@example.com", model.Mail);
        }

        [Fact]
        public async Task UDOverview_UserNotAuthenticated_RedirectsToRegistrationForm()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await controller.UDOverview();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RegistrationForm", redirectResult.ActionName);
        }


        [Fact]
        public async Task Profile_AuthenticatedUserExists_ReturnsViewWithUserUpdateModel()
        {
            // Arrange
            var userId = 2;
            var user = new User
            {
                UserID = userId,
                UserName = "testuser",
                Mail = "testuser@example.com",
                Password = "hashedpassword"
            };

            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.Profile();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserUpdate>(viewResult.Model);
            Assert.Equal(userId, model.UserID);
            Assert.Equal("testuser", model.UserName);
            Assert.Equal("testuser@example.com", model.Mail);
        }

        [Fact]
        public async Task Profile_UserNotAuthenticated_RedirectsToLogInForm()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await controller.Profile();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("LogInForm", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Profile_AuthenticatedUserNotFound_RedirectsToLogInForm()
        {
            // Arrange
            var userId = 999; // Non-existent user ID
            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var dbContext = new ApplicationDbContext(options);

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.Profile();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("LogInForm", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}