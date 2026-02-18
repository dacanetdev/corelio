using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Customers;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Customers;

/// <summary>
/// Implementation of customer service using AuthenticatedHttpClient.
/// </summary>
public class CustomerService(AuthenticatedHttpClient httpClient, ILogger<CustomerService> logger) : ICustomerService
{
    private const string BaseUrl = "/api/v1/customers";

    public async Task<Result<PagedResult<CustomerListModel>>> GetCustomersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<CustomerListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<CustomerListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<CustomerListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading customers");
            return Result<PagedResult<CustomerListModel>>.Failure($"Error loading customers: {ex.Message}");
        }
    }

    public async Task<Result<CustomerModel>> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<CustomerModel>(
                    JsonOptions.Default, cancellationToken);
                if (model is not null)
                {
                    return Result<CustomerModel>.Success(model);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<CustomerModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading customer {Id}", id);
            return Result<CustomerModel>.Failure($"Error loading customer: {ex.Message}");
        }
    }

    public async Task<Result<List<CustomerListModel>>> SearchCustomersAsync(
        string term,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/search?q={Uri.EscapeDataString(term)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadFromJsonAsync<List<CustomerListModel>>(
                    JsonOptions.Default, cancellationToken);
                return Result<List<CustomerListModel>>.Success(results ?? []);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<List<CustomerListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching customers");
            return Result<List<CustomerListModel>>.Failure($"Error searching customers: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreateCustomerAsync(
        CustomerFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, model, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<Guid>.Success(result.CustomerId);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating customer");
            return Result<Guid>.Failure($"Error creating customer: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateCustomerAsync(
        Guid id,
        CustomerFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", model, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating customer {Id}", id);
            return Result<bool>.Failure($"Error updating customer: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting customer {Id}", id);
            return Result<bool>.Failure($"Error deleting customer: {ex.Message}");
        }
    }

    private sealed record CreateCustomerResponse(Guid CustomerId);
}
