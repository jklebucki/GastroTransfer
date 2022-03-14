using GastroTransfer.Data;
using GastroTransfer.Models;
using LsiEndpointSupport;
using System;
using System.Linq;
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

        public async Task<ServiceMessage> TransferTrashDocument(DateTime trashDocumentDate)
        {
            var respMessage = new ServiceMessage { IsError = false, ItemId = 0, Message = "" };
            try
            {
                if (!_dbService.CheckLsiConnection())
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak połączenia z bazą danych systemu LSI" };

                LsiDbService lsiDbService = new LsiDbService(_dbService.GetLsiConnectionString());
                var docTypes = lsiDbService.GetDocumentsTypes();
                var documentType = docTypes.FirstOrDefault(x => x.Symbol == _config.TrashDocumentSymbol);

                if (documentType == null)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Nie znalazłem dokumentu {_config.ProductionDocumentSymbol} w systemie LSI." };

                if (string.IsNullOrEmpty(_config.EndpointUrl))
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak konfiguracji usługi LSI" };

                var productionService = new ProductionService(_appDbContext);
                var dateTo = new DateTime(trashDocumentDate.Year, trashDocumentDate.Month, trashDocumentDate.Day, 23, 59, 59);
                var currentTrashProducts = productionService
                    .GetProduction(false)
                    .Where(d => d.ProductionItem.Registered <= dateTo && d.ProductionItem.OperationType == 2)
                    .ToList();
                if (currentTrashProducts.Count == 0)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak pozycji do dokumentu straty." };


                var sumTrashItems = currentTrashProducts.GroupBy(i => i.ProductionItem.ProducedItemId)
                    .Select(r => new { Id = r.First().ProducedItem.ProducedItemId, Index = r.First().ProducedItem.ExternalId, Q = r.Sum(q => q.ProductionItem.Quantity) })
                    .ToList();

                var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
                foreach (var product in sumTrashItems)
                {
                    respMessage.ItemId = product.Id;
                    if (product.Q > 0)
                        products.Add(new LsiService.UtworzDokumentRozchodowyRequestProduktObject { Ilosc = product.Q, ProduktID = product.Index, Cena = 0 });
                }

                if (products.Count == 0)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania.\nZerowy lub ujemny bilans pozycji." };

                var lsiEndpointService = new LsiEndpointService(_config.EndpointUrl);
                var warehouses = await lsiEndpointService.GetWarehouses();
                var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains(_config.WarehouseSymbol)).MagazynID;
                if (warehouseId == null)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Nie odnaleziono magazynu {_config.WarehouseSymbol}" };
                var response = await lsiEndpointService.CreateDocument(documentType.DocumentTypeId, warehouseId, products);
                var docResp = response.Body.UtworzDokumentRozchodowyResult.Dokument;

                if (docResp != null)
                {
                    if (docResp.ID > 0)
                    {
                        await productionService.ChangeTransferStatus(
                              currentTrashProducts.Select(i => i.ProductionItem.ProductionItemId).ToArray(),
                              docResp.ID,
                              documentType.DocumentTypeId,
                              true);
                    }

                    if (docResp.KodBledu != 0)
                    {
                        respMessage.IsError = true;
                        respMessage.Message = $"Kod błędu: {docResp.KodBledu}\nOpis błędu: {docResp.OpisBledu}";
                    }
                    respMessage.Message = $"Dokument {_config.ProductionDocumentSymbol} utworzony.";
                }
                else
                {
                    respMessage.Message = "Totalny błąd.\nPrawdopodobne przyczyny:\n" +
                        "1. Endpoint LSI wyłączony.\n" +
                        "2. Endpint w niewłasciwej wersji.\n" +
                        "Nalezy to zgłosić do LSI.";
                    respMessage.IsError = true;
                }
            }
            catch (Exception ex)
            {
                respMessage.IsError = true;
                respMessage.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }

            return respMessage;
        }
    }
}
