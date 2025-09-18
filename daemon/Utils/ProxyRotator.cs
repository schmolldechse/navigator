using System.Net;
using Microsoft.Extensions.Logging;

namespace daemon.Utils;

public class ProxyRotator : IDisposable
{
	private readonly bool _proxyEnabled;
	private readonly ILogger<ProxyRotator> _logger;

	private readonly HttpClient _defaultClient = new() { Timeout = TimeSpan.FromMinutes(1) };
	private readonly List<HttpClient> _proxies = new();

	private bool _disposed;

	public ProxyRotator(ILogger<ProxyRotator> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
		
		bool.TryParse(Environment.GetEnvironmentVariable("PROXY_ENABLED"), out _proxyEnabled);
		if (!_proxyEnabled)
		{
			_logger.LogInformation("Proxy usage is disabled via configuration");
			return;
		}

		var proxyList = (Environment.GetEnvironmentVariable("PROXIES") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (proxyList.Length == 0) return;
		proxyList.ToList().ForEach(proxy => _proxies.Add(CreateHttpClientWithProxy(proxy))); 
		_logger.LogInformation("ProxyRotator initialized with {Count} proxies", _proxies.Count);
	}

	public HttpClient GetRandomProxy()
	{
		if (!_proxyEnabled || _proxies.Count == 0)
			return _defaultClient;
		var randomIndex = Random.Shared.Next(_proxies.Count);
		return _proxies[randomIndex];
	}

	private HttpClient CreateHttpClientWithProxy(string proxyUrl)
	{
		var webProxy = new WebProxy();

		if (Uri.TryCreate(proxyUrl, UriKind.Absolute, out Uri? proxyUri))
		{
			webProxy.Address = new Uri($"{proxyUri.Scheme}://{proxyUri.Host}:{proxyUri.Port}");
			if (string.IsNullOrEmpty(proxyUri.UserInfo)) return new();
			
			string[] userInfoParts = proxyUri.UserInfo.Split(':', 2); 
			if (userInfoParts.Length == 2) 
			{ 
				webProxy.Credentials = new NetworkCredential(userInfoParts[0], userInfoParts[1]);
			}
		}
		else webProxy.Address = new Uri(proxyUrl);

		var httpClientHandler = new HttpClientHandler
		{
			Proxy = webProxy,
			UseProxy = true,
			PreAuthenticate = true,
		};
		return new(httpClientHandler) { Timeout = TimeSpan.FromMinutes(1)};
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;
		_defaultClient.Dispose();

		_proxies.ForEach(client => client.Dispose());
		_proxies.Clear();

		_logger.LogInformation("ProxyRotator disposed");
	}
}
