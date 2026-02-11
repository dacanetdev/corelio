namespace Corelio.BlazorApp.Models.Common;

public record ProductFilterCriteria(
    string? SearchTerm,
    Guid? CategoryId,
    bool? IsActive);
