using System.Threading.Tasks;
using abandoned_vehicle_service.Models;

namespace abandoned_vehicle_service.Services
{
    public interface IAbandonedVehicleService
    {
        Task<string> CreateCase(AbandonedVehicleReport formData);
    }
}
