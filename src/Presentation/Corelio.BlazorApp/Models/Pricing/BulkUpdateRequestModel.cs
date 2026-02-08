namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for bulk pricing update requests (form-bindable).
/// </summary>
public class BulkUpdateRequestModel
{
    public List<Guid> ProductIds { get; set; } = [];
    public string UpdateType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int? TierNumber { get; set; }
}
