using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    static MongoDbContext()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("customers");
}
