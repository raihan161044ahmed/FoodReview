using FoodReview.Model;
using FoodReview.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodReview.Repository.Interface;

namespace FoodReview.Service.BusinessLogic
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<LocationService> _logger;

        public LocationService(ILocationRepository locationRepository, ILogger<LocationService> logger)
        {
            _locationRepository = locationRepository;
            _logger = logger;
        }

        public async Task<Location?> GetLocationByIdAsync(int id)
        {
            try
            {
                return await _locationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location by ID: {Id}", id);
                return null; 
            }
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            try
            {
                return await _locationRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all locations");
                return new List<Location>();
            }
        }

        public async Task<IEnumerable<Location>> GetLocationsByAreaAsync(string area)
        {
            try
            {
                return await _locationRepository.GetByAreaAsync(area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locations by area: {area}", area);
                return new List<Location>();
            }
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            try
            {
                if (location == null)
                    throw new ArgumentNullException(nameof(location));

                await _locationRepository.AddAsync(location);
                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding location: {LocationName}", location.Name);
                return null;
            }
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            try
            {
                if (location == null)
                    throw new ArgumentNullException(nameof(location));

                await _locationRepository.UpdateAsync(location);
                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location: {LocationId}", location.Id);
                return null;
            }
        }

        public async Task DeleteLocationAsync(int id)
        {
            try
            {
                await _locationRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location: {Id}", id);
            }
        }
    }

}
