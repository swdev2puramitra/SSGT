namespace WebApp.Models;

public partial class FwMasterBill
{
    public long FinYearId { get; set; }

    public int FinYear { get; set; }

    public string BillType { get; set; } = null!;

    public int Bnumber { get; set; }
}
