namespace WebApp.Models;

/// <summary>
/// Durable record of an item-based invoice split.
/// </summary>
public partial class FwInvoiceSplitHistory
{
    public long SplitHistoryId { get; set; }
    public long BillId { get; set; }
    public long SourceInvoiceId { get; set; }
    public long SplitInvoiceId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
