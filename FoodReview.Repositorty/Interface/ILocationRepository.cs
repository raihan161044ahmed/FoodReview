using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Repository.Interface
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int id);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<Location>> GetByAreaAsync(string area);
        Task AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(int id);
    }
}
