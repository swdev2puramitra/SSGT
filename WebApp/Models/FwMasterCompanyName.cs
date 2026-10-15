namespace WebApp.Models;

public partial class FwMasterCompanyName
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public decimal? SemiWholeSale { get; set; }

    public decimal? Retail { get; set; }

    public virtual ICollection<FwMasterItem> FwMasterItems { get; set; } = new List<FwMasterItem>();
}
