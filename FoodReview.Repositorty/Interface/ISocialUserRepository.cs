using FoodReview.Model;
using System.Threading.Tasks;

namespace FoodReview.Repository.Interface
{
    public interface ISocialUserRepository
    {
        Task<SocialUser?> GetByProviderUserIdAsync(string provider, string providerUserId);
        Task<SocialUser?> GetByUserIdAsync(int userId);
        Task AddAsync(SocialUser socialUser);
        Task UpdateAsync(SocialUser socialUser);
        Task DeleteAsync(int id);
    }
}