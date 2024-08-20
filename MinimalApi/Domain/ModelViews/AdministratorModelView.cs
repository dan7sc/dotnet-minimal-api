using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.ModelViews;

public record AdministratorModelView
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string Profile { get; set; }
}