namespace FireWorksEntity.Masters
{
    public class ItemTypeEntity
    {
        public int SlNo { get; set; }
        public int? ItemTypeId { get; set; }

        public string ItemTypeName { get; set; }

        public char DMLType { get; set; }

        public int ItemOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
