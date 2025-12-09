using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;

namespace Navigator.Data.Repository.StationTransportRepository;

public class StationTransportRepository(DataContext dataContext) : IStationTransportRepository
{
    public async Task<ILookup<int, TransportType>> GetTransportByEvaNumbers(int[] evaNumbers)
    {
        var transports = await dataContext.StationTransport
            .Where(transport => evaNumbers.Contains(transport.EvaNumber))
            .Select(transport => new { transport.EvaNumber, transport.TransportType })
            .ToListAsync();
        return transports.ToLookup(key => key.EvaNumber, value => value.TransportType);
    }
}
