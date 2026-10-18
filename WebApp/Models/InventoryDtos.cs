namespace WebApp.Models;

public sealed class InventoryRowDto
{
    public int SlNo { get; set; }

    public int ItemId { get; set; }
    public int CompanyId { get; set; }

    public string ItemCode { get; set; } = string.Empty;

    public string ItemName { get; set; } = string.Empty;

    public string StockPosition { get; set; } = string.Empty;

    public decimal WholeSalePrice { get; set; }

    public decimal SemiWholeSalePrice { get; set; }

    public decimal RetailPrice { get; set; }

    public decimal SemiWholeSaleMargin { get; set; }

    public decimal RetailMargin { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string SubCategoryName { get; set; } = string.Empty;

    public string ConversionName { get; set; } = string.Empty;

    public string Contains { get; set; } = string.Empty;

    public int? ConversionId { get; set; }
}

public sealed class InventoryPriceUpdateDto
{
    public int ItemId { get; set; }

    public decimal WholeSalePrice { get; set; }

    public decimal SemiWholeSalePrice { get; set; }

    public decimal RetailPrice { get; set; }
}

public sealed class InventoryAutocompleteOptionDto
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
}

public sealed class InventoryGridResponseDto
{
    public IReadOnlyList<InventoryRowDto> Data { get; set; } = Array.Empty<InventoryRowDto>();

    public int Page { get; set; }

    public int TotalPages { get; set; }

    public int Total { get; set; }
}
