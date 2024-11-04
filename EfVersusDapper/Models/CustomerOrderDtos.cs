// Dtos/CustomerDto.cs


public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<OrderDto> Orders { get; set; }
}

// Dtos/OrderDto.cs

public class OrderDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}

// Dtos/OrderItemDto.cs

public class OrderItemDto
{
    public Guid OrderItemId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}