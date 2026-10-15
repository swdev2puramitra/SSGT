using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWorksEntity
{
    public class SavedBills
    {
        public int SlNo { get; set; }
        public Int64 BillId { get; set; }
        public string BillNo { get; set; }
        public string Name { get; set; }
        public decimal BillAmount { get; set; }
        public string BillDate { get; set; }
        public Int64? Mobile { get; set; }
        public string Estimate { get; set; }
        public string CustType { get; set; }
        public string InvoiceNo { get; set; }
    }
}
