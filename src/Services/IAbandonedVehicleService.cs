using abandoned_vehicle_service.Models;
using System.Threading.Tasks;

namespace abandoned_vehicle_service.Services
{
    public interface IAbandonedVehicleService
    {
        Task<string> CreateCase(AbandonedVehicleReport formData);
    }
}
