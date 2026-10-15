using System;
using System.Collections.Generic;

namespace FireWorksEntity
{
    public class GoodsReceiptEntity
    {
        public Int64 StockId { get; set; }
        public int GodownId { get; set; }
        public string GodownName { get; set; }
        public string StockFrom { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string Comments { get; set; }
        public char DMLType { get; set; }

        public List<StockItemsList> stockItems { get; set; }
    }

    public class StockItemsList
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Detailed { get; set; }
        public int ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public Int64 Quantity { get; set; }

    }
}
