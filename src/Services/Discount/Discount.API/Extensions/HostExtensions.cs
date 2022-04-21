using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var configuration = service.GetRequiredService<IConfiguration>();
                var logger = service.GetRequiredService<ILogger<TContext>>();
                try
                {
                    using (var connection = new NpgsqlConnection(
                        configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            command.ExecuteNonQuery();
                            command.CommandText = @"CREATE TABLE Coupon(
                                    Id SERIAL PRIMARY KEY,
                                    ProductName VARCHAR(24) NOT NULL,
                                    Description TEXT,
                                    Amount INT
                                )";
                            command.ExecuteNonQuery();
                            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Iphone 11', 'New Iphone Product', 100)";
                            command.ExecuteNonQuery();
                        }
                        logger.LogInformation("Migrate postgres database");
                    }
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, ex.Message);
                    if(retryForAvailability > 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
                return host;
            }
        }
    }
}
