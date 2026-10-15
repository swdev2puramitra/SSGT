namespace WebApp.Models;

public partial class FwBill
{
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

    public decimal? AddlDisc { get; set; }

    public decimal? FinalAmount { get; set; }

    public bool? IsVerified { get; set; }

    public string? AddDesc { get; set; }

    public int? VerifiedBy { get; set; }

    public DateTime? VerifiedDate { get; set; }

    /// <summary>
    /// I Initiated, D dispatched, P Partial update, C Completed
    /// </summary>
    public string? DispatchStatus { get; set; }

    // Navigation properties
    public virtual FwUserDetail CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<FwBillsItem> FwBillsItems { get; set; } = new List<FwBillsItem>();

    // Convenience navigation to invoice if converted
    public virtual ICollection<FwInvoice> FwInvoices { get; set; } = new List<FwInvoice>();
}
