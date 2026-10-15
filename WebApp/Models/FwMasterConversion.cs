namespace WebApp.Models;

public partial class FwMasterConversion
{
    public int ConversionId { get; set; }

    public string ConversionName { get; set; } = null!;

    public int SlNo { get; set; }

    public int ItemTypeId { get; set; }

    public int ItemOrder { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsActive { get; set; }
}
