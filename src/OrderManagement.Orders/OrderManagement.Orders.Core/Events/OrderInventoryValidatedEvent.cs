namespace OrderManagement.Orders.Core.Events;

public class OrderInventoryValidatedEvent : IDomainEvent
{
    public int OrderId { get; set; }
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }
}
