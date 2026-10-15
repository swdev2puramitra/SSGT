namespace WebApp.Models;

public partial class FwInvoice
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

    public string? Invoice { get; set; }

    public string? WayBill { get; set; }

    public string? VehicleNo { get; set; }

    public string? OwnerName { get; set; }

    public string? Pos { get; set; }

    public string? StateCode { get; set; }

    public string? Pincode { get; set; }

    public string? Loc { get; set; }

    public string? IsJson { get; set; }

    public int? InvoiceStatus { get; set; }

    public DateTime? CancelledDate { get; set; }

    public int? CancelledBy { get; set; }

    public virtual FwUserDetail CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<FwInvoiceItem> FwInvoiceItems { get; set; } = new List<FwInvoiceItem>();
}
