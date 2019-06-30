using GastroTransfer.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Data
{
    class DbContextFactory : IDbContextFactory<AppDbContext>
    {
        public AppDbContext Create()
        {
            ConfigService configService = new ConfigService(new CryptoService());
            var config = configService.GetConfig();
            DbService dbService = new DbService(config);

            return new AppDbContext(dbService.GetConnectionString());
        }
    }
}
