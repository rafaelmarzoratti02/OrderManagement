namespace OrderManagement.Products.API.Entities;

public class BaseEntity
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
}