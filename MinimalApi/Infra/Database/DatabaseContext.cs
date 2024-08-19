using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infra.Database;


public class DatabaseContext : DbContext
{
    private readonly IConfiguration _appConfigurationSettings;

    public DatabaseContext(IConfiguration appConfigurationSettings)
    {
        _appConfigurationSettings = appConfigurationSettings;
    }

    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator
            {
                Id = 1,
                Email = "administrator@teste.com",
                Password = "12345",
                Profile = "Adm",
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _appConfigurationSettings.GetConnectionString("mysql")?.ToString();
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }
}