using System.Collections.Concurrent;
using System.Net;

namespace Navigator.Data.Infrastructure;

public class ProxyHttpClientFactory : IDisposable
{
    private readonly List<string> _proxyAddresses;
    private readonly ConcurrentDictionary<string, Lazy<HttpMessageHandler>> _handlers;

    private int _currentProxyIndex;
    private readonly object _lock = new();

    private bool _disposed;

    public ProxyHttpClientFactory()
    {
        var proxies = Environment.GetEnvironmentVariable("PROXIES");
        _proxyAddresses = string.IsNullOrWhiteSpace(proxies)
            ? new List<string>()
            : proxies.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
        _handlers = new ConcurrentDictionary<string, Lazy<HttpMessageHandler>>();
        _currentProxyIndex = 0;
    }

    public HttpClient CreateClient()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ProxyHttpClientFactory));

        if (_proxyAddresses.Count == 0)
        {
            var handler = GetOrCreateHandler("default");
            return new HttpClient(handler, disposeHandler: false);
        }

        var proxyAddress = GetNextProxy();
        var messageHandler = GetOrCreateHandler(proxyAddress);

        return new HttpClient(messageHandler, disposeHandler: false);
    }

    private HttpMessageHandler GetOrCreateHandler(string key)
    {
        var lazyHandler = _handlers.GetOrAdd(key, _ => new Lazy<HttpMessageHandler>(() =>
        {
            if (key == "default") return new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };

            var proxy = new WebProxy(key)
            {
                UseDefaultCredentials = false
            };

            return new SocketsHttpHandler
            {
                Proxy = proxy,
                UseProxy = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };
        }));
        return lazyHandler.Value;
    }

    private string GetNextProxy()
    {
        lock (_lock)
        {
            var proxy = _proxyAddresses[_currentProxyIndex];
            _currentProxyIndex = (_currentProxyIndex + 1) % _proxyAddresses.Count;
            return proxy;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var handler in _handlers.Values)
        {
            if (handler.IsValueCreated) handler.Value?.Dispose();
        }

        _handlers.Clear();
    }
}
