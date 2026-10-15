namespace WebApp.Models;

public sealed class StockReceiptItemRequest
{
    public int ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public int ItemTypeId { get; set; }
    public string? ItemTypeName { get; set; }
    public int Quantity { get; set; }
}

public sealed class StockReceiptRequest
{
    public long? StockId { get; set; }
    public int GodownId { get; set; }
    public string? StockFrom { get; set; }
    public string? InvoiceNo { get; set; }
    public DateOnly ArrivalDate { get; set; }
    public string? Comments { get; set; }
    public bool IsBaseQuantity { get; set; }
    public List<StockReceiptItemRequest> Items { get; set; } = new();
}

public sealed class StockListRowDto
{
    public long StockId { get; set; }
    public string? StockFrom { get; set; }
    public string? Godown { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int GodownId { get; set; }
    public string? InvoiceNo { get; set; }
    public DateOnly? ArrivalDate { get; set; }
    public string? Comments { get; set; }
}

public sealed class StockListResponse
{
    public IReadOnlyList<StockListRowDto> Data { get; init; } = Array.Empty<StockListRowDto>();
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int Total { get; init; }
}
