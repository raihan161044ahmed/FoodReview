using FoodReview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodReview.Service.Interface
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> RegisterUserAsync(RegisterUserRequest request);
        Task<User?> LoginUserAsync(LoginRequest request);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
