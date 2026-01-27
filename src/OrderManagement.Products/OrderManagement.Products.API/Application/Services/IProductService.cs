using OrderManagement.Products.API.Application.Models;
using OrderManagement.Products.API.Entities;

namespace OrderManagement.Products.API.Application.Services;

public interface IProductService
{
    Task<Guid> CreateProduct(CreateProductInputModel model);
    Task<ProductViewModel> GetProductById(Guid id);
}

