using FoodReview.Model;
using System;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using FoodReview.Repository.Interface;

namespace FoodReview.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<User>("SELECT * FROM Users");
            }
        }

        public async Task AddAsync(User user)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Users (Email, PasswordHash, Role) VALUES (@Email, @PasswordHash, @Role); SELECT CAST(SCOPE_IDENTITY() as int)";
                user.Id = await db.ExecuteScalarAsync<int>(sql, user);
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Users SET Email = @Email, PasswordHash = @PasswordHash, Role = @Role WHERE Id = @Id";
                await db.ExecuteAsync(sql, user);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
            }
        }
    }
}
