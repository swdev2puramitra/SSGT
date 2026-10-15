# USP_GET_BILLS Column Update - Fixed

## Issue
```
System.InvalidOperationException: The required column 'Address' was not present 
in the results of a 'FromSql' operation.
```

The `BillsListDto` DTO properties didn't match the actual columns returned by `USP_GET_BILLS` stored procedure.

## Resolution

### 1. Updated BillsListDto Properties
**File:** `WebApp/Models/BillsListDto.cs`

**Actual Columns from USP_GET_BILLS:**
```sql
SlNo, B.BillId, B.BillNo, C.Name CustomerName, C.CustAddress, 
B.TotalBillAmount BillAmount, BillDate, C.MobileNo PhoneNo, Estimate, 
CustomerType, I.InvoiceNo, B.IsInState, B.WayBill, InvoiceStatus
```

**Updated DTO Properties (case-sensitive match to SQL column names):**
```csharp
public int SlNo { get; set; }
public long BillId { get; set; }
public string BillNo { get; set; }
public string CustomerName { get; set; }
public string CustAddress { get; set; }          // Was: Address
public decimal BillAmount { get; set; }
public string BillDate { get; set; }
public long PhoneNo { get; set; }
public string Estimate { get; set; }
public string CustomerType { get; set; }
public long? InvoiceNo { get; set; }
public bool? IsInState { get; set; }             // NEW
public string WayBill { get; set; }              // NEW
public string InvoiceStatus { get; set; }        // NEW
```

### 2. Updated ag-Grid Columns
**File:** `WebApp/Views/Home/ViewBills.cshtml`

**New Grid Columns Added:**
- ✅ "Address" column (field: `custAddress`)
- ✅ "In-State?" column (field: `isInState`, formatted as Yes/No)
- ✅ "Way Bill" column (field: `wayBill`)
- ✅ "Invoice Status" column (field: `invoiceStatus`)

**All columns are:**
- Sortable ✓
- Filterable ✓
- Auto-formatted for their data type ✓

### 3. Grid Action Buttons
The grid maintains the three action buttons:
- 📦 **Packing** (Green)
- ✎ **Edit** (Yellow)
- 🖨 **Print** (Orange)

## Key Mapping Rules

When using `FromSqlRaw()`, property names must **exactly match** SQL column aliases:

| SQL Column Alias | DTO Property |
|------------------|--------------|
| `C.Name` → CustomerName | `public string CustomerName` |
| `C.CustAddress` → CustAddress | `public string CustAddress` |
| `C.MobileNo` → PhoneNo | `public long PhoneNo` |
| `B.IsInState` → IsInState | `public bool? IsInState` |
| Other columns | Match by alias or renamed column |

## Column Display Order in Grid

1. SL
2. Bill No
3. Customer Name
4. Address
5. Customer Type
6. Phone No
7. Bill Amount
8. Bill Date
9. Invoice No
10. Estimate?
11. In-State?
12. Way Bill
13. Invoice Status
14. Actions (pinned right)

## Data Types Handled

- **Numeric:** BillAmount (formatted to 2 decimals)
- **DateTime:** BillDate (filterable as date)
- **Boolean:** IsInState (formatted as Yes/No)
- **String:** All other columns
- **Long/Long?:** BillId, PhoneNo, InvoiceNo (nullable)

## Files Modified

- ✅ `WebApp/Models/BillsListDto.cs` - Updated properties to match SQL columns
- ✅ `WebApp/Views/Home/ViewBills.cshtml` - Updated grid columns and row data mapping

## Build Status
✅ **Successful** - No compilation errors

## How to Ensure Accuracy

When updating the DTO:
1. Check SQL stored procedure SELECT statement
2. Match property names to column aliases (case-insensitive in EF mapping)
3. Match property data types to SQL data types
4. Update grid columns to match new properties
5. Update row data serialization to include new properties
6. Rebuild and test

## Testing Checklist

- [ ] Navigate to `/Home/ViewBills`
- [ ] Verify all 14 columns display
- [ ] Test sorting on each column
- [ ] Test filtering on each column
- [ ] Test pagination
- [ ] Test Packing/Edit/Print buttons
- [ ] Verify numeric formatting (BillAmount)
- [ ] Verify boolean display (IsInState shows Yes/No)
- [ ] Test responsive layout
- [ ] Verify no SQL errors in browser console

---

**Status:** ✅ Ready for Testing
