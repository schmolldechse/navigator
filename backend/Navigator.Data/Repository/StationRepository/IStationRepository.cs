using Navigator.Data.Entities.Station;
using Navigator.Data.Models.Ris;
using Navigator.Data.Models.StaDa;
using Navigator.Data.Models.Station;

namespace Navigator.Data.Repository.StationRepository;

public interface IStationRepository
{
    Task<IEnumerable<VendoStation>> GetVendoStationsAsync(VendoStationsBySearchRequest request);
    Task<IEnumerable<RisStations.StopPlaceSearchResult>> GetRisStationsByCoordinatesAsync(RisStationsByCoordinatesRequest request);
    Task<IEnumerable<StaDa.Station>> GetStaDaAsync();
    Task<IEnumerable<Station>> GetStationsByCoordinatesAsync(StationsByCoordinateRequest dto);
    Task<IEnumerable<Station>> GetStationBatch(IEnumerable<int> evaNumbers);
    Task<Station?> GetRandomStationAsync(ShuffledStationRequest request);
    Task<Station?> GetByEvaNumberAsync(int evaNumber);
    Task SaveStationsAsync(IEnumerable<Station> stations);
}