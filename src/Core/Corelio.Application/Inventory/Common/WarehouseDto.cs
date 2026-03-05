namespace Corelio.Application.Inventory.Common;

/// <summary>
/// Data transfer object for a warehouse.
/// </summary>
public record WarehouseDto(Guid Id, string Name, bool IsDefault);
