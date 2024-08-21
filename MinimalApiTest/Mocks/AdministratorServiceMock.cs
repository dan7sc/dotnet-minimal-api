using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace MinimalApiTest.Mocks;

public class AdministratorServiceMock : IAdministratorService
{
    private static List<Administrator> administrators = new List<Administrator>() {
        new Administrator {
            Id = 1,
            Email = "adm@test.com",
            Password = "1234",
            Profile = "Adm"
        },
        new Administrator {
            Id = 2,
            Email = "editor@test.com",
            Password = "1234",
            Profile = "Editor"
        },
    };

    public List<Administrator> All(int? page)
    {
        return administrators;
    }

    public Administrator? FindById(int id)
    {
        return administrators.Find(a => a.Id == id);
    }

    public Administrator? Insert(Administrator administrator)
    {
        administrator.Id = administrators.Count() + 1;
        administrators.Add(administrator);

        return administrator;
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }
}