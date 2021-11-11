using GastroTransfer.Data;
using GastroTransfer.Models;
using LsiEndpointSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    public class ProductionTransferService
    {
        private DbService dbService { get; set; }
        private AppDbContext appDbContext { get; set; }
        private Config config { get; set; }
        public ProductionTransferService(DbService dbService, AppDbContext appDbContext, Config config)
        {
            this.dbService = dbService;
            this.appDbContext = appDbContext;
            this.config = config;
        }
        public async Task<ServiceMessage> StartProduction(DateTime productionDate, bool swapProduction)
        {
            var respMessage = new ServiceMessage { IsError = false, ItemId = 0, Message = "" };
            try
            {
                if (!dbService.CheckLsiConnection())
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak połączenia z bazą danych systemu LSI" };

                LsiDbService lsiDbService = new LsiDbService(dbService.GetLsiConnectionString());
                var documentType = lsiDbService.GetDocumentsTypes().FirstOrDefault(x => x.Symbol == config.ProductionDocumentSymbol);

                if (documentType == null)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Niewłaściwy typ dokumentu produkcji - {config.ProductionDocumentSymbol}" };

                if (string.IsNullOrEmpty(config.EndpointUrl))
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak konfiguracji usługi LSI" };

                var productionService = new ProductionService(appDbContext);
                var selectedDate = productionDate;
                var currentProduction = productionService.GetProduction(false).Where(d => d.ProductionItem.Registered <= new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 23, 59, 59)).ToList();
                if (currentProduction.Count == 0)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania." };

                var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
                var sum = currentProduction.GroupBy(i => i.ProductionItem.ProducedItemId)
                    .Select(r => new { Id = r.First().ProducedItem.ProducedItemId, Index = r.First().ProducedItem.ExternalId, Q = r.Sum(q => q.ProductionItem.Quantity) })
                    .ToList();
                //return;
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

                LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
                var productsGroups = await service.GetProductsGroups();
                var warehouses = await service.GetWarehouses();
                var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains(config.WarehouseSymbol)).MagazynID;
                if (warehouseId == null)
                    return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Nie odnaleziono magazynu {config.WarehouseSymbol}" };
                var response = await service.CreateDocument(documentType.DocumentTypeId, warehouseId, products);
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
                    respMessage.Message = $"Dokument {config.ProductionDocumentSymbol} utworzony.";
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
