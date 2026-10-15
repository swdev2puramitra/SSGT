namespace FireWorksEntity.Masters
{    
    public class GodownEntity
    {
        public int? GodownId { get; set; }

        public string GodownName { get; set; }
        public string GodownAddress { get; set; }
        public char DMLType { get; set; }
        public int? Createdby { get; set; }

        public bool IsActive { get; set; }

    }
}
