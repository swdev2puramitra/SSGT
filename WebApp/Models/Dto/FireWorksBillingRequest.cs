namespace WebApp.Models.Dto
{
    public class FireWorksBillingItemDto
    {
        public int SlNo { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int? ItemTypeId { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemCost { get; set; }
        public int Quantity { get; set; }
        public int? SellType { get; set; }
        public int? SellQty { get; set; }
        // New fields used by server-side save logic
        // ChangedPrice: user-modified per-unit price (null if not modified)
        public decimal? ChangedPrice { get; set; }
        // IsPriceModified: indicates whether the user explicitly modified the per-item price
        public bool? IsPriceModified { get; set; }
        // OriginalCost: master price for the selected type (optional, fallback to ItemPrice)
        public decimal? OriginalCost { get; set; }
        // QtyPerType: number of base units per displayed type-unit (e.g., units per box)
        public int? QtyPerType { get; set; }
        // DisplayPrice: price shown to user per type-unit (optional)
        public decimal? DisplayPrice { get; set; }
        // Tax amounts per-line (absolute). If client sends rates instead, controller will need to convert to amounts.
        // Tax percentages per-line (preferred): send rates (e.g., 9 for 9%) from autocomplete
        public decimal? CgstPercent { get; set; }
        public decimal? SgstPercent { get; set; }
        public decimal? IgstPercent { get; set; }
        // Backwards-compatible absolute amounts (legacy clients may still send these)
        public decimal? CgstAmount { get; set; }
        public decimal? SgstAmount { get; set; }
        public decimal? IgstAmount { get; set; }
        // Total cost for the row (including taxes) as computed by client. Server will accept this but validate.
        public decimal TotalCost { get; set; }
    }

    public class FireWorksBillingRequest
    {
        public int? BillId { get; set; }
        public long? InvoiceId { get; set; }
        // CustomerId set by client when existing customer is selected/created
        public int? CustomerId { get; set; }
        public int CustomerType { get; set; } = 1;
        public bool IsEstimate { get; set; } = true;
        public string? Name { get; set; }
        public long? MobileNo { get; set; }
        // Optional second mobile (whatsapp) number
        public long? Mobile2 { get; set; }
        public string? Address { get; set; }
        // Optional: invoice header fields set by client
        public string? InvoiceNo { get; set; }
        public string? EWayBillNo { get; set; }
        public string? VehicleNo { get; set; }
        public string? OwnerName { get; set; }
        // Invoice header fields required when converting estimate to invoice
        public string? Invoice { get; set; }
        public string? POS { get; set; }
        public string? StateCode { get; set; }
        public string? Pincode { get; set; }
        public string? Loc { get; set; }
        public decimal? DiscountPerItem { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsInState { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? BalanceAmount { get; set; }
        // Optional: Customer GSTIN to be saved on the invoice/bill
        public string? CustomerGstin { get; set; }
        public List<FireWorksBillingItemDto> Items { get; set; } = new List<FireWorksBillingItemDto>();
    }
}
