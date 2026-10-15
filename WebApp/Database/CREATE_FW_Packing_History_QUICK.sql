-- =====================================================
-- QUICK START: Copy and Paste This Entire Script
-- Database: SSGT
-- =====================================================

USE [SSGT];
GO

-- Create the table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FW_Packing_History]') AND type in (N'U'))
BEGIN
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

	-- Create Indexes
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_BillId] ON [dbo].[FW_Packing_History]([BillId]);
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_ItemId] ON [dbo].[FW_Packing_History]([ItemId]);
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_GodownId] ON [dbo].[FW_Packing_History]([GodownId]);
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedByUserId] ON [dbo].[FW_Packing_History]([PackedByUserId]);
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_PackedDateTime] ON [dbo].[FW_Packing_History]([PackedDateTime]);
	CREATE NONCLUSTERED INDEX [IX_FW_Packing_History_Bill_Item] ON [dbo].[FW_Packing_History]([BillId], [ItemId]);

	PRINT '✓ Table created successfully';
END
ELSE
BEGIN
	PRINT '✓ Table already exists';
END
GO
