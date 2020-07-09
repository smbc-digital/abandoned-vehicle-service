using abandoned_vehicle_service.Models;
using StockportGovUK.NetStandard.Models.Enums;

namespace abandoned_vehicle_service.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(Person person, EMailTemplate template, string caseReference, StockportGovUK.NetStandard.Models.Addresses.Address street);
    }
}
