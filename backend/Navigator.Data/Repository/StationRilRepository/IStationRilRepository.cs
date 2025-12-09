namespace Navigator.Data.Repository.StationRilRepository;

public interface IStationRilRepository
{
    Task<ILookup<int, string>> GetRilByEvaNumbers(int[] evaNumbers);
}