namespace Navigator.Data.Repository.StationRilRepository;

public interface IStationRilRepository
{
    Task<ILookup<int, string>> GetRilByEvaNumbersAsync(int[] evaNumbers);
}