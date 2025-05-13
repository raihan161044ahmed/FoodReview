using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Repository.Interface
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<IEnumerable<Review>> GetByLocationIdAsync(int locationId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
    }
}
