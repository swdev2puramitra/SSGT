# Quick Reference: Keeping DTO Properties in Sync with Stored Procedures

## When You Add New Columns to USP_GET_BILLS (or Any Stored Procedure)

### Step 1: Update the DTO
**File:** `WebApp/Models/BillsListDto.cs`

For each new column in the SQL SELECT statement:
```sql
-- Add new column to SELECT
SELECT SlNo, BillId, BillNo, ... NEW_COLUMN_NAME

-- DTO must have matching property
public MyDataType NewColumnName { get; set; }
```

### Step 2: Update the Grid (if displaying data)
**File:** `WebApp/Views/Home/ViewBills.cshtml`

In the `columnDefs` array:
```javascript
{
	headerName: "Display Label",
	field: "newColumnName",      // Exact case match to rowData property
	width: 120,
	sortable: true,
	filter: "agTextColumnFilter" // or appropriate filter type
}
```

### Step 3: Update Row Data
In the same file, update the `rowData` serialization:
```javascript
rowData: @Html.Raw(System.Text.Json.JsonSerializer.Serialize(bills.Select(b => new {
	// ... existing properties ...
	newColumnName = b.NewColumnName  // ADD THIS
})))
```

### Step 4: Test
- Rebuild solution
- Run application
- Navigate to the page
- Verify column appears with data
- Check browser console for errors

## Common Data Type Mappings

| SQL Type | C# Type | DTO Property | Grid Filter |
|----------|---------|--------------|-------------|
| INT | int | `public int PropertyName` | `"agNumberColumnFilter"` |
| BIGINT | long | `public long PropertyName` | `"agNumberColumnFilter"` |
| BIGINT NULL | long? | `public long? PropertyName` | `"agNumberColumnFilter"` |
| DECIMAL | decimal | `public decimal PropertyName` | `"agNumberColumnFilter"` |
| VARCHAR/NVARCHAR | string | `public string PropertyName` | `"agTextColumnFilter"` |
| DATE/DATETIME | string | `public string PropertyName` | `"agDateColumnFilter"` |
| BIT/BOOLEAN | bool | `public bool PropertyName` | `"agTextColumnFilter"` |
| BIT/BOOLEAN NULL | bool? | `public bool? PropertyName` | `"agTextColumnFilter"` |

## Common Mistakes to Avoid

❌ **Wrong:** Property name doesn't match SQL column alias
```sql
SELECT NewColumn AS SomeAlias  -- SQL column is "SomeAlias"
// But in DTO:
public string NewColumn { get; set; }  // Property doesn't match!
```

✅ **Correct:** Property name matches SQL column alias (case-insensitive in EF)
```sql
SELECT NewColumn AS NewColumn
// In DTO:
public string NewColumn { get; set; }
```

❌ **Wrong:** Missing property in row data selector
```javascript
rowData: @Html.Raw(System.Text.Json.JsonSerializer.Serialize(bills.Select(b => new {
	billId = b.BillId,
	// Missing: newColumn = b.NewColumn
})))
```

✅ **Correct:** All used properties included
```javascript
rowData: @Html.Raw(System.Text.Json.JsonSerializer.Serialize(bills.Select(b => new {
	billId = b.BillId,
	newColumn = b.NewColumn
})))
```

## Troubleshooting

### Error: "The required column 'PropertyName' was not present"
- Check SQL SELECT statement - column exists?
- Check DTO property name - case sensitive?
- Check SQL column alias - does it match DTO property?

### Grid column shows [object Object]
- Check data type formatting needed
- Add `valueFormatter` function:
```javascript
valueFormatter: function(params) {
	if (params.value === null) return 'N/A';
	return params.value.toString();
}
```

### Grid column is empty
- Check row data serialization includes the property
- Check property name matches field name (camelCase in JS)
- Check data exists in database

### Grid column not filterable
- Check filter type is appropriate for data type
- Verify column has `filter: "agTextColumnFilter"` or similar
- Check data type matches (don't filter numeric as text)

## Checklist for Adding New Columns

- [ ] Column added to SQL SELECT statement with alias
- [ ] Property added to BillsListDto with matching name
- [ ] Property data type matches SQL column type
- [ ] Property is nullable if SQL column can be NULL
- [ ] Column definition added to grid columnDefs array
- [ ] Field name in grid matches camelCase DTO property name
- [ ] Property included in row data .Select() statement
- [ ] Solution rebuilt successfully
- [ ] Page tested in browser
- [ ] No console errors
- [ ] Data displays correctly
- [ ] Sorting works
- [ ] Filtering works

---

**Apply to all stored procedure DTOs and views using this pattern.**
