namespace WebApp.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class PackingDto
    {
        public Int64 BillId { get; set; }
        public Int32 ItemId { get; set; }
        public string BillNo { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int BillQty { get; set; }
        public decimal ItemCost { get; set; }
        public decimal TotalCost { get; set; }
        public int GodownId { get; set; }
        public string GodownDescription { get; set; }
        public Int32 GodownStock { get; set; }
        [NotMapped]
        public string StockDescription { get; set; } = string.Empty;
        [NotMapped]
        public string DispatchStockDescription { get; set; } = string.Empty;
        public int AllocatedQty { get; set; }
        // Total dispatched quantity across allocations/dispatches for this item on the bill
        public int TotalDispatchedQty { get; set; }
        // Remaining quantity = AllocatedQty - TotalDispatchedQty (computed by caller)
        public int RemainingQty { get; set; }
        // Quantity user is requesting to dispatch in this update/operation
        public int? DispatchQty { get; set; }
        public string AllocatedStockPosition { get; set; }
        public int AllocatedNoOfCases { get; set; }
        public int SlNo { get; set; }
        public bool? IsPacked { get; set; }

        // Packing history fields - populated when item is packed
        public string? PackedByUserName { get; set; }
        public DateTime? PackedDateTime { get; set; }
        public int? QuantityPacked { get; set; }
    }
}
