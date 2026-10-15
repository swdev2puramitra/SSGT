using System.Data;

namespace FireWorksEntity
{
    public static class ShopDetails
    {
        public static string ShopName { get; set; }
        public static string Address { get; set; }
        public static string GST { get; set; }
        public static string PhoneNo { get; set; }
        public static string LicenceNo { get; set; }
        public static string BankName { get; set; }
        public static string BranchName { get; set; }
        public static string AccountNo { get; set; }
        public static string IFSC { get; set; }
        public static DataTable dtShopDetails { get; set; }
    }
}
