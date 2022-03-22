using GastroTransfer.Data;
using GastroTransfer.Models;
using LsiEndpointSupport;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    public class ProductionTransferService
    {
        private readonly DbService _dbService;
        private readonly AppDbContext _appDbContext;
        private readonly Config _config;
        private readonly Logger _logger;
        public ProductionTransferService(DbService dbService, AppDbContext appDbContext, Config config, Logger logger)
        {
            _dbService = dbService;
            _appDbContext = appDbContext;
            _config = config;
            _logger = logger;
        }
        public async Task<ServiceMessage> StartProduction(DateTime productionDate, bool swapProduction)
        {
            var respMessage = new ServiceMessage { IsError = false, ItemId = 0, Message = "" };
            try
            {
                if (!_dbService.CheckLsiConnection())
                {
                    var message = "Brak połączenia z bazą danych systemu LSI";
                    _logger.Error(message);
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = message };
                }


                LsiDbService lsiDbService = new LsiDbService(_dbService.GetLsiConnectionString());
                var documentType = lsiDbService.GetDocumentsTypes().FirstOrDefault(x => x.Symbol == _config.ProductionDocumentSymbol);

                if (documentType == null)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Nie znalazłem dokumentu {_config.ProductionDocumentSymbol} w systemie LSI." };

                if (string.IsNullOrEmpty(_config.EndpointUrl))
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak konfiguracji usługi LSI" };

                var productionService = new ProductionService(_appDbContext);
                var dateTo = new DateTime(productionDate.Year, productionDate.Month, productionDate.Day, 23, 59, 59);
                var currentProduction = productionService
                    .GetProduction(false)
                    .Where(d => d.ProductionItem.Registered <= dateTo && (d.ProductionItem.OperationType == null || d.ProductionItem.OperationType == 1))
                    .ToList();
                if (currentProduction.Count == 0)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania." };


                var sum = currentProduction.GroupBy(i => i.ProductionItem.ProducedItemId)
                    .Select(r => new { Id = r.First().ProducedItem.ProducedItemId, Index = r.First().ProducedItem.ExternalId, Q = r.Sum(q => q.ProductionItem.Quantity) })
                    .ToList();

                var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
                var productsToSwap = new List<ProductionItem>();
                foreach (var product in sum)
                {
                    respMessage.ItemId = product.Id;
                    if (product.Q > 0)
                        products.Add(new LsiService.UtworzDokumentRozchodowyRequestProduktObject { Ilosc = product.Q, ProduktID = product.Index, Cena = 0 });
                    else if (product.Q < 0)
                        if (swapProduction)
                            productsToSwap.Add(new ProductionItem { ProducedItemId = product.Id, Quantity = product.Q, TransferType = -2 });
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
                              currentProduction.Select(i => i.ProductionItem.ProductionItemId).ToArray(),
                              docResp.ID,
                              documentType.DocumentTypeId,
                              swapProduction);

                        if (swapProduction)
                            await productionService.ChangeSwapStatus(docResp.ID);
                        lsiDbService.AddDocumentInfo(docResp.ID, $"Produkcja, Ilość razem: {sum.Sum(x => x.Q)}", productionDate);
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
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.Error(message);
                respMessage.IsError = true;
                respMessage.Message = message;
            }

            return respMessage;
        }
    }
}
