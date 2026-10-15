# jqGrid Quick Reference - Copy & Paste Template

## Basic Grid Template (Copy This!)

### Step 1: HTML (in your .cshtml view)
```html
<div class="container-fluid mt-0 mb-1">
	<div class="dashboard-header">
		<h3>Your Page Title</h3>
	</div>

	<div class="jqgrid-wrapper">
		<table id="myGrid"></table>
		<div id="myGrid_pager"></div>
	</div>
</div>
```

### Step 2: Script (in your @section Scripts)
```razor
@section Scripts {
	@await Html.PartialAsync("~/Views/Shared/_JqGridScripts.cshtml")
	<script>
		JqGridHelper.createPaginatedGrid(
			'#myGrid',
			[
				{ label: 'ID', name: 'id', width: 50, hidden: true, key: true },
				{ label: 'Name', name: 'name', width: 200 },
				{ label: 'Email', name: 'email', width: 200 },
				{ label: 'Date', name: 'createdDate', width: 150 },
				{
					label: 'Actions',
					name: 'actions',
					width: 100,
					align: 'center',
					sortable: false,
					formatter: function(v, opt) {
						return JqGridHelper.createActionButton('Edit', opt.rowId, 'edit-btn btn btn-sm btn-primary');
					}
				}
			],
			'/YourController/GetData'
		).then(function(gridApi) {
			JqGridHelper.attachActionHandler('#myGrid', 'edit-btn', function(id, rowData) {
				window.location.href = '/YourController/Edit/' + id;
			});
		});
	</script>
}
```

### Step 3: C# API Endpoint
```csharp
public IActionResult GetData(int page = 1, int size = 10)
{
	var query = _context.YourEntity.AsQueryable();
	var total = query.Count();

	var data = query
		.OrderByDescending(x => x.Id)
		.Skip((page - 1) * size)
		.Take(size)
		.Select(x => new
		{
			x.Id,
			x.Name,
			x.Email,
			CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy")
		})
		.ToList();

	var lastPage = (int)Math.Ceiling(total / (double)size);

	return Json(new
	{
		data = data,
		page = page,
		records = total,
		total = lastPage
	});
}
```

---

## Common Column Types

### Regular Column
```javascript
{ label: 'Name', name: 'name', width: 150 }
```

### Currency
```javascript
{ 
	label: 'Amount', 
	name: 'amount', 
	width: 100, 
	align: 'right',
	formatter: function(v) { 
		return JqGridHelper.formatCurrency(v); 
	}
}
```

### Date
```javascript
{ 
	label: 'Date', 
	name: 'date', 
	width: 120,
	formatter: function(v) { 
		return JqGridHelper.formatDate(v); 
	}
}
```

### Custom Status Badge
```javascript
{ 
	label: 'Status', 
	name: 'status', 
	width: 100,
	formatter: function(v) { 
		var color = v === 'Active' ? 'success' : 'warning';
		return '<span class="badge badge-' + color + '">' + v + '</span>';
	}
}
```

### Multiple Actions
```javascript
{
	label: 'Actions',
	name: 'actions',
	width: 150,
	align: 'center',
	sortable: false,
	formatter: function(v, opt) {
		return JqGridHelper.createActionButton('View', opt.rowId, 'view-btn btn btn-sm btn-info') + ' ' +
			   JqGridHelper.createActionButton('Edit', opt.rowId, 'edit-btn btn btn-sm btn-warning') + ' ' +
			   JqGridHelper.createActionButton('Delete', opt.rowId, 'delete-btn btn btn-sm btn-danger');
	}
}
```

---

## Common Event Handlers

### Single Action
```javascript
JqGridHelper.attachActionHandler('#myGrid', 'edit-btn', function(id) {
	window.location.href = '/Controller/Edit/' + id;
});
```

### Multiple Actions
```javascript
JqGridHelper.attachActionHandler('#myGrid', 'view-btn', function(id) {
	window.location.href = '/Controller/View/' + id;
});

JqGridHelper.attachActionHandler('#myGrid', 'edit-btn', function(id) {
	window.location.href = '/Controller/Edit/' + id;
});

JqGridHelper.attachActionHandler('#myGrid', 'delete-btn', function(id) {
	if (confirm('Are you sure?')) {
		// Handle delete
	}
});
```

### Action with Row Data
```javascript
JqGridHelper.attachActionHandler('#myGrid', 'approve-btn', function(id, rowData) {
	console.log('Approving:', rowData);
	// Send to server
	fetch('/Controller/Approve/' + id, { method: 'POST' })
		.then(r => r.json())
		.then(data => jQuery('#myGrid').jqGrid('setGridParam', {}).trigger('reloadGrid'));
});
```

---

## Grid Options (Advanced)

Pass options object to customize behavior:
```javascript
JqGridHelper.createPaginatedGrid(
	'#myGrid',
	columns,
	'/GetData',
	{
		rowNum: 20,              // Rows per page
		rowList: [10, 20, 50],   // Available page sizes
		sortorder: 'asc',        // Sort order
		multiselect: false,      // Multi-select rows
		rownumbers: true,        // Show row numbers
		height: 'auto',          // Grid height
	}
)
```

---

## Complete Example (Checklist)

```markdown
- [ ] Create HTML grid container (copy template above)
- [ ] Include _JqGridScripts.cshtml partial
- [ ] Define column configuration array
- [ ] Set API endpoint URL
- [ ] Call JqGridHelper.createPaginatedGrid()
- [ ] Attach action handlers
- [ ] Create API endpoint in controller
- [ ] Test data loads correctly
- [ ] Test pagination works
- [ ] Test sorting works (if enabled)
- [ ] Test action buttons work
- [ ] Verify styling matches dark theme
```

---

## File Locations Reference

| What | Where | File |
|------|-------|------|
| Initialize Grid | Any view | Use `JqGridHelper.createPaginatedGrid()` |
| Include Assets | Script section | `@await Html.PartialAsync("~/Views/Shared/_JqGridScripts.cshtml")` |
| Styling | CSS | Auto-loaded from `wwwroot/css/jqgrid-custom.css` |
| Helper Library | JS | Auto-loaded from `wwwroot/js/jqgrid-init.js` |
| Dark Theme Colors | CSS | Defined in `wwwroot/css/theme/theme.css` |

---

## Troubleshooting Quick Fix

**Grid not showing?**
→ Check browser console → Look for JavaScript errors

**Data not loading?**
→ Open Network tab → Check API response has `data`, `page`, `records`, `total`

**Buttons not working?**
→ Verify button CSS class matches `attachActionHandler` selector → Check console for errors

**Wrong styling?**
→ Check `jqgrid-custom.css` is included → Verify CSS variables are set

---

## Need Full Documentation?
See `JQGRID_IMPLEMENTATION_GUIDE.md` in the WebApp project root.
