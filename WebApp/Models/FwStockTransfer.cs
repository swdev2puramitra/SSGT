namespace WebApp.Models;

public partial class FwStockTransfer
{
    public int TransferId { get; set; }

    public int FromGodown { get; set; }

    public int ToGodown { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual FwUserDetail CreatedByNavigation { get; set; } = null!;

    public virtual FwMasterGodown FromGodownNavigation { get; set; } = null!;

    public virtual FwMasterGodown ToGodownNavigation { get; set; }
    public virtual ICollection<FwStockTransferItem> Items { get; set; } = new List<FwStockTransferItem>();
}
