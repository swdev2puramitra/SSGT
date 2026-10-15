namespace WebApp.Models;

public partial class FwStockTransferItem
{
    public int TransferId { get; set; }

    public int ItemId { get; set; }
    public int ItemTypeId { get; set; }

    // Quantity holds the UI-level quantity (in selected item type)
    public int Quantity { get; set; }

    // QtyLowest holds the converted lowest-level quantity stored for inventory adjustments
    public int QtyLowest { get; set; }

    public virtual FwMasterItem Item { get; set; } = null!;

    public virtual FwStockTransfer Transfer { get; set; } = null!;
}
