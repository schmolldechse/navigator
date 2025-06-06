using System.Net;
using daemon.Models;
using Microsoft.Extensions.Logging;

namespace daemon.Utils;

public class ProxyRotator
{
    private readonly ILogger<ProxyRotator> _logger;
    private readonly List<string> _proxies = new();
    private readonly Random _random = new();
    private readonly bool _proxyEnabled;

    public ProxyRotator(ILogger<ProxyRotator> logger, AppConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        _proxyEnabled = configuration.ProxyEnabled;
        
        if (!_proxyEnabled)
        {
            _logger.LogInformation("Proxy usage is disabled via configuration");
            return;
        }
        
        var proxyList = configuration.Proxies?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (proxyList != null && proxyList.Length > 0)
        {
            _proxies.AddRange(proxyList);
            _logger.LogInformation("ProxyRotator initialized with {Count} proxies", _proxies.Count);
        }
    }

    public HttpClient GetRandomProxy()
    {
        if (!_proxyEnabled || _proxies.Count == 0) return new HttpClient();

        var selectedProxy = _proxies[_random.Next(_proxies.Count)];
        var webProxy = new WebProxy();
        
        // Parse the proxy URL
        if (Uri.TryCreate(selectedProxy, UriKind.Absolute, out Uri? proxyUri))
        {
            webProxy.Address = new Uri($"{proxyUri.Scheme}://{proxyUri.Host}:{proxyUri.Port}");
            if (!string.IsNullOrEmpty(proxyUri.UserInfo))
            {
                string[] userInfoParts = proxyUri.UserInfo.Split(':', 2);
                if (userInfoParts.Length == 2)
                    webProxy.Credentials = new NetworkCredential(userInfoParts[0], userInfoParts[1]);
            }
        }
        else webProxy.Address = new Uri(selectedProxy);
        
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = webProxy,
            UseProxy = true
        };

        return new HttpClient(httpClientHandler);
    }
}
