using abandoned_vehicle_service.Helpers;
using abandoned_vehicle_service.Models;
using abandoned_vehicle_service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Threading.Tasks;
using Xunit;
using Address = StockportGovUK.NetStandard.Models.Addresses.Address;

namespace abandoned_vehicle_service_tests.Services
{
    public class AbandonedVehicleServiceTests
    {
        private Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private AbandonedVehicleService _service;
        private Mock<ILogger<AbandonedVehicleService>> _mocklogger = new Mock<ILogger<AbandonedVehicleService>>();
        private Mock<IMailHelper> _mockMailHelper = new Mock<IMailHelper>();
        private AbandonedVehicleReport _abandonedVehicleReportData = new AbandonedVehicleReport
        {
            FirstName = "Joe",
            LastName = "Bloggs",
            Email = "joe@test.com",
            Phone = "0161 123 1234",
            FurtherDetails = "Further detail test",
            AbandonedReason = "Further detail test",
            ImageOrVideo = "Further detail test",
            VehicleMake = "Further detail test",
            VehicleModel = "Further detail test",
            VehicleColour = "Further detail test",
            VehicleRegistration = "Further detail test",
            StreetAddress = new Address
            {
                AddressLine1 = "1 Oxford Road",
                AddressLine2 = "Ardwick",
                Postcode = "M1",
            },
            CustomersAddress = new Address
            {
                AddressLine1 = "118 London Road",
                AddressLine2 = "",
                Postcode = "M1 2SD",
            }
        };
        private IConfiguration config = InitConfiguration();

        public AbandonedVehicleServiceTests()
        {
            _service = new AbandonedVehicleService(_mockVerintServiceGateway.Object,
            config,
            _mockMailHelper.Object);
        }

        [Fact]
        public async Task CreateCase_ShouldReThrowCreateCaseException_CaughtFromVerintGateway()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Throws(new Exception("TestException"));

            Exception result = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_abandonedVehicleReportData));
            Assert.Contains($"CRMService CreateAbandonedVehicleService an exception has occured while creating the case in verint service", result.Message);
        }

        [Fact]
        public async Task CreateCase_ShouldThrowException_WhenIsNotSuccessStatusCode()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = false
                });

            _ = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_abandonedVehicleReportData));
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            string result = await _service.CreateCase(_abandonedVehicleReportData);

            Assert.Contains("test", result);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCRMCase()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            _ = await _service.CreateCase(_abandonedVehicleReportData);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Equal(_abandonedVehicleReportData.StreetAddress.PlaceRef, crmCaseParameter.Street.Reference);
            Assert.Contains(_abandonedVehicleReportData.FurtherDetails, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.AbandonedReason, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.ImageOrVideo, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.ImageOrVideo, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleColour, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleMake, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleModel, crmCaseParameter.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleRegistration, crmCaseParameter.Description);
        }

        public static IConfiguration InitConfiguration()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();
            return config;
        }
    }
}
