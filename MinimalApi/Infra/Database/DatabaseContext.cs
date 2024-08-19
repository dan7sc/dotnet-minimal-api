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