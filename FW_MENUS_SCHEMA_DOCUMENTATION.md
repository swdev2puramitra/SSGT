# FW_Menus Table Schema Documentation

## Overview
The `FW_Menus` table stores application menu items with hierarchical support (parent-child relationships). Menus are role-based (UserLevel) and dynamically loaded based on the current user's role.

## Current Schema (Required Columns)

| Column | Type | Length | NULL | Key | Default | Description |
|--------|------|--------|------|-----|---------|-------------|
| MenuID | int | - | NO | PK | - | Unique menu identifier |
| MenuName | varchar | 50 | NO | - | - | Display name of the menu item |
| ParentID | int | - | NO | - | 0 | Parent menu ID for hierarchy (0 = root level) |
| UserLevel | tinyint | - | NO | - | - | Role/Permission level (maps to user's RoleId) |
| SortOrder | int | - | NO | - | 0 | Display order of menu items |
| FormName | varchar | 50 | YES | - | NULL | Controller/Action URL (e.g., 'Home/Index', 'Bills/List') |

## Recommended Additional Columns

### MenuIcon (Optional but Recommended)
```
Type: varchar(50)
NULL: YES
Description: Menu icon emoji or icon class (e.g., '📊', '⚡', 'fas fa-home')
```

### MenuDescription (Optional)
```
Type: varchar(255)
NULL: YES
Description: Tooltip text displayed on hover
```

### IsActive (Optional but Recommended)
```
Type: bit
NULL: NO
Default: 1 (true)
Description: Controls menu visibility without deleting records
```

### MenuUrl (Optional)
```
Type: varchar(255)
NULL: YES
Description: Override URL if different from FormName (e.g., 'https://external-app.com')
```

## Database Migration Script

### Add Recommended Columns
```sql
-- Add optional columns to FW_Menus table
ALTER TABLE FW_Menus
	ADD MenuIcon VARCHAR(50) NULL,
		MenuDescription VARCHAR(255) NULL,
		IsActive BIT NOT NULL DEFAULT 1,
		MenuUrl VARCHAR(255) NULL;
```

### Sample Data Structure

```sql
-- Root level menus (ParentID = 0)
INSERT INTO FW_Menus (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, IsActive)
VALUES 
	(1, 'Transactions', 0, 1, 10, NULL, '💼', 1),
	(2, 'Masters', 0, 1, 20, NULL, '⚙️', 1),
	(3, 'Reports', 0, 1, 30, NULL, '📊', 1),
	(4, 'Administration', 0, 2, 40, NULL, '🔧', 1);

-- Sub-level menus (ParentID > 0)
INSERT INTO FW_Menus (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, IsActive)
VALUES 
	-- Transactions submenu
	(10, 'Bills', 1, 1, 10, 'Bills/List', '📄', 1),
	(11, 'Invoices', 1, 1, 20, 'Invoices/List', '🧾', 1),
	(12, 'Payments', 1, 1, 30, 'Payments/List', '💳', 1),

	-- Masters submenu
	(20, 'Customers', 2, 1, 10, 'Customers/List', '👥', 1),
	(21, 'Items', 2, 1, 20, 'Items/List', '📦', 1),
	(22, 'Categories', 2, 1, 30, 'Categories/List', '🏷️', 1),

	-- Reports submenu
	(30, 'Sales Report', 3, 1, 10, 'Reports/Sales', '📈', 1),
	(31, 'Purchase Report', 3, 1, 20, 'Reports/Purchase', '📉', 1),

	-- Admin submenu
	(40, 'Users', 4, 2, 10, 'Admin/Users', '👨‍💼', 1),
	(41, 'Roles', 4, 2, 20, 'Admin/Roles', '🔐', 1),
	(42, 'Audit Log', 4, 2, 30, 'Admin/AuditLog', '📋', 1);
```

## Menu Hierarchy Rules

### UserLevel (Role-Based Access)
- **UserLevel = 1**: Regular User
- **UserLevel = 2**: Admin/Supervisor
- **UserLevel = 3**: Manager (if applicable)

Each menu item specifies which roles can access it via the `UserLevel` column.

### ParentID (Hierarchy)
- **ParentID = 0**: Root level menu (appears in main navbar)
- **ParentID > 0**: Submenu (appears in dropdown under parent)

### SortOrder
Menus are sorted by this value in ascending order within their level.

## URL Generation

The `FormName` field should contain the controller/action in one of these formats:

### Format 1: Controller/Action
```
Examples: 'Bills/List', 'Customers/Edit', 'Reports/Sales'
Generated URL: /bills/list (automatically converted to lowercase)
```

### Format 2: Full Path with Action
```
Examples: 'Home/Index', 'Account/Login'
Generated URL: /home/index
```

### Format 3: External URL (via MenuUrl)
```
If MenuUrl is specified, it overrides FormName
Example: MenuUrl = 'https://external-app.com'
Generated URL: https://external-app.com
```

## Example Menu Structure

```
Dashboard (Home - hardcoded in navbar)
├── Transactions (Root, ParentID=0, UserLevel=1)
│   ├── Bills (ParentID=1, UserLevel=1)
│   ├── Invoices (ParentID=1, UserLevel=1)
│   └── Payments (ParentID=1, UserLevel=1)
├── Masters (Root, ParentID=0, UserLevel=1)
│   ├── Customers (ParentID=2, UserLevel=1)
│   ├── Items (ParentID=2, UserLevel=1)
│   └── Categories (ParentID=2, UserLevel=1)
└── Administration (Root, ParentID=0, UserLevel=2)
	├── Users (ParentID=4, UserLevel=2)
	├── Roles (ParentID=4, UserLevel=2)
	└── Audit Log (ParentID=4, UserLevel=2)
```

## Key Considerations

### 1. UserLevel Filtering
Menus are filtered by the current user's `ClaimTypes.Role` claim value, which should match the `UserLevel` in the database.

### 2. Active Flag
Set `IsActive = 0` to hide a menu without deleting it from the database.

### 3. Caching
Menus are cached for 60 minutes. After updating the database, caching is automatically invalidated when:
- The application restarts
- Manual cache invalidation is called via the service

### 4. Null Handling
- `FormName` can be NULL for parent menus without direct navigation
- `MenuIcon` and `MenuDescription` are optional
- `MenuUrl` overrides `FormName` if both are specified

## Testing Menu Access

After populating the database, test menu visibility:

1. **Test with different user roles**
   - Login as User (UserLevel=1) → Should see basic menus
   - Login as Admin (UserLevel=2) → Should see admin menus

2. **Test hierarchy**
   - Verify parent menus show dropdown
   - Verify child menus appear in dropdown
   - Click menu items → Verify correct page loads

3. **Test sorting**
   - Adjust `SortOrder` values
   - Refresh page → Verify menus reorder correctly

4. **Test icons and descriptions**
   - Add emoji icons → Verify display in navbar
   - Hover over menu → Verify tooltip appears

## Application Code Integration

### Service Methods Available

```csharp
// Get menus for a specific user role
var menus = await menuService.GetMenusByUserLevelAsync(userLevel);

// Get all menus (for admin)
var allMenus = await menuService.GetAllMenusAsync();

// Get specific menu
var menu = await menuService.GetMenuByIdAsync(menuId);

// Get child menus
var children = await menuService.GetChildMenusAsync(parentId);

// Check user access
bool hasAccess = await menuService.UserHasAccessAsync(menuId, userLevel);

// Invalidate cache after updates
menuService.InvalidateCache();
```

## Performance Notes

- **Caching**: Menus cached for 60 minutes using IMemoryCache
- **Query**: Single query per user level (loads all relevant menus at once)
- **Hierarchy**: Built in-memory after data retrieval (recursive BuildHierarchy method)
- **Rendering**: Hierarchical structure efficiently rendered in Razor views

## Future Enhancements

- [ ] Add menu permissions (separate from role-based access)
- [ ] Add user-specific menu customization
- [ ] Add menu usage analytics
- [ ] Add menu drag-drop reordering UI
- [ ] Add internationalization (multi-language support)
- [ ] Add menu search functionality
- [ ] Add breadcrumb generation from menu structure
