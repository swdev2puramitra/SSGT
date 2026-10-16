using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public partial class SsgtContext : DbContext
{
    public SsgtContext()
    {
    }

    public SsgtContext(DbContextOptions<SsgtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FwBill> FwBills { get; set; }

    public virtual DbSet<FwBillsBackup> FwBillsBackups { get; set; }

    public virtual DbSet<FwBillsBackupItem> FwBillsBackupItems { get; set; }

    public virtual DbSet<FwBillsItem> FwBillsItems { get; set; }

    public virtual DbSet<FwErrorLog> FwErrorLogs { get; set; }

    public virtual DbSet<FwInvoice> FwInvoices { get; set; }

    public virtual DbSet<FwInvoiceItem> FwInvoiceItems { get; set; }

    public virtual DbSet<FwInvoiceSplitHistory> FwInvoiceSplitHistories { get; set; }

    public virtual DbSet<FwInvoiceRemoved> FwInvoiceRemoveds { get; set; }

    public virtual DbSet<FwInvoiceRemovedItem> FwInvoiceRemovedItems { get; set; }

    public virtual DbSet<FwMasterBill> FwMasterBills { get; set; }

    public virtual DbSet<FwMasterCategory> FwMasterCategories { get; set; }

    public virtual DbSet<FwMasterCompanyName> FwMasterCompanyNames { get; set; }

    public virtual DbSet<FwMasterConversion> FwMasterConversions { get; set; }

    public virtual DbSet<FwMasterCustomer> FwMasterCustomers { get; set; }

    public virtual DbSet<FwMasterGodown> FwMasterGodowns { get; set; }

    public virtual DbSet<FwMasterHsn> FwMasterHsns { get; set; }

    public virtual DbSet<FwMasterItem> FwMasterItems { get; set; }

    public virtual DbSet<FwMasterItemType> FwMasterItemTypes { get; set; }

    public virtual DbSet<FwMasterItemsConversion> FwMasterItemsConversions { get; set; }

    // New: Master table for customer types
    public virtual DbSet<FwMasterCustomerType> FwMasterCustomerTypes { get; set; }

    public virtual DbSet<FwMasterSubCategory> FwMasterSubCategories { get; set; }

    public virtual DbSet<FwMenu> FwMenus { get; set; }

    public virtual DbSet<FwPackingHistory> FwPackingHistories { get; set; }

    public virtual DbSet<FwRole> FwRoles { get; set; }

    public virtual DbSet<FwShopDetail> FwShopDetails { get; set; }

    public virtual DbSet<FwStatus> FwStatuses { get; set; }

    public virtual DbSet<FwStock> FwStocks { get; set; }

    public virtual DbSet<FwStockBackup> FwStockBackups { get; set; }

    public virtual DbSet<FwUserDetail> FwUserDetails { get; set; }

    public virtual DbSet<PackingDto> PackingDtos { get; set; }

    public virtual DbSet<BillsListDto> BillsLists { get; set; }

    public virtual DbSet<ItemSearchDto> ItemsLists { get; set; }
    public virtual DbSet<ItemTypeDto> ItemTypes { get; set; }
    public virtual DbSet<ItemLowestPriceDto> ItemLowestPrices { get; set; }

    public virtual DbSet<FwStockTransfer> FwStockTransfers { get; set; }

    public virtual DbSet<FwStockTransferItem> FwStockTransferItems { get; set; }

    public virtual DbSet<StockTransferDto> StockTransfers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string is configured in Program.cs via dependency injection
        // This allows for environment-specific configurations and secure credential management
        if (!optionsBuilder.IsConfigured)
        {
            // This will only be used if DbContext is instantiated without options
            // In normal usage, options are provided through dependency injection
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FwBill>(entity =>
        {
            entity.HasKey(e => e.BillId);

            entity.ToTable("FW_Bills");

            entity.HasIndex(e => e.BillNo, "IX_FW_Bills_BillNo")
                .IsUnique()
                .HasFilter("([BillNo] IS NOT NULL)");

            entity.HasIndex(e => new { e.CreatedDate, e.CustomerType, e.IsEstimateInvoice, e.BillNo }, "_dta_index_FW_Bills_5_1150627142__K22D_K4_K5_K2").IsDescending(true, false, false, false);

            entity.Property(e => e.AddDesc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddlDisc).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustGstin)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CustGSTIN");
            entity.Property(e => e.DiscountPerItem).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.DispatchStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("I Initiated, D dispatched, P Partial update, C Completed");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsEstimateInvoice)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("E", "DF_FW_Bills_IsEstimateInvoice");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalBillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VerifiedDate).HasColumnType("datetime");
            entity.Property(e => e.DispatchStatus)
            .HasDefaultValue("I");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FwBills)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Bills_FW_User_Details");
        });

        // Keyless DTO for item search results returned by stored procedures
        modelBuilder.Entity<ItemSearchDto>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.ItemPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CGST).HasColumnType("decimal(10,2)");
            entity.Property(e => e.SGST).HasColumnType("decimal(10,2)");
            entity.Property(e => e.IGST).HasColumnType("decimal(10,2)");
            // This DTO is not mapped to a table
            entity.ToView(null);
        });

        modelBuilder.Entity<ItemTypeDto>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasMaxLength(3);
            entity.ToView(null);
        });

        modelBuilder.Entity<ItemLowestPriceDto>(entity =>
        {
            entity.HasNoKey();
            //entity.Property(e => e.QtyPerType).HasColumnType("decimal(18,6)");
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.ToView(null);
        });

        modelBuilder.Entity<FwBillsBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Bills_Backup");

            entity.Property(e => e.AddDesc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddlDisc).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustGstin)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CustGSTIN");
            entity.Property(e => e.DelInv)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiscountPerItem).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsEstimateInvoice)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RemovedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalBillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VerifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FwBillsBackupItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Bills_Backup_Items");

            entity.Property(e => e.Cgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CGST");
            entity.Property(e => e.ChangedPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DelInv)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Igst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("IGST");
            entity.Property(e => e.ItemCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OriginalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RemoveDt).HasColumnType("datetime");
            entity.Property(e => e.Sgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("SGST");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<FwBillsItem>(entity =>
        {
            entity.HasKey(e => new { e.BillId, e.ItemId });

            entity.ToTable("FW_Bills_Items");

            entity.HasIndex(e => e.ItemId, "IDX_FW_Bills_Items_ITEMD");

            entity.Property(e => e.Cgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CGST");
            entity.Property(e => e.ChangedPrice).HasColumnType("decimal(10, 2)");
            // Map IsPriceModified explicitly to ensure EF knows column type and default
            entity.Property(e => e.IsPriceModified)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .ValueGeneratedNever()
                .HasColumnName("IsPriceModified");
            entity.Property(e => e.Igst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("IGST");
            entity.Property(e => e.ItemCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OriginalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Sgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("SGST");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsPacked)
            .HasColumnType("bit")
            .HasDefaultValue(false);

            entity.HasOne(d => d.Bill).WithMany(p => p.FwBillsItems)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Bills_Items_FW_Bills");

            entity.HasOne(d => d.Item).WithMany(p => p.FwBillsItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Bills_Items_FW_Master_Items");

            entity.HasOne(d => d.ItemType).WithMany(p => p.FwBillsItems)
                .HasForeignKey(d => d.ItemTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Bills_Items_FW_Master_ItemType");
        });

        modelBuilder.Entity<FwErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorId);   // <-- use PK
            entity.ToTable("FW_ErrorLog");

            entity.Property(e => e.ErrDate)
                .HasDefaultValueSql("(getdate())", "DF_ErrorLog_ErrDate")
                .HasColumnType("datetime");
            entity.Property(e => e.Error)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ErrorId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ErrorID");
            entity.Property(e => e.Methodname)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryKeyDetails)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<FwInvoice>(entity =>
        {
            // Invoice primary key is InvoiceId (not BillId)
            entity.HasKey(e => e.InvoiceId);
            // An estimate may be split into multiple invoices by item group.
            entity.HasIndex(e => e.BillId).IsUnique(false);

            entity.ToTable("FW_Invoice");

            entity.Property(e => e.InvoiceId).ValueGeneratedOnAdd();
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CancelledDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustGstin)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CustGSTIN");
            entity.Property(e => e.DiscountPerItem).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Invoice)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsEstimateInvoice)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("E", "DF_FW_Invoice_IsEstimateInvoice");
            entity.Property(e => e.IsJson)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("is_json");
            entity.Property(e => e.Loc)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Pos)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("POS");
            entity.Property(e => e.StateCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TotalBillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.WayBill)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VehicleNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.OwnerName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceStatus)
            .HasDefaultValue(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FwInvoices)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Invoice_FW_User_Details");

            // Link invoice to its bill using BillId foreign key
            entity.HasOne<FwBill>()
                .WithMany(b => b.FwInvoices)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Invoice_FW_Bills");
        });

        modelBuilder.Entity<FwInvoiceItem>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ItemId });

            entity.ToTable("FW_Invoice_Items");

            // InvoiceId is the invoice-item foreign key and must be mapped explicitly.
            // This keeps EF's relationship fixup/tracking stable when an invoice is edited
            // and its item rows are replaced.
            entity.HasOne(d => d.Invoice)
                .WithMany(p => p.FwInvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_FW_Invoice_Items_FW_Invoice");

            entity.Property(e => e.Cgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CGST");
            entity.Property(e => e.ChangedPrice).HasColumnType("decimal(10, 2)");
            // Map IsPriceModified explicitly to ensure EF treats it as regular property (not store-generated)
            entity.Property(e => e.IsPriceModified)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .ValueGeneratedNever()
                .HasColumnName("IsPriceModified");
            entity.Property(e => e.Igst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("IGST");
            entity.Property(e => e.ItemCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OriginalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Sgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("SGST");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.FwInvoiceItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Invoice_Items_FW_Master_Items");

            entity.HasOne(d => d.ItemType).WithMany(p => p.FwInvoiceItems)
                .HasForeignKey(d => d.ItemTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Invoice_Items_FW_Master_ItemType");
        });

        modelBuilder.Entity<FwInvoiceSplitHistory>(entity =>
        {
            entity.HasKey(e => e.SplitHistoryId);
            entity.ToTable("FW_Invoice_Split_History");
            entity.Property(e => e.SplitHistoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.HasIndex(e => e.SourceInvoiceId).IsUnique();
            entity.HasIndex(e => e.SplitInvoiceId).IsUnique();
            entity.HasIndex(e => e.BillId);
            entity.HasOne<FwInvoice>()
                .WithMany()
                .HasForeignKey(e => e.SourceInvoiceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_FW_Invoice_Split_History_Source");
            entity.HasOne<FwInvoice>()
                .WithMany()
                .HasForeignKey(e => e.SplitInvoiceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_FW_Invoice_Split_History_Split");
        });

        modelBuilder.Entity<FwInvoiceRemoved>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Invoice_Removed");

            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CancelledDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustGstin)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CustGSTIN");
            entity.Property(e => e.DiscountPerItem).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsEstimateInvoice)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RemoveDt).HasColumnType("datetime");
            entity.Property(e => e.TotalBillAmount).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<FwInvoiceRemovedItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Invoice_Removed_Items");

            entity.Property(e => e.Cgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CGST");
            entity.Property(e => e.ChangedPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Igst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("IGST");
            entity.Property(e => e.ItemCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RemoveDt).HasColumnType("datetime");
            entity.Property(e => e.Sgst)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("SGST");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<FwMasterBill>(entity =>
        {
            entity.HasKey(e => e.FinYearId);

            entity.ToTable("FW_Master_Bills");

            entity.Property(e => e.BillType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Bnumber).HasColumnName("BNumber");
            // No-op: Ensure FwMasterBill model is present in the context.
        });

        modelBuilder.Entity<FwMasterCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FW_Category");

            entity.ToTable("FW_Master_Category");

            entity.HasIndex(e => e.CatDescription, "IX_FW_Category").IsUnique();

            entity.HasIndex(e => e.Id, "NonClusteredIndex-20201024-194201");

            entity.Property(e => e.CatDescription)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FwMasterCompanyName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FW_CompanyName");

            entity.ToTable("FW_Master_CompanyNames");

            entity.HasIndex(e => e.Id, "NonClusteredIndex-20201024-194101");

            entity.Property(e => e.CompanyName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Retail).HasColumnType("numeric(5, 2)");
            entity.Property(e => e.SemiWholeSale).HasColumnType("numeric(5, 2)");
        });

        modelBuilder.Entity<FwMasterConversion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Master_Conversion");

            entity.HasIndex(e => e.ConversionId, "IDX_FW_Master_Conversion_ConversionId");

            entity.HasIndex(e => e.ConversionId, "IX_FW_Master_Conversion");

            entity.Property(e => e.ConversionName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FwMasterCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("FW_Master_Customers");

            entity.HasIndex(e => new { e.CustomerId, e.MobileNo, e.Name }, "_dta_index_FW_Master_Customers_5_1422628111__K1_K3_K2_5");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustAddress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FwMasterGodown>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FW_Godown");

            entity.ToTable("FW_Master_Godown");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GodownDescription)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FwMasterHsn>(entity =>
        {
            entity.HasKey(e => e.Hsnid);

            entity.ToTable("FW_Master_HSN");

            entity.Property(e => e.Hsnid).HasColumnName("HSNId");
            entity.Property(e => e.Cgst)
                .HasColumnType("numeric(10, 2)")
                .HasColumnName("CGST");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Hsncode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HSNCode");
            entity.Property(e => e.Igst)
                .HasColumnType("numeric(10, 2)")
                .HasColumnName("IGST");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Sgst)
                .HasColumnType("numeric(10, 2)")
                .HasColumnName("SGST");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FwMasterHsns)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_HSN_FW_User_Details");
        });

        modelBuilder.Entity<FwMasterItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.ToTable("FW_Master_Items");

            entity.HasIndex(e => new { e.ItemType, e.IsActive }, "IDX_FW_Master_Items_ItemType");

            entity.HasIndex(e => e.ItemCode, "IX_FW_Master_Items_ItemCode").IsUnique();

            entity.HasIndex(e => e.ItemName, "IX_FW_Master_Items_ItemName").IsUnique();

            entity.HasIndex(e => new { e.ConversionId, e.IsActive }, "NCI_FW_Master_Items_Conversion");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Hsnid).HasColumnName("HSNID");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RetailPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SemiWholeSalePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubCategoryId).HasColumnName("SubCategoryID");
            entity.Property(e => e.WholeSalePrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.FwMasterItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_FW_Master_Category");

            entity.HasOne(d => d.Company).WithMany(p => p.FwMasterItems)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_FW_Master_CompanyNames");

            entity.HasOne(d => d.DefaultSaleTypeNavigation).WithMany(p => p.FwMasterItemDefaultSaleTypeNavigations)
                .HasForeignKey(d => d.DefaultSaleType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_FW_Master_ItemType1");

            entity.HasOne(d => d.ItemTypeNavigation).WithMany(p => p.FwMasterItemItemTypeNavigations)
                .HasForeignKey(d => d.ItemType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_FW_Master_ItemType");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.FwMasterItems)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_FW_Master_SubCategory");
        });

        modelBuilder.Entity<FwMasterItemType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FW_ItemType");

            entity.ToTable("FW_Master_ItemType");

            entity.HasIndex(e => e.Id, "NonClusteredIndex-20201024-193632");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FwMasterItemsConversion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Master_Items_Conversions");

            entity.HasIndex(e => e.ItemId, "IDX_FW_Master_Items_Conversions_ItemId");

            entity.Property(e => e.Retail).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SemiWholeSale).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.WholeSale).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_Conversions_FW_Master_Items");

            entity.HasOne(d => d.ItemType).WithMany()
                .HasForeignKey(d => d.ItemTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Master_Items_Conversions_FW_Master_ItemType");
        });

        modelBuilder.Entity<FwMasterSubCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FW_SubCategory");

            entity.ToTable("FW_Master_SubCategory");

            entity.HasIndex(e => e.Id, "NonClusteredIndex-20201024-193931");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SubCatDescription)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.FwMasterSubCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_fw_master_subcategory_CategoryId_CategoryId");
        });

        modelBuilder.Entity<FwMenu>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Menus");

            entity.HasIndex(e => e.ParentId, "IX_FwMenus_ParentId");

            entity.HasIndex(e => new { e.UserLevel, e.IsActive }, "IX_FwMenus_UserLevel_IsActive");

            entity.Property(e => e.FormName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MenuDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MenuIcon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.MenuName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MenuUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<FwRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("FW_Roles");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FwShopDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_ShopDetails");

            entity.Property(e => e.AccountNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.BankName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gst)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GST");
            entity.Property(e => e.Ifsc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IFSC");
            entity.Property(e => e.LicenceNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ShopName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FwStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Status");

            entity.Property(e => e.PayStatus).HasColumnType("datetime");
        });

        modelBuilder.Entity<FwStock>(entity =>
        {
            entity.HasKey(e => new { e.StockId, e.ItemId });
            entity.ToTable("FW_Stock");

            entity.HasIndex(e => e.ItemId, "NonClusteredIndex-20201026-162205");

            entity.Property(e => e.Comments)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())", "DF_FW_Stock_CreatedDate")
                .HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StockFrom)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Stock_FW_User_Details");

            entity.HasOne(d => d.Godown).WithMany()
                .HasForeignKey(d => d.GodownId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Stock_FW_Godown");
        });

        modelBuilder.Entity<FwStockBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FW_Stock_Backup");

            entity.Property(e => e.Comments)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RemovedDate).HasColumnType("datetime");
            entity.Property(e => e.StockFrom)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FwUserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("FW_User_Details");

            entity.HasIndex(e => e.UserName, "IX_FW_User_Details_UserName").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())", "DF_FW_User_Details_CreatedDate")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200);
        });

        modelBuilder.Entity<FwStockTransfer>(entity =>
        {
            entity.HasKey(e => e.TransferId);

            entity.ToTable("FW_Stock_Transfer");

            entity.Property(e => e.TransferId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FwStockTransfers)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Stock_Transfer_FW_User_Details");

            entity.HasOne(d => d.FromGodownNavigation).WithMany(p => p.FwStockTransferFromGodownNavigations)
                .HasForeignKey(d => d.FromGodown)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Stock_Transfer_FW_Master_Godown");

            entity.HasOne(d => d.ToGodownNavigation).WithMany(p => p.FwStockTransferToGodownNavigations)
                .HasForeignKey(d => d.ToGodown)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Stock_Transfer_FW_Master_Godown1");
        });

        modelBuilder.Entity<FwStockTransferItem>(entity =>
        {
            entity.HasKey(e => new { e.TransferId, e.ItemId });

            entity.ToTable("FW_Stock_Transfer_Items");

            entity.Property(e => e.Quantity);
            entity.Property(e => e.QtyLowest);

            entity.HasOne(d => d.Item)
                  .WithMany()
                  .HasForeignKey(d => d.ItemId)
                  .HasConstraintName("FK_FW_Stock_Transfer_Items_FW_Master_Items");

            entity.HasOne(d => d.Transfer)
                  .WithMany(p => p.Items)
                  .HasForeignKey(d => d.TransferId)
                  .HasConstraintName("FK_FW_Stock_Transfer_ITEMS_FW_Stock_Transfer");
        });

        modelBuilder.Entity<FwPackingHistory>(entity =>
        {
            entity.HasKey(e => e.PackingHistoryId);

            entity.ToTable("FW_Packing_History");

            entity.Property(e => e.PackingHistoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.PackedDateTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.PackingNotes).HasMaxLength(500).IsUnicode(false);
            entity.Property(e => e.PackingStatus).HasMaxLength(1).IsUnicode(false).IsFixedLength();
            entity.Property(e => e.AllocatedQty).HasColumnName("AllocatedQty");
            entity.Property(e => e.RemainingQty)
                .HasColumnName("RemainingQty")
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            entity.Property(e => e.DispatchType).HasMaxLength(20).IsUnicode(false);

            entity.HasOne(d => d.Bill).WithMany()
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Packing_History_FW_Bills");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Packing_History_FW_Master_Items");

            entity.HasOne(d => d.Godown).WithMany()
                .HasForeignKey(d => d.GodownId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Packing_History_FW_Master_Godown");

            entity.HasOne(d => d.PackedByUser).WithMany()
                .HasForeignKey(d => d.PackedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Packing_History_FW_User_Details_PackedBy");

            entity.HasOne(d => d.ModifiedByUser).WithMany()
                .HasForeignKey(d => d.ModifiedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Packing_History_FW_User_Details_ModifiedBy");
        });

        modelBuilder.Entity<PackingDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null); // This DTO is not mapped to a database table or view
        });

        modelBuilder.Entity<BillsListDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null); // This DTO is for stored procedure results only
        });

        modelBuilder.Entity<ItemSearchDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null); // This DTO is for stored procedure results only
        });

        modelBuilder.Entity<StockTransferDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });

        modelBuilder.Entity<FwMasterCustomerType>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("FW_Master_CustomerType");

            entity.Property(e => e.CustTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
