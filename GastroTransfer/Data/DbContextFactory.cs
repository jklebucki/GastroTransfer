using GastroTransfer.Services;
using System.Data.Entity.Infrastructure;

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
