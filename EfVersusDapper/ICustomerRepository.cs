
namespace EfVersusDapper;

public interface ICustomerRepository
{
    Task<CustomerDto?> GetCustomerWithOrdersAsync(Guid customerId);
    Task<List<CustomerDto>> GetAllCustomersAsync();
    Task AddCustomerAsync(CustomerDto customerDto);
    Task<IReadOnlyList<Guid>> GetAllCustomerIdsAsync();
    Task<int> GetCustomersCountAsync();

    Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId);
    Task<Customer?> GetCustomerEntityByIdAsync(Guid customerId);
    Task<Customer?> GetCustomerEntityByIdNoTrackingAsync(Guid customerId);

    Task<CustomerDto?> GetCustomerByIdRawAsync(Guid customerId);
}