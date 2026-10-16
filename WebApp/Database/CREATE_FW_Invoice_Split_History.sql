/*
  Durable audit relationship for item-based invoice splits.
  Run this once against the application database before deploying the matching WebApp build.
*/
IF OBJECT_ID(N'dbo.FW_Invoice_Split_History', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FW_Invoice_Split_History
    (
        SplitHistoryId BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_FW_Invoice_Split_History PRIMARY KEY,
        BillId BIGINT NOT NULL,
        SourceInvoiceId BIGINT NOT NULL,
        SplitInvoiceId BIGINT NOT NULL,
        CreatedBy INT NOT NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_FW_Invoice_Split_History_CreatedDate DEFAULT (GETDATE()),
        CONSTRAINT CK_FW_Invoice_Split_History_DifferentInvoices
            CHECK (SourceInvoiceId <> SplitInvoiceId),
        CONSTRAINT FK_FW_Invoice_Split_History_Source
            FOREIGN KEY (SourceInvoiceId) REFERENCES dbo.FW_Invoice (InvoiceId),
        CONSTRAINT FK_FW_Invoice_Split_History_Split
            FOREIGN KEY (SplitInvoiceId) REFERENCES dbo.FW_Invoice (InvoiceId)
    );

    CREATE UNIQUE INDEX UX_FW_Invoice_Split_History_Source
        ON dbo.FW_Invoice_Split_History (SourceInvoiceId);
    CREATE UNIQUE INDEX UX_FW_Invoice_Split_History_Split
        ON dbo.FW_Invoice_Split_History (SplitInvoiceId);
    CREATE INDEX IX_FW_Invoice_Split_History_Bill
        ON dbo.FW_Invoice_Split_History (BillId);
END;
