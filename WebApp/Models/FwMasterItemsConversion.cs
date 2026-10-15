namespace WebApp.Models;

public partial class FwMasterItemsConversion
{
    public int ItemId { get; set; }

    public int ItemTypeId { get; set; }

    public int Quantity { get; set; }

    public decimal WholeSale { get; set; }

    public decimal SemiWholeSale { get; set; }

    public decimal Retail { get; set; }

    public int? QtyPerType { get; set; }

    public bool? IsActive { get; set; }

    public virtual FwMasterItem Item { get; set; } = null!;

    public virtual FwMasterItemType ItemType { get; set; } = null!;
}
