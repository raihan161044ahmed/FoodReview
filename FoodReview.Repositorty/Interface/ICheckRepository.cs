using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Repository.Interface
{
    public interface ICheckInRepository
    {
        Task<CheckIn?> GetByIdAsync(int id);
        Task<IEnumerable<CheckIn>> GetAllAsync();
        Task<IEnumerable<CheckIn>> GetByUserIdAndDateAsync(int userId, DateTime date);
        Task AddAsync(CheckIn checkIn);
        Task DeleteAsync(int id);
    }
}
