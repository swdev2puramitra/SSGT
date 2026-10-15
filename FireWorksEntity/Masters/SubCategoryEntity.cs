namespace FireWorksEntity.Masters
{
    public class SubCategoryEntity
    {
        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }

        public char DMLType { get; set; }

        public bool IsActive { get; set; }
    }
}
