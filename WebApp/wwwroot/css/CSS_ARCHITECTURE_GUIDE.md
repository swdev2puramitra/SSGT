# CSS ARCHITECTURE GUIDE

## Overview

This project uses a **layered CSS architecture** to organize styles into modular, maintainable layers. All styles are external—no inline `style=""` attributes or `<style>` tags in Razor views.

**Master Stylesheet:** `~/css/index.css`
- Single import in `Views/Shared/_Layout.cshtml`
- Orchestrates all CSS layers in the correct order

---

## CSS Layer Structure

### Layer 1: Themes (`~/css/themes/`)

**Purpose:** Color schemes, CSS variables, and root-level configuration.

**Files:**
- `theme-dark.css` — Dark theme (primary, currently in use)
- `theme-light.css` — (Optional) Light theme
- `theme-custom.css` — (Optional) Additional themes

**Key Variables Defined:**
```css
:root {
	--color-accent-primary: #00d4ff;
	--color-accent-secondary: #00a8cc;
	--color-accent-warning: #ffd700;

	--color-bg-primary: #0d0b09;
	--color-bg-secondary: #1a1814;
	--color-bg-tertiary: #222018;

	--color-text-primary: #e0e0e0;
	--color-text-secondary: #b0b0b0;
	--color-text-muted: #808080;
	--color-text-hint: #606060;

	--color-border-primary: rgba(255, 255, 255, 0.1);
	--color-border-secondary: rgba(255, 255, 255, 0.05);
}
```

**When to Modify:**
- Add new color theme
- Adjust accent or text colors across the entire app
- Define new CSS variables for reuse

---

### Layer 2: Layout (`~/css/layout.css`)

**Purpose:** Structural elements—navbar, page containers, grids, headers, spacing.

**Contains:**
- `.custom-navbar` — Application navigation bar
- `.packing-dashboard` — Main content container
- `.dashboard-header` — Page header section
- `.header-top` — Header with flex layout
- `.bill-info` — Bill details container
- `.godowns-grid` — 2-column responsive grid for godown sections
- `.godown-section` — Individual godown container
- `.godown-header` — Godown header with title and case count
- `.packing-actions` — Action button container
- Responsive design rules (`@media (max-width: 991px)`)

**Example Usage in Razor:**
```html
<div class="packing-dashboard">
	<div class="dashboard-header">
		<div class="header-top">
			<h3 class="mb-0">Packing Details</h3>
			<button class="btn btn-print hidden-print">Print</button>
		</div>
	</div>
	<div class="godowns-grid">
		<!-- Godown sections go here -->
	</div>
</div>
```

**When to Modify:**
- Add new page sections or containers
- Adjust spacing or grid layout
- Add responsive breakpoints
- Define component structure styles

---

### Layer 3: Components (`~/css/components/`)

**Purpose:** Styles for reusable UI components—buttons, tables, forms, AG Grid.

**Files:**

#### `tables.css`
- `.packing-table` — Main table structure
- Table header, body, cell styles
- `.packed-item` — Opacity for completed items
- Hover states and compact padding

**Example:**
```html
<table class="packing-table">
	<thead>
		<tr>
			<th>Item</th>
			<th>Stock</th>
		</tr>
	</thead>
	<tbody>
		<tr class="packed-item">
			<td class="item-name">Product A</td>
			<td class="stock-info">Rack 1</td>
		</tr>
	</tbody>
</table>
```

#### `buttons.css`
- `.btn` — Base button style
- `.btn-primary` — Primary action button
- `.btn-secondary` — Secondary action button
- `.btn-print` — Print button (outlined)
- `.btn-success`, `.btn-danger`, `.btn-warning` — Status buttons
- `.btn-group` — Button groups
- `.btn-icon` — Icon-only circular buttons

**Example:**
```html
<button class="btn btn-primary">
	<i class="fas fa-save"></i> Save
</button>
<a href="#" class="btn btn-secondary">Cancel</a>
<button class="btn btn-icon" title="Settings">
	<i class="fas fa-cog"></i>
</button>
```

#### `controls.css`
- `.form-group` — Form field container
- Input styles (`input`, `select`, `textarea`)
- `.alert` — Alert boxes (warning, success, danger, info)
- Validation feedback (`.is-invalid`, `.is-valid`)
- Help text (`.form-text`)

**Example:**
```html
<div class="form-group">
	<label for="name">Customer Name</label>
	<input type="text" id="name" class="form-control" />
	<span class="form-text">Enter customer full name</span>
</div>

<div class="alert alert-warning">
	<i class="fas fa-exclamation"></i>
	<span>Please verify the data before submitting.</span>
</div>
```

#### `aggrid.css`
- AG Grid container and cell styles
- Header, body, row styling
- Selection, pagination, filter menu
- Overlay styles (no data, loading)

---

### Layer 4: Utilities (`~/css/utilities.css`)

**Purpose:** Helper classes for spacing, text, display, alignment.

**Spacing Utilities:**
- `m-0`, `mb-0`, `mb-2`, `mb-3` — Margin helpers
- `mt-0`, `mt-1`, `mt-3` — Margin-top helpers
- `me-2` — Margin-right helpers
- `p-0`, `px-2`, `py-2` — Padding helpers

**Text Utilities:**
- `text-muted` — Muted gray text
- `text-secondary` — Secondary text color
- `text-accent` — Accent color (cyan)
- `text-center`, `text-end`, `text-start` — Alignment
- `fw-bold`, `fw-600`, `fw-500` — Font weight

**Display Utilities:**
- `d-flex` — `display: flex`
- `d-grid` — `display: grid`
- `d-none` — `display: none`
- `d-block` — `display: block`

**Flex Utilities:**
- `flex-between` — Space-between with centered items
- `flex-center` — Center items
- `gap-2`, `gap-3` — Gap between flex items

**Other Utilities:**
- `border-bottom` — Bottom border
- `opacity-50`, `opacity-75` — Opacity levels
- `w-100` — Full width
- `h-auto` — Auto height
- `hidden-print` — Hide on print (shows normally, hides in `@media print`)

**Example Usage:**
```html
<div class="d-flex flex-between gap-3 mb-3">
	<h3 class="m-0 fw-600">Title</h3>
	<button class="btn btn-primary">Action</button>
</div>

<div class="text-center mt-2">Centered text</div>
```

**When to Add:**
- New spacing scale needed
- Repeated ad-hoc alignment patterns
- Common display patterns used across views

---

### Layer 5: Print (`~/css/print.css`)

**Purpose:** Print-specific styles via `@media print` rules.

**Key Behaviors:**
- Hides `.custom-navbar` (application header)
- Hides `.btn-print`, `.packing-actions`
- Sets white background and black text
- Disables hover/active states
- Adds `page-break-inside: avoid` to `.godown-section`
- Configures `@page` margin (1cm)

**How It Works:**
- The `hidden-print` utility class is overridden in `@media print`
- Interactive elements (buttons, actions) are hidden by default in print
- Print preview/output shows only content and compact layouts

**Example:**
```html
<!-- This button hides on print -->
<button class="btn btn-print hidden-print">Print</button>

<!-- These hide on print -->
<div class="packing-actions hidden-print">
	<button>Save</button>
</div>
```

---

## How to Use the Stylesheet System

### Include in _Layout.cshtml

```html
<head>
	<!-- Other meta, title, etc. -->

	<!-- Bootstrap & Font Awesome (kept for base utilities) -->
	<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
	<link rel="stylesheet" href="~/lib/font-awesome-6/css/all.min.css" />

	<!-- Master CSS (imports all layers) -->
	<link rel="stylesheet" href="~/css/index.css" asp-append-version="true" />
</head>
```

### Add New Styles

**Do NOT:**
```html
<style>
	.my-class { color: red; }
</style>
```

**Instead:**
1. Identify which layer the style belongs to
2. Add to the appropriate file in that layer
3. Use CSS variables from `theme-dark.css`
4. Reference in Razor using the class name

**Example: Adding a new container style**

Edit `~/css/layout.css`:
```css
.my-new-container {
	background: var(--color-bg-secondary);
	padding: 15px;
	border: 1px solid var(--color-border-secondary);
	border-radius: 3px;
}
```

Use in Razor:
```html
<div class="my-new-container">
	<p>My content here</p>
</div>
```

---

## Rules for Razor Views

### ✅ Correct

```html
<div class="packing-dashboard">
	<h3 class="mb-3 fw-600">Title</h3>
	<div class="d-flex gap-2 flex-between">
		<span>Left</span>
		<span class="text-accent">Right</span>
	</div>
</div>
```

### ❌ Incorrect

```html
<div style="background: #0d0b09; padding: 15px;">
	<h3 style="margin-bottom: 12px; font-weight: 600;">Title</h3>
	<style>
		.custom { display: flex; }
	</style>
</div>
```

---

## Common Patterns

### Button with Icon
```html
<button class="btn btn-primary">
	<i class="fas fa-save"></i> Save
</button>
```

### Flex Layout (Space Between)
```html
<div class="d-flex flex-between gap-2">
	<span>Label</span>
	<span>Value</span>
</div>
```

### Compact Table
```html
<table class="packing-table">
	<thead>
		<tr>
			<th>Column 1</th>
			<th>Column 2</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>Data</td>
			<td>Data</td>
		</tr>
	</tbody>
</table>
```

### Form Group
```html
<div class="form-group">
	<label for="name">Name</label>
	<input type="text" id="name" class="form-control" />
</div>
```

### Alert
```html
<div class="alert alert-warning">
	<i class="fas fa-exclamation-triangle"></i>
	<span>Warning message</span>
</div>
```

### Hide on Print
```html
<button class="btn btn-print hidden-print">Print this page</button>
```

---

## Customization & Extension

### Themes
To create a light theme:
1. Copy `~/css/themes/theme-dark.css` to `~/css/themes/theme-light.css`
2. Update CSS variables with light colors
3. Add import to `index.css`:
   ```css
   @media (prefers-color-scheme: light) {
	   @import url('themes/theme-light.css');
   }
   ```

### Add Component Styles
1. Create new file: `~/css/components/my-component.css`
2. Define styles
3. Import in `index.css` within Layer 3

### Responsive Design
- Default: Desktop/tablet
- `@media (max-width: 991px)` — Tablet/mobile adjustments (defined in `layout.css`)
- Add more breakpoints as needed

---

## Key Files Reference

| File | Purpose |
|------|---------|
| `index.css` | Master importer; orchestrates all layers |
| `themes/theme-dark.css` | Color variables, root configuration |
| `layout.css` | Page structure, grids, spacing |
| `components/tables.css` | Table styles |
| `components/buttons.css` | Button variants |
| `components/controls.css` | Forms, inputs, alerts |
| `components/aggrid.css` | AG Grid customization |
| `utilities.css` | Helper classes |
| `print.css` | Print media queries |

---

## Tips & Best Practices

1. **Use CSS variables** — `var(--color-accent-primary)` instead of hardcoding colors
2. **Avoid `!important`** — Indicates CSS order issue; check layer priority
3. **Keep utilities lightweight** — Small, single-purpose classes
4. **Name classes semantically** — `.error-message` not `.red-text`
5. **Test responsive** — Use media queries and test on mobile
6. **Print preview** — Always test print output; use `hidden-print` for interactive UI
7. **Compact spacing** — Use `6px`, `8px`, `12px`, `15px` padding/margins
8. **Consistent font sizes** — Default `12px` for body, `14px`, `24px` for headings
9. **Border radius** — Use `3px` or `4px` for consistency
10. **Flex over floats** — Use flexbox for layouts; avoid legacy float-based layouts

---

## Troubleshooting

### Styles not applying?
1. Check if class name is spelled correctly
2. Verify CSS file is in the import chain in `index.css`
3. Check browser dev tools for CSS specificity issues
4. Clear browser cache and hard refresh (Ctrl+Shift+R)

### Print looks wrong?
1. Check if element has `hidden-print` class
2. Verify `@media print` rules in `print.css`
3. Use browser print preview (Ctrl+P)
4. Check for `!important` overrides

### Colors off?
1. Verify CSS variables are defined in `theme-dark.css`
2. Check if using correct variable name
3. Ensure `theme-dark.css` is imported first in `index.css`

