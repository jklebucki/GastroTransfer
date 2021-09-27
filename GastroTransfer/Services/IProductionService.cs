
using GastroTransfer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    interface IProductionService
    {
        ServiceMessage AddProduction(ProductionViewModel model);
        ServiceMessage AddProduction(ProductionItem item);
        List<ProductionViewModel> GetProduction(bool fullData);
        ServiceMessage RemoveProduction(int productionId);
        Task<ServiceMessage> ChangeTransferStatus(int[] productionIds, int packageNumber, int documentType, bool swapStatus);
    }
}
