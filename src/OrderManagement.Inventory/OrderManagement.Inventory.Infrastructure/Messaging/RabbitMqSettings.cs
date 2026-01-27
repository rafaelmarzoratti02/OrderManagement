namespace OrderManagement.Inventory.Infrastructure.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; } 
    public int Port { get; set; }
    public string UserName { get; set; } 
    public string Password { get; set; } 
    public string ExchangeName { get; set; } 
    public string QueueName { get; set; } 
    
    public string OrdersExchangeName { get; set; } 
    public string OrderCreatedQueueName { get; set; }
}
