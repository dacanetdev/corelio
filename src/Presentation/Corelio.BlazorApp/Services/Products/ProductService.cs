using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Products;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Products;

/// <summary>
/// Implementation of product service using AuthenticatedHttpClient to call backend API.
/// </summary>
public class ProductService(AuthenticatedHttpClient httpClient) : IProductService
{
    private const string BaseUrl = "/api/v1/products";
    private const string CategoriesUrl = "/api/v1/product-categories";

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
                var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductListDto>>(cancellationToken: cancellationToken);
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
            return Result<PagedResult<ProductListDto>>.Failure($"Error loading products: {ex.Message}");
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
                var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: cancellationToken);
                if (product is not null)
                {
                    return Result<ProductDto>.Success(product);
                }
            }

            return Result<ProductDto>.Failure("Product not found");
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Error loading product: {ex.Message}");
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
                var products = await response.Content.ReadFromJsonAsync<List<ProductListDto>>(cancellationToken: cancellationToken);
                if (products is not null)
                {
                    return Result<List<ProductListDto>>.Success(products);
                }
            }

            return Result<List<ProductListDto>>.Failure("Search failed");
        }
        catch (Exception ex)
        {
            return Result<List<ProductListDto>>.Failure($"Error searching products: {ex.Message}");
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
                var productId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
                return Result<Guid>.Success(productId);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating product: {ex.Message}");
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
            return Result<bool>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error updating product: {ex.Message}");
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
            return Result<bool>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error deleting product: {ex.Message}");
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
                var categories = await response.Content.ReadFromJsonAsync<List<ProductCategoryDto>>(cancellationToken: cancellationToken);
                if (categories is not null)
                {
                    return Result<List<ProductCategoryDto>>.Success(categories);
                }
            }

            return Result<List<ProductCategoryDto>>.Failure("Failed to load categories");
        }
        catch (Exception ex)
        {
            return Result<List<ProductCategoryDto>>.Failure($"Error loading categories: {ex.Message}");
        }
    }
}
