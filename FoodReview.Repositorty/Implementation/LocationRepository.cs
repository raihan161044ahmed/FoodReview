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
    public class LocationRepository : ILocationRepository
    {
        private readonly string _connectionString;

        public LocationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Location>("SELECT * FROM Locations WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Location>("SELECT * FROM Locations");
            }
        }
        public async Task<IEnumerable<Location>> GetByAreaAsync(string area)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Location>("SELECT * FROM Locations WHERE Area = @Area", new { Area = area });
            }
        }

        public async Task AddAsync(Location location)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Locations (Name, Area, Latitude, Longitude) VALUES (@Name, @Area, @Latitude, @Longitude); SELECT CAST(SCOPE_IDENTITY() as int)";
                location.Id = await db.ExecuteScalarAsync<int>(sql, location);
            }
        }

        public async Task UpdateAsync(Location location)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Locations SET Name = @Name, Area = @Area, Latitude = @Latitude, Longitude = @Longitude WHERE Id = @Id";
                await db.ExecuteAsync(sql, location);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("DELETE FROM Locations WHERE Id = @Id", new { Id = id });
            }
        }
    }

}
