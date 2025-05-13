using FoodReview.Model;
using FoodReview.Repository.Interface;
using FoodReview.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FoodReview.Service.BusinessLogic
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly ILogger<CheckInService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckInService(ICheckInRepository checkInRepository, ILogger<CheckInService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _checkInRepository = checkInRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CheckIn?> GetCheckInByIdAsync(int id)
        {
            try
            {
                return await _checkInRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-in by ID: {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<CheckIn>> GetAllCheckInsAsync()
        {
            try
            {
                return await _checkInRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all check-ins");
                return new List<CheckIn>();
            }
        }

        public async Task<CheckIn> AddCheckInAsync(CheckIn checkIn, int userId)
        {
            try
            {
                if (checkIn == null)
                {
                    throw new ArgumentNullException(nameof(checkIn));
                }

                if (await HasCheckedInTodayAsync(userId))
                {
                    _logger.LogWarning("User {UserId} has already checked in today", userId);
                    throw new Exception("User has already checked in today");
                }
                checkIn.UserId = userId;
                checkIn.CheckInTime = DateTime.UtcNow;
                await _checkInRepository.AddAsync(checkIn);
                return checkIn;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding check-in for UserId: {UserId}", userId);
                return null;
            }
        }

        public async Task DeleteCheckInAsync(int id)
        {
            try
            {
                await _checkInRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting check-in: {Id}", id);
            }
        }

        public async Task<bool> HasCheckedInTodayAsync(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var checkIns = await _checkInRepository.GetByUserIdAndDateAsync(userId, today);
                return checkIns != null && checkIns.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking check-in history for user: {UserId}", userId);
                return false;
            }
        }
    }
}
