namespace stations.Models;

public record AppConfiguration
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
};