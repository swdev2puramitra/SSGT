using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Helpers;

public sealed class ItemQuantityConversionHelper
{
    private readonly SsgtContext _context;

    public ItemQuantityConversionHelper(SsgtContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public (int normalizedQty, decimal normalizedAmount) ConvertToLowestLevel(
        int itemId,
        int itemTypeId,
        int quantity,
        decimal rate)
    {
        if (itemId <= 0 || itemTypeId <= 0 || quantity <= 0 || rate < 0m)
            return (0, 0m);

        var qtyPerType = _context.FwMasterItemsConversions
            .AsNoTracking()
            .Where(x => x.ItemId == itemId && x.ItemTypeId == itemTypeId)
            .Select(x => x.QtyPerType)
            .FirstOrDefault() ?? 1;

        if (qtyPerType <= 0)
            qtyPerType = 1;

        var normalizedQty = checked(quantity * qtyPerType);
        return (normalizedQty, normalizedQty * rate);
    }
}