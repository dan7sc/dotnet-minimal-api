using MinimalApi.Domain.Entities;

namespace MinimalApiTest.Domain.Entities;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var administrator = new Administrator
        {
            // Act
            Id = 1,
            Email = "test@test.com",
            Password = "1234",
            Profile = "Adm",
        };

        // Assert
        Assert.AreEqual(1, administrator.Id);
        Assert.AreEqual("test@test.com", administrator.Email);
        Assert.AreEqual("1234", administrator.Password);
        Assert.AreEqual("Adm", administrator.Profile);
    }
}