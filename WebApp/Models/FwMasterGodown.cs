namespace WebApp.Models;

public partial class FwMasterGodown
{
    public int Id { get; set; }

    public string GodownDescription { get; set; } = null!;
    public string Address { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FwStockTransfer> FwStockTransferFromGodownNavigations { get; set; } = new List<FwStockTransfer>();

    public virtual ICollection<FwStockTransfer> FwStockTransferToGodownNavigations { get; set; } = new List<FwStockTransfer>();
}
