namespace WebApp.Models;

public sealed class ItemStockOrderReportRow
{
    public int SlNo { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public string AvailableDetails { get; set; } = string.Empty;
    public int OrderedQuantity { get; set; }
    public string OrderedDetails { get; set; } = string.Empty;
    public int RequiredQuantity { get; set; }
    public string RequiredDetails { get; set; } = string.Empty;
}
