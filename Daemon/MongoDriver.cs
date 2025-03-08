using MongoDB.Driver;

namespace Daemon;

public class MongoDriver
{
    private static IMongoDatabase? _database;
    private static readonly MongoClient _client = new(Environment.GetEnvironmentVariable("MONGO_URL"));

    public static async Task<IMongoDatabase> ConnectToDatabaseAsync()
    {
        return _database ??= _client.GetDatabase("data");
    }

    public static async Task<IMongoCollection<TDocument>> GetCollectionAsync<TDocument>(string collectionName)
    {
        var database = await ConnectToDatabaseAsync();
        return database.GetCollection<TDocument>(collectionName);
    }
}