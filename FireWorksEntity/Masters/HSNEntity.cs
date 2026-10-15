namespace FireWorksEntity.Masters
{
    public class HSNEntity
    {
        public int? HSNId { get; set; }
        public string HSNCode { get; set; }

        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public char DMLType { get; set; }
        public int? Createdby { get; set; }
        public bool IsActive { get; set; }
    }
}
