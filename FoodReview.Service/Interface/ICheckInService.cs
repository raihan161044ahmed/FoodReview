using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Service.Interface
{
    public interface ICheckInService
    {
        Task<CheckIn?> GetCheckInByIdAsync(int id);
        Task<IEnumerable<CheckIn>> GetAllCheckInsAsync();
        Task<CheckIn> AddCheckInAsync(CheckIn checkIn, int userId);
        Task DeleteCheckInAsync(int id);
        Task<bool> HasCheckedInTodayAsync(int userId);
    }
}
