using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using FoodReview.Service.Interface;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public GoogleAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");

            if (!result.Succeeded)
            {
                return BadRequest("Google authentication failed");
            }

            var user = await _authService.ProcessSocialLoginAsync(result.Principal, "Google");

            // Create application cookie
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Ok(new
            {
                UserId = user.Id,
                user.Email,
                user.Role
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully");
        }
    }
}