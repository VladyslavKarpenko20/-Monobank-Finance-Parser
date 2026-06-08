using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;


namespace Parsing.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<AddDbContext>
    {
        public AddDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            var optionsBuilde = new DbContextOptionsBuilder<AddDbContext>();

            optionsBuilde.UseNpgsql(configuration["Database:ConnectionString"], optionsSql => optionsSql.EnableRetryOnFailure());

            return new AddDbContext(optionsBuilde.Options);
        }
    }
}
