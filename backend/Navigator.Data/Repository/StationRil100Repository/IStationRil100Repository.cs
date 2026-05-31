namespace Navigator.Data.Repository.StationRil100Repository;

public interface IStationRil100Repository
{
    Task<ILookup<int, string>> GetRilByEvaNumbersAsync(int[] evaNumbers);
    Task<int[]> ExpandEvaNumbersByRil100Async(int[] evaNumbers);
}
