using GastroTransfer.Models;
using System.Data.Entity;

namespace GastroTransfer.Data
{
    class DbInitializer : IDatabaseInitializer<AppDbContext>
    {
        public void InitializeDatabase(AppDbContext context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
                context.ProductGroups.Add(new ProductGroup() { GroupName = "Wszystkie" });
                context.SaveChanges();
            }
        }
    }
}
