using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using OrderManagement.Products.API.Application.Models;
using OrderManagement.Products.API.Entities.Events;
using OrderManagement.Products.API.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Persistence;

namespace OrderManagement.Products.API.Application.Services;

public class ProductService : IProductService
{
    private readonly MongoDbContext _context;
    private readonly IEventPublisher  _eventPublisher;
    private readonly ILogger<ProductService> _logger;
    
    public ProductService(MongoDbContext context, IEventPublisher eventPublisher, ILogger<ProductService> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }
    
    public async Task<Guid> CreateProduct(CreateProductInputModel model)
    {
        var product = model.ToEntity();
        await _context.Products.InsertOneAsync(product);

        var @event = new ProductCreatedEvent(product.Sku, product.Quantity, product.Title);

        await _eventPublisher.PublishAsync(@event);

        _logger.LogInformation(
            "Product created and ProductCreatedEvent published. ProductId: {ProductId}, Sku: {Sku}",
            product.Id,
            product.Sku);

        return product.Id;
    }

    public async Task<ProductViewModel?> GetProductById(Guid id)
    {
        var product = await _context.Products
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (product is null)
            return null;

        return ProductViewModel.FromEntity(product);
    }

    public async Task<List<ProductViewModel>> GetAll()
    {
        var products = await _context.Products
            .Find(_ => true)
            .ToListAsync();

        return products.Select(ProductViewModel.FromEntity).ToList();
    }
}