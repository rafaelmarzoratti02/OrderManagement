using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Application.Models;

public class CreateCustomerInputModel
{
    public CreateCustomerInputModel(string name, string email, CustomerAddressInputModel address)
    {
        Name = name;
        Email = email;
        Address = address;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    
    public CustomerAddressInputModel Address { get; set; }

    public Customer ToEntity()
    {
        return new Customer(Name, Email);
    }
}