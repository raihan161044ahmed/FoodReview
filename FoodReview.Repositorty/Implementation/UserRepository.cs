using FoodReview.Model;
using FoodReview.Repositorty.Interface;
using System;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;

namespace FoodReview.Repositorty.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            }
        }

        public async Task<User> AddAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"
                    INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PasswordSalt, Role, Verified, VerificationToken, CreatedAt)
                    VALUES (@FirstName, @LastName, @Email, @PasswordHash, @PasswordSalt, @Role, @Verified, @VerificationToken, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = await connection.QuerySingleAsync<int>(sql, user);
                user.Id = id; // Set the ID after insertion
                return user;
            }
        }

        public async Task<User> UpdateAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"
                    UPDATE Users
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        PasswordHash = @PasswordHash,
                        PasswordSalt = @PasswordSalt,
                        Role = @Role,
Verified = @Verified,
                        VerificationToken = @VerificationToken,
                        PasswordResetToken = @PasswordResetToken,
                        ResetTokenExpiresAt = @ResetTokenExpiresAt,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
                await connection.ExecuteAsync(sql, user);
                return user;
            }
        }

        public async Task<User> GetUserByVerificationTokenAsync(string token)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE VerificationToken = @Token", new { Token = token });
            }
        }

        public async Task<User> GetUserByResetTokenAsync(string token)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE PasswordResetToken = @Token", new { Token = token });
            }
        }
    }
}
