namespace daemon.Models;

public record AppConfiguration(
    string DbClientId, 
    string DbClientSecret, 
    string? Proxies = null, 
    bool ProxyEnabled = false
);