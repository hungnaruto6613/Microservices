using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.ExecuteAsync(
                    "INSERT INTO coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)", 
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });
                if (result == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.ExecuteAsync(
                    "DELETE coupon WHERE ProductName=@ProductName",
                    new { ProductName = productName });
                if (result == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using(var connection = new NpgsqlConnection(_connectionString))
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                    "SELECT * FROM coupon WHERE ProductName = @ProductName", new { ProductName = productName });
                if(coupon == null)
                {
                    return new Coupon
                    {
                        ProductName = "No Discount",
                        Amount = 0,
                        Description = "No Discount Desciption"
                    };
                }
                return coupon;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.ExecuteAsync(
                    "UPDATE coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });
                if (result == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
