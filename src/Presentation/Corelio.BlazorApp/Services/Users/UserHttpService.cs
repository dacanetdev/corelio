using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Users;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Users;

public class UserHttpService(AuthenticatedHttpClient httpClient, ILogger<UserHttpService> logger)
    : IUserHttpService
{
    private const string BaseUrl = "/api/v1/users";

    public async Task<Result<PagedResult<UserListModel>>> GetUsersAsync(
        int pageNumber = 1, int pageSize = 20, string? search = null, bool? isActive = null,
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

            if (isActive.HasValue)
            {
                queryParams.Add($"isActive={isActive.Value.ToString().ToLowerInvariant()}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<UserListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<UserListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<UserListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading users");
            return Result<PagedResult<UserListModel>>.Failure($"Error loading users: {ex.Message}");
        }
    }

    public async Task<Result<UserModel>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<UserModel>(
                    JsonOptions.Default, cancellationToken);
                if (model is not null)
                {
                    return Result<UserModel>.Success(model);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<UserModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading user {Id}", id);
            return Result<UserModel>.Failure($"Error loading user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateUserAsync(Guid id, UserFormModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                model.FirstName,
                model.LastName,
                model.Phone,
                model.Mobile,
                model.Position,
                model.EmployeeCode,
                model.HireDate,
                model.IsActive
            };

            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {Id}", id);
            return Result<bool>.Failure($"Error updating user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignRolesAsync(Guid id, string[] roleCodes, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(
                $"{BaseUrl}/{id}/roles",
                new { RoleCodes = roleCodes },
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error assigning roles to user {Id}", id);
            return Result<bool>.Failure($"Error assigning roles: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsync($"{BaseUrl}/{id}/deactivate", null, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deactivating user {Id}", id);
            return Result<bool>.Failure($"Error deactivating user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsync($"{BaseUrl}/{id}/activate", null, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error activating user {Id}", id);
            return Result<bool>.Failure($"Error activating user: {ex.Message}");
        }
    }
}
