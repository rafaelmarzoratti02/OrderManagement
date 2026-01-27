using MongoDB.Driver;
using OrderManagement.Products.API.Application.Models;
using OrderManagement.Products.API.Entities.Events;
using OrderManagement.Products.API.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Persistence;

namespace OrderManagement.Products.API.Application.Services;

public class ProductService : IProductService
{
    private readonly MongoDbContext _context;
    private readonly IEventPublisher  _eventPublisher;
    
    public ProductService(MongoDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }
    
    public async Task<Guid> CreateProduct(CreateProductInputModel model)
    {
        var product = model.ToEntity();
        await _context.Products.InsertOneAsync(product);

        var @event = new ProductCreatedEvent(product.Sku, product.Quantity, product.Title);
        
        await _eventPublisher.PublishAsync(@event);
        
        return product.Id;
    }

    public Task<ProductViewModel> GetProductById(Guid id)
    {
        throw new NotImplementedException();
    }
}