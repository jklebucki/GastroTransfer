using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<TransferredItem> TransferredItems { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProducedItem>().ToTable("ProducedItems", schemaName: "PSP");
            modelBuilder.Entity<ProductGroup>().ToTable("ProductGroups", schemaName: "PSP");
            modelBuilder.Entity<TransferredItem>().ToTable("TransferredItems", schemaName: "PSP");
            modelBuilder.Entity<ProducedItem>()
                .Property(x => x.ConversionRate).HasPrecision(18, 4);
            modelBuilder.Entity<TransferredItem>()
                .Property(x => x.Quantity).HasPrecision(18, 4);
        }
    }
}
