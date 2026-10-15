using System.Text;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class ItemTypeDescriptionHelper
    {
        // Replicates FN_GET_ItemTypewiseDesc SQL function logic in C# so it can be reused across the app
        public static string GetItemTypewiseDescription(SsgtContext context, int itemId, int quantity)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (itemId <= 0 || quantity <= 0) return string.Empty;

            // Build ordered conversion rows for the item
            var rows = (from ic in context.FwMasterItemsConversions
                        join i in context.FwMasterItems on ic.ItemId equals i.ItemId
                        join c in context.FwMasterConversions on i.ConversionId equals c.ConversionId
                        join t in context.FwMasterItemTypes on ic.ItemTypeId equals t.Id
                        where ic.ItemId == itemId && i.ConversionId != null && c.ItemTypeId == ic.ItemTypeId
                        orderby c.ItemOrder
                        select new
                        {
                            TypeName = t.TypeName,
                            QtyPerType = ic.QtyPerType ?? 0
                        }).ToList();

            if (rows == null || rows.Count == 0) return string.Empty;

            int bal = 0;
            var sb = new StringBuilder();
            for (int idx = 0; idx < rows.Count; idx++)
            {
                var row = rows[idx];
                int denom = row.QtyPerType == 0 ? 1 : row.QtyPerType;
                int count = 0;
                if (idx == 0)
                {
                    count = quantity / denom;
                    bal = quantity % denom;
                }
                else
                {
                    count = bal / denom;
                    bal = bal % denom;
                }

                sb.Append(count.ToString()).Append(' ').Append(row.TypeName).Append("s ");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
