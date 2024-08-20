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

    public List<Administrator> All(int? page)
    {
        var query = _context.Administrators.AsQueryable();

        int itemsPerPage = 10;

        if (page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return query.ToList();
    }

    public Administrator? Insert(Administrator administrator)
    {
        _context.Administrators.Add(administrator);
        _context.SaveChanges();

        return administrator;
    }

    public Administrator? FindById(int id)
    {
        return _context.Administrators.Where(a => a.Id == id).FirstOrDefault();
    }
}