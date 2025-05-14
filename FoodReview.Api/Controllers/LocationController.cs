using FoodReview.Model;
using FoodReview.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReview.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all locations");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            try
            {
                var location = await _locationService.GetLocationByIdAsync(id);
                if (location == null)
                {
                    return NotFound(new { message = "Location not found." });
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("area/{area}")]
        public async Task<IActionResult> GetLocationsByArea(string area)
        {
            try
            {
                var locations = await _locationService.GetLocationsByAreaAsync(area);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locations by area");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")] // Only administrators can add locations
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _locationService.AddLocationAsync(location);
                return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding location");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")] // Only administrators can update locations
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            try
            {
                var existingLocation = await _locationService.GetLocationByIdAsync(id);
                if (existingLocation == null)
                {
                    return NotFound(new { message = "Location not found." });
                }

                await _locationService.UpdateLocationAsync(location);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")] 
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                var location = await _locationService.GetLocationByIdAsync(id);
                if (location == null)
                {
                    return NotFound(new { message = "Location not found." });
                }

                await _locationService.DeleteLocationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }
    }
}
