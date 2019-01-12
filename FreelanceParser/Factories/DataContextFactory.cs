using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FreelanceParser.Data
{
    public class DataContextFactory : IDataContextFactory
    {
        private DbContextOptions<FreelanceContext> GetConnectionOptions()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionBuilder = new DbContextOptionsBuilder<FreelanceContext>();
            var options = optionBuilder
                .UseSqlServer(connectionString)
                .Options;

            return options;
        }

        FreelanceContext IDataContextFactory.Create()
        {
            var options = GetConnectionOptions();

            return new FreelanceContext(options);
        }
    }
}
