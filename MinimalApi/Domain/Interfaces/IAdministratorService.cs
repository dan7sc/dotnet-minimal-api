using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministratorService
{
    public Administrator? Login(LoginDTO loginDTO);
}