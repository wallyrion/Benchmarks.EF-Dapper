// Models/Customer.cs

namespace EfVersusDapper;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
    
}

// Models/Order.cs
public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    
    public string? Notes { get; set; }
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