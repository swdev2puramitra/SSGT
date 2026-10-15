using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWorksEntity
{
    public class TaxInvoiceDtls
    {
        public string Version { get; set; } = "1.1";
        public TransactionDetails TranDtls { get; set; } = new TransactionDetails();
        public DocumentDetails DocDtls { get; set; }
        public BuyerSeller SellerDtls { get; set; } = new BuyerSeller()
        {
            Gstin = "37AEJFS7987P1ZN",
            LglNm = "SRI SRINIVASA ELECTRICALS AND FIREWORKS",
            Addr1 = "SURVEY NO 161/7 , , CHODAVARAM",
            Addr2 = "VENKAYYAGARI PETA NEAR ANNAVARAM WAY TO GAVARAVARAM ROAD",
            Loc = "CHODAVARAM",
            Pin = 531036,
            Stcd = "37",
            Ph = "9440326202",
            Em = "VUPPALASATISH2021@GMAIL.COM"
        };
        public BuyerSeller BuyerDtls { get; set; }
        public AmountDetails ValDtls { get; set; }
        public ReferenceDetails RefDtls { get; set; }
        public List<Items> ItemList { get; set; }
    }
    public class TransactionDetails
    {
        public string TaxSch { get; set; } = "GST";
        public string SupTyp { get; set; } = "B2B";
        public string IgstOnIntra { get; set; } = "N";
        public string RegRev { get; set; } = "N";
        public string EcmGstin { get; set; }
    }
    public class DocumentDetails
    {
        public string Typ { get; set; } = "INV";
        public string No { get; set; }
        public string Dt { get; set; } 
    }
    public class BuyerSeller
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public Int32 Pin { get; set; }
        public string Pos { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }
    public class AmountDetails
    {
        public decimal AssVal { get; set; } = 0;
        public decimal IgstVal { get; set; } = 0;
        public decimal CgstVal { get; set; } = 0;
        public decimal SgstVal { get; set; } = 0;
        public decimal CesVal { get; set; } = 0;
        public decimal StCesVal { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public decimal OthChrg { get; set; } = 0;
        public decimal RndOffAmt { get; set; } = 0;
        public decimal TotInvVal { get; set; } = 0;
    }
    public class ReferenceDetails
    {
        public string InvRm { get; set; } = "NICGEPP2.0";
    }
    public class Items
    {
        public string SlNo { get; set; }
        public string PrdDesc { get; set; }
        public string IsServc { get; set; }
        public string HsnCd { get; set; }
        public int Qty { get; set; }
        public int FreeQty { get; set; } = 0;
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotAmt { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal PreTaxVal { get; set; } = 0;
        public decimal AssAmt { get; set; }
        public decimal GstRt { get; set; }
        public decimal IgstAmt { get; set; }
        public decimal CgstAmt { get; set; } = 0;
        public decimal SgstAmt { get; set; } = 0;
        public decimal CesRt { get; set; } = 0;
        public decimal CesAmt { get; set; } = 0;
        public decimal CesNonAdvlAmt { get; set; } = 0;
        public decimal StateCesRt { get; set; } = 0;
        public decimal StateCesAmt { get; set; } = 0;
        public decimal StateCesNonAdvlAmt { get; set; } = 0;
        public decimal OthChrg { get; set; } = 0;
        public decimal TotItemVal { get; set; }
    }
}
