using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OrderManagement.Products.API.Entities;

namespace OrderManagement.Products.API.Infrastructure.Persistence;

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

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
}