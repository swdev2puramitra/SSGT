namespace WebApp.Models;

public partial class FwMasterHsn
{
    public long Hsnid { get; set; }

    public string Hsncode { get; set; } = null!;

    public decimal Cgst { get; set; }

    public decimal Sgst { get; set; }

    public decimal Igst { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual FwUserDetail CreatedByNavigation { get; set; } = null!;
}
