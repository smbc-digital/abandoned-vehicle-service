using System;
using System.Threading.Tasks;
using abandoned_vehicle_service.Helpers;
using abandoned_vehicle_service.Mappers;
using abandoned_vehicle_service.Models;
using Microsoft.Extensions.Options;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationEFromExtensions;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Enums;

namespace abandoned_vehicle_service.Services
{
    public class AbandonedVehicleService : IAbandonedVehicleService
    {
        private readonly IVerintServiceGateway _verintServiceGateway;
        private readonly IMailHelper _mailHelper;
        private readonly VerintOptions _verintOptions;
        private readonly ConfirmIntegrationEFormOptions _VOFConfiguration;

        public AbandonedVehicleService(IVerintServiceGateway verintServiceGateway,
                                       IMailHelper mailHelper,
                                       IOptions<VerintOptions> verintOptions,
                                       IOptions<ConfirmIntegrationEFormOptions> VOFConfiguration)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailHelper = mailHelper;
            _verintOptions = verintOptions.Value;
            _VOFConfiguration = VOFConfiguration.Value;
        }

        public async Task<string> CreateCase(AbandonedVehicleReport abandonedVehicleReport)
        {
            var crmCase = abandonedVehicleReport
                .ToCase(_VOFConfiguration, _verintOptions)
                .ToConfirmIntegrationEFormCase(_VOFConfiguration);

            try
            {
                var response = await _verintServiceGateway.CreateVerintOnlineFormCase(crmCase);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("AbandonedVehicleService.CreateCase: Status code not successful");

                var person = new Person
                {
                    FirstName = abandonedVehicleReport.FirstName,
                    LastName = abandonedVehicleReport.LastName,
                    Email = abandonedVehicleReport.Email,
                    Phone = abandonedVehicleReport.Phone,
                };

                _mailHelper.SendEmail(
                    person,
                    EMailTemplate.AbandonedVehicleReport,
                    response.ResponseContent.VerintCaseReference,
                    abandonedVehicleReport.StreetAddress);

                return response.ResponseContent.VerintCaseReference;
            }
            catch (Exception ex)
            {
                throw new Exception($"AbandonedVehicleService.CreateCase: CRMService CreateAbandonedVehicleService an exception has occured while creating the case in verint service", ex);
            }
        }
    }
}
