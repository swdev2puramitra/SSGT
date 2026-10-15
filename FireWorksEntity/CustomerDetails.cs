using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWorksEntity
{
    public class CustomerDetails
    {
        public string Name { get; set; }

        public Int64 MobileNo { get; set; }

        public int CustomerType { get; set; }

        public string Address { get; set; }
        public bool IsSuccess { get; set; }
    }
}
