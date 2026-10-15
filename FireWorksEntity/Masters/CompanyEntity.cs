namespace FireWorksEntity.Masters
{
    public class CompanyEntity
    {
        public int? CompanyId { get; set; }

        public string CompanyName { get; set; }
        public decimal SemiWholeSale { get; set; }
        public decimal Retail { get; set; }

        public char DMLType { get; set; }

        public bool IsActive { get; set; }
    }
}
