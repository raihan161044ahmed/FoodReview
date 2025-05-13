using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Service.Interface
{
    public interface ILocationService
    {
        Task<Location?> GetLocationByIdAsync(int id);
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<IEnumerable<Location>> GetLocationsByAreaAsync(string area);
        Task<Location> AddLocationAsync(Location location);
        Task<Location> UpdateLocationAsync(Location location);
        Task DeleteLocationAsync(int id);
    }
}
