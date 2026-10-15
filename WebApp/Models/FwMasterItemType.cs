namespace WebApp.Models;

public partial class FwMasterItemType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FwBillsItem> FwBillsItems { get; set; } = new List<FwBillsItem>();

    public virtual ICollection<FwInvoiceItem> FwInvoiceItems { get; set; } = new List<FwInvoiceItem>();

    public virtual ICollection<FwMasterItem> FwMasterItemDefaultSaleTypeNavigations { get; set; } = new List<FwMasterItem>();

    public virtual ICollection<FwMasterItem> FwMasterItemItemTypeNavigations { get; set; } = new List<FwMasterItem>();
}
