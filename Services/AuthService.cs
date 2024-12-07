using DELLight.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DELLight.Services;

public class AuthService
{
    private readonly IMongoCollection<User> collection;

    public AuthService(IOptions<AuthDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        collection = mongoDatabase.GetCollection<User>(settings.Value.UsersCollectionName);
    }

    public User? GetUser(string login, string password)
    {
        return collection
            .Find(user => user.Login == login && user.Password == password)
            .FirstOrDefault();
    }
}
