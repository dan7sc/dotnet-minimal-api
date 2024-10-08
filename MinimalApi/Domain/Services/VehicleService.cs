using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infra.Database;

namespace MinimalApi.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DatabaseContext _context;

    public VehicleService(DatabaseContext context)
    {
        _context = context;
    }

    public List<Vehicle>? All(string? name, string? make, int? page = 1)
    {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"${name}"));
        }

        int itemsPerPage = 10;

        if (page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return query.ToList();
    }

    public void Remove(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public void Insert(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? FindById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}