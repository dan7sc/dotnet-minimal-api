using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministratorService
{
    public Administrator? Login(LoginDTO loginDTO);
    public Administrator? Insert(Administrator administrator);
    public Administrator? FindById(int id);
    public List<Administrator> All(int? page);
}