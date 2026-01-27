namespace OrderManagement.Products.API.Entities;

public class Product : BaseEntity
{
    public Product(string title, string description, decimal price, string brand, int quantity)
    {
        Title = title;
        Description = description;
        Price = price;
        Brand = brand;
        Quantity = quantity;
        Sku = GenerateSku(brand, title);
    }

    public string Sku { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; }
    public int Quantity { get; set; }

    private static string GenerateSku(string brand, string title)
    {
        var brandPrefix = GetPrefix(brand, 3);
        var titlePrefix = GetPrefix(title, 3);
        var uniqueId = DateTime.UtcNow.Ticks.ToString()[^8..];

        return $"{brandPrefix}-{titlePrefix}-{uniqueId}".ToUpperInvariant();
    }

    private static string GetPrefix(string value, int length)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new string('X', length);

        var cleaned = new string(value
            .Where(char.IsLetterOrDigit)
            .Take(length)
            .ToArray());

        return cleaned.Length < length
            ? cleaned.PadRight(length, 'X')
            : cleaned;
    }
}