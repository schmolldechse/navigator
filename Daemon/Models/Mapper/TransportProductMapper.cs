using System.Text.RegularExpressions;

namespace Daemon.Models.Mapper;

public class TransportProductMapper
{
    private static readonly Dictionary<string, TransportProducts> KeyValueMap = new()
    {
        { "highspeedtrain", TransportProducts.Hochgeschwindigkeitszuege },
        { "nationalexpress", TransportProducts.Hochgeschwindigkeitszuege },

        { "intercitytrain", TransportProducts.IntercityUndEurocityZuege },
        { "national", TransportProducts.IntercityUndEurocityZuege },

        { "interregionaltrain", TransportProducts.InterregioUndSchnellzuege },

        { "regionaltrain", TransportProducts.NahverkehrsonstigeZuege },
        { "regionalexpress", TransportProducts.NahverkehrsonstigeZuege },
        { "regional", TransportProducts.NahverkehrsonstigeZuege },

        { "citytrain", TransportProducts.Sbahnen },
        { "suburban", TransportProducts.Sbahnen },

        { "bus", TransportProducts.Busse },

        { "ferry", TransportProducts.Schiffe },

        { "subway", TransportProducts.UBahn },

        { "tram", TransportProducts.Strassenbahn },

        { "shuttle", TransportProducts.AnrufpflichtigeVerkehre },
        { "taxi", TransportProducts.AnrufpflichtigeVerkehre }
    };

    public static TransportProducts GetTransportProduct(string transport)
    {
        if (string.IsNullOrEmpty(transport))
            return TransportProducts.Unknown;

        string key = Regex.Replace(transport, "[_-]", "").ToLowerInvariant();
        return KeyValueMap.TryGetValue(key, out var product) ? product : TransportProducts.Unknown;
    }
}