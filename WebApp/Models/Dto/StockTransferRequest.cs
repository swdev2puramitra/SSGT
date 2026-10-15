using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Dto
{
    public class StockTransferItemDto
    {
        public int SlNo { get; set; }
        [Required]
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal? ItemPrice { get; set; }
        public int Quantity { get; set; }
        public int? ItemTypeId { get; set; }
    }

    public class StockTransferRequest
    {
        // Optional TransferId for edit scenarios; null for new transfers
        public int? TransferId { get; set; }
        [Required]
        public int FromGodown { get; set; }
        [Required]
        public int ToGodown { get; set; }
        public List<StockTransferItemDto> Items { get; set; } = new List<StockTransferItemDto>();
    }
}
