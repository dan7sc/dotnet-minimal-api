using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.Entities;

public class AdministratorDTO
{
    public int Id { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required Profile? Profile { get; set; }
}