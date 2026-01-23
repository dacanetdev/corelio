using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Queries.SearchProducts;

/// <summary>
/// Query to search products by barcode, SKU, or name.
/// Optimized for POS fast search.
/// </summary>
/// <param name="Query">The search query (barcode, SKU, or product name).</param>
/// <param name="Limit">Maximum number of results to return (default 20).</param>
public record SearchProductsQuery(
    string Query,
    int Limit = 20) : IRequest<Result<List<ProductListDto>>>;
