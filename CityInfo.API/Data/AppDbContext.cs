using Microsoft.EntityFrameworkCore;

using CityInfo.API.Models;
using CityInfo.API.Data.Entities;

namespace CityInfo.API.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public DbSet<City> Cities { get; set; }
    public DbSet<PointOfInterest> PointOfInterests { get; set; }

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string connectionString = _configuration["ConnectionStrings:Default"] ?? string.Empty;
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}