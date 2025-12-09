using Navigator.Data.Entities.Station;
using Navigator.Data.Models.Station;

namespace Navigator.Data.Repository.StationRepository;

public interface IStationRepository
{
    Task<IEnumerable<VendoStation>> GetVendoStationsAsync(VendoStationsBySearchRequest request);
    Task<IEnumerable<Station>> GetStationsByCoordinatesAsync(StationsByCoordinateRequest dto);
    Task<Station?> GetByEvaNumberAsync(int evaNumber);
    Task SaveStationsAsync(IEnumerable<Station> stations);
}