/**
 * SQL Script: Verify FW_Packing_History Dependencies
 * This script checks that all required tables exist before creating FW_Packing_History
 */

USE [SSGT];
GO

PRINT '========================================='
PRINT 'Verifying FW_Packing_History Dependencies'
PRINT '=========================================';
PRINT '';

-- Check FW_Bills table
IF OBJECT_ID('[dbo].[FW_Bills]', 'U') IS NOT NULL
	PRINT '✓ [dbo].[FW_Bills] EXISTS'
ELSE
	PRINT '✗ [dbo].[FW_Bills] MISSING - Required for foreign key'
GO

-- Check FW_Master_Item table
IF OBJECT_ID('[dbo].[FW_Master_Item]', 'U') IS NOT NULL
	PRINT '✓ [dbo].[FW_Master_Item] EXISTS'
ELSE
	PRINT '✗ [dbo].[FW_Master_Item] MISSING - Required for foreign key'
GO

-- Check FW_Master_Godown table
IF OBJECT_ID('[dbo].[FW_Master_Godown]', 'U') IS NOT NULL
	PRINT '✓ [dbo].[FW_Master_Godown] EXISTS'
ELSE
	PRINT '✗ [dbo].[FW_Master_Godown] MISSING - Required for foreign key'
GO

-- Check FW_User_Detail table
IF OBJECT_ID('[dbo].[FW_User_Detail]', 'U') IS NOT NULL
	PRINT '✓ [dbo].[FW_User_Detail] EXISTS'
ELSE
	PRINT '✗ [dbo].[FW_User_Detail] MISSING - Required for foreign key'
GO

-- Check if FW_Packing_History exists
PRINT '';
IF OBJECT_ID('[dbo].[FW_Packing_History]', 'U') IS NOT NULL
BEGIN
	PRINT '✓ [dbo].[FW_Packing_History] EXISTS'
	PRINT '';
	PRINT 'Table Row Count:'
	SELECT COUNT(*) as RecordCount FROM [dbo].[FW_Packing_History];
END
ELSE
	PRINT '✗ [dbo].[FW_Packing_History] DOES NOT EXIST'
GO

PRINT ''
PRINT 'Dependency Check Complete'
