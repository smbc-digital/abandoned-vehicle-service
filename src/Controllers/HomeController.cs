using abandoned_vehicle_service.Models;
using abandoned_vehicle_service.Services;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.AspNetCore.Availability.Managers;
using System.Threading.Tasks;

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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AbandonedVehicleReport abandonedVehicleReport)
            => Ok(await _abandonedVehicleService.CreateCase(abandonedVehicleReport));
    }
}