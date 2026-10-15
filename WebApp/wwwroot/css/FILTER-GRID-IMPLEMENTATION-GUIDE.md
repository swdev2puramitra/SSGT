/* ════════════════════════════════════════════════════════════════════════════

   FILTER & GRID IMPLEMENTATION QUICK START

   This guide shows developers how to implement filter sections, results
   displays, and AG-Grid tables using the new CSS classes.

   ════════════════════════════════════════════════════════════════════════════ */


/* ================================
   FILTER SECTION MARKUP TEMPLATE
================================ */

// BASIC FILTER FORM WITH RESPONSIVE GRID

<div class="filters-section">
	<form method="get" class="filter-form">
		<!-- First Row of Filters -->
		<div class="filter-row">
			<div class="filter-group">
				<label for="filterDate">Date</label>
				<input type="date" id="filterDate" name="date" 
					   class="filter-input" />
			</div>

			<div class="filter-group">
				<label for="filterName">Customer Name</label>
				<input type="text" id="filterName" name="name" 
					   class="filter-input" 
					   placeholder="Enter customer name" />
			</div>

			<div class="filter-group">
				<label for="filterBill">Bill No</label>
				<input type="text" id="filterBill" name="billNo" 
					   class="filter-input" 
					   placeholder="Enter bill number" />
			</div>
		</div>

		<!-- Second Row of Filters & Actions -->
		<div class="filter-row">
			<div class="filter-group">
				<label for="filterType">Type</label>
				<select id="filterType" name="type" class="filter-select">
					<option value="">-- Select --</option>
					<option value="1">Option 1</option>
					<option value="2">Option 2</option>
				</select>
			</div>

			<div class="filter-group">
				<label for="filterStatus">Status</label>
				<select id="filterStatus" name="status" class="filter-select">
					<option value="">-- Select --</option>
					<option value="active">Active</option>
					<option value="inactive">Inactive</option>
				</select>
			</div>

			<!-- Action Buttons (aligned to flex-end) -->
			<div class="filter-actions">
				<button type="submit" class="btn btn-primary">
					<i class="fas fa-search"></i> Search
				</button>
				<a href="@Url.Action('Index')" class="btn btn-secondary">
					<i class="fas fa-redo"></i> Clear
				</a>
			</div>
		</div>
	</form>
</div>


/* ================================
   RESULTS INFO DISPLAY
================================ */

// SHOW RECORD COUNT

<div class="results-info">
	<p>Found <strong>@results.Count</strong> record(s)</p>
</div>

// WITH ADDITIONAL INFO

<div class="results-info">
	<p>
		<strong>@results.Count</strong> result(s) found
		<span class="text-muted">(Page @currentPage of @totalPages)</span>
	</p>
</div>


/* ================================
   AG-GRID TABLE IMPLEMENTATION
================================ */

// STANDARD AG-GRID (600px)

<div id="billsGrid" class="ag-theme-dark ag-grid-standard"></div>

// COMPACT AG-GRID (400px)

<div id="itemsGrid" class="ag-theme-dark ag-grid-compact"></div>

// LARGE AG-GRID (800px)

<div id="detailsGrid" class="ag-theme-dark ag-grid-large"></div>

// FULL HEIGHT AG-GRID

<div id="mainGrid" class="ag-theme-dark ag-grid-full-height"></div>


/* ════════════════════════════════════════════════════════════════════════════
   COMPLETE PAGE EXAMPLE - BILL LISTING PAGE
   ════════════════════════════════════════════════════════════════════════════ */

@{
	ViewData["Title"] = "View Bills";
	var bills = ViewBag.Bills as List<BillDto> ?? new List<BillDto>();
}

<div class="packing-dashboard">
	<!-- Page Header -->
	<div class="dashboard-header">
		<h3 class="mb-0">View Bills</h3>
	</div>

	<!-- Filter Section with ALL NEW CSS Classes -->
	<div class="filters-section">
		<form method="get" action="@Url.Action("ViewBills")" class="filter-form">
			<!-- Row 1: Date, Customer, Bill No, Mobile -->
			<div class="filter-row">
				<div class="filter-group">
					<label for="createdDate">Bill Date</label>
					<input type="date" id="createdDate" name="createdDate"
						   value="@(ViewBag.SelectedCreatedDate?.ToString("yyyy-MM-dd") ?? "")"
						   class="filter-input" />
				</div>

				<div class="filter-group">
					<label for="name">Customer Name</label>
					<input type="text" id="name" name="name"
						   placeholder="Enter customer name"
						   value="@ViewBag.SelectedName"
						   class="filter-input" />
				</div>

				<div class="filter-group">
					<label for="billNo">Bill No</label>
					<input type="text" id="billNo" name="billNo"
						   placeholder="Enter bill number"
						   value="@ViewBag.SelectedBillNo"
						   class="filter-input" />
				</div>

				<div class="filter-group">
					<label for="mobileNo">Mobile No</label>
					<input type="tel" id="mobileNo" name="mobileNo"
						   placeholder="Enter mobile number"
						   value="@ViewBag.SelectedMobileNo"
						   class="filter-input" />
				</div>
			</div>

			<!-- Row 2: Customer Type, Type, and Action Buttons -->
			<div class="filter-row">
				<div class="filter-group">
					<label for="customerType">Customer Type</label>
					<select id="customerType" name="customerType" class="filter-select">
						<option value="">-- Select --</option>
						<option value="1" selected="@(ViewBag.SelectedCustomerType == 1)">Wholesale</option>
						<option value="2" selected="@(ViewBag.SelectedCustomerType == 2)">Semi-Wholesale</option>
						<option value="3" selected="@(ViewBag.SelectedCustomerType == 3)">Retail</option>
					</select>
				</div>

				<div class="filter-group">
					<label for="isEstimate">Type</label>
					<select id="isEstimate" name="isEstimate" class="filter-select">
						<option value="">-- Select --</option>
						<option value="Y" selected="@(ViewBag.SelectedIsEstimate == "Yes")">Estimate</option>
						<option value="N" selected="@(ViewBag.SelectedIsEstimate == "No")">Invoice</option>
					</select>
				</div>

				<!-- Action Buttons -->
				<div class="filter-actions">
					<button type="submit" class="btn btn-primary">
						<i class="fas fa-search"></i> Search
					</button>
					<a href="@Url.Action("ViewBills")" class="btn btn-secondary">
						<i class="fas fa-redo"></i> Clear
					</a>
				</div>
			</div>
		</form>
	</div>

	<!-- Error Message Section -->
	@if (!string.IsNullOrEmpty(ViewBag.Error))
	{
		<div class="alert alert-danger alert-dismissible fade show" role="alert">
			@ViewBag.Error
			<button type="button" class="btn-close" data-bs-dismiss="alert" 
					aria-label="Close"></button>
		</div>
	}

	<!-- Results Count with NEW CSS Class -->
	<div class="results-info">
		<p>Found <strong>@bills.Count</strong> bill(s)</p>
	</div>

	<!-- AG-Grid Container with NEW CSS Class -->
	<div id="pacingGrid" class="ag-theme-dark billing-grid-dark ag-grid-standard"></div>
</div>

@section Scripts {
	<script src="https://cdn.jsdelivr.net/npm/ag-grid-community@30.2.0/dist/ag-grid-community.min.js"></script>
	<script>
		// Initialize AG-Grid
		const gridOptions = {
			theme: 'ag-theme-dark',
			columnDefs: [
				// Column definitions here
			],
			rowData: @Html.Raw(Json.Serialize(bills))
		};

		new agGrid.Grid(document.getElementById('pacingGrid'), gridOptions);
	</script>
}


/* ════════════════════════════════════════════════════════════════════════════
   CSS CLASSES REFERENCE TABLE
   ════════════════════════════════════════════════════════════════════════════ */

CLASS NAME              PURPOSE                         USAGE
────────────────────────────────────────────────────────────────────────────────
.filters-section       Container for entire form       Wrapper div
.filter-form           Form flex layout                Form element
.filter-row            Grid layout for groups          Div containing groups
.filter-group          Single field with label         Label + Input container
.filter-input          Text/Date/Tel inputs            Input elements
.filter-select         Select dropdown                 Select elements
.filter-actions        Action buttons container        Buttons div
.results-info          Results count display           Stats div
.ag-grid-standard      600px height grid               AG-Grid div
.ag-grid-compact       400px height grid               AG-Grid div
.ag-grid-large         800px height grid               AG-Grid div
.ag-grid-small         300px height grid               AG-Grid div
.ag-grid-scrollable    Scrollable grid area            AG-Grid div
.ag-grid-full-height   Full viewport grid              AG-Grid div


/* ════════════════════════════════════════════════════════════════════════════
   COMMON PATTERNS
   ════════════════════════════════════════════════════════════════════════════ */

// PATTERN 1: Simple Search Bar

<div class="filters-section">
	<form method="get" class="filter-form">
		<div class="filter-row">
			<div class="filter-group">
				<label for="search">Search</label>
				<input type="text" id="search" name="q" 
					   class="filter-input" placeholder="Search..." />
			</div>
			<div class="filter-actions">
				<button type="submit" class="btn btn-primary">Search</button>
			</div>
		</div>
	</form>
</div>


// PATTERN 2: Date Range Filters

<div class="filters-section">
	<form method="get" class="filter-form">
		<div class="filter-row">
			<div class="filter-group">
				<label for="fromDate">From Date</label>
				<input type="date" id="fromDate" name="fromDate" 
					   class="filter-input" />
			</div>
			<div class="filter-group">
				<label for="toDate">To Date</label>
				<input type="date" id="toDate" name="toDate" 
					   class="filter-input" />
			</div>
			<div class="filter-actions">
				<button type="submit" class="btn btn-primary">Filter</button>
			</div>
		</div>
	</form>
</div>


// PATTERN 3: Multi-Select Filters

<div class="filters-section">
	<form method="get" class="filter-form">
		<div class="filter-row">
			<div class="filter-group">
				<label for="category">Category</label>
				<select id="category" name="category" class="filter-select">
					<option value="">All Categories</option>
					<option value="cat1">Category 1</option>
					<option value="cat2">Category 2</option>
				</select>
			</div>
			<div class="filter-group">
				<label for="status">Status</label>
				<select id="status" name="status" class="filter-select">
					<option value="">All Status</option>
					<option value="active">Active</option>
					<option value="inactive">Inactive</option>
				</select>
			</div>
			<div class="filter-actions">
				<button type="submit" class="btn btn-primary">Filter</button>
				<a href="#" class="btn btn-secondary">Reset</a>
			</div>
		</div>
	</form>
</div>


/* ════════════════════════════════════════════════════════════════════════════
   RESPONSIVE BEHAVIOR
   ════════════════════════════════════════════════════════════════════════════ */

Desktop (1200px+):
  • filter-row: 4 columns (auto-fit, 200px min)
  • Filters and buttons on same row
  • ag-grid-standard: 600px height

Tablet (768px - 991px):
  • filter-row: 2-3 columns (auto-fit, 200px min)
  • Filters may wrap to second row
  • ag-grid-standard: 400px height (mobile-adjusted)

Mobile (<768px):
  • filter-row: 1 column (full width)
  • Each filter on its own row
  • Buttons stack vertically
  • ag-grid-standard: 400px height (scrollable)


/* ════════════════════════════════════════════════════════════════════════════
   CSS VARIABLE REFERENCE
   ════════════════════════════════════════════════════════════════════════════ */

USED BY FILTER SECTION:
  • background: var(--color-bg-secondary)         #1a1814
  • border: var(--color-border-secondary)         rgba(255,255,255,0.05)
  • color: var(--color-text-primary)              #e0e0e0
  • color: var(--color-text-secondary)            #b0b0b0
  • focus-border: var(--color-accent-primary)     #00d4ff

USED BY RESULTS INFO:
  • background: var(--color-bg-secondary)         #1a1814
  • border: var(--color-border-secondary)         rgba(255,255,255,0.05)
  • strong color: var(--color-accent-primary)     #00d4ff

All colors defined in: WebApp/wwwroot/css/themes/theme-dark.css


/* ════════════════════════════════════════════════════════════════════════════
   DO's AND DON'Ts
   ════════════════════════════════════════════════════════════════════════════ */

✅ DO:
  • Use class="filters-section" for filter containers
  • Use class="filter-input" for text/date/tel inputs
  • Use class="filter-select" for dropdowns
  • Use class="filter-actions" for buttons
  • Use class="ag-grid-standard" for grids
  • Use CSS variables: var(--color-*)
  • Use semantic HTML (label for, id matching)
  • Test responsive behavior

❌ DON'T:
  • Use inline style="..." attributes
  • Hardcode colors (#0d0b09, #e0e0e0, etc.)
  • Mix Bootstrap spacing with custom spacing
  • Use old CSS grid sizing (style="height: 600px")
  • Add new <style> tags in Razor files
  • Use !important in CSS (unless absolutely necessary)
  • Forget to test responsive layouts


/* ════════════════════════════════════════════════════════════════════════════
   BUILD & DEPLOYMENT
   ════════════════════════════════════════════════════════════════════════════ */

Before committing changes:

☐ Run build (F6): dotnet build
☐ Check compilation: ✅ No errors
☐ Test in browser: localhost:5133
☐ Test on mobile: F12 - Responsive mode
☐ Verify filters work: Search, Clear, Filter
☐ Verify grid displays: 600px height, scrolls
☐ Verify styling: Dark theme, no gaps

CSS files involved:
  • WebApp/wwwroot/css/layout.css (filters, results)
  • WebApp/wwwroot/css/components/aggrid.css (grid sizing)
  • WebApp/wwwroot/css/themes/theme-dark.css (colors)
  • WebApp/wwwroot/css/index.css (master import)


════════════════════════════════════════════════════════════════════════════════

VERSION: 1.0
LAST UPDATED: 2024
BUILD STATUS: ✅ SUCCESSFUL
CSS VALIDATION: ✅ COMPLETE

════════════════════════════════════════════════════════════════════════════════
