using FoodReview.Model;
using FoodReview.Repository.Interface;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FoodReview.Repository.Implementation
{
    public class SocialUserRepository : ISocialUserRepository
    {
        private readonly string _connectionString;

        public SocialUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<SocialUser?> GetByProviderUserIdAsync(string provider, string providerUserId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<SocialUser>(
                    "SELECT * FROM SocialUsers WHERE Provider = @Provider AND ProviderUserId = @ProviderUserId",
                    new { Provider = provider, ProviderUserId = providerUserId });
            }
        }

        public async Task<SocialUser?> GetByUserIdAsync(int userId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<SocialUser>(
                    "SELECT * FROM SocialUsers WHERE UserId = @UserId",
                    new { UserId = userId });
            }
        }

        public async Task AddAsync(SocialUser socialUser)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO SocialUsers 
                          (Provider, ProviderUserId, Email, Name, ProfilePictureUrl, UserId) 
                          VALUES 
                          (@Provider, @ProviderUserId, @Email, @Name, @ProfilePictureUrl, @UserId);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                socialUser.Id = await db.ExecuteScalarAsync<int>(sql, socialUser);
            }
        }

        public async Task UpdateAsync(SocialUser socialUser)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE SocialUsers SET 
                          Email = @Email, 
                          Name = @Name, 
                          ProfilePictureUrl = @ProfilePictureUrl
                          WHERE Id = @Id";
                await db.ExecuteAsync(sql, socialUser);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("DELETE FROM SocialUsers WHERE Id = @Id", new { Id = id });
            }
        }
    }
}