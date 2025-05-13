using FoodReview.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using FoodReview.Repository.Interface;

namespace FoodReview.Repository.Implementation
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;

        public ReviewRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Review>("SELECT * FROM Reviews WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Review>("SELECT * FROM Reviews");
            }
        }

        public async Task<IEnumerable<Review>> GetByLocationIdAsync(int locationId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Review>("SELECT * FROM Reviews WHERE LocationId = @LocationId", new { LocationId = locationId });
            }
        }

        public async Task AddAsync(Review review)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Reviews (UserId, LocationId, Rating, Comment, CreatedAt) VALUES (@UserId, @LocationId, @Rating, @Comment, @CreatedAt); SELECT CAST(SCOPE_IDENTITY() as int)";
                review.Id = await db.ExecuteScalarAsync<int>(sql, review);
            }
        }

        public async Task UpdateAsync(Review review)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Reviews SET UserId = @UserId, LocationId = @LocationId, Rating = @Rating, Comment = @Comment, CreatedAt = @CreatedAt WHERE Id = @Id";
                await db.ExecuteAsync(sql, review);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("DELETE FROM Reviews WHERE Id = @Id", new { Id = id });
            }
        }
    }
}
