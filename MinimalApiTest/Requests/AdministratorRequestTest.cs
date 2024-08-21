using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ModelViews;
using MinimalApiTest.Helpers;

namespace MinimalApiTest.Requests;

[TestClass]
public class AdministratorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@test.com",
            Password = "1234",
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administrators/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdm = JsonSerializer.Deserialize<LoggedAdministrator>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdm?.Email ?? "");
        Assert.IsNotNull(loggedAdm?.Profile ?? "");
        Assert.IsNotNull(loggedAdm?.Token ?? "");
    }
}