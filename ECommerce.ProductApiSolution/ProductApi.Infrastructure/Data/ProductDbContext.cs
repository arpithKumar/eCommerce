using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}

// dotnet ef migrations add InitialCreate -o ../ProductApi.Infrastructure/Data/migrations --project ../ProductApi.Infrastructure
// dotnet ef database update