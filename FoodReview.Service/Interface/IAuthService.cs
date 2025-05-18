using FoodReview.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodReview.Service.Interface
{
    public interface IAuthService
    {
        Task<User> ProcessSocialLoginAsync(ClaimsPrincipal principal, string provider);
        Task<User?> GetUserByIdAsync(int id);
    }
}