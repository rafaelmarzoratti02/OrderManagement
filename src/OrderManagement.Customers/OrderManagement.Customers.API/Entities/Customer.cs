namespace OrderManagement.Customers.API.Entities;

public class Customer : BaseEntity
{
    public Customer(string name, string email)
    {
        Name = name;
        Email = email;
        Addresses = [];
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public List<CustomerAddress> Addresses { get; set; }

    public void AddAddress(CustomerAddress address)
    {
        Addresses.Add(address);
    }

    public void RemoveAddress(Guid addressId)
    {
        Addresses.RemoveAll(a => a.Id == addressId);
    }

}