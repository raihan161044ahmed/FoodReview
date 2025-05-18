using FoodReview.Model;
using FoodReview.Repository.Interface;
using FoodReview.Service.Interface;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodReview.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISocialUserRepository _socialUserRepository;

        public AuthService(
            IUserRepository userRepository,
            ISocialUserRepository socialUserRepository)
        {
            _userRepository = userRepository;
            _socialUserRepository = socialUserRepository;
        }

        public async Task<User> ProcessSocialLoginAsync(ClaimsPrincipal principal, string provider)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name);
            var providerUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var picture = principal.FindFirstValue("urn:google:picture");

            // Check if social user exists
            var socialUser = await _socialUserRepository.GetByProviderUserIdAsync(provider, providerUserId);

            if (socialUser != null)
            {
                return await _userRepository.GetByIdAsync(socialUser.UserId);
            }

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                // Create new user
                user = new User
                {
                    Email = email,
                    Role = "User",
                    PasswordHash = null
                };
                await _userRepository.AddAsync(user);
            }

            // Create social user record
            var newSocialUser = new SocialUser
            {
                Provider = provider,
                ProviderUserId = providerUserId,
                Email = email,
                Name = name,
                ProfilePictureUrl = picture,
                UserId = user.Id
            };

            await _socialUserRepository.AddAsync(newSocialUser);

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}