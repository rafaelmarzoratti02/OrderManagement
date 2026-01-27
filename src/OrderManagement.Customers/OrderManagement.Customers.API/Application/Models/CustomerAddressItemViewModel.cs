using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Application.Models;

public class CustomerAddressItemViewModel
{
    public CustomerAddressItemViewModel(Guid id,string street, string number,
        string city, string state, string country)
    {
        Id = id;
        Street = street;
        Number = number;
        City = city;
        State = state;
        Country = country;
    }

    public Guid Id { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }

    public static CustomerAddressItemViewModel FromEntity(CustomerAddress address) 
        => new CustomerAddressItemViewModel(address.Id, address.Street, address.Number, address.City, address.State, address.Country);
}