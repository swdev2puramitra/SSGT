namespace WebApp.Models
{
    // Intermediate DTO for EF query combining bills and invoices
    public class BillsQueryItem
    {
        public long BillId { get; set; }
        public string? BillNo { get; set; }
        public decimal? TotalBillAmount { get; set; }
        public DateTime? BillDate { get; set; }
        public string? IsEstimateInvoice { get; set; }
        public byte CustomerType { get; set; }
        public int CustomerId { get; set; }
        public string? WayBill { get; set; }
        public bool? IsInState { get; set; }
        public string? DispatchStatus { get; set; }
        public string? InvoiceNo { get; set; }
        public int InvoiceStatus { get; set; }
        public long PhoneNo { get; set; }
        public string? CustomerName { get; set; }
        public string? CustAddress { get; set; }
        public string? CustomerTypeText { get; set; }
        public bool? OrderConfirmed { get; set; }
    }
}
