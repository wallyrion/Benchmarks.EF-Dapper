using Dapper;
using Npgsql;

namespace EfVersusDapper;

public class DapperCustomerRepository(IConfiguration configuration) : ICustomerRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
                                           SELECT * FROM "Customers" WHERE "Id" = @Id;
                                           SELECT * FROM "Orders" WHERE "CustomerId" = @Id;
                                           SELECT * FROM "OrderItems" WHERE "OrderId" IN (SELECT "Id" FROM "Orders" WHERE "CustomerId" = @Id);
                           """;

        await using var multi = await connection.QueryMultipleAsync(sql, new { Id = customerId });

        var customer = await multi.ReadSingleOrDefaultAsync<Customer>();
        var orders = (await multi.ReadAsync<Order>()).ToList();
        var orderItems = (await multi.ReadAsync<OrderItem>()).ToList();

        if (customer == null)
        {
            return null;
        }

        foreach (var order in orders)
        {
            order.OrderItems = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
        }

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Orders = orders.Select(order => new OrderDto
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
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
                                           SELECT * FROM "Customers" c
                                           LEFT JOIN "Orders" o ON c."Id" = o."CustomerId"
                                           LEFT JOIN "OrderItems" oi ON o."Id" = oi."OrderId"
                                           ORDER BY c."Name", c."Id", o."OrderDate", o."Id"
                           """;

        var customerDict = new Dictionary<Guid, CustomerDto>();

        var result = await connection.QueryAsync<CustomerDto, OrderDto?, OrderItemDto?, CustomerDto>(
            sql,
            (customerDto, orderDto, orderItemDto) =>
            {
                if (!customerDict.TryGetValue(customerDto.Id, out var currentCustomer))
                {
                    currentCustomer = customerDto;
                    currentCustomer.Orders = new List<OrderDto>();
                    customerDict[currentCustomer.Id] = currentCustomer;
                }

                if (orderDto != null)
                {
                    if (!currentCustomer.Orders.Contains(orderDto))
                    {
                        orderDto.OrderItems = new List<OrderItemDto>();
                        currentCustomer.Orders.Add(orderDto);
                    }

                    if (orderItemDto != null)
                    {
                        orderDto.OrderItems.Add(orderItemDto);
                    }
                }

                return currentCustomer;
            },
            splitOn: "Id, Id"
        );

        return result.Distinct().ToList();
    }

    public async Task AddCustomerAsync(CustomerDto customerDto)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var sql =
            """
            INSERT INTO "Customers" ("Id", "Name") VALUES (@Id, @Name);
            INSERT INTO "Orders" ("Id", "OrderDate", "CustomerId") VALUES (@OrderId, @OrderDate, @CustomerId);
            INSERT INTO "OrderItems" ("Id", "OrderId", "ProductName", "Quantity", "Price") VALUES (@OrderItemId, @OrderId, @ProductName, @Quantity, @Price);
            """;

        await using var transaction = await connection.BeginTransactionAsync();

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

        await connection.ExecuteAsync(sql, customer, transaction);
        await transaction.CommitAsync();
    }
    
    
// Add this method below the existing methods in the DapperCustomerRepository class

public async Task<IReadOnlyList<Guid>> GetAllCustomerIdsAsync()
{
    await using var connection = new NpgsqlConnection(_connectionString);

    const string sql = """
                       SELECT "Id" FROM "Customers"
                       """;

    var customerIds = await connection.QueryAsync<Guid>(sql);

    return customerIds.ToList();
}
}