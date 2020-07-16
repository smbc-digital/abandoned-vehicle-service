using System;
using System.Text;
using abandoned_vehicle_service.Models;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationEFromExtensions;
using StockportGovUK.NetStandard.Models.Verint;

namespace abandoned_vehicle_service.Mappers
{
    public static class AbandonedVehicleReportMapper
    {
        public static Case ToCase(this AbandonedVehicleReport model,
            ConfirmIntegrationEFormOptions _VOFConfiguration,
            VerintOptions _verintOptions)
        {
            Case crmCase = new Case
            {
                EventCode = _VOFConfiguration.EventId,
                EventTitle = _verintOptions.EventTitle,
                Classification = _verintOptions.Classification,
                FurtherLocationInformation = model.FurtherDetails,
                Description = GenerateDescription(model)
            };

            if (!string.IsNullOrWhiteSpace(model.StreetAddress?.PlaceRef))
                crmCase.AssociatedWithBehaviour = AssociatedWithBehaviourEnum.Street;
                crmCase.Street = new Street
                {
                    Reference = model.StreetAddress.PlaceRef,
                    Description = string.Empty
                };

            if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = model.FirstName,
                    Surname = model.LastName
                };

                if (!string.IsNullOrEmpty(model.Email))
                {
                    crmCase.Customer.Email = model.Email;
                }

                if (!string.IsNullOrEmpty(model.Phone))
                {
                    crmCase.Customer.Telephone = model.Phone;
                }

                if (string.IsNullOrEmpty(model.CustomersAddress.PlaceRef))
                {
                    crmCase.Customer.Address = new Address
                    {
                        AddressLine1 = model.CustomersAddress.AddressLine1,
                        AddressLine2 = model.CustomersAddress.AddressLine2,
                        AddressLine3 = model.CustomersAddress.Town,
                        Postcode = model.CustomersAddress.Postcode,
                    };
                }
                else
                {
                    crmCase.Customer.Address = new Address
                    {
                        Reference = model.CustomersAddress.PlaceRef,
                        UPRN = model.CustomersAddress.PlaceRef
                    };
                }
            }

            return crmCase;
        }

        private static string GenerateDescription(AbandonedVehicleReport abandonedVehicleReport)
        {
            StringBuilder description = new StringBuilder();

            if (!string.IsNullOrEmpty(abandonedVehicleReport.FurtherDetails))
                description.Append($"Further details: {abandonedVehicleReport.FurtherDetails}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.AbandonedReason))
                description.Append($"Abandoned Reason: {abandonedVehicleReport.AbandonedReason}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.ImageOrVideo))
                description.Append($"Image Or Video: {abandonedVehicleReport.ImageOrVideo}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleMake))
                description.Append($"Vehicle Make: {abandonedVehicleReport.VehicleMake}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleModel))
                description.Append($"Vehicle Model: {abandonedVehicleReport.VehicleModel}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleColour))
                description.Append($"Vehicle Colour: {abandonedVehicleReport.VehicleColour}{Environment.NewLine}");

            if (!string.IsNullOrEmpty(abandonedVehicleReport.VehicleRegistration))
                description.Append($"Vehicle Registration: {abandonedVehicleReport.VehicleRegistration}{Environment.NewLine}");

            return description.ToString();
        }
    }
}
