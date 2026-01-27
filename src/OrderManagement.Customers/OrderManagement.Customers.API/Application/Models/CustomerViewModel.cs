using OrderManagement.Customers.API.Entities;
using System.Linq;

namespace OrderManagement.Customers.API.Application.Models;

public class CustomerViewModel
{
    public CustomerViewModel(Guid id, string name, string email, List<CustomerAddressItemViewModel> addresses)
    {
        Id = id;
        Name = name;
        Email = email;
        Addresses = addresses;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<CustomerAddressItemViewModel> Addresses { get; set; }

    public static CustomerViewModel FromEntity(Customer customer)
    {
        return new CustomerViewModel(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Addresses.Select(x => CustomerAddressItemViewModel.FromEntity(x)).ToList()
        );
    }
}
