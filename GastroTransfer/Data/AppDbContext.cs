using GastroTransfer.Models;
using System.Data.Entity;

namespace GastroTransfer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<AppDbContext>(new CreateDatabaseIfNotExists<AppDbContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, GastroTransfer.Migrations.Configuration>());
        }

        public DbSet<ProducedItem> ProducedItems { get; set; }
        public DbSet<ProductionItem> TransferredItems { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProducedItem>().ToTable("ProducedItems", schemaName: "PSP");
            modelBuilder.Entity<ProductGroup>().ToTable("ProductGroups", schemaName: "PSP");
            modelBuilder.Entity<ProductionItem>().ToTable("Production", schemaName: "PSP");

            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ConversionRate).HasPrecision(18, 4);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ExternalId).HasMaxLength(20);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ExternalName).HasMaxLength(50);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ExternalIndex).HasMaxLength(50);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.Name).HasMaxLength(50);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ExternalUnitOfMesure).HasMaxLength(10);
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.UnitOfMesure).HasMaxLength(10);

            modelBuilder.Entity<ProductionItem>()
                .Property(x => x.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionItem>()
                .HasIndex(i => i.IsSentToExternalSystem).HasName("idx_isSent");
            modelBuilder.Entity<ProductionItem>()
                .HasIndex(i => i.ProducedItemId).HasName("idx_product");
            modelBuilder.Entity<ProductionItem>()
                .HasIndex(i => i.Registered).HasName("idx_registerDate");
            modelBuilder.Entity<ProductionItem>()
                .HasIndex(i => i.SentToExternalSystem).HasName("idx_sentDate");

            modelBuilder.Entity<ProductGroup>()
                .Property(x => x.GroupName).HasMaxLength(50);
        }
    }
}
