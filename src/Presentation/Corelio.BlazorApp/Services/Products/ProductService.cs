using System.Net.Http.Json;
using Corelio.Application.Products.Common;
using Corelio.Application.ProductCategories.Common;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Resources;
using Corelio.BlazorApp.Services.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Corelio.BlazorApp.Services.Products;

/// <summary>
/// Response DTO for product creation endpoint.
/// </summary>
internal sealed record CreateProductResponse(Guid ProductId);

/// <summary>
/// Implementation of product service using AuthenticatedHttpClient to call backend API.
/// </summary>
public partial class ProductService(
    AuthenticatedHttpClient httpClient,
    IStringLocalizer<SharedResource> localizer,
    ILogger<ProductService> logger) : IProductService
{
    private const string BaseUrl = "/api/v1/products";
    private const string CategoriesUrl = "/api/v1/product-categories";

    // Source-generated logging methods
    [LoggerMessage(Level = LogLevel.Error, Message = "Error loading products")]
    private partial void LogErrorLoadingProducts(Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error loading product with ID: {productId}")]
    private partial void LogErrorLoadingProduct(Exception ex, Guid productId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error searching products with query: {query}")]
    private partial void LogErrorSearchingProducts(Exception ex, string query);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create product. Status: {statusCode}, Error: {error}")]
    private partial void LogWarningCreatingProduct(int statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception while creating product")]
    private partial void LogErrorCreatingProduct(Exception ex);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to update product {productId}. Status: {statusCode}, Error: {error}")]
    private partial void LogWarningUpdatingProduct(Guid productId, int statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception while updating product {productId}")]
    private partial void LogErrorUpdatingProduct(Exception ex, Guid productId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to delete product {productId}. Status: {statusCode}, Error: {error}")]
    private partial void LogWarningDeletingProduct(Guid productId, int statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception while deleting product {productId}")]
    private partial void LogErrorDeletingProduct(Exception ex, Guid productId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error loading product categories")]
    private partial void LogErrorLoadingCategories(Exception ex);

    /// <inheritdoc />
    public async Task<Result<PagedResult<ProductListDto>>> GetProductsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        Guid? categoryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            if (categoryId.HasValue)
            {
                queryParams.Add($"categoryId={categoryId.Value}");
            }

            if (isActive.HasValue)
            {
                queryParams.Add($"isActive={isActive.Value}");
            }

            var queryString = string.Join("&", queryParams);
            var response = await httpClient.GetAsync($"{BaseUrl}?{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductListDto>>(JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<ProductListDto>>.Success(result);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<ProductListDto>>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            LogErrorLoadingProducts(ex);
            return Result<PagedResult<ProductListDto>>.Failure(localizer["ErrorLoadingProducts"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductDto>(JsonOptions.Default, cancellationToken);
                if (product is not null)
                {
                    return Result<ProductDto>.Success(product);
                }
            }

            return Result<ProductDto>.Failure(localizer["ProductNotFound"]);
        }
        catch (Exception ex)
        {
            LogErrorLoadingProduct(ex, id);
            return Result<ProductDto>.Failure(localizer["ErrorLoadingProduct"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<List<ProductListDto>>> SearchProductsAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryString = $"query={Uri.EscapeDataString(query)}&limit={limit}";
            var response = await httpClient.GetAsync($"{BaseUrl}/search?{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<ProductListDto>>(JsonOptions.Default, cancellationToken);
                if (products is not null)
                {
                    return Result<List<ProductListDto>>.Success(products);
                }
            }

            return Result<List<ProductListDto>>.Failure(localizer["ErrorLoadingProducts"]);
        }
        catch (Exception ex)
        {
            LogErrorSearchingProducts(ex, query);
            return Result<List<ProductListDto>>.Failure(localizer["ErrorLoadingProducts"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateProductResponse>(JsonOptions.Default, cancellationToken);
                return Result<Guid>.Success(result?.ProductId ?? Guid.Empty);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            LogWarningCreatingProduct((int)response.StatusCode, errorMessage);
            return Result<Guid>.Failure(localizer["ErrorCreatingProduct"]);
        }
        catch (Exception ex)
        {
            LogErrorCreatingProduct(ex);
            return Result<Guid>.Failure(localizer["ErrorCreatingProduct"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> UpdateProductAsync(
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{request.Id}", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            LogWarningUpdatingProduct(request.Id, (int)response.StatusCode, errorMessage);
            return Result<bool>.Failure(localizer["ErrorUpdatingProduct"]);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingProduct(ex, request.Id);
            return Result<bool>.Failure(localizer["ErrorUpdatingProduct"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            LogWarningDeletingProduct(id, (int)response.StatusCode, errorMessage);
            return Result<bool>.Failure(localizer["ErrorDeletingProduct"]);
        }
        catch (Exception ex)
        {
            LogErrorDeletingProduct(ex, id);
            return Result<bool>.Failure(localizer["ErrorDeletingProduct"]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<List<ProductCategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(CategoriesUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<ProductCategoryDto>>(JsonOptions.Default, cancellationToken);
                if (categories is not null)
                {
                    return Result<List<ProductCategoryDto>>.Success(categories);
                }
            }

            return Result<List<ProductCategoryDto>>.Failure(localizer["ErrorLoadingCategories"]);
        }
        catch (Exception ex)
        {
            LogErrorLoadingCategories(ex);
            return Result<List<ProductCategoryDto>>.Failure(localizer["ErrorLoadingCategories"]);
        }
    }
}
