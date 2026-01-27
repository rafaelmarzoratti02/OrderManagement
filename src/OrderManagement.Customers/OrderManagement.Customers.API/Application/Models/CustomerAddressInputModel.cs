using OrderManagement.Customers.API.Entities;

namespace OrderManagement.Customers.API.Application.Models;

public class CustomerAddressInputModel
{
    public CustomerAddressInputModel(string street, string number, string? complement, string neighborhood,
        string city, string state, string country, string zipCode)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public string Street { get; set; }
    public string Number { get; set; }
    public string? Complement { get; set; }
    public string Neighborhood { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }

    public CustomerAddress ToEntity()
    {
        return new CustomerAddress(Street, Number, Complement, Neighborhood, City, State, Country, ZipCode);
    }
}
