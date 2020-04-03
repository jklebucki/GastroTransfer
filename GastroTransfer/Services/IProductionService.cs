
using GastroTransfer.Models;
using System.Collections.Generic;

namespace GastroTransfer.Services
{
    interface IProductionService
    {
        ServiceMessage AddProduction(ProductionViewModel model);
        ServiceMessage AddProduction(ProductionItem item);
        List<ProductionViewModel> GetProduction(bool fullData);
        ServiceMessage RemoveProduction(int productionId);
        ServiceMessage ChangeTransferStatus(int[] productionIds, int packageNumber, int documentType);
    }
}
