namespace WebApp.Models
{
    public class ItemSearchDto
    {
        public long SlNo { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public int ItemType { get; set; }
        public string? TypeName { get; set; }
        public decimal ItemPrice { get; set; }
        public string? StockPosition { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
    }
}
