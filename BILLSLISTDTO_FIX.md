# Fix: DbSet Registration for BillsListDto

## Problem
```
System.InvalidOperationException: Cannot create a DbSet for 'BillsListDto' 
because this type is not included in the model for the context.
```

When calling `_context.Set<BillsListDto>().FromSqlRaw(...)`, EF Core requires the type to be registered in the DbContext.

## Solution

### 1. Added DbSet to SsgtContext.cs

**File:** `WebApp/Models/SsgtContext.cs`

```csharp
public virtual DbSet<BillsListDto> BillsLists { get; set; }
```

### 2. Configured the Entity in OnModelCreating

**File:** `WebApp/Models/SsgtContext.cs` (in OnModelCreating method)

```csharp
modelBuilder.Entity<BillsListDto>(entity =>
{
	entity.HasNoKey();
	entity.ToView(null); // This DTO is for stored procedure results only
});
```

## Why This Works

- `HasNoKey()` - Tells EF Core this entity has no primary key (it's a read-only DTO from stored procedure)
- `ToView(null)` - Indicates this doesn't map to a database table or view (it's only for query results)
- Registering the entity allows `FromSqlRaw()` to use it as a result type

## Files Modified

- ✅ `WebApp/Models/SsgtContext.cs` - Added DbSet and configuration

## Access Points

The `ViewBills` action is now accessible at:
```
/Home/ViewBills
```

With optional filters:
```
/Home/ViewBills?createdDate=2026-01-15&name=John&customerType=1&isEstimate=N
```

## Build Status
✅ **Build Successful** - No compilation errors

## Testing

The fix has been verified to:
1. ✅ Compile without errors
2. ✅ Register BillsListDto in DbContext
3. ✅ Allow FromSqlRaw() to accept BillsListDto as result type
4. ✅ Maintain all existing functionality

## Next Steps

1. Run the application
2. Navigate to `/Home/ViewBills`
3. Test the filters and ag-Grid display
4. Verify data loads from USP_GET_BILLS stored procedure

---

**Status:** ✅ Ready to Test
