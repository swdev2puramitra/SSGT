# Quick Fix Reference: EF Core FromSqlRaw() with DTOs

## When You See This Error
```
System.InvalidOperationException: Cannot create a DbSet for 'YourDtoType' 
because this type is not included in the model for the context.
```

## Quick Fix (3 Steps)

### Step 1: Add DbSet to Your Context
```csharp
public virtual DbSet<YourDtoType> YourDtoTypes { get; set; }
```

### Step 2: Add Configuration in OnModelCreating
```csharp
modelBuilder.Entity<YourDtoType>(entity =>
{
	entity.HasNoKey();
	entity.ToView(null);
});
```

### Step 3: Now This Works
```csharp
var results = _context.Set<YourDtoType>()
	.FromSqlRaw("EXEC YourStoredProcedure @param = {0}", paramValue)
	.AsNoTracking()
	.ToList();
```

## Key Configuration Options

| Option | Use When |
|--------|----------|
| `HasNoKey()` | DTO has no primary key (usual for stored procedures) |
| `ToView(null)` | Results only, no table/view mapping |
| `.AsNoTracking()` | Read-only query, improves performance |
| `.ToList()` | Execute query immediately |

## Example DTOs Needing This

- Stored procedure results
- Custom query projections without primary keys
- Temporary data objects
- Reporting/Analytics DTOs
- Readonly data from views

## Best Practice Checklist

- [ ] DTO has no primary key defined
- [ ] DbSet added to context
- [ ] Entity configured in OnModelCreating
- [ ] Using `Set<DtoType>()` for queries
- [ ] Using `FromSqlRaw()` with parameters safely
- [ ] Using `AsNoTracking()` for read-only data
- [ ] No modification tracking needed

---

**Reference:** Applied in WebApp/Models/SsgtContext.cs for BillsListDto
