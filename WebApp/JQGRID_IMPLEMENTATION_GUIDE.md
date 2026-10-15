# jqGrid Plug & Play Implementation Guide

## Overview
This guide explains how to use the jqGrid implementation for any page in the application. The infrastructure is designed to be reusable across multiple pages with minimal configuration.

## Architecture

### Components Created
1. **jqgrid-custom.css** - Dark theme styling matching the existing UI/UX
2. **jqgrid-init.js** - Shared helper library (`JqGridHelper`) for grid initialization
3. **_JqGridScripts.cshtml** - Partial view to include jqGrid assets
4. **ViewStockTransfers.cshtml** - Example implementation

### Why Plug & Play?
- **Centralized CSS**: All theme styling is in one place
- **Shared Helper**: Common functionality via `JqGridHelper`
- **Minimal Code**: Each page only needs to define columns and specify a data URL
- **Consistent UX**: All grids follow the same visual style

## How to Use (Step by Step)

### Step 1: Include jqGrid Assets
In your Razor view's `@section Scripts`, include:
```razor
@await Html.PartialAsync("~/Views/Shared/_JqGridScripts.cshtml")
```

### Step 2: Create Grid Container
Add this HTML to your view (replace `gridId` with your grid ID):
```html
<div class="jqgrid-wrapper">
	<table id="gridId"></table>
	<div id="gridId_pager"></div>
</div>
```

### Step 3: Initialize Grid with Helper
In your script section:
```javascript
JqGridHelper.createPaginatedGrid(
	'#gridId',
	[
		{ label: 'Column 1', name: 'field1', width: 100 },
		{ label: 'Column 2', name: 'field2', width: 150 },
		// ... more columns
	],
	'/Controller/GetData'  // API endpoint
).then(function(gridApi) {
	// Optional: attach event handlers
	JqGridHelper.attachActionHandler('#gridId', 'edit-button', function(id, rowData) {
		// Handle action
	});
}).catch(function(err) {
	console.error('Grid initialization failed:', err);
});
```

## API Response Format

Your server endpoint must return JSON in this format:
```json
{
	"data": [
		{ "id": 1, "field1": "value1", "field2": "value2" },
		{ "id": 2, "field1": "value3", "field2": "value4" }
	],
	"page": 1,
	"records": 100,
	"total": 10
}
```

**Field Definitions:**
- `data` - Array of row objects (required)
- `page` - Current page number (required for jqGrid)
- `records` - Total number of records (required for jqGrid)
- `total` - Total number of pages (required for jqGrid)

### C# Example (ASP.NET Core)
```csharp
public IActionResult GetData(int page = 1, int size = 10)
{
	var query = _context.YourEntity.AsQueryable();
	var total = query.Count();

	var data = query
		.Skip((page - 1) * size)
		.Take(size)
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

## Column Configuration

Each column is a JavaScript object with properties:

```javascript
{
	label: 'Column Header',      // Display name (required)
	name: 'fieldName',           // Data field name (required)
	width: 100,                  // Column width in pixels
	align: 'center',             // 'left', 'center', 'right'
	sortable: false,             // Enable/disable sorting
	hidden: false,               // Hide column from view
	key: false,                  // Set to true for primary key (usually ID)
	formatter: function(cellValue, options) {
		// Custom cell rendering
		return cellValue;
	}
}
```

### Common Formatters

**Action Button:**
```javascript
{
	label: 'Actions',
	name: 'actions',
	width: 100,
	align: 'center',
	sortable: false,
	formatter: function(cellValue, options) {
		var id = options.rowId;
		return JqGridHelper.createActionButton(
			'✎ Edit',
			id,
			'edit-button btn btn-sm btn-primary'
		);
	}
}
```

**Currency:**
```javascript
{
	label: 'Amount',
	name: 'amount',
	width: 100,
	align: 'right',
	formatter: function(cellValue, options) {
		return JqGridHelper.formatCurrency(cellValue);
	}
}
```

**Date:**
```javascript
{
	label: 'Date',
	name: 'createdDate',
	width: 120,
	formatter: function(cellValue, options) {
		return JqGridHelper.formatDate(cellValue);
	}
}
```

## Event Handling

### Action Button Handler
```javascript
JqGridHelper.attachActionHandler('#gridId', 'edit-button', function(id, rowData) {
	console.log('Edit row:', id, rowData);
	// Your action logic here
});
```

The handler receives:
- `id` - The row ID
- `rowData` - The complete row data object

## Styling

### Dark Theme Colors (from theme.css)
All colors are automatically applied via CSS variables in `jqgrid-custom.css`:
- Background: `#0d0b09`
- Surface: `#1a1816`
- Text Primary: `#ffffff`
- Text Secondary: `#b0b0b0`
- Primary Color: `#00B0FF`

### Custom Styling
To customize styling for a specific grid, add CSS:
```css
/* Target specific grid */
.jqgrid-wrapper .ui-jqgrid {
	/* Your custom styles */
}

/* Target specific buttons */
.jqgrid-wrapper .my-custom-button {
	background-color: #ff6d00;
}
```

## Advanced Features

### Get Selected Row
```javascript
var rowId = jQuery('#gridId').jqGrid('getGridParam', 'selrow');
```

### Get All Selected Rows
```javascript
var rowIds = jQuery('#gridId').jqGrid('getGridParam', 'selarrrow');
```

### Get Row Data
```javascript
var rowData = jQuery('#gridId').jqGrid('getRowData', rowId);
```

### Reload Grid
After the grid is initialized:
```javascript
jQuery('#gridId').jqGrid('setGridParam', {
	url: '/Controller/GetData',
	postData: { filter: 'value' }
}).trigger('reloadGrid', [{ page: 1 }]);
```

### Access Grid API
The `createPaginatedGrid` returns a promise with the grid API:
```javascript
JqGridHelper.createPaginatedGrid(/* ... */).then(function(gridApi) {
	gridApi.reload({ extraParam: 'value' });
});
```

## Real-World Example: Orders Grid

### HTML
```html
<div class="container-fluid">
	<div class="dashboard-header">
		<h3>Orders</h3>
	</div>

	<div class="jqgrid-wrapper">
		<table id="ordersGrid"></table>
		<div id="ordersGrid_pager"></div>
	</div>
</div>
```

### JavaScript
```javascript
JqGridHelper.createPaginatedGrid(
	'#ordersGrid',
	[
		{ label: 'Order ID', name: 'orderId', width: 80, key: true },
		{ label: 'Customer', name: 'customerName', width: 150 },
		{ 
			label: 'Amount', 
			name: 'amount', 
			width: 100, 
			align: 'right',
			formatter: function(v) { return JqGridHelper.formatCurrency(v); }
		},
		{ 
			label: 'Date', 
			name: 'orderDate', 
			width: 120,
			formatter: function(v) { return JqGridHelper.formatDate(v); }
		},
		{
			label: 'Status',
			name: 'status',
			width: 100,
			formatter: function(v) { return '<span class="badge">' + v + '</span>'; }
		},
		{
			label: 'Actions',
			name: 'actions',
			width: 120,
			align: 'center',
			sortable: false,
			formatter: function(v, opt) {
				return JqGridHelper.createActionButton('View', opt.rowId, 'view-btn btn btn-sm btn-info') + ' ' +
					   JqGridHelper.createActionButton('Edit', opt.rowId, 'edit-btn btn btn-sm btn-warning');
			}
		}
	],
	'/Orders/GetOrders'
).then(function(gridApi) {
	JqGridHelper.attachActionHandler('#ordersGrid', 'view-btn', function(id) {
		window.location.href = '/Orders/View/' + id;
	});

	JqGridHelper.attachActionHandler('#ordersGrid', 'edit-btn', function(id) {
		window.location.href = '/Orders/Edit/' + id;
	});
});
```

### C# Controller
```csharp
public IActionResult GetOrders(int page = 1, int size = 10)
{
	var query = _context.Orders
		.Include(o => o.Customer)
		.AsQueryable();

	var total = query.Count();

	var orders = query
		.OrderByDescending(o => o.OrderId)
		.Skip((page - 1) * size)
		.Take(size)
		.Select(o => new
		{
			o.OrderId,
			o.Customer.CustomerName,
			o.Amount,
			OrderDate = o.OrderDate.ToString("dd-MM-yyyy"),
			o.Status
		})
		.ToList();

	var lastPage = (int)Math.Ceiling(total / (double)size);

	return Json(new
	{
		data = orders,
		page = page,
		records = total,
		total = lastPage
	});
}
```

## Migration Checklist for New Pages

- [ ] Add `<div class="jqgrid-wrapper"><table id="gridId"></table><div id="gridId_pager"></div></div>`
- [ ] Include `@await Html.PartialAsync("~/Views/Shared/_JqGridScripts.cshtml")`
- [ ] Define column configuration array
- [ ] Create/update API endpoint to return correct format (data, page, records, total)
- [ ] Initialize grid with `JqGridHelper.createPaginatedGrid()`
- [ ] Attach action handlers if needed
- [ ] Test pagination, sorting, and actions
- [ ] Update any print stylesheets if grid will be printed

## Troubleshooting

### Grid not appearing
- Check browser console for errors
- Verify jQuery and jqGrid CDN links are loading
- Ensure grid container ID matches selector in initialization

### Data not loading
- Check network tab - API request should complete successfully
- Verify API response format matches specification
- Check `page`, `records`, `total` fields are present

### Action buttons not working
- Ensure button has `data-id` attribute
- Verify CSS class matches `attachActionHandler` selector
- Check browser console for JavaScript errors

### Styling issues
- Verify jqgrid-custom.css is included
- Check CSS variable definitions in `:root`
- Use browser dev tools to inspect actual CSS applied

## Files Reference

| File | Purpose | Location |
|------|---------|----------|
| jqgrid-custom.css | Dark theme styling | `wwwroot/css/` |
| jqgrid-init.js | Helper library | `wwwroot/js/` |
| _JqGridScripts.cshtml | Asset partial | `Views/Shared/` |
| ViewStockTransfers.cshtml | Example page | `Views/Billing/` |

## Support

For issues or questions, refer to:
- **jqGrid Documentation**: http://www.trirand.com/jqgridwiki/doku.php
- **Example Implementation**: `ViewStockTransfers.cshtml`
- **Helper API**: Methods in `jqgrid-init.js` are well-commented
