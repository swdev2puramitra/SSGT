using System;
using System.Collections.Generic;

namespace FireWorksEntity
{
    public class BilledItems
    {
        public int SlNo { get; set; }
        public int ItemId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public int ItemTypeId { get; set; }

        public string ItemTypeName { get; set; }
        public string UnitName { get; set; }

        public string Detailed { get; set; }
        public string HSNCode { get; set; }

        public int Quantity { get; set; }

        public decimal IndividualCost { get; set; }

        public decimal ItemCost { get; set; }

        public decimal TotalCost { get; set; }
        public decimal OriginalCost { get; set; }

        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? ChangedPrice { get; set; }
        public int? SellTypeId { get; set; }
        public int? SellQty { get; set; }
    }

    public class Bill
    {
        public string BillId { get; set; }
        public string BillNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Int64? PhoneNo { get; set; }
        public Int64? Mobile2 { get; set; }
        public int CustomerType { get; set; }
        public char? IsEstimateInvoice { get; set; }
        public decimal BillAmount { get; set; }
        public decimal? DiscountPerItem { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsInState { get; set; }
        //public decimal? CGST { get; set; }
        //public decimal? CGSTAmt { get; set; }
        //public decimal? SGST { get; set; }
        //public decimal? SGSTAmt { get; set; }
        //public decimal? IGST { get; set; }
        //public decimal? IGSTAmt { get; set; }
        public string CustGSTIN { get; set; }
        //public string HSNCode { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public List<BilledItems> billedItems { get; set; }
        public char DMLType { get; set; }
        public decimal AddlDisc { get; set; }
        public decimal TotalFinalAmount { get; set; }
        public string AddDesc { get; set; }
        public string Pincode { get; set; }
        public string POS { get; set; }
        public string StateCode { get; set; }
        public string Location { get; set; }
    }

    public class rptBill
    {
        public Int64? BillId { get; set; }
        public string BillNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Int64 PhoneNo { get; set; }
        public int CustomerType { get; set; }
        public char? IsEstimateInvoice { get; set; }
        public decimal BillAmount { get; set; }
        public decimal? DiscountPerItem { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsInState { get; set; }
        public decimal? CGST { get; set; }
        public decimal? CGSTAmt { get; set; }
        public decimal? SGST { get; set; }
        public decimal? SGSTAmt { get; set; }
        public decimal? IGST { get; set; }
        public decimal? IGSTAmt { get; set; }
        public string CustGSTIN { get; set; }
        public string HSNCode { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public int SlNo { get; set; }
        public int ItemId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public int ItemTypeId { get; set; }

        public string ItemTypeName { get; set; }

        public string Detailed { get; set; }

        public int Quantity { get; set; }

        public decimal IndividualCost { get; set; }

        public decimal ItemCost { get; set; }

        public decimal TotalCost { get; set; }
    }
}
