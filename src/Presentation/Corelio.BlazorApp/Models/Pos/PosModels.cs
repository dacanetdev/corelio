using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Models.Pos;

/// <summary>
/// Product search result from POS search API.
/// </summary>
public class PosProductModel
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Stock { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public bool IvaEnabled { get; set; }
    public decimal TaxRate { get; set; }
}

/// <summary>
/// A line item in the POS shopping cart.
/// </summary>
public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal DiscountPercentage { get; set; } = 0;
    public bool IvaEnabled { get; set; }
    public decimal TaxRate { get; set; }

    public decimal LineSubtotal => UnitPrice * Quantity;
    public decimal LineDiscount => LineSubtotal * (DiscountPercentage / 100m);
    public decimal LineNet => LineSubtotal - LineDiscount;
    public decimal LineTax => IvaEnabled ? LineNet * TaxRate : 0;
    public decimal LineTotal => LineNet + LineTax;
}

/// <summary>
/// State of the POS shopping cart.
/// </summary>
public class CartState
{
    public List<CartItem> Items { get; set; } = [];

    public decimal SubTotal => Items.Sum(i => i.LineNet);
    public decimal TaxAmount => Items.Sum(i => i.LineTax);
    public decimal Total => Items.Sum(i => i.LineTotal);
    public int ItemCount => Items.Count;
    public bool IsEmpty => Items.Count == 0;

    public void AddProduct(PosProductModel product)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is not null)
        {
            existing.Quantity++;
        }
        else
        {
            Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSku = product.Sku,
                UnitPrice = product.SalePrice,
                Quantity = 1,
                IvaEnabled = product.IvaEnabled,
                TaxRate = product.TaxRate
            });
        }
    }

    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
    }

    public void Clear()
    {
        Items.Clear();
    }
}

/// <summary>
/// Request to create a sale from the Blazor POS.
/// </summary>
public class CreateSaleRequestModel
{
    public List<CartItemRequestModel> Items { get; set; } = [];
    public Guid? CustomerId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string Type { get; set; } = "Pos";
    public string? Notes { get; set; }
}

/// <summary>
/// Single item in a create sale request.
/// </summary>
public class CartItemRequestModel
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
}

/// <summary>
/// Payment entry in a complete sale request.
/// </summary>
public class PaymentRequestModel
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

/// <summary>
/// Full sale model returned from API.
/// </summary>
public class SaleModel
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SaleItemModel> Items { get; set; } = [];
    public List<PaymentModel> Payments { get; set; } = [];
}

/// <summary>
/// Sale item model.
/// </summary>
public class SaleItemModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Payment model.
/// </summary>
public class PaymentModel
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Summary sale model for list views.
/// </summary>
public class SaleListModel
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public decimal Total { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
