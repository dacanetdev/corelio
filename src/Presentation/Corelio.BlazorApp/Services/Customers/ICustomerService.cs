using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Customers;

namespace Corelio.BlazorApp.Services.Customers;

/// <summary>
/// Service interface for customer operations.
/// </summary>
public interface ICustomerService
{
    Task<Result<PagedResult<CustomerListModel>>> GetCustomersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<Result<CustomerModel>> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<List<CustomerListModel>>> SearchCustomersAsync(
        string term,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateCustomerAsync(
        CustomerFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateCustomerAsync(
        Guid id,
        CustomerFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
