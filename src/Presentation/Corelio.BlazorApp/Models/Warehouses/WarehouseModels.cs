namespace Corelio.BlazorApp.Models.Warehouses;

public class WarehouseListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WarehouseFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "main";
    public bool IsDefault { get; set; }
}
