namespace WebApp.Models
{
    /// <summary>
    /// DTO for bills list from USP_GET_BILLS stored procedure
    /// Properties must match column names returned by the stored procedure exactly
    /// Used in Packing Dashboard and reports
    /// </summary>
    public class BillsListDto
    {
        public Int64 SlNo { get; set; }
        public long BillId { get; set; }
        public string? BillNo { get; set; }
        public string? CustomerName { get; set; }
        public string? CustAddress { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime? BillDate { get; set; }
        public long PhoneNo { get; set; }
        public string? Estimate { get; set; }
        public string? CustomerType { get; set; }
        public string? InvoiceNo { get; set; }
        public bool IsInState { get; set; }
        public string? WayBill { get; set; }
        public int InvoiceStatus { get; set; }
        public string? DispatchStatus { get; set; }
        // When SP returns paged result include total count (COUNT(*) OVER())
        public int TotalCount { get; set; }
    }

}
