using MongoDB.Driver;
using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly MongoDbContext _context;

    public DatabaseSeeder(MongoDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var hasCustomers = await _context.Customers.Find(_ => true).AnyAsync();

        if (hasCustomers)
            return;

        var customers = GetSeedCustomers();
        await _context.Customers.InsertManyAsync(customers);
    }

    private static List<Customer> GetSeedCustomers()
    {
        var customer1 = new Customer("Maria Silva", "maria.silva@email.com");
        customer1.AddAddress(new CustomerAddress(
            street: "Rua das Flores",
            number: "123",
            complement: "Apto 101",
            neighborhood: "Centro",
            city: "Sao Paulo",
            state: "SP",
            country: "Brasil",
            zipCode: "01001-000"
        ));

        var customer2 = new Customer("Joao Santos", "joao.santos@email.com");
        customer2.AddAddress(new CustomerAddress(
            street: "Avenida Brasil",
            number: "456",
            complement: null,
            neighborhood: "Jardim America",
            city: "Rio de Janeiro",
            state: "RJ",
            country: "Brasil",
            zipCode: "20040-020"
        ));

        var customer3 = new Customer("Ana Oliveira", "ana.oliveira@email.com");
        customer3.AddAddress(new CustomerAddress(
            street: "Rua Tiradentes",
            number: "789",
            complement: "Casa 2",
            neighborhood: "Savassi",
            city: "Belo Horizonte",
            state: "MG",
            country: "Brasil",
            zipCode: "30130-000"
        ));

        return [customer1, customer2, customer3];
    }
}
