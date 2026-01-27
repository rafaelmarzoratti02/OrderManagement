using MongoDB.Driver;
using OrderManagement.Products.API.Entities;
using OrderManagement.Products.API.Entities.Events;
using OrderManagement.Products.API.Infrastructure.Messaging;

namespace OrderManagement.Products.API.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly MongoDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public DatabaseSeeder(MongoDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task SeedAsync()
    {
        var hasProducts = await _context.Products.Find(_ => true).AnyAsync();

        if (hasProducts)
            return;

        var products = GetSeedProducts();

        foreach (var product in products)
        {
            await _context.Products.InsertOneAsync(product);

            var @event = new ProductCreatedEvent(product.Sku, product.Quantity, product.Title);
            await _eventPublisher.PublishAsync(@event);
        }
    }

    private static List<Product> GetSeedProducts()
    {
        return
        [
            new Product(
                title: "Notebook Gamer",
                description: "Notebook gamer com processador Intel i7, 16GB RAM, SSD 512GB e placa de video RTX 3060",
                price: 5999.99m,
                brand: "Dell",
                quantity: 15
            ),
            new Product(
                title: "Smartphone Pro Max",
                description: "Smartphone com tela AMOLED 6.7 polegadas, 256GB armazenamento e camera 108MP",
                price: 4299.00m,
                brand: "Samsung",
                quantity: 50
            ),
            new Product(
                title: "Fone de Ouvido Bluetooth",
                description: "Fone de ouvido sem fio com cancelamento de ruido ativo e bateria de 30 horas",
                price: 899.90m,
                brand: "Sony",
                quantity: 100
            ),
            new Product(
                title: "Monitor Ultrawide",
                description: "Monitor curvo 34 polegadas, resolucao WQHD 3440x1440, 144Hz",
                price: 2499.00m,
                brand: "LG",
                quantity: 25
            ),
            new Product(
                title: "Teclado Mecanico RGB",
                description: "Teclado mecanico com switches Cherry MX Red, iluminacao RGB e layout ABNT2",
                price: 599.90m,
                brand: "Logitech",
                quantity: 80
            )
        ];
    }
}
