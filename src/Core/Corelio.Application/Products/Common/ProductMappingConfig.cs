using Corelio.Domain.Entities;
using Mapster;

namespace Corelio.Application.Products.Common;

/// <summary>
/// Mapster configuration for Product entity mappings.
/// </summary>
public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Product -> ProductDto
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);

        // Product -> ProductListDto
        config.NewConfig<Product, ProductListDto>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);
    }
}
