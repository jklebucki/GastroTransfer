using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Data
{
    class DbInitializer: IDatabaseInitializer<AppDbContext>
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
