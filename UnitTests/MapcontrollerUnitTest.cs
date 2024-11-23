using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Kartverket.Controllers;
using Kartverket.Services; 
using Kartverket.Models; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Kartverket.Data;
using Kartverket.API_Models;

namespace Kartverket.Test
{
    public class MapcontrollerUnitTest
    {
        [Fact]
        public async Task GetKommuneInfo_ReturnsJsonResult_WithKommuneInfo()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MapController>>();
            var mockKommuneInfoApiService = new Mock<IKommuneInfoApiService>();
            var expectedKommuneInfo = new API_Models.KommuneInfo
            {
                // Set properties of KommuneInfo as needed for your test
                kommunenummer = "0301",
                kommunenavn = "Oslo"
            };

            mockKommuneInfoApiService
                .Setup(service => service.GetKommuneInfoAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(expectedKommuneInfo);

            var controller = new MapController(mockLogger.Object, mockKommuneInfoApiService.Object);

            // Act
            var result = await controller.GetKommuneInfo(59.9139, 10.7522);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var kommuneInfo = Assert.IsType<API_Models.KommuneInfo>(jsonResult.Value);
            Assert.Equal(expectedKommuneInfo.kommunenummer, kommuneInfo.kommunenummer);
            Assert.Equal(expectedKommuneInfo.kommunenavn, kommuneInfo.kommunenavn);
        }

        [Fact]
        public void CorrectMap_ValidModel_ReturnsViewWithPositions()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MapController>>();
            var mockKommuneInfoApiService = new Mock<IKommuneInfoApiService>();
            var controller = new MapController(mockLogger.Object, mockKommuneInfoApiService.Object);
            var model = new PositionModel
            {
                Latitude = "59.9139",
                Longitude = "10.7522",
                Description = "Oslo"
            };

            // Act
            var result = controller.CorrectMap(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CorrectionOverview", result.ViewName);
            var positions = Assert.IsType<List<PositionModel>>(result.Model);
            Assert.Single(positions);
            Assert.Equal(model, positions[0]);
        }

        [Fact]
        public void CorrectMap_InvalidModel_ReturnsDefaultView()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MapController>>();
            var mockKommuneInfoApiService = new Mock<IKommuneInfoApiService>();
            var controller = new MapController(mockLogger.Object, mockKommuneInfoApiService.Object);
            controller.ModelState.AddModelError("Latitude", "Required");

            // Act
            var result = controller.CorrectMap(new PositionModel()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.ViewName);
        }
    }
}
