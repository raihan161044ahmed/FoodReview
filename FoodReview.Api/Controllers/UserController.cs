using FoodReview.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodReview.Service.Interface;

namespace FoodReview.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool result = await _userService.RegisterUserAsync(request);
                if (result)
                {
                    return Ok(new { message = "User registered successfully." });
                }
                else
                {
                    return BadRequest(new { message = "User registration failed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.LoginUserAsync(request);
                if (user != null)
                {
                    //  Return a safe DTO, not the entire User object.
                    var response = new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = user.Role,
                        Message = "Login successful"
                    };
                    return Ok(response);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPost("logout")]
        [Authorize] 
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet("profile")]
        [Authorize] 
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID." });
                }

                var user = await _userService.GetUserByIdAsync(userId); 
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var profile = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    // Add other safe properties
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }
    }
}
