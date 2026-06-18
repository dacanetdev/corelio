using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Roles;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Roles;

public class RoleHttpService(AuthenticatedHttpClient httpClient, ILogger<RoleHttpService> logger)
    : IRoleHttpService
{
    private const string BaseUrl = "/api/v1/roles";

    public async Task<Result<List<RoleModel>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(BaseUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var roles = await response.Content.ReadFromJsonAsync<List<RoleModel>>(
                    JsonOptions.Default, cancellationToken);
                return Result<List<RoleModel>>.Success(roles ?? []);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<List<RoleModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading roles");
            return Result<List<RoleModel>>.Failure($"Error loading roles: {ex.Message}");
        }
    }
}
