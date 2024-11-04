using Microsoft.EntityFrameworkCore;

namespace EfVersusDapper;

public class EfCustomerRepository(ApplicationDbContext context, IConfiguration configuration) : ICustomerRepository
{
    public async Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId)
    {
        var isSplitQuery = configuration.GetValue<bool>("UseSplitQuery");

        var customerQuery = context.Customers
            .AsNoTracking();

        if (isSplitQuery)
        {
            customerQuery = customerQuery.AsSplitQuery();
        }

        var customer =
            await customerQuery.Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(c => c.Id == customerId);

        // Transform model to DTO
        if (customer == null) return null;

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Orders = customer.Orders.Select(order => new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList()
        };

        return customerDto;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await context.Customers
            .OrderBy(c => c.Name)
            .Include(c => c.Orders.OrderBy(o => o.OrderDate))
            .ThenInclude(o => o.OrderItems)
            .ToListAsync();

        // Transform models to DTOs
        var customerDtos = customers.Select(customer => new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Orders = customer.Orders.Select(order => new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList()
        });

        return customerDtos;
    }

    public async Task AddCustomerAsync(CustomerDto customerDto)
    {
        // Transform DTO to model
        var customer = new Customer
        {
            Id = customerDto.Id,
            Name = customerDto.Name,
            Orders = customerDto.Orders.Select(orderDto => new Order
            {
                Id = orderDto.Id,
                OrderDate = orderDto.OrderDate,
                OrderItems = orderDto.OrderItems.Select(oiDto => new OrderItem
                {
                    Id = oiDto.Id,
                    ProductName = oiDto.ProductName,
                    Quantity = oiDto.Quantity,
                    Price = oiDto.Price
                }).ToList()
            }).ToList()
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Guid>> GetAllCustomerIdsAsync()
    {
        return await context.Customers.Select(c => c.Id).ToListAsync();
    }
}