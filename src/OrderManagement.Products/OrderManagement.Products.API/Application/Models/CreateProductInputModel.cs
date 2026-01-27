using OrderManagement.Products.API.Entities;

namespace OrderManagement.Products.API.Application.Models;

public class CreateProductInputModel
{
    public CreateProductInputModel(string title, string description, decimal price, string brand, int quantity)
    {
        Title = title;
        Description = description;
        Price = price;
        Brand = brand;
        Quantity = quantity;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; }
    public int Quantity { get; set; }
    
    public Product ToEntity()
        => new Product(Title, Description, Price, Brand, Quantity);
}