using System.Collections.Generic;

namespace FireWorksEntity
{
    public class ItemsEntity
    {
        public int? SlNo { get; set; }

        public int? ItemId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public int ItemType { get; set; }

        public string TypeName { get; set; }

        public int CategoryId { get; set; }

        public string CatDescription { get; set; }

        public int SubCategoryId { get; set; }

        public string SubCatDescription { get; set; }

        public int DefaultSaleType { get; set; }

        public string DefaultSale { get; set; }

        public decimal WholeSalePrice { get; set; }

        public decimal SemiWholeSalePrice { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal SemiWholeSaleMargin { get; set; }

        public decimal RetailMargin { get; set; }

        public decimal ItemPrice { get; set; }

        public decimal CGST { get; set; }
        
        public decimal SGST { get; set; }
        
        public decimal IGST { get; set; }

        public char DMLType { get; set; }

        public int ConversionId { get; set; }

        public int HSNId { get; set; }

        public string ConversionName { get; set; }

        public bool IsActive { get; set; }
        public string Stock { get; set; }

        public List<ItemConversionQuantity> quantities { get; set; }
    }

    public class ItemConversionQuantity
    {
        public int ItemTypeId { get; set; }

        public string ItemTypeName { get; set; }

        public int Quantity { get; set; }
        public int QuantityPerType { get; set; }
        public decimal WholeSale { get; set; }
        public decimal SemiWholeSale { get; set; }
        public decimal Retail { get; set; }
    }
}
