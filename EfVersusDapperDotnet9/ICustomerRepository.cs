
namespace EfVersusDapper;

public interface ICustomerRepository
{
    Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task AddCustomerAsync(CustomerDto customerDto);
    Task<IReadOnlyList<Guid>> GetAllCustomerIdsAsync();
}