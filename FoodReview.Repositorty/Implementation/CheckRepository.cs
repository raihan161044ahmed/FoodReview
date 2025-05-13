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
    public class CheckInRepository : ICheckInRepository
    {
        private readonly string _connectionString;

        public CheckInRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CheckIn?> GetByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<CheckIn>("SELECT * FROM CheckIns WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<CheckIn>> GetAllAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<CheckIn>("SELECT * FROM CheckIns");
            }
        }

        public async Task<IEnumerable<CheckIn>> GetByUserIdAndDateAsync(int userId, DateTime date)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Use a date range to find check-ins on a specific date
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);
                return await db.QueryAsync<CheckIn>(
                    "SELECT * FROM CheckIns WHERE UserId = @UserId AND CheckInTime >= @StartDate AND CheckInTime < @EndDate",
                    new { UserId = userId, StartDate = startDate, EndDate = endDate });
            }
        }


        public async Task AddAsync(CheckIn checkIn)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO CheckIns (UserId, LocationId, CheckInTime) VALUES (@UserId, @LocationId, @CheckInTime); SELECT CAST(SCOPE_IDENTITY() as int)";
                checkIn.Id = await db.ExecuteScalarAsync<int>(sql, checkIn);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("DELETE FROM CheckIns WHERE Id = @Id", new { Id = id });
            }
        }
    }
}
