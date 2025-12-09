using Navigator.Data.Enums;

namespace Navigator.Data.Repository.StationTransportRepository;

public interface IStationTransportRepository
{
    Task<ILookup<int, TransportType>> GetTransportByEvaNumbers(int[] evaNumbers);
}