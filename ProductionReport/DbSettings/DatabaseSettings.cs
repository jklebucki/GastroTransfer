using LinqToDB.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ProductionReport.DbSettings
{
    class DatabaseSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SqlServer";
        public string DefaultDataProvider => "SqlServer";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "GastroTransfer",
                        ProviderName = "SqlServer",
                        ConnectionString = @"Server=.;Database=GastroTransfer;User id=sa;Password=#sa2015!;"
                    };
            }
        }
    }
}
