using OrderManagement.Products.API.Entities;

namespace OrderManagement.Products.API.Application.Models;

public class ProductViewModel
{
    public ProductViewModel(string title, string sku, string description, decimal price, string brand, int quantity)
    {
        Title = title;
        SKU = sku;
        Description = description;
        Price = price;
        Brand = brand;
        Quantity = quantity;
    }

    public string Title { get; set; }
    public string SKU { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; }
    public int Quantity { get; set; }
    
    public static ProductViewModel FromEntity(Product product)
    => new ProductViewModel(product.Title,product.Sku, product.Description, product.Price, product.Brand, product.Quantity);

}