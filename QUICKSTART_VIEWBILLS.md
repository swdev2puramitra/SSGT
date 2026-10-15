# Quick Start: ViewBills Page

## What Was Fixed

1. ✅ **InvalidCastException** - DTO types now match SQL types
2. ✅ **Default Date** - Page loads with today's bills
3. ✅ **Grid Formatters** - Data displays correctly (Wholesale/Retail/Distributor, Yes/No, etc.)

## How to Use

### First Visit
```
Navigate to: /Home/ViewBills
Result:     Shows all bills for TODAY (date is pre-selected)
```

### Search/Filter
1. **By Date:** Change the "Bill Date" field
2. **By Customer:** Enter name in "Customer Name"
3. **By Bill #:** Enter number in "Bill No"
4. **By Phone:** Enter mobile number
5. **By Type:** Select from Customer Type dropdown
   - Wholesale
   - Retail
   - Distributor
6. **By Status:** Select from Type dropdown
   - Estimate (Y)
   - Invoice (N)

Click **Search** button to filter.

### Using the Grid

**Sort:** Click any column header (↑↓ arrow appears)

**Filter:** Click filter icon in column header
- Text columns: search by text
- Number columns: search by numbers
- Date columns: pick date range

**Pagination:** Bottom of grid shows page numbers
- Default: 20 rows per page
- Navigate with < > buttons

**Action Buttons (right side):**
- 📦 **Packing** - Open packing workflow
- ✎ **Edit** - Edit bill details
- 🖨 **Print** - Print bill

## Data Displayed

| Column | Type | Example |
|--------|------|---------|
| SL | Number | 1, 2, 3 |
| Bill No | Text | B001, B002 |
| Customer Name | Text | John Smith |
| Address | Text | 123 Main St |
| Customer Type | Dropdown | **Wholesale**, Retail, Distributor |
| Phone No | Number | 9876543210 |
| Bill Amount | Currency | 1,000.00 |
| Bill Date | Date | 2024-01-15 |
| Invoice No | Number | INV001 |
| Estimate? | Yes/No | **Yes**, No |
| In-State? | Yes/No | **Yes**, No |
| Way Bill | Text | AWB123456 |
| Invoice Status | Text | Paid, Pending, etc. |
| Actions | Buttons | Packing, Edit, Print |

## Troubleshooting

### **Issue:** No results shown
- ✅ Check today's date is selected
- ✅ Check if bills exist for today in database
- ✅ Try clearing filters and search again

### **Issue:** Wrong data type showing
- ✅ Refresh page (F5)
- ✅ Clear browser cache
- ✅ Check console for errors (F12)

### **Issue:** Filter not working
- ✅ Make sure to click **Search** button
- ✅ Clear filters and try again
- ✅ Check browser console for errors

### **Issue:** Date field empty
- ✅ Today's date should auto-fill
- ✅ If not, refresh the page
- ✅ If still empty, check server logs

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| Enter | Submit search/filter |
| Tab | Move between fields |
| Ctrl+F | Browser find (to search on page) |
| F12 | Open browser developer tools |

## Mobile Support

✅ **Fully Responsive**
- Filters stack vertically on mobile
- Grid scrolls horizontally if needed
- Touch-friendly buttons
- Auto-adjusts to screen size

## Performance Tips

- 📌 **Use filters** - Narrower results load faster
- 📌 **Set date filter** - Default today, but can narrow further
- 📌 **Avoid filtering on all fields** - Use most relevant filters
- 📌 **Check pagination** - Page 2, 3 etc. load on demand

## Reports/Exports

Currently supported:
- 📋 **View Online** - In grid
- 🖨 **Print** - Click Print button per row
- 🔄 **Refresh** - Click Clear button to reset filters

Future enhancements:
- 📊 Export to Excel
- 📄 Export to PDF
- 📧 Email report

## Common Workflows

### **To Pack a Bill**
1. Find bill in grid
2. Click **Packing** button
3. Follow packing workflow

### **To Edit Bill Details**
1. Find bill in grid
2. Click **Edit** button
3. Update details
4. Save changes

### **To Print a Bill**
1. Find bill in grid
2. Click **Print** button
3. Print or save as PDF

### **To See Tomorrow's Bills**
1. Change "Bill Date" to tomorrow's date
2. Click **Search**
3. Grid updates with tomorrow's bills

## Customer Type Mapping

| ID | Type | Description |
|----|------|-------------|
| 1 | **Wholesale** | Bulk orders, distributor |
| 2 | **Retail** | Small orders, storefront |
| 3 | **Distributor** | Regional distribution |

## Invoice Status Reference

Common statuses:
- **Pending** - Waiting for invoice
- **Generated** - Invoice created
- **Sent** - Sent to customer
- **Paid** - Payment received
- **Cancelled** - Voided

## Support

If you encounter issues:
1. Check this guide first ↑
2. Check browser console (F12) for errors
3. Try refreshing page
4. Clear browser cache
5. Contact IT support if problem persists

---

**Last Updated:** 2024
**Status:** ✅ Production Ready
