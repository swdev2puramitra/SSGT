namespace WebApp.Models;

public partial class FwStock
{
    public long StockId { get; set; }

    public string? StockFrom { get; set; }

    public int GodownId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? InvoiceNo { get; set; }

    public DateOnly? ArrivalDt { get; set; }

    public string? Comments { get; set; }

    public virtual FwUserDetail CreatedByNavigation { get; set; } = null!;

    public virtual FwMasterGodown Godown { get; set; } = null!;
}
