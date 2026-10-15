namespace WebApp.Models;

public partial class FwBillsItem
{
    public long BillId { get; set; }

    public int ItemId { get; set; }

    public int ItemTypeId { get; set; }

    public int Quantity { get; set; }

    public decimal ItemCost { get; set; }

    public decimal TotalCost { get; set; }

    public decimal? Cgst { get; set; }

    public decimal? Sgst { get; set; }

    public decimal? Igst { get; set; }

    public int? SlNo { get; set; }

    public decimal? ChangedPrice { get; set; }

    public int? SellType { get; set; }

    public int? SellQty { get; set; }

    public decimal? OriginalCost { get; set; }

    // New column: indicates whether the user modified the price for this line item
    // true => ChangedPrice contains user-entered price and discounts should apply on ChangedPrice
    // false => ChangedPrice is null and discounts should apply on OriginalCost
    // Nullable because legacy DB rows may have NULL; use (IsPriceModified ?? false) when evaluating
    public bool? IsPriceModified { get; set; }

    public bool? IsPacked { get; set; }

    public virtual FwBill Bill { get; set; } = null!;

    public virtual FwMasterItem Item { get; set; } = null!;

    public virtual FwMasterItemType ItemType { get; set; } = null!;
}
