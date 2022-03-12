using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    public class TrashDocumentTransferService
    {
        private DbService _dbService { get; set; }
        private AppDbContext _appDbContext { get; set; }
        private Config _config { get; set; }
        public TrashDocumentTransferService(DbService dbService, AppDbContext appDbContext, Config config)
        {
            _dbService = dbService;
            _appDbContext = appDbContext;
            _config = config;
        }
    }
}
