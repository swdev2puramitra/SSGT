namespace WebApp.Models
{
    public class ItemLowestPriceDto
    {
        public int RowId { get; set; }          // column 1
        public decimal Price { get; set; }      // column 2
        public int QtyPerType { get; set; }     // column 3
        public int ItemTypeId { get; set; }     // column 4
        public int ConversionId { get; set; }   // column 5
        public string TypeName { get; set; }    // column 6
    }

}
