using abandoned_vehicle_service.Helpers;
using abandoned_vehicle_service.Models;
using Microsoft.Extensions.Configuration;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Text;
using System.Threading.Tasks;

namespace abandoned_vehicle_service.Services
{
    public class AbandonedVehicleService : IAbandonedVehicleService
    {
        private readonly IVerintServiceGateway _verintServiceGateway;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AbandonedVehicleService(IVerintServiceGateway verintServiceGateway,
                                       IConfiguration configuration,
                                       IMailHelper mailHelper)
        {
            _verintServiceGateway = verintServiceGateway;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        public async Task<string> CreateCase(AbandonedVehicleReport abandonedVehicleReport)
        {
            Case crmCase = CreateCrmCaseObject(abandonedVehicleReport);

            try
            {
                StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string> response = await _verintServiceGateway.CreateCase(crmCase);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Status code not successful");
                }

                Person person = new Person
                {
                    FirstName = abandonedVehicleReport.FirstName,
                    LastName = abandonedVehicleReport.LastName,
                    Email = abandonedVehicleReport.Email,
                    Phone = abandonedVehicleReport.Phone,
                };

                _mailHelper.SendEmail(person, EMailTemplate.AbandonedVehicleReport, response.ResponseContent, abandonedVehicleReport.StreetAddress);
                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateAbandonedVehicleService an exception has occured while creating the case in verint service", ex);
            }
        }

        private Case CreateCrmCaseObject(AbandonedVehicleReport abandonedVehicleReport)
        {
            Case crmCase = new Case
            {
                EventCode = Int32.Parse(_configuration.GetSection("CrmCaseSettings").GetSection("EventCode").Value),
                EventTitle = _configuration.GetSection("CrmCaseSettings").GetSection("EventTitle").Value,
                Classification = _configuration.GetSection("CrmCaseSettings").GetSection("Classification").Value,
                Description = GenerateDescription(abandonedVehicleReport),
                Street = new Street
                {
                    Reference = abandonedVehicleReport.StreetAddress?.PlaceRef
                }
            };

            if (!string.IsNullOrEmpty(abandonedVehicleReport.FirstName) && !string.IsNullOrEmpty(abandonedVehicleReport.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = abandonedVehicleReport.FirstName,
                    Surname = abandonedVehicleReport.LastName
                };

                if (!string.IsNullOrEmpty(abandonedVehicleReport.Email))
                {
                    crmCase.Customer.Email = abandonedVehicleReport.Email;
                }

                if (!string.IsNullOrEmpty(abandonedVehicleReport.Phone))
                {
                    crmCase.Customer.Telephone = abandonedVehicleReport.Phone;
                }

                if (string.IsNullOrEmpty(abandonedVehicleReport.CustomersAddress.PlaceRef))
                {
                    crmCase.Customer.Address = new Address
                    {
                        AddressLine1 = abandonedVehicleReport.CustomersAddress.AddressLine1,
                        AddressLine2 = abandonedVehicleReport.CustomersAddress.AddressLine2,
                        AddressLine3 = abandonedVehicleReport.CustomersAddress.Town,
                        Postcode = abandonedVehicleReport.CustomersAddress.Postcode,
                    };
                }
                else
                {
                    crmCase.Customer.Address = new Address
                    {
                        Reference = abandonedVehicleReport.CustomersAddress.PlaceRef,
                        UPRN = abandonedVehicleReport.CustomersAddress.PlaceRef
                    };
                }
            }

            return crmCase;
        }

        private string GenerateDescription(AbandonedVehicleReport abandonedVehicleReport)
        {
            StringBuilder description = new StringBuilder();

            if (!string.IsNullOrEmpty(abandonedVehicleReport.FurtherDetails))
            {
                description.Append($"Further details: {abandonedVehicleReport.FurtherDetails}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.AbandonedReason))
            {
                description.Append($"Abandoned Reason: {abandonedVehicleReport.AbandonedReason}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.ImageOrVideo))
            {
                description.Append($"Image Or Video: {abandonedVehicleReport.ImageOrVideo}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleMake))
            {
                description.Append($"Vehicle Make: {abandonedVehicleReport.VehicleMake}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleModel))
            {
                description.Append($"Vehicle Model: {abandonedVehicleReport.VehicleModel}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleColour))
            {
                description.Append($"Vehicle Colour: {abandonedVehicleReport.VehicleColour}");
                description.Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleRegistration))
            {
                description.Append($"Vehicle Registration: {abandonedVehicleReport.VehicleRegistration}");
                description.Append(Environment.NewLine);
            }
            return description.ToString();

        }
    }
}
