using Kartverket.Controllers;
using Kartverket.Data;
using Kartverket.Models;
using Kartverket.Models.Logins;
using Kartverket.Models.ModelsDB;
using Kartverket.Models.UserAndAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Kartverket.Test
{
    public class AdminControllerTests
    {
        private readonly Mock<ILogger<AdminController>> _mockLogger;
        private readonly Mock<IPasswordHasher<CaseWorker>> _mockPasswordHasher;
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly ApplicationDbContext _dbContext;

        public AdminControllerTests()
        {
            _mockLogger = new Mock<ILogger<AdminController>>();
            _mockPasswordHasher = new Mock<IPasswordHasher<CaseWorker>>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AdminTestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
        }

        [Fact]
        public void LogInFormAdmin_ReturnsViewResult()
        {
            // Arrange
            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object);

            // Act
            var result = controller.LogInFormAdmin();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginAdmin>(viewResult.Model);
        }

        [Fact]
        public async Task LogInFormAdmin_ValidCredentials_RedirectsToOverviewCaseworker()
        {
            // Arrange
            var mockContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            var mockResponse = new Mock<HttpResponse>();
            var mockSession = new Mock<ISession>();
            var mockUser = new Mock<ClaimsPrincipal>();

            mockContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            mockContext.Setup(ctx => ctx.Response).Returns(mockResponse.Object);
            mockContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);
            mockContext.Setup(ctx => ctx.User).Returns(mockUser.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object
            };

            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object)
            {
                ControllerContext = controllerContext
            };

            var model = new LoginAdmin
            {
                Mail = "test@example.com",
                Password = "password"
            };

            // Mock the database context to return a valid employee and case worker
            var employee = new KartverketEmployee
            {
                EmployeeID = 1,
                Mail = "test@example.com",
                Firstname = "John",
                Lastname = "Doe",
                Title = "Developer",
                PhoneNo = 123456789,
                Wage = 50000
            };

            var caseWorker = new CaseWorker
            {
                CaseWorkerID = 1,
                KartverketEmployee_EmployeeID = 1,
                Password = "hashedpassword",
                MustChangePassword = false
            };

            _dbContext.KartverketEmployee.Add(employee);
            _dbContext.CaseWorkers.Add(caseWorker);
            await _dbContext.SaveChangesAsync();

            _mockPasswordHasher.Setup(ph => ph.VerifyHashedPassword(caseWorker, caseWorker.Password, model.Password))
                .Returns(PasswordVerificationResult.Success);

            // Act
            var result = await controller.LogInFormAdmin(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Overviewcaseworker", redirectResult.ActionName);
            Assert.Equal("Case", redirectResult.ControllerName);
        }

        [Fact]
        public async Task LogInFormAdmin_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object);
            var model = new LoginAdmin { Mail = "invalid@kartverket.no", Password = "wrongpassword" };

            // Act
            var result = await controller.LogInFormAdmin(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey(string.Empty));
        }

        [Fact]
        public void ChangePasswordAdmin_Get_ReturnsViewResult()
        {
            // Arrange
            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object);
            var caseWorkerId = 1;

            // Act
            var result = controller.ChangePasswordAdmin(caseWorkerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AdminPasswordUpdate>(viewResult.Model);
            Assert.Equal(caseWorkerId, model.CaseWorkerID);
        }

        [Fact]
        public async Task ChangePasswordAdmin_Post_ValidModel_ChangesPasswordAndRedirects()
        {
            // Arrange
            var caseWorker = new CaseWorker { CaseWorkerID = 2, Password = "oldpassword" };

            if (!_dbContext.CaseWorkers.Any(cw => cw.CaseWorkerID == caseWorker.CaseWorkerID))
            {
                _dbContext.CaseWorkers.Add(caseWorker);
                await _dbContext.SaveChangesAsync();
            }

            _mockPasswordHasher.Setup(ph => ph.HashPassword(caseWorker, "newpassword")).Returns("hashedpassword");

            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object);
            var model = new AdminPasswordUpdate { CaseWorkerID = 2, NewPasswordAdmin = "newpassword", ConfirmPasswordAdmin = "newpassword" };

            // Act
            var result = await controller.ChangePasswordAdmin(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("LogInFormAdmin", redirectResult.ActionName);

            var updatedCaseWorker = await _dbContext.CaseWorkers.FindAsync(2);
            Assert.Equal("hashedpassword", updatedCaseWorker.Password);
            Assert.False(updatedCaseWorker.MustChangePassword);
        }

        [Fact]
        public async Task ChangePasswordAdmin_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new AdminController(_dbContext, _mockPasswordHasher.Object, _mockAuthenticationService.Object);
            controller.ModelState.AddModelError("NewPasswordAdmin", "Required");

            var model = new AdminPasswordUpdate { CaseWorkerID = 1, NewPasswordAdmin = "newpassword", ConfirmPasswordAdmin = "newpassword" };

            // Act
            var result = await controller.ChangePasswordAdmin(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }
    }
}