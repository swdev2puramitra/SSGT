-- =====================================================
-- COPY THIS ENTIRE SCRIPT TO SQL MANAGEMENT STUDIO
-- AND PRESS F5 TO EXECUTE
-- =====================================================

USE [SSGT];
GO

PRINT '======================================';
PRINT 'Creating FW_Packing_History Table';
PRINT '======================================';
PRINT '';

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FW_Packing_History]') AND type in (N'U'))
BEGIN
	PRINT 'Creating table...';

	CREATE TABLE [dbo].[FW_Packing_History] (
		[PackingHistoryId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[BillId] BIGINT NOT NULL,
		[ItemId] BIGINT NOT NULL,
		[GodownId] BIGINT NOT NULL,
		[PackedByUserId] BIGINT NOT NULL,
		[ModifiedByUserId] BIGINT NULL,
		[QuantityPacked] INT NOT NULL,
		[NumberOfCasesPacked] INT NULL,
		[PackingStatus] NCHAR(1) NULL,
		[PackingNotes] NVARCHAR(500) NULL,
		[PackedDateTime] DATETIME2 NOT NULL,
		[CreatedDateTime] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
		[ModifiedDateTime] DATETIME2 NULL,
		CONSTRAINT [FK_FW_Packing_History_FW_Bills] FOREIGN KEY ([BillId]) REFERENCES [dbo].[FW_Bills]([BillId]) ON DELETE CASCADE,
		CONSTRAINT [FK_FW_Packing_History_FW_Master_Item] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[FW_Master_Item]([ItemId]),
		CONSTRAINT [FK_FW_Packing_History_FW_Master_Godown] FOREIGN KEY ([GodownId]) REFERENCES [dbo].[FW_Master_Godown]([GodownId]),
		CONSTRAINT [FK_FW_Packing_History_FW_User_Detail_Packed] FOREIGN KEY ([PackedByUserId]) REFERENCES [dbo].[FW_User_Detail]([UserId]),
		CONSTRAINT [FK_FW_Packing_History_FW_User_Detail_Modified] FOREIGN KEY ([ModifiedByUserId]) REFERENCES [dbo].[FW_User_Detail]([UserId])
	);
	PRINT '✓ Table structure created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_BillId] ON [dbo].[FW_Packing_History]([BillId]);
	PRINT '✓ Index 1/6 created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_ItemId] ON [dbo].[FW_Packing_History]([ItemId]);
	PRINT '✓ Index 2/6 created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_GodownId] ON [dbo].[FW_Packing_History]([GodownId]);
	PRINT '✓ Index 3/6 created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedByUserId] ON [dbo].[FW_Packing_History]([PackedByUserId]);
	PRINT '✓ Index 4/6 created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedDateTime] ON [dbo].[FW_Packing_History]([PackedDateTime]);
	PRINT '✓ Index 5/6 created';

	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_Bill_Item] ON [dbo].[FW_Packing_History]([BillId], [ItemId]);
	PRINT '✓ Index 6/6 created';

	PRINT '';
	PRINT '✅ SUCCESS: Table [FW_Packing_History] created with all indexes!';
END
ELSE
BEGIN
	PRINT '✓ Table [FW_Packing_History] already exists';
END

PRINT '';
PRINT 'Verifying table...';
GO

-- Verify the table exists and show structure
SELECT 
	'Table Verification' as Status,
	TABLE_NAME,
	COUNT(*) as ColumnCount
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'FW_Packing_History'
GROUP BY TABLE_NAME;

PRINT '';
PRINT '======================================';
PRINT 'Setup Complete!';
PRINT '======================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Close and reopen Visual Studio';
PRINT '2. Clean Solution (Build -> Clean)';
PRINT '3. Rebuild Solution (Build -> Rebuild)';
PRINT '4. Press F5 to debug';
PRINT '5. Navigate to Packing Dashboard';
PRINT '';
PRINT 'Done! 🎉';
