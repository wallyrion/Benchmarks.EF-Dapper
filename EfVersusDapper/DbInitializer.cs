// Data/DbInitializer.cs

using Bogus;
using Microsoft.EntityFrameworkCore;

namespace EfVersusDapper;

public class DbInitializer
{
    private const int CustomersNumber = 10000;
    private const int Seed = 1223;
    private const int ChunkSize = 50;
    
    private static readonly Faker<OrderItem> OrderItemFaker = new Faker<OrderItem>()
        .UseSeed(Seed)
        .RuleFor(oi => oi.Id, f => Guid.NewGuid())
        .RuleFor(oi => oi.ProductName, f => f.Commerce.ProductName())
        .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 100))
        .RuleFor(oi => oi.Price, f => f.Finance.Amount());
    
    private static readonly Faker<Order> OrderFaker = new Faker<Order>()
        .UseSeed(Seed)
        .RuleFor(o => o.Id, f => Guid.NewGuid())
        .RuleFor(o => o.OrderDate, f => f.Date.Past().ToUniversalTime())
        .RuleFor(o => o.OrderItems, f => OrderItemFaker.Generate(5));
    
    private static readonly Faker<Customer> CustomerFaker = new Faker<Customer>()
        .UseSeed(Seed)
        .RuleFor(c => c.Id, f => Guid.NewGuid())
        .RuleFor(c => c.Orders, () => OrderFaker.Generate(10))
        .RuleFor(c => c.Name, f => f.Name.FullName());


    public static async Task Initialize(IServiceProvider serviceProvider, ILogger appLogger)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var customersCount = context.Customers.Count();

        if (customersCount < CustomersNumber)
        {
            for (int i = customersCount; i < CustomersNumber; i += ChunkSize)
            {
                appLogger.LogInformation("Creating customers chunk {CustomersChunk}", i);
                var customers = GenerateCustomers(ChunkSize);
                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                GC.Collect();
            }
        }
    }

    private static List<Customer> GenerateCustomers(int count)
    {
        return CustomerFaker.Generate(count);
    }
}

// Modify Program.cs to initialize DB if needed