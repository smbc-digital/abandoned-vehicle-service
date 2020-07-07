using abandoned_vehicle_service.Models;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Models.ComplimentsComplaints;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Mail;

namespace abandoned_vehicle_service.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IMailingServiceGateway _mailingServiceGateway;

        public MailHelper(IMailingServiceGateway mailingServiceGateway)
        {
            _mailingServiceGateway = mailingServiceGateway;
        }

        public void SendEmail(Person person, EMailTemplate template, string caseReference)
        {
            ComplaintsMailModel submissionDetails = new ComplaintsMailModel
            {
                Subject = "Abandoned Vehicle Report - submission",
                Reference = caseReference,
                FirstName = person.FirstName,
                LastName = person.LastName,
                RecipientAddress = person.Email
            };

            _mailingServiceGateway.Send(new Mail
            {
                Payload = JsonConvert.SerializeObject(submissionDetails),
                Template = template
            });
        }
    }
}