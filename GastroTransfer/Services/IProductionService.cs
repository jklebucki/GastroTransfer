
using GastroTransfer.Models;
using System.Collections.Generic;

namespace GastroTransfer.Services
{
    interface IProductionService
    {
        ServiceMessage AddProduction(ProductionViewModel model);
        List<ProductionViewModel> GetProduction(bool fullData);
        ServiceMessage RemoveProduction(int productionId);
        ServiceMessage ChangeTransferStatus(int productionId);
    }
}
