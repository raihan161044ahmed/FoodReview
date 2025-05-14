using FoodReview.Model;
using FoodReview.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodReview.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;
        private readonly IUserService _userService; //to get user

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger, IUserService userService)
        {
            _reviewService = reviewService;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);
                if (review == null)
                {
                    return NotFound(new { message = "Review not found." });
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetReviewsByLocationId(int locationId)
        {
            try
            {
                var reviews = await _reviewService.GetByLocationIdAsync(locationId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews by location ID");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize] // Only authenticated users can add reviews
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                return Unauthorized(new { message = "Invalid user." });
            }
            review.UserId = userId;
            review.CreatedAt = DateTime.Now;

            try
            {
                await _reviewService.AddAsync(review);
                return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [Authorize] // Only authenticated users can update reviews
        public async Task<IActionResult> UpdateReview(int id, [FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != review.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            try
            {
                var existingReview = await _reviewService.GetByIdAsync(id);
                if (existingReview == null)
                {
                    return NotFound(new { message = "Review not found." });
                }
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID." });
                }
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid user." });
                }
                // Only allow the user who created the review to update it
                if (existingReview.UserId != userId)
                {
                    return Forbid("You are not authorized to update this review.");
                }

                review.UserId = userId; 
                review.CreatedAt = existingReview.CreatedAt;  
                await _reviewService.UpdateAsync(review);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize] 
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var existingReview = await _reviewService.GetByIdAsync(id);
                if (existingReview == null)
                {
                    return NotFound(new { message = "Review not found." });
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
                    return Unauthorized(new { message = "Invalid user." });
                }
                if (existingReview.UserId != userId)
                {
                    return Forbid("You are not authorized to delete this review.");
                }

                await _reviewService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }
    }
}
