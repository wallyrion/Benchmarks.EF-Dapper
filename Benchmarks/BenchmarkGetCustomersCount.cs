using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[MemoryDiagnoser(false)]
public class BenchmarkGetCustomersCount : BaseBenchmark
{
    [Benchmark]
    public async Task<int> GetCustomersCount()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return await repository.GetCustomersCountAsync();
    }
    
    [Benchmark]
    public async Task<int> GetCustomersCountDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return await repository.GetCustomersCountAsync();
    }

    
}