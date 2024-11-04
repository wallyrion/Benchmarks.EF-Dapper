using Microsoft.EntityFrameworkCore;

namespace EfVersusDapper;

public class EfCustomerRepository(ApplicationDbContext context, IConfiguration configuration) : ICustomerRepository
{
    private static readonly Func<ApplicationDbContext, Guid, Task<Customer?>> _getCustomerByIdCompiledQuery
        = EF.CompileAsyncQuery(
            (ApplicationDbContext context, Guid customerId) =>
                context.Customers
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(x => x.Orders).ThenInclude(o => o.OrderItems)
                    .FirstOrDefault(x => x.Id == customerId));
    
    public async Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId)
    {
        var useCompiledQuery = configuration.GetValue<bool>("UseCompiledQuery");

        if (useCompiledQuery)
        {
            var res =  await _getCustomerByIdCompiledQuery.Invoke(context, customerId);

            if (res == null) return null;
            
            return new CustomerDto
            {
                Id = res.Id,
                Name = res.Name,
                Orders = res.Orders.Select(order => new OrderDto
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.Id,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                }).ToList()
            };
        }
        
        var isSplitQuery = configuration.GetValue<bool>("UseSplitQuery");
        var asNoTracking = configuration.GetValue<bool>("AsNoTracking");

        var customerQuery = context.Customers.AsQueryable();

        if (asNoTracking)
        {
            customerQuery = customerQuery.AsNoTracking();
        }
        
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
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.Id,
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
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.Id,
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
                Id = orderDto.OrderId,
                OrderDate = orderDto.OrderDate,
                OrderItems = orderDto.OrderItems.Select(oiDto => new OrderItem
                {
                    Id = oiDto.OrderItemId,
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