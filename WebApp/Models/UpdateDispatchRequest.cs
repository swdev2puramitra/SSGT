namespace WebApp.Models
{
    public class UpdateDispatchRequest
    {
        public long BillId { get; set; }
        public List<PackedItem> PackedItems { get; set; } = new List<PackedItem>();
        public string PackingNotes { get; set; }
    }

    public class PackedItem
    {
        public int ItemId { get; set; }
        public long BillId { get; set; }
        public int GodownId { get; set; }
        public int Qty { get; set; }
        // Quantity to dispatch for this packed item in the current operation
        public int DispatchQty { get; set; }
    }
}
