namespace WebApp.Models;

public partial class FwMasterCustomer
{
    public long CustomerId { get; set; }

    public string? Name { get; set; }

    public long? MobileNo { get; set; }

    public byte CustomerType { get; set; }

    public string? CustAddress { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? Mobile2 { get; set; }
}
