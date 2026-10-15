/**
 * SQL Script: Create FW_Packing_History Table
 * Database: SSGT
 * SQL Server Version: 2025+
 * 
 * This script creates the FW_Packing_History table to track when items are packed,
 * by whom, when they were packed, and which godown they are stored in.
 */

-- =============================================
-- Use the SSGT database
-- =============================================
USE [SSGT];
GO

-- =============================================
-- Drop table if it exists (OPTIONAL - uncomment only if rebuilding)
-- =============================================
-- IF OBJECT_ID('[dbo].[FW_Packing_History]', 'U') IS NOT NULL
--     DROP TABLE [dbo].[FW_Packing_History];
-- GO

-- =============================================
-- Create FW_Packing_History Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FW_Packing_History]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[FW_Packing_History] (
		-- Primary Key
		[PackingHistoryId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,

		-- Foreign Keys
		[BillId] BIGINT NOT NULL,
		[ItemId] BIGINT NOT NULL,
		[GodownId] BIGINT NOT NULL,
		[PackedByUserId] BIGINT NOT NULL,
		[ModifiedByUserId] BIGINT NULL,

		-- Packing Details
		[QuantityPacked] INT NOT NULL,
		[NumberOfCasesPacked] INT NULL,
		[PackingStatus] NCHAR(1) NULL,  -- P = Partial, C = Complete
		[PackingNotes] NVARCHAR(500) NULL,

		-- Timestamps
		[PackedDateTime] DATETIME2 NOT NULL,
		[CreatedDateTime] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
		[ModifiedDateTime] DATETIME2 NULL,

		-- Indexes and Constraints
		CONSTRAINT [FK_FW_Packing_History_FW_Bills] FOREIGN KEY ([BillId]) 
			REFERENCES [dbo].[FW_Bills]([BillId]) ON DELETE CASCADE,
		CONSTRAINT [FK_FW_Packing_History_FW_Master_Item] FOREIGN KEY ([ItemId]) 
			REFERENCES [dbo].[FW_Master_Item]([ItemId]),
		CONSTRAINT [FK_FW_Packing_History_FW_Master_Godown] FOREIGN KEY ([GodownId]) 
			REFERENCES [dbo].[FW_Master_Godown]([GodownId]),
		CONSTRAINT [FK_FW_Packing_History_FW_User_Detail_Packed] FOREIGN KEY ([PackedByUserId]) 
			REFERENCES [dbo].[FW_User_Detail]([UserId]),
		CONSTRAINT [FK_FW_Packing_History_FW_User_Detail_Modified] FOREIGN KEY ([ModifiedByUserId]) 
			REFERENCES [dbo].[FW_User_Detail]([UserId])
	);

	-- =============================================
	-- Create Indexes for Performance
	-- =============================================

	-- Index on BillId for quick lookups by bill
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_BillId] 
		ON [dbo].[FW_Packing_History]([BillId]);

	-- Index on ItemId for item lookups
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_ItemId] 
		ON [dbo].[FW_Packing_History]([ItemId]);

	-- Index on GodownId for godown-wise reports
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_GodownId] 
		ON [dbo].[FW_Packing_History]([GodownId]);

	-- Index on PackedByUserId for user activity tracking
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedByUserId] 
		ON [dbo].[FW_Packing_History]([PackedByUserId]);

	-- Index on PackedDateTime for date-range queries
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedDateTime] 
		ON [dbo].[FW_Packing_History]([PackedDateTime]);

	-- Composite index for common query pattern: BillId + ItemId
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_Bill_Item] 
		ON [dbo].[FW_Packing_History]([BillId], [ItemId]);

	PRINT 'Table [dbo].[FW_Packing_History] created successfully.'
END
ELSE
BEGIN
	PRINT 'Table [dbo].[FW_Packing_History] already exists.'
END
GO

-- =============================================
-- Verify Table Creation
-- =============================================
IF OBJECT_ID('[dbo].[FW_Packing_History]', 'U') IS NOT NULL
BEGIN
	SELECT 
		TABLE_SCHEMA,
		TABLE_NAME,
		COLUMN_NAME,
		DATA_TYPE,
		IS_NULLABLE
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'FW_Packing_History'
	ORDER BY ORDINAL_POSITION;

	PRINT 'Table structure verified successfully.'
END
GO
