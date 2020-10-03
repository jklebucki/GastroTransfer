using LinqToDB;
using LinqToDB.Data;
using ProductionReportWF.DbSettings.Models;

namespace ProductionReportWF.DbSettings
{
    public class DbGastroTransfer : DataConnection
    {
        public DbGastroTransfer(string connectionString) : base("SqlServer", connectionString) { }
        public ITable<Product> Products => GetTable<Product>();
        public ITable<Production> Productions => GetTable<Production>();
    }
}
