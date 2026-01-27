using MongoDB.Driver;
using OrderManagement.Customers.API.Application.Models;
using OrderManagement.Customers.API.Entities;
using OrderManagement.Customers.API.Infrastructure.Persistence;

namespace OrderManagement.Customers.API.Application.Services;

public class CustomerService : ICustomerService
{
    public CustomerService(MongoDbContext context)
    {
        _context = context;
    }

    private readonly MongoDbContext _context;


    public async Task<Guid> CreateCustomer(CreateCustomerInputModel model)
    {
        var customer = model.ToEntity();
        customer.AddAddress(model.Address.ToEntity());
        
        await _context.Customers.InsertOneAsync(customer);
        return customer.Id;
    }

    public async Task<CustomerViewModel> GetCustomerById(Guid id)
    {
        var customer = await _context.Customers.Find(x => x.Id == id).FirstOrDefaultAsync();

        var model = CustomerViewModel.FromEntity(customer);
        
        return model;
    }
}