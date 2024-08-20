using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.ModelViews;

public record LoggedAdministrator
{
    public required string Email { get; set; }
    public required string Profile { get; set; }
    public required string Token { get; set; }
}