using System.ComponentModel.DataAnnotations;
using StockportGovUK.NetStandard.Models.Addresses;

namespace abandoned_vehicle_service.Models
{
    public class AbandonedVehicleReport
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string FurtherDetails { get; set; }
        [Required]
        public string AbandonedReason { get; set; }
        [Required]
        public string ImageOrVideo { get; set; }
        [Required]
        public Address StreetAddress { get; set; }
        [Required]
        public Address CustomersAddress { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColour { get; set; }
        public string VehicleRegistration { get; set; }
        public string Email { get; set; }
    }
}
