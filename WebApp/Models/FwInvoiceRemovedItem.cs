namespace WebApp.Models;

public partial class FwInvoiceRemovedItem
{
    public long InvoiceId { get; set; }

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

    public DateTime RemoveDt { get; set; }
}
