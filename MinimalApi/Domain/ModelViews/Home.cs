namespace MinimalApi.Domain.ModelViews;

public struct Home
{
    public readonly string Message { get => "Welcome to Vehicle API"; }

    public readonly string Doc { get => "/swagger"; }
}