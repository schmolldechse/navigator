using Navigator.Data.Entities.Station;

namespace Navigator.Preflight.Infrastructure.Merging;

public interface IStationMerging
{
    Task<IEnumerable<Station>> StartMergingAsync();
}
