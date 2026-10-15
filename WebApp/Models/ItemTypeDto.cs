namespace WebApp.Models
{
    public class ItemTypeDto
    {
        public int ItemTypeId { get; set; }
        public string? TypeName { get; set; }
        public int QtyPerType { get; set; }
        public string? IsActive { get; set; }
    }
}
