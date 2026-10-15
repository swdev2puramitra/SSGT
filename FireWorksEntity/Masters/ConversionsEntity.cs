using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWorksEntity.Masters
{
    public class ConversionsEntity
    {
        public int SlNo1 { get; set; }

        public int? ConversionId { get; set; }

        public string ConversionName { get; set; }

        public List<ItemTypeEntity> ItemType { get; set; }

        public char DMLType { get; set; }

        public bool IsActive { get; set; }

    }
}
