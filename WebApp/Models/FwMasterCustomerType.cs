namespace WebApp.Models
{
    public partial class FwMasterCustomerType
    {
        public int TypeId { get; set; }
        public string CustTypeName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
