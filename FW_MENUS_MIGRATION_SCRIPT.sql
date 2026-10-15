-- ============================================================================
-- FW_Menus Table Migration Script
-- Purpose: Add optional columns to support enhanced menu functionality
-- Version: 1.0
-- Date: 2026
-- ============================================================================

-- ============================================================================
-- SECTION 1: Add Optional Columns (if not already present)
-- ============================================================================

-- Check and add MenuIcon column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'FW_Menus' AND COLUMN_NAME = 'MenuIcon')
BEGIN
	ALTER TABLE [FW_Menus]
	ADD MenuIcon VARCHAR(50) NULL;
	PRINT 'Added MenuIcon column';
END
ELSE
	PRINT 'MenuIcon column already exists';

-- Check and add MenuDescription column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'FW_Menus' AND COLUMN_NAME = 'MenuDescription')
BEGIN
	ALTER TABLE [FW_Menus]
	ADD MenuDescription VARCHAR(255) NULL;
	PRINT 'Added MenuDescription column';
END
ELSE
	PRINT 'MenuDescription column already exists';

-- Check and add IsActive column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'FW_Menus' AND COLUMN_NAME = 'IsActive')
BEGIN
	ALTER TABLE [FW_Menus]
	ADD IsActive BIT NOT NULL DEFAULT 1;
	PRINT 'Added IsActive column';
END
ELSE
	PRINT 'IsActive column already exists';

-- Check and add MenuUrl column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'FW_Menus' AND COLUMN_NAME = 'MenuUrl')
BEGIN
	ALTER TABLE [FW_Menus]
	ADD MenuUrl VARCHAR(255) NULL;
	PRINT 'Added MenuUrl column';
END
ELSE
	PRINT 'MenuUrl column already exists';

-- ============================================================================
-- SECTION 2: Add Indexes for Performance
-- ============================================================================

-- Index on UserLevel and IsActive (for menu filtering by role)
IF NOT EXISTS (SELECT * FROM SYS.INDEXES 
	WHERE NAME = 'IX_FwMenus_UserLevel_IsActive')
BEGIN
	CREATE INDEX IX_FwMenus_UserLevel_IsActive 
	ON [FW_Menus](UserLevel, IsActive);
	PRINT 'Created index IX_FwMenus_UserLevel_IsActive';
END
ELSE
	PRINT 'Index IX_FwMenus_UserLevel_IsActive already exists';

-- Index on ParentID (for menu hierarchy lookups)
IF NOT EXISTS (SELECT * FROM SYS.INDEXES 
	WHERE NAME = 'IX_FwMenus_ParentId')
BEGIN
	CREATE INDEX IX_FwMenus_ParentId 
	ON [FW_Menus](ParentID);
	PRINT 'Created index IX_FwMenus_ParentId';
END
ELSE
	PRINT 'Index IX_FwMenus_ParentId already exists';

-- ============================================================================
-- SECTION 3: Sample Menu Data (OPTIONAL - Uncomment to use)
-- ============================================================================

-- TRANSACTION MENUS (ParentID = 0)
/*
INSERT INTO [FW_Menus] (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, MenuDescription, IsActive, MenuUrl)
VALUES 
	(1, 'Transactions', 0, 1, 10, NULL, '💼', 'Transaction Management', 1, NULL),
	(2, 'Masters', 0, 1, 20, NULL, '⚙️', 'Master Data Management', 1, NULL),
	(3, 'Reports', 0, 1, 30, NULL, '📊', 'View Reports', 1, NULL),
	(4, 'Administration', 0, 2, 40, NULL, '🔧', 'System Administration', 1, NULL);

-- TRANSACTION SUBMENU (ParentID = 1)
INSERT INTO [FW_Menus] (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, MenuDescription, IsActive, MenuUrl)
VALUES 
	(10, 'Bills', 1, 1, 10, 'Bills/List', '📄', 'View and Manage Bills', 1, NULL),
	(11, 'Invoices', 1, 1, 20, 'Invoices/List', '🧾', 'View and Manage Invoices', 1, NULL),
	(12, 'Payments', 1, 1, 30, 'Payments/List', '💳', 'View and Manage Payments', 1, NULL);

-- MASTERS SUBMENU (ParentID = 2)
INSERT INTO [FW_Menus] (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, MenuDescription, IsActive, MenuUrl)
VALUES 
	(20, 'Customers', 2, 1, 10, 'Customers/List', '👥', 'Customer Master', 1, NULL),
	(21, 'Items', 2, 1, 20, 'Items/List', '📦', 'Item Master', 1, NULL),
	(22, 'Categories', 2, 1, 30, 'Categories/List', '🏷️', 'Category Master', 1, NULL);

-- REPORTS SUBMENU (ParentID = 3)
INSERT INTO [FW_Menus] (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, MenuDescription, IsActive, MenuUrl)
VALUES 
	(30, 'Sales Report', 3, 1, 10, 'Reports/Sales', '📈', 'Sales Analysis', 1, NULL),
	(31, 'Purchase Report', 3, 1, 20, 'Reports/Purchase', '📉', 'Purchase Analysis', 1, NULL);

-- ADMINISTRATION SUBMENU (ParentID = 4 - Admin Only, UserLevel = 2)
INSERT INTO [FW_Menus] (MenuID, MenuName, ParentID, UserLevel, SortOrder, FormName, MenuIcon, MenuDescription, IsActive, MenuUrl)
VALUES 
	(40, 'Users', 4, 2, 10, 'Admin/Users', '👨‍💼', 'Manage Users', 1, NULL),
	(41, 'Roles', 4, 2, 20, 'Admin/Roles', '🔐', 'Manage Roles', 1, NULL),
	(42, 'Audit Log', 4, 2, 30, 'Admin/AuditLog', '📋', 'View Audit Logs', 1, NULL);
*/

-- ============================================================================
-- SECTION 4: Verification Queries
-- ============================================================================

-- View current table structure
SELECT 
	COLUMN_NAME,
	DATA_TYPE,
	IS_NULLABLE,
	COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'FW_Menus'
ORDER BY ORDINAL_POSITION;

-- View all indexes
SELECT 
	INDEX_NAME,
	COLUMN_NAME
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_NAME = 'FW_Menus'
ORDER BY INDEX_NAME, SEQ_IN_INDEX;

-- View menu statistics
SELECT 
	'Total Menus' AS Metric,
	COUNT(*) AS Value
FROM [FW_Menus]
UNION ALL
SELECT 'Active Menus', COUNT(*) 
FROM [FW_Menus] 
WHERE IsActive = 1
UNION ALL
SELECT 'Parent Menus (Level 0)', COUNT(*) 
FROM [FW_Menus] 
WHERE ParentID = 0
UNION ALL
SELECT 'Sub Menus (Level > 0)', COUNT(*) 
FROM [FW_Menus] 
WHERE ParentID > 0;

-- View menu hierarchy preview (first 20 items)
SELECT TOP 20
	MenuID,
	MenuName,
	ParentID,
	UserLevel,
	SortOrder,
	CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
	CASE WHEN ParentID = 0 THEN 'ROOT' ELSE 'CHILD' END AS Type,
	COALESCE(MenuIcon, '-') AS Icon
FROM [FW_Menus]
ORDER BY UserLevel, SortOrder;

-- ============================================================================
-- SECTION 5: Data Validation Queries
-- ============================================================================

-- Check for orphaned menus (ParentID references non-existent parent)
SELECT 
	m.MenuID,
	m.MenuName,
	m.ParentID,
	'Orphaned - Parent Not Found' AS Issue
FROM [FW_Menus] m
WHERE m.ParentID > 0 
  AND m.ParentID NOT IN (SELECT MenuID FROM [FW_Menus])
UNION ALL
-- Check for duplicate MenuIDs
SELECT 
	MenuID,
	MenuName,
	0 AS ParentID,
	'Duplicate MenuID' AS Issue
FROM [FW_Menus]
GROUP BY MenuID, MenuName
HAVING COUNT(*) > 1
UNION ALL
-- Check for NULL required columns
SELECT 
	MenuID,
	'NULL' AS MenuName,
	0 AS ParentID,
	'Missing MenuName' AS Issue
FROM [FW_Menus]
WHERE MenuName IS NULL OR MenuName = '';

-- ============================================================================
-- SECTION 6: Update Existing Data (if columns exist)
-- ============================================================================

-- Set default IsActive value for any NULL entries
UPDATE [FW_Menus]
SET IsActive = 1
WHERE IsActive IS NULL;

-- ============================================================================
-- NOTES
-- ============================================================================
-- 
-- 1. BEFORE RUNNING THIS SCRIPT:
--    - Back up your FW_Menus table
--    - Test in development environment first
--    - Review sample data before uncommenting
--
-- 2. SAMPLE DATA:
--    - MenuID values: 1-4 (root), 10-12, 20-22, 30-31, 40-42 (submenus)
--    - UserLevel 1 = Regular User
--    - UserLevel 2 = Admin
--    - Adjust MenuIDs if they conflict with existing data
--
-- 3. AFTER RUNNING:
--    - Verify indexes were created
--    - Check data integrity with validation queries
--    - Populate with your menu data
--    - Test menu display in application
--
-- 4. PERFORMANCE:
--    - Indexes improve filtering by UserLevel and ParentID
--    - Consider additional indexes based on query patterns
--    - Monitor query performance with actual users
--
-- ============================================================================

PRINT 'Migration script completed successfully';
