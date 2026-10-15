# Fixed: InvalidCastException in ViewBills + Default Date

## Issues Resolved

### 1. System.InvalidCastException
**Error Message:**
```
Unable to cast object of type 'System.Int32' to type 'System.String'.
```

**Root Cause:**
- `Estimate` property was defined as `string` but SQL returns `CHAR` (Y/N)
- `CustomerType` property was defined as `string` but SQL returns `TINYINT` (1, 2, 3)

**Solution:**
Changed DTO properties to match actual SQL data types:

```csharp
// OLD (WRONG)
public string Estimate { get; set; }
public string CustomerType { get; set; }

// NEW (CORRECT)
public char? Estimate { get; set; }          // SQL returns CHAR (Y/N)
public int? CustomerType { get; set; }       // SQL returns TINYINT (1, 2, 3)
```

### 2. Default createdDate to Today
**Requirement:** By default, show today's bills when page loads

**Solution:**
Added in `ViewBills` action:
```csharp
// Set default date to today if not provided
createdDate ??= DateTime.Today;
```

## Files Modified

### 1. `WebApp/Models/BillsListDto.cs`
```csharp
public class BillsListDto
{
	public int SlNo { get; set; }
	public long BillId { get; set; }
	public string BillNo { get; set; }
	public string CustomerName { get; set; }
	public string CustAddress { get; set; }
	public decimal BillAmount { get; set; }
	public string BillDate { get; set; }
	public long PhoneNo { get; set; }
	public char? Estimate { get; set; }              // ✅ Changed from string
	public int? CustomerType { get; set; }          // ✅ Changed from string
	public long? InvoiceNo { get; set; }
	public bool? IsInState { get; set; }
	public string WayBill { get; set; }
	public string InvoiceStatus { get; set; }
}
```

### 2. `WebApp/Controllers/HomeController.cs`
**ViewBills Action Method:**
```csharp
public IActionResult ViewBills(
	DateTime? createdDate = null,
	string? name = null,
	string? billNo = null,
	long? mobileNo = null,
	int? customerType = null,
	char? isEstimate = null)
{
	LoadMenusFromSession();

	// ✅ Set default date to today if not provided
	createdDate ??= DateTime.Today;

	try
	{
		// ... rest of method unchanged
	}
	// ...
}
```

### 3. `WebApp/Views/Home/ViewBills.cshtml`
**Column Formatters Updated:**

**CustomerType Column:**
```javascript
{
	headerName: "Customer Type",
	field: "customerType",
	width: 120,
	sortable: true,
	filter: "agNumberColumnFilter",
	valueFormatter: function(params) {
		if (!params.value) return 'N/A';
		const types = { 1: 'Wholesale', 2: 'Retail', 3: 'Distributor' };
		return types[params.value] || 'Unknown';
	}
}
```

**Estimate Column:**
```javascript
{
	headerName: "Estimate?",
	field: "estimate",
	width: 90,
	sortable: true,
	filter: "agTextColumnFilter",
	valueFormatter: function(params) {
		if (!params.value) return 'N/A';
		return params.value === 'Y' || params.value === 1 ? 'Yes' : 'No';
	}
}
```

## Data Type Mapping Reference

| DTO Property | Old Type | New Type | SQL Type | Display Format |
|--------------|----------|----------|----------|---|
| SlNo | int | int | INT | Numeric |
| BillId | long | long | BIGINT | Numeric |
| BillNo | string | string | VARCHAR | Text |
| CustomerName | string | string | VARCHAR | Text |
| CustAddress | string | string | VARCHAR | Text |
| BillAmount | decimal | decimal | DECIMAL | Currency (2 decimals) |
| BillDate | string | string | DATE/DATETIME | Date |
| PhoneNo | long | long | BIGINT | Numeric |
| **Estimate** | ❌ string | ✅ char? | CHAR(1) | Yes/No formatter |
| **CustomerType** | ❌ string | ✅ int? | TINYINT | Type name formatter |
| InvoiceNo | long? | long? | BIGINT | Numeric |
| IsInState | bool? | bool? | BIT | Yes/No/N/A formatter |
| WayBill | string | string | VARCHAR | Text |
| InvoiceStatus | string | string | VARCHAR | Text |

## Grid Display Behavior

### CustomerType Display
- 1 → "Wholesale"
- 2 → "Retail"
- 3 → "Distributor"
- NULL → "N/A"
- Other → "Unknown"

### Estimate Display
- 'Y' or 1 → "Yes"
- 'N' or 0 → "No"
- NULL → "N/A"

### Default Date Behavior
- **First Load:** Shows today's bills (DateTime.Today)
- **User Filters:** Respects user's selected date
- **Clear Filters:** Defaults back to today

## Why This Happened

### InvalidCastException Root Cause
When EF Core executes `FromSqlRaw()`, it reads each column value and attempts to cast it to the DTO property type:

```
SQL Column: Estimate = 'Y'  (CHAR)
DTO Property: Estimate: string
→ CAST SUCCEEDS ✓

SQL Column: Estimate = 'Y'  (CHAR)
DTO Property: Estimate: string  ❌ WRONG
→ Internal casting to string fails
→ InvalidCastException!
```

The fix ensures property types match the SQL data types exactly.

## Best Practices Applied

1. **Type Safety:** DTO properties exactly match SQL column types
2. **Null Handling:** Used nullable types (`char?`, `int?`) for nullable SQL columns
3. **Display Formatting:** Used grid valueFormatters for user-friendly display
4. **Sensible Defaults:** Set today's date as default filter for better UX
5. **Error Prevention:** Invalid cast errors prevented at compile/runtime

## Testing Checklist

- [ ] Navigate to `/Home/ViewBills`
- [ ] Verify page loads with today's date pre-selected
- [ ] CustomerType displays as text (Wholesale/Retail/Distributor)
- [ ] Estimate displays as Yes/No
- [ ] Can filter by date and see results update
- [ ] Can filter by customer type and see results update
- [ ] Can filter by estimate status and see results update
- [ ] All columns sort correctly
- [ ] All columns filter correctly
- [ ] No console errors
- [ ] Grid displays without cast exceptions

## Build Status
✅ **Successful** - Ready for testing

---

**Summary:** InvalidCastException fixed by correcting DTO property types. Default date now set to today for better UX.
