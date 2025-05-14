using FoodReview.Model;
using FoodReview.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodReview.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;
        private readonly ILogger<CheckInController> _logger;
        private readonly IUserService _userService;

        public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger, IUserService userService)
        {
            _checkInService = checkInService;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCheckIns()
        {
            try
            {
                var checkIns = await _checkInService.GetAllCheckInsAsync();
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all check-ins");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckInById(int id)
        {
            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                if (checkIn == null)
                {
                    return NotFound(new { message = "Check-in not found." });
                }
                return Ok(checkIn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-in by ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddCheckIn([FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid User" });
            }

            checkIn.UserId = userId;
            checkIn.CheckInTime = DateTime.Now; 

            try
            {
                await _checkInService.AddCheckInAsync(checkIn, userId);
                return CreatedAtAction(nameof(GetCheckInById), new { id = checkIn.Id }, checkIn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding check-in");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> DeleteCheckIn(int id)
        {
            try
            {
                var existingCheckIn = await _checkInService.GetCheckInByIdAsync(id);
                if (existingCheckIn == null)
                {
                    return NotFound(new { message = "Check-in not found." });
                }

                // Get the user's ID from the claims.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID." });
                }
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid User" });
                }
                // Only allow the user who created the checkin to delete it
                if (existingCheckIn.UserId != userId)
                {
                    return Forbid("You are not authorized to delete this check-in.");
                }
                await _checkInService.DeleteCheckInAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting check-in: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }
    }
}
