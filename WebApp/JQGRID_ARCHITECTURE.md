# jqGrid Architecture Diagram

## Component Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    RAZOR VIEW (.cshtml)                          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ <div class="jqgrid-wrapper">                             │   │
│  │   <table id="myGrid"></table>                            │   │
│  │   <div id="myGrid_pager"></div>                          │   │
│  │ </div>                                                   │   │
│  │                                                          │   │
│  │ @section Scripts {                                       │   │
│  │   @await Html.PartialAsync("_JqGridScripts.cshtml")      │   │
│  │   <script>                                               │   │
│  │     JqGridHelper.createPaginatedGrid(...)               │   │
│  │   </script>                                              │   │
│  │ }                                                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
		 │
		 ├─ Includes: _JqGridScripts.cshtml
		 │
		 └─ Calls: JqGridHelper.createPaginatedGrid()

┌─────────────────────────────────────────────────────────────────┐
│                   _JqGridScripts.cshtml                          │
│                    (Shared Partial)                              │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Loads:                                                   │   │
│  │ • jQuery UI CSS (CDN)                                    │   │
│  │ • jqGrid CSS (CDN)                                       │   │
│  │ • jqgrid-custom.css (dark theme)                         │   │
│  │ • jQuery UI JS (CDN)                                     │   │
│  │ • jqGrid JS (CDN)                                        │   │
│  │ • jqgrid-init.js (helper library)                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
		 │
		 ├─ Styling: jqgrid-custom.css
		 │
		 └─ Helper: jqgrid-init.js (JqGridHelper namespace)

┌─────────────────────────────────────────────────────────────────┐
│                      jqgrid-init.js                              │
│                   (JqGridHelper API)                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Methods:                                                 │   │
│  │ • createGrid()         - Basic initialization            │   │
│  │ • createPaginatedGrid() - With pagination (⭐ Use This)  │   │
│  │ • formatCurrency()     - $1,234.56 format              │   │
│  │ • formatDate()         - dd-MM-yyyy format             │   │
│  │ • createActionButton() - Generate action buttons        │   │
│  │ • attachActionHandler()- Event delegation               │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
		 │
		 ├─ Calls API endpoint with pagination params
		 │
		 └─ Renders grid and attaches event handlers

┌─────────────────────────────────────────────────────────────────┐
│                   API ENDPOINT (Controller)                      │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ GET /Controller/GetData?page=1&size=10                   │   │
│  │                                                          │   │
│  │ Returns JSON:                                            │   │
│  │ {                                                        │   │
│  │   "data": [...],         // Row data array              │   │
│  │   "page": 1,             // Current page                │   │
│  │   "records": 100,        // Total records               │   │
│  │   "total": 10            // Total pages                 │   │
│  │ }                                                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                   jqgrid-custom.css                              │
│                   (Dark Theme Styling)                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ CSS Variables (from theme.css):                          │   │
│  │ • --jqgrid-bg-dark: #0d0b09                              │   │
│  │ • --jqgrid-surface: #1a1816                              │   │
│  │ • --jqgrid-text-primary: #ffffff                         │   │
│  │ • --jqgrid-primary-color: #00B0FF                        │   │
│  │                                                          │   │
│  │ Styles:                                                  │   │
│  │ • .ui-jqgrid (container)                                 │   │
│  │ • .ui-jqgrid-htable (header)                             │   │
│  │ • .ui-jqgrid-btable (body)                               │   │
│  │ • .ui-jqgrid-pager (pagination)                          │   │
│  │ • .edit-transfer (action buttons)                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

```
VIEW LOADS
	│
	├─ Includes _JqGridScripts.cshtml
	│
	├─ Initializes JqGridHelper.createPaginatedGrid()
	│
	├─ jqgrid-init.js loads (waits for jQuery & jqGrid)
	│
	├─ jqGrid library initializes
	│
	├─ Grid makes AJAX GET to /Controller/GetData?page=1&size=10
	│
	├─ Controller returns JSON: { data: [...], page: 1, records: 100, total: 10 }
	│
	├─ jqGrid renders rows with column formatters
	│
	├─ Action buttons created via formatter
	│
	├─ Event handlers attached via attachActionHandler()
	│
	└─ Grid Ready! User can:
		├─ Click pagination buttons
		├─ Click sort headers
		├─ Click action buttons → Callbacks execute
		└─ Change page size
```

## Integration Points

```
Your Application
│
├─ Views/Billing/ViewStockTransfers.cshtml (or any page)
│  │
│  ├─ HTML: <div class="jqgrid-wrapper"><table id="gridId"></table></div>
│  │
│  └─ Script:
│     └─ JqGridHelper.createPaginatedGrid(
│          '#gridId',
│          [ /* columns */ ],
│          '/API/GetData'
│        )
│
├─ Controllers/BillingController.cs (or any controller)
│  │
│  └─ public IActionResult GetData(int page = 1, int size = 10)
│     {
│         // Query database
│         // Return JSON with: data, page, records, total
│     }
│
├─ Views/Shared/_JqGridScripts.cshtml
│  │
│  ├─ Links: jQuery UI CSS, jqGrid CSS, jqgrid-custom.css
│  │
│  └─ Scripts: jQuery UI JS, jqGrid JS, jqgrid-init.js
│
└─ wwwroot/
   ├─ css/jqgrid-custom.css (Dark theme styling)
   └─ js/jqgrid-init.js (JqGridHelper API)
```

## Plug & Play Pattern

```
STEP 1: HTML                    STEP 2: Script              STEP 3: API Endpoint
┌──────────────────────┐        ┌──────────────────────────┐  ┌──────────────────┐
│ <div class=           │        │ JqGridHelper.            │  │ public IAction   │
│  "jqgrid-wrapper">    │        │ createPaginatedGrid(     │  │ Result GetData   │
│  <table id="grid">    │        │  '#grid',                │  │ {                │
│  </table>             │        │  [ columns ],            │  │   var total =... │
│  <div id="grid_pager" │        │  '/API/GetData'          │  │   var data = ... │
│  ></div>              │        │ ).then(handler)          │  │   return Json({  │
│ </div>                │        │                          │  │    data,         │
└──────────────────────┘        └──────────────────────────┘  │    page,         │
															   │    records,      │
INCLUDE:                        DEFINE:                       │    total         │
@await Html.PartialAsync(       Column array with:           │   })             │
  "_JqGridScripts.cshtml"       - label, name, width        └──────────────────┘
)                               - formatter (optional)
								- align, sortable

						 ATTACH EVENTS:
						 JqGridHelper.attachActionHandler(
						  '#grid', 'btn-class',
						  callback
						 )
```

## CSS Cascade

```
Global Theme
└─ theme.css
   ├─ CSS Variables
   │  ├─ --color-primary-blue: #00B0FF
   │  ├─ --color-bg-dark: #0d0b09
   │  └─ etc.
   │
   └─ Component Classes
	  └─ (btn, badge, etc.)

jqGrid Theme
└─ jqgrid-custom.css
   ├─ Local CSS Variables (mapped from theme.css)
   │  ├─ --jqgrid-bg-dark: var(--color-bg-dark)
   │  └─ etc.
   │
   └─ Component Styles
	  ├─ .ui-jqgrid
	  ├─ .ui-jqgrid-htable
	  ├─ .ui-jqgrid-btable
	  ├─ .ui-jqgrid-pager
	  └─ .edit-transfer (action buttons)
```

## Reusability Pattern

```
Page 1: Stock Transfers
├─ ViewStockTransfers.cshtml
│  └─ JqGridHelper.createPaginatedGrid(
│     '#stockTransferGrid',
│     StockTransfer columns,
│     '/Billing/GetStockTransfers'
│    )
│
Page 2: Orders (New Page)
├─ ViewOrders.cshtml
│  └─ JqGridHelper.createPaginatedGrid(
│     '#ordersGrid',
│     Order columns,
│     '/Orders/GetOrders'
│    )
│
Page 3: Invoices (New Page)
├─ ViewInvoices.cshtml
│  └─ JqGridHelper.createPaginatedGrid(
│     '#invoicesGrid',
│     Invoice columns,
│     '/Invoices/GetInvoices'
│    )

All pages:
✅ Use same JqGridHelper (no code duplication)
✅ Use same CSS (consistent dark theme)
✅ Follow same patterns (pagination, sorting, actions)
✅ Load same assets (_JqGridScripts.cshtml)
✅ Return same API format (data, page, records, total)
```

## Future Extensibility

```
Current Features:
├─ Basic pagination ✅
├─ Sorting ✅
├─ Action buttons ✅
├─ Custom formatters ✅
└─ Dark theme ✅

Can be Added:
├─ Multi-select rows
├─ Search/filtering
├─ Column visibility toggle
├─ Export to Excel/CSV
├─ Inline editing
├─ Row grouping
├─ Master-detail (expandable rows)
└─ Excel-like column resizing

All without modifying existing pages!
(Just extend JqGridHelper or add CSS)
```
