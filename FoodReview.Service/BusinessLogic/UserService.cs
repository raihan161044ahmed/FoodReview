using FoodReview.Model;
using FoodReview.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FoodReview.Repository.Interface;


namespace FoodReview.Service.BusinessLogic
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return null; 
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return null;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterUserRequest request)
        {
            if (request == null)
            {
                _logger.LogError("RegisterUserAsync: Request is null");
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogError("RegisterUserAsync: Email or Password is empty");
                return false;
            }

            if (request.Password != request.ConfirmPassword)
            {
                _logger.LogError("RegisterUserAsync: Passwords do not match");
                return false;
            }

            try
            {
                // Check if the user already exists
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("RegisterUserAsync: Email already exists: {Email}", request.Email);
                    return false;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    Email = request.Email,
                    PasswordHash = System.Text.Encoding.UTF8.GetBytes(passwordHash), 
                    Role = "User" 
                };

                await _userRepository.AddAsync(newUser);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", request.Email);
                return false;
            }
        }

        public async Task<User?> LoginUserAsync(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogError("LoginUserAsync: Request is null or Email/Password is empty");
                return null;
            }

            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("LoginUserAsync: User not found: {Email}", request.Email);
                    return null;
                }

                string storedHash = System.Text.Encoding.UTF8.GetString(user.PasswordHash);
                if (!BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
                {
                    _logger.LogWarning("LoginUserAsync: Invalid password for user: {Email}", request.Email);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user: {Email}", request.Email);
                return null;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", user.Id);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
            }
        }
    }
}
