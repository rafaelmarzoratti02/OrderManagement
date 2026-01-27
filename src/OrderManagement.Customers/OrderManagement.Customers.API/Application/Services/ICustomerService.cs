using OrderManagement.Customers.API.Application.Models;
using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Application.Services;

public interface ICustomerService
{
    
    Task<Guid> CreateCustomer(CreateCustomerInputModel model);
    Task<CustomerViewModel> GetCustomerById(Guid id);
    
}