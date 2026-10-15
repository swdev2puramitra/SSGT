namespace WebApp.Models;

public partial class FwMasterCategory
{
    public int Id { get; set; }

    public string CatDescription { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public int? CatOrder { get; set; }

    public virtual ICollection<FwMasterItem> FwMasterItems { get; set; } = new List<FwMasterItem>();

    public virtual ICollection<FwMasterSubCategory> FwMasterSubCategories { get; set; } = new List<FwMasterSubCategory>();
}
