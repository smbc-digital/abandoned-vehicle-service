using System;
using System.Threading.Tasks;
using abandoned_vehicle_service.Models;
using abandoned_vehicle_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;

namespace abandoned_vehicle_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAbandonedVehicleService _abandonedVehicleService;

        public HomeController(ILogger<HomeController> logger,
                              IAbandonedVehicleService abandonedVehicleService)
        {
            _logger = logger;
            _abandonedVehicleService = abandonedVehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AbandonedVehicleReport abandonedVehicleReport)
        {
            if (!ModelState.IsValid)
                throw new ArgumentException("Invalid request parameters.");

            return Ok(await _abandonedVehicleService.CreateCase(abandonedVehicleReport));
        }
    }
}