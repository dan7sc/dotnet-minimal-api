using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infra.Database;

namespace MinimalApiTest.Domain.Services;

[TestClass]
public class AdministratorServiceTest
{
    private DatabaseContext CreateTestContext()
    {
        var asssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(asssemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DatabaseContext(configuration);
    }

    [TestMethod]
    public void TestSaveAdministrator()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var administrator = new Administrator
        {
            Id = 1,
            Email = "test@test.com",
            Password = "1234",
            Profile = "Adm",
        };
        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Insert(administrator);

        // Assert
        Assert.AreEqual(1, administratorService.All(1).Count());
    }

    [TestMethod]
    public void TestFindById()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var administrator = new Administrator
        {
            Id = 1,
            Email = "test@test.com",
            Password = "1234",
            Profile = "Adm",
        };
        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Insert(administrator);
        var foundAdministrator = administratorService.FindById(administrator.Id);

        // Assert
        Assert.AreEqual(1, foundAdministrator?.Id);
    }
}