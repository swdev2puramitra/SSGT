# CSS LAYER SYSTEM - QUICK REFERENCE

## 📁 File Structure

```
wwwroot/css/
├── index.css                     ← Master importer (include ONLY this in _Layout.cshtml)
├── layout.css                    ← Page structure, grids, containers
├── utilities.css                 ← Helper classes (spacing, text, display)
├── print.css                     ← Print-specific styles
├── themes/
│   └── theme-dark.css            ← Color variables & theme configuration
└── components/
	├── tables.css                ← Table styles (packing tables)
	├── buttons.css               ← Button variants (primary, secondary, etc.)
	├── controls.css              ← Forms, inputs, alerts
	└── aggrid.css                ← AG Grid customization
```

## 🎯 Quick Start

### 1. Include Master CSS in _Layout.cshtml
```html
<link rel="stylesheet" href="~/css/index.css" asp-append-version="true" />
```

### 2. Use Class-Based Styling Only
```html
<!-- ✅ Correct -->
<div class="packing-dashboard mb-3">
	<h3 class="fw-600">Title</h3>
</div>

<!-- ❌ Wrong -->
<div style="background: #0d0b09; padding: 15px;">
	<h3 style="font-weight: 600;">Title</h3>
</div>
```

### 3. Reference CSS Architecture Guide
Read `CSS_ARCHITECTURE_GUIDE.md` for comprehensive documentation.

### 4. View Markup Examples
Check `RAZOR_MARKUP_EXAMPLES.html` for common patterns.

---

## 📋 File Purposes

### Master Layer
- **index.css** — Orchestrates all CSS layer imports in correct order

### Theme Layer
- **theme-dark.css** — CSS variables, colors, root-level config

### Layout Layer
- **layout.css** — Navbar, containers, grids, page structure, responsive design

### Component Layers
- **tables.css** — Packing table styles, row states, cell alignment
- **buttons.css** — All button variants, groups, icon buttons
- **controls.css** — Forms, inputs, alerts, validation feedback
- **aggrid.css** — AG Grid grid customization & theming

### Utilities Layer
- **utilities.css** — Spacing, text, display, flex helpers, print utilities

### Print Layer
- **print.css** — Print media queries, page setup, element hiding

---

## 🚀 Common Tasks

### Add a New Page Section
1. Create class in `layout.css`
2. Use in Razor: `<div class="my-section">...</div>`

### Add New Component (e.g., Cards)
1. Create `components/cards.css`
2. Add import line to `index.css`
3. Define `.card` and variant styles

### Customize Colors
1. Edit `themes/theme-dark.css`
2. Update CSS variables (or add new ones)
3. Use `var(--color-xyz)` in other layers

### Change Button Style
1. Edit `.btn` or `.btn-primary` in `components/buttons.css`
2. Rebuild (F5 in browser)

### Hide Element on Print
1. Add `hidden-print` class: `<button class="hidden-print">Print</button>`
2. That's it! (Print CSS handles the rest)

### Add Spacing to Element
1. Use utility classes: `mb-2`, `mt-1`, `px-2`, `p-3`
2. Or define using `margin`/`padding` in appropriate CSS file

### Create Form Field
1. Wrap in `form-group` class
2. Add `label` with `for` attribute
3. Add `input`, `select`, or `textarea` with class
4. Optional: add `form-text` for help text

```html
<div class="form-group">
	<label for="email">Email</label>
	<input type="email" id="email" class="form-control" />
	<span class="form-text">Enter valid email</span>
</div>
```

---

## ✨ Key CSS Variables

```css
/* Colors */
--color-accent-primary: #00d4ff;        /* Cyan - main accent */
--color-accent-warning: #ffd700;        /* Gold - warnings */

/* Backgrounds */
--color-bg-primary: #0d0b09;            /* Darkest - page bg */
--color-bg-secondary: #1a1814;          /* Darker - components */
--color-bg-tertiary: #222018;           /* Dark - headers, hover */

/* Text */
--color-text-primary: #e0e0e0;          /* Main text */
--color-text-secondary: #b0b0b0;        /* Secondary text */
--color-text-muted: #808080;            /* Disabled, hints */

/* Borders */
--color-border-primary: rgba(255,255,255,0.1);    /* 10% opacity */
--color-border-secondary: rgba(255,255,255,0.05); /* 5% opacity */
```

Use these instead of hardcoding colors!

---

## 📐 Spacing Scale

```
m-0        → 0
mb-2       → 0.5rem (8px)
mt-1       → 0.25rem (4px)
mt-3       → 1rem (16px)
px-2       → 0.5rem (left & right)
py-2       → 0.5rem (top & bottom)
```

---

## 🎨 Button Variants

```html
<button class="btn btn-primary">Primary</button>
<button class="btn btn-secondary">Secondary</button>
<button class="btn btn-print">Print</button>
<button class="btn btn-success">Success</button>
<button class="btn btn-danger">Danger</button>
<button class="btn btn-warning">Warning</button>
<button class="btn btn-icon"><i class="fas fa-cog"></i></button>
```

---

## 🚨 Alert Variants

```html
<div class="alert alert-warning">
	<i class="fas fa-exclamation-triangle"></i>
	<span>Warning message</span>
</div>

<div class="alert alert-success">
	<i class="fas fa-check-circle"></i>
	<span>Success message</span>
</div>

<div class="alert alert-danger">
	<i class="fas fa-times-circle"></i>
	<span>Error message</span>
</div>

<div class="alert alert-info">
	<i class="fas fa-info-circle"></i>
	<span>Info message</span>
</div>
```

---

## 📱 Responsive Breakpoints

### Desktop (default)
- 2-column grid for godowns
- Full spacing
- All UI elements visible

### Tablet/Mobile (`max-width: 991px`)
- 1-column grid for godowns
- Reduced padding/margins
- Navbar becomes collapsible
- Buttons stack vertically
- Text and font sizes reduced

Add more breakpoints in `layout.css` as needed.

---

## 🖨️ Print Behavior

### Automatically Hidden on Print
- `.custom-navbar` (application header)
- `.btn-print` (print button)
- `.packing-actions` (action buttons)
- Any element with `hidden-print` class

### Automatically Styled for Print
- White background, black text
- Page margins: 1cm
- Section: `page-break-inside: avoid`
- All colors converted to grayscale

### How to Use
```html
<!-- This shows normally, hides on print -->
<button class="btn hidden-print">Don't Print Me</button>

<!-- Visible normally AND on print -->
<div class="alert alert-info">This prints too</div>
```

---

## 🔍 CSS Loading Order

1. **Bootstrap** - Base utilities (kept for grid/tooltips)
2. **Font Awesome** - Icons
3. **AG Grid** - Grid CSS (if used)
4. **index.css** - Master importer containing:
   - Theme (colors, variables)
   - Layout (structure)
   - Components (buttons, tables, forms)
   - Utilities (helpers)
   - Print (media queries)

---

## ⚠️ Important Rules

✅ **DO:**
- Use CSS variable names
- Add classes to HTML elements
- Keep styles in `.css` files
- Use semantic class names
- Reference utilities for common patterns
- Test print output
- Comment complex rules

❌ **DON'T:**
- Use `style="..."` attributes
- Add `<style>` tags in views
- Hardcode color hex values
- Use `!important` (indicates specificity issue)
- Mix Bootstrap classes with custom classes without caution
- Forget to include icon `<i>` elements in buttons/alerts

---

## 📞 File Reference

| Need to... | Edit file... |
|------------|--------------|
| Change theme colors | `themes/theme-dark.css` |
| Add page section | `layout.css` |
| Adjust spacing globally | `utilities.css` |
| Fix table styling | `components/tables.css` |
| Add button variant | `components/buttons.css` |
| Add form control | `components/controls.css` |
| Hide element on print | Add `hidden-print` class or edit `print.css` |
| Customize AG Grid | `components/aggrid.css` |

---

## 🎓 Further Reading

- **CSS_ARCHITECTURE_GUIDE.md** — Comprehensive guide with examples
- **RAZOR_MARKUP_EXAMPLES.html** — Live markup examples for Razor views
- Browser DevTools (F12) — Inspect element, check computed styles

---

## ✅ Checklist Before Deploying

- [ ] No inline `style="..."` attributes in views
- [ ] No `<style>` tags in Razor files
- [ ] All styling uses class names
- [ ] CSS variables used for colors (not hardcoded hex)
- [ ] Print preview tested and looks correct
- [ ] Responsive design tested on mobile
- [ ] Build is successful (no errors)
- [ ] Dark theme variables match design
- [ ] Button and alert styles are correct

---

**Last Updated:** 2024
**Theme:** Dark (Cyan Accent)
**Breakpoint:** 991px (tablet/mobile)
**Main Font:** Arial, Helvetica, sans-serif
**Base Font Size:** 12px
