using Navigator.Data.DTOs.Station;
using Navigator.Data.Entities;

namespace Navigator.Data.Repository.Stations;

public interface IStationsRepository
{
    Task<IEnumerable<Station>> GetStationsAsync(StationSearchRequestDTO dto);
}