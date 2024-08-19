namespace MinimalApi.Domain.DTOs;

public class VehicleDTO
{
    public required string Name { get; set; }
    public required string Make { get; set; }
    public required int Year { get; set; }
}