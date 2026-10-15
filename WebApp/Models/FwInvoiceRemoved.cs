namespace WebApp.Models;

public partial class FwInvoiceRemoved
{
    public long InvoiceId { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public long BillId { get; set; }

    public string? BillNo { get; set; }

    public int CustomerId { get; set; }

    public byte CustomerType { get; set; }

    public string IsEstimateInvoice { get; set; } = null!;

    public decimal BillAmount { get; set; }

    public decimal? DiscountPerItem { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public bool? IsInState { get; set; }

    public string? CustGstin { get; set; }

    public decimal? TotalBillAmount { get; set; }

    public decimal? AmountPaid { get; set; }

    public decimal? BalanceAmount { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime RemoveDt { get; set; }

    public int? InvoiceStatus { get; set; }

    public DateTime? CancelledDate { get; set; }

    public int? CancelledBy { get; set; }
}
