using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.Modules.Catalog.Infrastructure.Persistence;

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING") 
                               ?? "Server=localhost;Database=eShopDB;User Id=sa;Password=Password123!;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new CatalogDbContext(optionsBuilder.Options);
    }
}