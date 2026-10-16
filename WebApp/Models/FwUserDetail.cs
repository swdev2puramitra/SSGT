namespace WebApp.Models;

public partial class FwUserDetail
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public long? ContactNo { get; set; }

    public int? RoleId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FwBill> FwBills { get; set; } = new List<FwBill>();

    public virtual ICollection<FwInvoice> FwInvoices { get; set; } = new List<FwInvoice>();

    public virtual ICollection<FwMasterHsn> FwMasterHsns { get; set; } = new List<FwMasterHsn>();
    public virtual ICollection<FwStockTransfer> FwStockTransfers { get; set; } = new List<FwStockTransfer>();
}
