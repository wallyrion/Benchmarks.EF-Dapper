// Models/Customer.cs

using EfVersusDapper.Models;

namespace EfVersusDapper;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
}

// Models/Order.cs
public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

// Models/OrderItem.cs
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}