# Menu System - Quick Reference Guide

## Overview
The WebApp uses a **database-driven, hierarchical menu system** loaded from the `FW_Menus` table with role-based filtering and automatic Under Construction redirects for unimplemented features.

---

## Database Schema (FW_Menus Table)

| Column | Type | Purpose | Required |
|--------|------|---------|----------|
| MenuId | int | Primary key | ✅ |
| MenuName | nvarchar(255) | Display text in navbar | ✅ |
| ParentId | int | Parent menu ID (0 = root) | ✅ |
| UserLevel | byte | Role requirement (matches ClaimTypes.Role) | ✅ |
| SortOrder | int | Display order | ✅ |
| **MenuUrl** | nvarchar(255) | Direct URL override | ❌ Optional |
| **FormName** | nvarchar(255) | Legacy form reference (NOT USED for nav) | ❌ |
| **MenuIcon** | nvarchar(50) | Emoji or icon HTML | ❌ |
| **MenuDescription** | nvarchar(255) | Hover tooltip | ❌ |
| **IsActive** | bit | Enable/disable menu | ✅ Default: 1 |

---

## Menu Hierarchy Rules

### Root Menus (ParentId = 0)
- Top-level items in navbar
- Visible only if they have children or have explicit MenuUrl
- Display order controlled by `SortOrder`

### Parent Menus (HasChildren = true)
- Display as **Bootstrap dropdown**
- URL always returns `#` (no navigation)
- Click expands/collapses child items
- Show on hover (desktop) / click (mobile)

### Leaf Menus (HasChildren = false)
- Single menu item or dropdown item
- **URL Resolution Logic:**
  1. If `MenuUrl` provided → **Use it**
  2. If `MenuUrl` is null → **Redirect to `/Home/UnderConstruction?menuName={MenuName}`**
  3. ⚠️ **FormName is NOT used** (prevents 404s)

---

## URL Resolution Examples

| MenuUrl | FormName | HasChildren | Result |
|---------|----------|------------|--------|
| `/Bills/List` | `Bills/List` | false | → `/Bills/List` |
| `Bills/List` | NULL | false | → `/Bills/List` |
| NULL | `Bills/List` | false | → `/Home/UnderConstruction?menuName=Bills` |
| NULL | NULL | false | → `/Home/UnderConstruction?menuName=Category` |
| NULL | NULL | **true** | → `#` (dropdown, no nav) |

---

## Role-Based Filtering

### UserLevel Mapping
| UserLevel | Role | Menus Visible |
|-----------|------|---------------|
| 1 | User | All menus with UserLevel = 1 |
| 2 | Admin | All menus with UserLevel ≤ 2 |
| 3+ | Extended | Custom roles |

### Example Menu Structure
```
Masters (UserLevel=1, ParentId=0)
├── Category (UserLevel=1, MenuUrl="/Category/List")
├── Sub-Category (UserLevel=1, MenuUrl="/SubCategory/List")
└── Company (UserLevel=1, MenuUrl=null → Under Construction)

Administration (UserLevel=2, ParentId=0)
├── Users (UserLevel=2, MenuUrl="/Admin/Users")
├── Roles (UserLevel=2, MenuUrl="/Admin/Roles")
└── Audit Log (UserLevel=2, MenuUrl=null → Under Construction)
```

---

## Backend Implementation

### MenuService.cs
- `GetMenusByUserLevelAsync(userLevel)` - Loads role-filtered menus with hierarchy
- `GetAllMenusAsync()` - Admin: all menus
- `GetChildMenusAsync(parentId)` - Child items for parent
- **Memory caching** - 60-minute TTL, auto-invalidation support

### MenuDTO.cs
- `FromEntity()` - Entity to DTO conversion with **backend URL resolution**
- `GetUrl()` - Simplified: returns pre-resolved URL
- `HasChildren` - Computed property: true if Children.Count > 0
- `ResolveMenuUrl()` - Private method: generates final URL in backend

### HomeController.cs
- `LoadMenusAsync()` - Called on every page load, filters by user role
- `GetMenus()` - AJAX endpoint returning menu JSON
- `UnderConstruction()` - Renders themed under-construction page

### Program.cs
```csharp
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IMenuService, MenuService>();
```

---

## Frontend Components

### _Navbar.cshtml
- Renders `ViewBag.UserMenus` (populated by HomeController)
- Iterates parent menus with `@foreach (var menu in userMenus)`
- Child menus rendered in Bootstrap dropdown
- Uses `menu.GetUrl()` for all href attributes

### navbar.css
- **Compact spacing** - Reduced padding/margins throughout
- **Hover dropdowns** - Show on hover (desktop only)
- **Responsive breakpoints:**
  - Desktop (>992px): Full navbar with hover dropdowns
  - Tablet (768-992px): Condensed, brand text hidden
  - Mobile (<576px): Hamburger menu, offcanvas sidebar
  - Extra small (<360px): Minimal layout

### site.js
```javascript
// Dropdown hover behavior (desktop only)
document.querySelectorAll('.nav-item.dropdown').forEach(dropdown => {
	dropdown.addEventListener('mouseenter', () => {
		dropdown.querySelector('.dropdown-menu')?.classList.add('show');
	});
	dropdown.addEventListener('mouseleave', () => {
		dropdown.querySelector('.dropdown-menu')?.classList.remove('show');
	});
});
```

---

## Adding/Modifying Menus

### To Add a Menu Item

**For implemented features:**
```sql
INSERT INTO FW_Menus 
(MenuId, MenuName, ParentId, UserLevel, SortOrder, MenuUrl, MenuIcon, MenuDescription, IsActive)
VALUES 
(25, 'My Feature', 2, 1, 50, '/MyFeature/List', '🎯', 'My Feature Description', 1);
```

**For unimplemented features (shows Under Construction):**
```sql
INSERT INTO FW_Menus 
(MenuId, MenuName, ParentId, UserLevel, SortOrder, MenuUrl, MenuIcon, MenuDescription, IsActive)
VALUES 
(26, 'Coming Soon Feature', 2, 1, 60, NULL, '🔨', 'Feature under development', 1);
-- MenuUrl = NULL → Auto-redirects to Under Construction page
```

### To Create a Parent Menu with Children

```sql
-- Parent menu
INSERT INTO FW_Menus 
(MenuId, MenuName, ParentId, UserLevel, SortOrder, MenuUrl, MenuIcon, MenuDescription, IsActive)
VALUES 
(10, 'Reports', 0, 1, 30, NULL, '📊', 'View Reports', 1);

-- Child menus
INSERT INTO FW_Menus 
(MenuId, MenuName, ParentId, UserLevel, SortOrder, MenuUrl, MenuIcon, MenuDescription, IsActive)
VALUES 
(11, 'Sales Report', 10, 1, 10, '/Reports/Sales', '📈', 'Sales Analysis', 1),
(12, 'Purchase Report', 10, 1, 20, '/Reports/Purchase', '📉', 'Purchase Analysis', 1);
```

---

## Cache Management

### Cache Keys
- `all_menus` - All menus (admin)
- `menus_by_level_{userLevel}` - Role-filtered menus

### Invalidating Cache
```csharp
// In MenuService
await _menuService.InvalidateCache();
// Clears all menu cache, forces fresh DB query on next load
```

### Cache Duration
- Default: **60 minutes**
- Controlled by `CACHE_DURATION_MINUTES` constant in MenuService

---

## Under Construction Page

**Triggered When:**
- Menu item has `MenuUrl = NULL` **AND**
- Item has no children (is a leaf menu)

**Behavior:**
- Redirect to `/Home/UnderConstruction?menuName={MenuName}`
- Shows themed page with construction icon
- "Back to Dashboard" button
- Menu name displayed for context

**Example URL:**
```
/Home/UnderConstruction?menuName=Category
/Home/UnderConstruction?menuName=Sub-Category
```

---

## Performance Considerations

1. **Caching** - Menus cached in memory for 60 minutes per user role
2. **Lazy Loading** - Child menus only loaded when parent expanded
3. **No N+1 Queries** - Menu hierarchy built in application layer
4. **Indexes** - Database indexes on ParentId and (UserLevel, IsActive)
5. **Payload Size** - Compact JSON for dropdown menus

---

## Troubleshooting

### Menu Not Showing
- Check `IsActive = 1` in database
- Verify `UserLevel` matches user's role claim
- Confirm `ParentId` is correct (0 for root)
- Check `SortOrder` value

### 404 Error on Menu Click
- **Do NOT set FormName** - Only MenuUrl controls navigation
- Verify `MenuUrl` path is correct (e.g., `/Bills/List`)
- For unimplemented features, set `MenuUrl = NULL`

### Dropdown Not Opening
- Desktop: Check hover effect in browser DevTools
- Mobile: Click parent menu to toggle
- Verify `HasChildren = true` (children must exist)
- Check CSS/JS aren't disabled

### Wrong User Sees Menu
- Verify user's `ClaimTypes.Role` value
- Check menu `UserLevel` matches role ID
- Confirm user is authenticated (not guest)
- Check role assignment in database

---

## Migration Checklist

When deploying menu changes:

1. **Database Changes**
   ```sql
   -- Apply FW_MENUS_MIGRATION_SCRIPT.sql if first time
   -- Add new menus as needed
   ```

2. **Code Changes**
   - Update MenuService if query logic changes
   - Update MenuDTO if new properties added
   - Clear cache if structure changes

3. **Testing**
   - Test navigation as different user roles
   - Test Under Construction redirects
   - Test on mobile/tablet/desktop
   - Verify responsive menu behavior

4. **Deployment**
   - Deploy code changes
   - Run database migrations
   - Warm cache on first request
   - Monitor browser console for JS errors

---

## See Also
- `UI-THEME-PROGRESS.md` - Implementation progress
- `WebApp/Services/MenuService.cs` - Service implementation
- `WebApp/Models/MenuDTO.cs` - Data model
- `WebApp/Views/Shared/_Navbar.cshtml` - View component
- `WebApp/wwwroot/css/theme/navbar.css` - Styling
