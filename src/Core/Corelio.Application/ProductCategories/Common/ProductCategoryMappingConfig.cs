using Corelio.Domain.Entities;
using Mapster;

namespace Corelio.Application.ProductCategories.Common;

/// <summary>
/// Mapster configuration for ProductCategory entity mappings.
/// </summary>
public class ProductCategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ProductCategory -> ProductCategoryDto
        config.NewConfig<ProductCategory, ProductCategoryDto>()
            .Map(dest => dest.ParentCategoryName, src => src.ParentCategory != null ? src.ParentCategory.Name : null);
    }
}
