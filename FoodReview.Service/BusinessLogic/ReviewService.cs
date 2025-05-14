using FoodReview.Model;
using FoodReview.Repository.Interface;
using FoodReview.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Service.BusinessLogic
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            try
            {
                return await _reviewRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review by ID: {Id}", id);
                throw; 
            }
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            try
            {
                return await _reviewRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetByLocationIdAsync(int locationId)
        {
            try
            {
                return await _reviewRepository.GetByLocationIdAsync(locationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews by location ID: {LocationId}", locationId);
                throw;
            }
        }

        public async Task AddAsync(Review review)
        {
            try
            {
                await _reviewRepository.AddAsync(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                throw;
            }
        }

        public async Task UpdateAsync(Review review)
        {
            try
            {
                await _reviewRepository.UpdateAsync(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review: {Id}", review.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _reviewRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review: {Id}", id);
                throw;
            }
        }
    }
}
