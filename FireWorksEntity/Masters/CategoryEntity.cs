namespace FireWorksEntity.Masters
{
    public class CategoryEntity
    {
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }
        public char DMLType { get; set; }
        public int? Createdby { get; set; }

        public bool IsActive { get; set; }

    }
}
