using abandoned_vehicle_service.Controllers;
using abandoned_vehicle_service.Models;
using abandoned_vehicle_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace abandoned_vehicle_service_tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _homeController;
        private readonly Mock<IAbandonedVehicleService> _mockAbandonedVehicleService = new Mock<IAbandonedVehicleService>();

        public HomeControllerTest()
        {
            _homeController = new HomeController(Mock.Of<ILogger<HomeController>>(), _mockAbandonedVehicleService.Object);
        }

        [Fact]
        public async Task Post_ShouldCallCreateCase()
        {
            _mockAbandonedVehicleService
                .Setup(_ => _.CreateCase(It.IsAny<AbandonedVehicleReport>()))
                .ReturnsAsync("test");

            IActionResult result = await _homeController.Post(null);

            _mockAbandonedVehicleService
                .Verify(_ => _.CreateCase(null), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnOkActionResult()
        {
            _mockAbandonedVehicleService
                .Setup(_ => _.CreateCase(It.IsAny<AbandonedVehicleReport>()))
                .ReturnsAsync("test");

            IActionResult result = await _homeController.Post(null);

            Assert.Equal("OkObjectResult", result.GetType().Name);
        }
    }
}
