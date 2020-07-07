﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abandoned_vehicle_service.Models
{
    public class AbandonedVehicleReport
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FurtherDetails { get; set; }
        public string AbandonedReason { get; set; }
        public string ImageOrVideo { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColour { get; set; }
        public string VehicleRegistration { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address StreetAddress { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address CustomersAddress { get; set; }
    }
}