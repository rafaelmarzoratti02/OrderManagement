namespace OrderManagement.Inventory.Core.Events;

public class OrderInventoryValidated : IDomainEvent
{
    public int OrderId { get; set; }
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }
}
