using Kartverket.Controllers;
using Kartverket.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Ganss.Xss;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Kartverket.Models;
using Microsoft.AspNetCore.SignalR;

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

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        public interface IUrlHelperWrapper
        {
            string Action(string actionName, string controllerName, object values, string protocol);
        }

        public class CustomUrlHelper : IUrlHelper
        {
            private readonly IUrlHelperWrapper _wrapper;

            public CustomUrlHelper(IUrlHelperWrapper wrapper)
            {
                _wrapper = wrapper;
            }

            // Implement the missing methods
            public string Content(string? contentPath) => throw new NotImplementedException();
            public string RouteUrl(UrlRouteContext routeContext) => throw new NotImplementedException();
            

            

            public string Action(UrlActionContext actionContext)
            {
                return _wrapper.Action(actionContext.Action, actionContext.Controller, actionContext.Values, actionContext.Protocol);
            }


            public string Action(string actionName, string controllerName, object values, string protocol, string host)
            {
                return _wrapper.Action(actionName, controllerName, values, protocol);
            }

            // Implement other IUrlHelper methods as needed, returning default values or throwing NotImplementedException
            public string Action(string actionName) => throw new NotImplementedException();
            public string Action(string actionName, object values) => throw new NotImplementedException();
            public string Action(string actionName, string controllerName) => throw new NotImplementedException();
            public string Action(string actionName, string controllerName, object values) => throw new NotImplementedException();
            public bool IsLocalUrl(string url) => throw new NotImplementedException();
            public string RouteUrl(object values) => throw new NotImplementedException();
            public string RouteUrl(string routeName) => throw new NotImplementedException();
            public string RouteUrl(string routeName, object values) => throw new NotImplementedException();
            public string Link(string routeName, object values) => throw new NotImplementedException();
            public ActionContext ActionContext => throw new NotImplementedException();
        }

        [Fact]
        public async Task UDOverview_Post_ValidModel_CreatesUserAndRedirects()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use a unique name for each test
                .Options;

            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("hashedpassword");

            using var dbContext = new ApplicationDbContext(options); // Use the in-memory context

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var services = new ServiceCollection();
            services.AddSingleton(authServiceMock.Object);
            services.AddSingleton(mockLogger.Object);
            services.AddSingleton(mockPasswordHasher.Object);

            var serviceProvider = services.BuildServiceProvider();

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProvider
                    }
                }
            };

            var urlHelperWrapperMock = new Mock<IUrlHelperWrapper>();
            urlHelperWrapperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns("http://localhost/UDOverview");

            controller.Url = new CustomUrlHelper(urlHelperWrapperMock.Object);

            var model = new User
            {
                UserName = "testuser",
                Mail = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await controller.UDOverview(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UDOverview", redirectResult.ActionName);

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Mail == "test@example.com");
            Assert.NotNull(user);
            Assert.Equal("testuser", user.UserName);
            Assert.Equal("hashedpassword", user.Password);

            // Verify that SignInAsync was called
            authServiceMock.Verify(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.Is<ClaimsPrincipal>(p =>
                    p.HasClaim(c => c.Type == ClaimTypes.Name && c.Value == "testuser") &&
                    p.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && int.Parse(c.Value) >= 100000 && int.Parse(c.Value) <= 999999)
                ),
                It.IsAny<AuthenticationProperties>()),
            Times.Once);
        }


        [Fact]
        public async Task UDOverview_Post_ExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            var dbContext = GetInMemoryDbContext();

            // Add existing user
            dbContext.Users.Add(new User { UserID = 1, UserName = "existinguser", Mail = "existing@example.com", Password = "hashedpassword" });
            await dbContext.SaveChangesAsync();

            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            var model = new User
            {
                UserName = "testuser",
                Mail = "existing@example.com", // Using existing email
                Password = "password123"
            };

            // Act
            var result = await controller.UDOverview(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("RegistrationForm", viewResult.ViewName);
            Assert.IsType<User>(viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey("Mail"));
            Assert.Contains("Denne e-postadressen er allerede i bruk.", controller.ModelState["Mail"].Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task UDOverview_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<UserController>>();
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            var dbContext = GetInMemoryDbContext();
            var controller = new UserController(mockLogger.Object, dbContext, mockPasswordHasher.Object);

            controller.ModelState.AddModelError("UserName", "The UserName field is required.");

            var model = new User(); // Empty model to trigger ModelState.IsValid == false

            // Act
            var result = await controller.UDOverview(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("RegistrationForm", viewResult.ViewName);
            Assert.IsType<User>(viewResult.Model);
        }
    }
}