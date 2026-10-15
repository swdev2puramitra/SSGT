# Data Type Casting Guide for FromSqlRaw()

## The Problem: InvalidCastException

When using `FromSqlRaw()` or `FromSql()`, EF Core attempts to **cast** SQL results into DTO properties. If the types don't match, you get a runtime exception.

### Example Error
```
System.InvalidCastException: Unable to cast object of type 'System.Int32' to type 'System.String'.
at Microsoft.Data.SqlClient.SqlDataReader.GetString(Int32 i)
at Microsoft.EntityFrameworkCore.Query.Internal.FromSqlQueryingEnumerable`1.Enumerator.MoveNext()
```

This means: **SQL returned an INT, but DTO property is string.**

## SQL to C# Type Mapping

### Required Mappings

| SQL Type | C# Type | Nullable Form | Example |
|----------|---------|---------------|---------|
| BIGINT | long | long? | `public long PhoneNo` |
| INT | int | int? | `public int SlNo` |
| TINYINT | byte | byte? | `public byte? Status` |
| SMALLINT | short | short? | `public short? Level` |
| DECIMAL | decimal | decimal? | `public decimal Amount` |
| FLOAT | double | double? | `public double? Rate` |
| REAL | float | float? | `public float? Score` |
| CHAR(n) | string or char? | — | `public char? Flag` or `public string Code` |
| VARCHAR(n) | string | — | `public string Name` |
| NVARCHAR(n) | string | — | `public string Description` |
| DATE | DateTime or string | DateTime? | `public DateTime? CreatedDate` (or string) |
| DATETIME | DateTime or string | DateTime? | `public DateTime? Timestamp` (or string) |
| DATETIME2 | DateTime or string | DateTime? | `public DateTime? UpdatedAt` (or string) |
| BIT | bool | bool? | `public bool IsActive` |
| UNIQUEIDENTIFIER | Guid | Guid? | `public Guid Id` |

## Common Mistakes

### ❌ WRONG: String Instead of Numeric
```sql
SELECT CustomerTypeId  -- SQL: INT (returns 1, 2, 3)
FROM Customers
```

```csharp
// WRONG - will throw InvalidCastException
public string CustomerTypeId { get; set; }

// CORRECT
public int CustomerTypeId { get; set; }
```

### ❌ WRONG: String Instead of Char
```sql
SELECT EstimateFlag  -- SQL: CHAR(1) (returns 'Y' or 'N')
FROM Bills
```

```csharp
// WRONG - may fail depending on SQL Driver
public string EstimateFlag { get; set; }

// CORRECT
public char EstimateFlag { get; set; }

// OR CORRECT (if nullable in SQL)
public char? EstimateFlag { get; set; }
```

### ❌ WRONG: Forgetting Nullable
```sql
SELECT Amount  -- SQL: DECIMAL NULL (can be NULL)
FROM Payments
```

```csharp
// WRONG - will fail on NULL values
public decimal Amount { get; set; }

// CORRECT
public decimal? Amount { get; set; }
```

### ❌ WRONG: DateTime vs String
```sql
SELECT CreatedDate  -- SQL: DATE
FROM Orders
```

```csharp
// Option 1: Use DateTime
public DateTime CreatedDate { get; set; }
public DateTime? CreatedDate { get; set; }

// Option 2: Use string (if you prefer)
public string CreatedDate { get; set; }

// WRONG - wrong type entirely
public int CreatedDate { get; set; }
```

## Best Practices

### 1. Know Your SQL Types First
**Always check the stored procedure or query definition:**
```sql
-- Run this to see column types:
EXEC sp_help 'USP_GET_BILLS'
-- OR
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Bills'
```

### 2. Match Exactly
```sql
-- SQL Definition
SELECT 
	CustomerId INT NOT NULL,          -- must be int
	CustomerName NVARCHAR(50) NOT NULL, -- must be string
	Balance DECIMAL(10,2) NULL,       -- must be decimal?
	IsActive BIT NOT NULL,            -- must be bool
	CreatedDate DATE NOT NULL         -- must be DateTime or string
```

```csharp
public class CustomerDto
{
	public int CustomerId { get; set; }              // ✅ INT → int
	public string CustomerName { get; set; }        // ✅ NVARCHAR → string
	public decimal? Balance { get; set; }           // ✅ DECIMAL NULL → decimal?
	public bool IsActive { get; set; }              // ✅ BIT → bool
	public DateTime CreatedDate { get; set; }       // ✅ DATE → DateTime (or string)
}
```

### 3. Handle Nullability Correctly
```csharp
// If SQL column is NOT NULL, DON'T use nullable
public int Status { get; set; }        // ✅ SQL: TINYINT NOT NULL

// If SQL column is NULL, MUST use nullable
public int? Status { get; set; }       // ✅ SQL: TINYINT NULL

// For strings, nullable is optional (default is nullable in C# 8+)
public string Name { get; set; }       // ✅ Can be null by design
public string? Name { get; set; }      // ✅ Explicitly nullable
```

### 4. Use Formatters for Display
Don't try to force SQL types to be strings for "convenience". Instead, use grid/display formatters:

```javascript
// WRONG: Force INT to string in DTO to avoid formatter
public string Status { get; set; }  // Will cause InvalidCastException!

// CORRECT: Keep correct type, use formatter
public int? Status { get; set; }

// In grid:
valueFormatter: function(params) {
	const statuses = { 1: 'Pending', 2: 'Approved', 3: 'Rejected' };
	return statuses[params.value] || 'Unknown';
}
```

## Debugging InvalidCastException

When you hit this error:

1. **Identify the column** from the stack trace or log
2. **Check the SQL type**:
   ```sql
   SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
   FROM INFORMATION_SCHEMA.COLUMNS
   WHERE TABLE_NAME = 'YourTable'
   ```
3. **Check the DTO property type**
4. **Verify they match** using the mapping table above
5. **Rebuild and test**

Example:
```
Error at line: CustomerType mapping failed
SQL Column: CustomerType is TINYINT NULL
DTO Property: public string CustomerType
→ CHANGE TO: public int? CustomerType
```

## Reference: Our Fix

**Before (WRONG):**
```csharp
public string Estimate { get; set; }       // CHAR in SQL
public string CustomerType { get; set; }   // TINYINT in SQL
```

**After (CORRECT):**
```csharp
public char? Estimate { get; set; }        // ✅ Matches CHAR
public int? CustomerType { get; set; }     // ✅ Matches TINYINT
```

**Grid Display (with formatter):**
```javascript
// Estimate: 'Y' / 'N' → "Yes" / "No"
// CustomerType: 1 / 2 / 3 → "Wholesale" / "Retail" / "Distributor"
```

## Summary

- **SQL Types Must Match DTO Types** (use mapping table above)
- **Use Formatters for Display**, not type conversions
- **Nullable SQL Columns = Nullable DTO Properties**
- **Always Verify** SQL schema before creating DTOs
- **Test Immediately** after DTO changes

---

**Apply this to all `FromSqlRaw()` / `FromSql()` operations in your codebase.**
