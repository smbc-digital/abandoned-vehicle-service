using abandoned_vehicle_service.Helpers;
using abandoned_vehicle_service.Models;
using abandoned_vehicle_service.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
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
            AbandonedReason = "Abandoned Reason test",
            ImageOrVideo = "Image Or Video test",
            VehicleMake = "Vehicle Make test",
            VehicleModel = "Vehicle Model test",
            VehicleColour = "Vehicle Colour test",
            VehicleRegistration = "Vehicle Registration test",
            StreetAddress = new Address
            {
                AddressLine1 = "1 Oxford Road",
                AddressLine2 = "Ardwick",
                Postcode = "M1",
                PlaceRef = "10000000"
            },
            CustomersAddress = new Address
            {
                AddressLine1 = "118 London Road",
                AddressLine2 = "",
                Town = "",
                Postcode = "M1 2SD",
            }
        };

        public AbandonedVehicleServiceTests()
        {
            var mockVerintOptions = new Mock<IOptions<VerintOptions>>();
            mockVerintOptions
                .SetupGet(_ => _.Value)
                .Returns(new VerintOptions
                {
                    Classification = "Test Classification",
                    EventTitle = "Test Event Title"                    
                });

            var mockConfirmIntegrationEFromOptions = new Mock<IOptions<ConfirmIntegrationFormOptions>>();
            mockConfirmIntegrationEFromOptions
                .SetupGet(_ => _.Value)
                .Returns(new ConfirmIntegrationFormOptions
                {
                    EventId = 1000,
                    ClassCode = "test ClassCode",
                    FollowUp = "test FollowUp",
                    ServiceCode = "test ServiceCode",
                    SubjectCode = "test SubjectCode"
                });

            _service = new AbandonedVehicleService(
                _mockVerintServiceGateway.Object,
                _mockMailHelper.Object,
                mockVerintOptions.Object,
                mockConfirmIntegrationEFromOptions.Object);
        }

        [Fact]
        public async Task CreateCase_ShouldReThrowCreateCaseException_CaughtFromVerintGateway()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.GetStreet(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<AddressSearchResult>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new AddressSearchResult
                    {
                        USRN = "test"
                    }
                });

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
                .Setup(_ => _.GetStreet(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<AddressSearchResult>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new AddressSearchResult
                    {
                        USRN = "test"
                    }
                });

            _mockVerintServiceGateway
                .Setup(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()))
                .ReturnsAsync(new HttpResponse<VerintOnlineFormResponse>
                {
                    IsSuccessStatusCode = false
                });

            _ = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_abandonedVehicleReportData));
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.GetStreet(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<AddressSearchResult>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new AddressSearchResult
                    {
                        USRN = "test"
                    }
                });

            _mockVerintServiceGateway
                .Setup(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()))
                .ReturnsAsync(new HttpResponse<VerintOnlineFormResponse>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new VerintOnlineFormResponse
                    {
                        VerintCaseReference = "test"
                    }
                });

            var result = await _service.CreateCase(_abandonedVehicleReportData);

            Assert.Contains("test", result);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCRMCase()
        {
            VerintOnlineFormRequest crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.GetStreet(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<AddressSearchResult>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new AddressSearchResult
                    {
                        USRN = "test"
                    }
                });

            _mockVerintServiceGateway
                .Setup(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()))
                .Callback<VerintOnlineFormRequest>(_ => crmCaseParameter = _)
                .ReturnsAsync(new HttpResponse<VerintOnlineFormResponse>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = new VerintOnlineFormResponse
                    {
                        VerintCaseReference = "test"
                    }
                });

            _ = await _service.CreateCase(_abandonedVehicleReportData);

            _mockVerintServiceGateway.Verify(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Contains(_abandonedVehicleReportData.FurtherDetails, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.AbandonedReason, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.ImageOrVideo, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.ImageOrVideo, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleColour, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleMake, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleModel, crmCaseParameter.VerintCase.Description);
            Assert.Contains(_abandonedVehicleReportData.VehicleRegistration, crmCaseParameter.VerintCase.Description);
        }
    }
}
