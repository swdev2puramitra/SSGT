namespace WebApp.Models;

public partial class FwBillsBackupItem
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

    public DateTime? RemoveDt { get; set; }

    public string? DelInv { get; set; }
}
