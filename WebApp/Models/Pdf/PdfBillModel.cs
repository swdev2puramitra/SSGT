namespace WebApp.Models.Pdf
{
    public class PdfBillModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string Gstin { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CustomerGstin { get; set; } = string.Empty;
        public string CustomerMobile { get; set; } = string.Empty;
        public string LicenceNo { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public string Ifsc { get; set; } = string.Empty;
        public string BillNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string VehicleNo { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string EWayBillNo { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public bool IsInState { get; set; }
        public decimal TotalAmount { get; set; }
        // Persisted tax-inclusive invoice total, when rendering an invoice.
        public decimal? InvoiceTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public List<PdfBillItem> Items { get; set; } = new List<PdfBillItem>();
        public List<PdfTaxSummary> TaxSummaries { get; set; } = new List<PdfTaxSummary>();
    }

    public class PdfTaxSummary
    {
        public decimal Taxable { get; set; }
        public decimal CgstPercent { get; set; }
        public decimal CgstAmount { get; set; }
        public decimal SgstPercent { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal IgstPercent { get; set; }
        public decimal IgstAmount { get; set; }
        public decimal TotalTax => CgstAmount + SgstAmount + IgstAmount;
    }

    public class PdfBillItem
    {
        public int SlNo { get; set; }
        // Company name extracted from ItemName first token for estimates
        public string CompanyName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal NormalizedQuantity { get; set; }
        public string OriginalEntry { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Discount { get; set; }
        public decimal Taxable { get; set; }
        public decimal GstPercent { get; set; }
        public decimal Cgst { get; set; }
        public decimal Sgst { get; set; }
        public decimal Igst { get; set; }
        // per-line tax amounts
        public decimal CgstAmount { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal IgstAmount { get; set; }
        // UPC if available
        public string Upc { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
