# FW_Packing_History Database Setup Guide

## Problem
The application was throwing a `System.ExecutionEngineException` on startup because the `FW_Packing_History` table doesn't exist in the database, but the code was trying to access it.

## Solution
Execute the SQL script to create the table, then test the application.

---

## Step 1: Execute the SQL Script

### Option A: Using SQL Server Management Studio (SSMS)
1. Open **SQL Server Management Studio**
2. Connect to your SQL Server 2025 instance
3. Open the file: `WebApp/Database/CREATE_FW_Packing_History.sql`
4. Execute the script (Press **F5** or click **Execute**)
5. You should see: "Table [dbo].[FW_Packing_History] created successfully."

### Option B: Using PowerShell (Command Line)
```powershell
# Navigate to the WebApp\Database folder
cd "F:\Projects\TFS\SSGT\Application\WebApp\Database"

# Execute the SQL script
sqlcmd -S . -d SSGT -U sa -P db2admin -i CREATE_FW_Packing_History.sql
```

### Option C: Using Visual Studio
1. Open **SQL Server Object Explorer** (View → SQL Server Object Explorer)
2. Connect to your local SQL Server
3. Navigate to: **Databases → SSGT**
4. Right-click → **New Query**
5. Copy the contents of `CREATE_FW_Packing_History.sql`
6. Paste and execute

---

## Step 2: Verify the Table Creation

Execute the verification script to confirm the table was created:

```sql
-- Copy and run the contents of VERIFY_Dependencies.sql
-- Or use this quick check:

USE SSGT;
GO

SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'FW_Packing_History';

-- Should return: FW_Packing_History
```

---

## Step 3: Test the Application

1. **Clean the project:**
   - Visual Studio → Build → Clean Solution
   - Delete the `bin` and `obj` folders

2. **Rebuild:**
   - Visual Studio → Build → Rebuild Solution

3. **Debug:**
   - Press **F5** to start debugging
   - Navigate to a bill and visit the Packing Dashboard
   - The page should load without errors

---

## SQL Script Contents

### Tables Created
- **FW_Packing_History** - Main table for tracking when items are packed

### Columns
| Column | Type | Purpose |
|--------|------|---------|
| PackingHistoryId | BIGINT (PK) | Unique identifier |
| BillId | BIGINT (FK) | Which bill the item belongs to |
| ItemId | BIGINT (FK) | Which item was packed |
| GodownId | BIGINT (FK) | Which godown the item is in |
| PackedByUserId | BIGINT (FK) | Which user packed the item |
| QuantityPacked | INT | How many units were packed |
| NumberOfCasesPacked | INT | How many cases (optional) |
| PackedDateTime | DATETIME2 | When it was packed |
| PackingStatus | NCHAR(1) | P=Partial, C=Complete |
| PackingNotes | NVARCHAR(500) | Optional notes |
| CreatedDateTime | DATETIME2 | Record creation time |
| ModifiedDateTime | DATETIME2 | Last modification time |
| ModifiedByUserId | BIGINT | Who last modified it |

### Indexes Created
- `IX_FW_Packing_History_BillId` - Quick lookups by bill
- `IX_FW_Packing_History_ItemId` - Quick lookups by item
- `IX_FW_Packing_History_GodownId` - Godown-wise reports
- `IX_FW_Packing_History_PackedByUserId` - User activity tracking
- `IX_FW_Packing_History_PackedDateTime` - Date-range queries
- `IX_FW_Packing_History_Bill_Item` - Composite index for bill+item lookups

### Foreign Keys
| Constraint | References |
|-----------|-----------|
| FK_FW_Packing_History_FW_Bills | FW_Bills(BillId) |
| FK_FW_Packing_History_FW_Master_Item | FW_Master_Item(ItemId) |
| FK_FW_Packing_History_FW_Master_Godown | FW_Master_Godown(GodownId) |
| FK_FW_Packing_History_FW_User_Detail_Packed | FW_User_Detail(UserId) |
| FK_FW_Packing_History_FW_User_Detail_Modified | FW_User_Detail(UserId) |

---

## Troubleshooting

### Error: "Foreign key reference to table 'FW_Bills' not found"
**Solution:** Make sure all these tables exist in your database:
- FW_Bills
- FW_Master_Item
- FW_Master_Godown
- FW_User_Detail

Run `VERIFY_Dependencies.sql` to check.

### Error: "Table already exists"
**Solution:** Run this to drop and recreate:
```sql
DROP TABLE [dbo].[FW_Packing_History];
-- Then re-run the CREATE_FW_Packing_History.sql script
```

### ExecutionEngineException still occurs
**Solution:** 
1. Delete all breakpoints
2. Clear the bin and obj folders
3. Close Visual Studio completely
4. Reopen and rebuild
5. Clear browser cache (Ctrl+Shift+Delete)

---

## Code Changes Made

To handle the case where the table might not exist initially, the following defensive code was added:

### In ReportsController.cs
- Wrapped `FwPackingHistories` queries in try-catch blocks
- If the table doesn't exist, the app gracefully continues without packing history
- Packing history is optional - the core packing functionality still works

This ensures:
✅ App doesn't crash on startup even if table is missing  
✅ Once table is created, packing history automatically starts working  
✅ No code changes needed after table creation  

---

## Testing Checklist

- [ ] SQL script executed successfully
- [ ] `VERIFY_Dependencies.sql` confirms table exists
- [ ] Application starts without ExecutionEngineException
- [ ] Packing Dashboard loads correctly
- [ ] Can pack items and see the checkmark
- [ ] Info icon tooltips show packing details
- [ ] Packing history is saved to database
- [ ] Can refresh and see previous packing records

---

## Next Steps

1. **Execute the SQL script** ← DO THIS FIRST
2. Rebuild the application
3. Test the Packing Dashboard
4. Verify packing history is saved and displayed correctly

If you need further assistance, check the `PACKING_HISTORY_IMPLEMENTATION.md` file in the WebApp folder for complete feature documentation.
