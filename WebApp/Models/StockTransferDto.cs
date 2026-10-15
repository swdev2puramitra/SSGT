namespace WebApp.Models
{
    public class StockTransferDto
    {
        public int TransferId { get; set; }
        public int RowNumber { get; set; }

        public int FromGodown { get; set; }

        public int ToGodown { get; set; }

        public string FromGodownDesc { get; set; } = null!;

        public string ToGodownDesc { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

    }
}
