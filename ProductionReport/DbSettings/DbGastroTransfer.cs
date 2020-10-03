using LinqToDB;
using ProductionReport.DbSettings.Models;

namespace ProductionReport.DbSettings
{
    public class DbGastroTransfer : LinqToDB.Data.DataConnection
    {
        public DbGastroTransfer() : base("GastroTransfer") { }
        public ITable<Product> Products => GetTable<Product>();
        public ITable<Production> Productions => GetTable<Production>();
    }
}
