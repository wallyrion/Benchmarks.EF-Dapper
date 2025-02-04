using Microsoft.EntityFrameworkCore;

namespace EfVersusDapper;

public class EfCustomerRepository(ApplicationDbContext context, IConfiguration configuration) : ICustomerRepository
{
    /*private static readonly Func<ApplicationDbContext, Guid, Task<Customer?>> _getCustomerByIdCompiledQuery
        = EF.CompileAsyncQuery(
            (ApplicationDbContext context, Guid customerId) =>
                context.Customers
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(x => x.Orders).ThenInclude(o => o.OrderItems)
                    .FirstOrDefault(x => x.Id == customerId));*/

    public async Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId)
    {
        /*var useCompiledQuery = configuration.GetValue<bool>("UseCompiledQuery");

        if (useCompiledQuery)
        {
            var res = await _getCustomerByIdCompiledQuery.Invoke(context, customerId);

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
        }*/

        var isSplitQuery = configuration.GetValue<bool>("UseSplitQuery");

        var customerQuery = context.Customers.AsQueryable();

        if (isSplitQuery)
        {
            customerQuery = customerQuery.AsSplitQuery();
        }

        var result =
            await customerQuery.Select(customer => new CustomerDto
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
                })
                .FirstOrDefaultAsync(c => c.Id == customerId);

        return result;
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await context.Customers
            .OrderBy(c => c.Name)
            .Select(customer => new CustomerDto
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
            })
            .ToListAsync();

        // Transform models to DTOs

        return customers;
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

    public async Task<int> GetCustomersCountAsync()
    {
        return await context.Customers.CountAsync();
    }

    public async Task<Customer?> GetCustomerEntityByIdAsync(Guid customerId)
    {
        return await context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(x => x.Id == customerId);
    }

    public async Task<Customer?> GetCustomerEntityByIdNoTrackingAsync(Guid customerId)
    {
        return await context.Customers.AsNoTracking().Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id == customerId);
    }

    public async Task<CustomerDto?> GetCustomerByIdRawAsync(Guid customerId)
    {
        var customer = context.Database.SqlQuery<CustomerDto>($"""
                                                                        SELECT "Id", "Name"
                                                                        FROM "Customers"
                                                                        WHERE "Id" = {customerId}
                                                                        """);
        return  await customer.FirstOrDefaultAsync();
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
    {
        var customer = await context.Customers.Select(x => new CustomerDto
        {
            Id = x.Id,
            Name = x.Name
        }).FirstOrDefaultAsync(c => c.Id == customerId);

        return customer;
    }
}