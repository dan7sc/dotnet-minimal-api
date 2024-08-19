using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IVehicleService
{
    public List<Vehicle>? All(int page, string? name = null, string? make = null);
    public Vehicle? SearchById(int id);
    public void Insert(Vehicle vehicle);
    public void Update(Vehicle vehicle);
    public void Remove(Vehicle vehicle);
}