using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infra.Database;

namespace MinimalApi.Domain.Services;

public class AdministratorService : IAdministratorService
{
    private readonly DatabaseContext _context;

    public AdministratorService(DatabaseContext context)
    {
        _context = context;
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return _context.Administrators
            .Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password)
            .FirstOrDefault();
    }
}