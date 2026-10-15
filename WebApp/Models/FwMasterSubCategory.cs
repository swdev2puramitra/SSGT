namespace WebApp.Models;

public partial class FwMasterSubCategory
{
    public int Id { get; set; }

    public string SubCatDescription { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual FwMasterCategory? Category { get; set; }

    public virtual ICollection<FwMasterItem> FwMasterItems { get; set; } = new List<FwMasterItem>();
}
