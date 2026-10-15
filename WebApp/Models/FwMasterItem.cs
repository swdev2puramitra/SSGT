namespace WebApp.Models;

public partial class FwMasterItem
{
    public int ItemId { get; set; }

    public string ItemCode { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public int ItemType { get; set; }

    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int DefaultSaleType { get; set; }

    public int? ConversionId { get; set; }

    public decimal WholeSalePrice { get; set; }

    public decimal SemiWholeSalePrice { get; set; }

    public decimal RetailPrice { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public int? Hsnid { get; set; }

    public virtual FwMasterCategory Category { get; set; } = null!;

    public virtual FwMasterCompanyName Company { get; set; } = null!;

    public virtual FwMasterItemType DefaultSaleTypeNavigation { get; set; } = null!;

    public virtual ICollection<FwBillsItem> FwBillsItems { get; set; } = new List<FwBillsItem>();

    public virtual ICollection<FwInvoiceItem> FwInvoiceItems { get; set; } = new List<FwInvoiceItem>();

    public virtual FwMasterItemType ItemTypeNavigation { get; set; } = null!;

    public virtual FwMasterSubCategory SubCategory { get; set; } = null!;
}
