// Data/BrokerageContext.cs
using Microsoft.EntityFrameworkCore;
public class BrokerageContext : DbContext
{
    public BrokerageContext(DbContextOptions<BrokerageContext> options)
        : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Security> Securities => Set<Security>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Execution> Executions => Set<Execution>();
    public DbSet<PriceTick> PriceTicks => Set<PriceTick>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Security>().HasKey(s => s.Symbol);

        modelBuilder.Entity<LedgerEntry>()
            .Property(l => l.Amount).HasColumnType("decimal(18,4)");

        modelBuilder.Entity<Order>()
            .Property(o => o.Quantity).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Order>()
            .Property(o => o.QuantityFilled).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Order>()
            .Property(o => o.LimitPrice).HasColumnType("decimal(18,4)");

        modelBuilder.Entity<Execution>()
            .Property(e => e.Price).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Execution>()
            .Property(e => e.Quantity).HasColumnType("decimal(18,4)");

        modelBuilder.Entity<PriceTick>()
            .Property(p => p.Price).HasColumnType("decimal(18,4)");
    }
}