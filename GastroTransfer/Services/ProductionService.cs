using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    class ProductionService : IProductionService
    {
        private AppDbContext _dbContext { get; set; }
        public ProductionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add product to database
        /// </summary>
        /// <param name="model">ProductionViewModel</param>
        /// <returns></returns>
        public ServiceMessage AddProduction(ProductionViewModel model)
        {
            ServiceMessage message = new ServiceMessage();
            try
            {
                model.ProductionItem.SentToExternalSystem = DateTime.Now;
                model.ProductionItem.ProducedItemId = model.ProducedItem.ProducedItemId;
                model.ProductionItem.IsSentToExternalSystem = false;
                model.ProductionItem.TransferType = -1;
                _dbContext.TransferredItems.Add(model.ProductionItem);
                _dbContext.SaveChanges();
                message.IsError = false;
                message.ItemId = model.ProductionItem.ProductionItemId;
                message.Message = "Added";
            }
            catch (Exception ex)
            {
                message.IsError = true;
                message.ItemId = model.ProductionItem.ProductionItemId;
                message.Message = ex.Message;
            }
            return message;
        }

        public ServiceMessage AddProduction(ProductionItem productionItem)
        {
            ServiceMessage message = new ServiceMessage();
            try
            {
                productionItem.SentToExternalSystem = DateTime.Now;
                productionItem.IsSentToExternalSystem = false;
                productionItem.Registered = DateTime.Now;
                productionItem.TransferType = -1;
                _dbContext.TransferredItems.Add(productionItem);
                _dbContext.SaveChanges();
                message.IsError = false;
                message.ItemId = productionItem.ProductionItemId;
                message.Message = "Added";
            }
            catch (Exception ex)
            {
                message.IsError = true;
                message.ItemId = productionItem.ProductionItemId;
                message.Message = ex.Message;
            }
            return message;
        }

        /// <summary>
        /// Get production 
        /// </summary>
        /// <param name="fullData">true - all records; false - only not sent to external system (current production)</param>
        /// <returns></returns>
        public List<ProductionViewModel> GetProduction(bool fullData)
        {
            List<ProductionViewModel> production = new List<ProductionViewModel>();
            var allItems = _dbContext.ProducedItems.ToList();
            List<ProductionItem> productionItems = new List<ProductionItem>();
            if (fullData)
                productionItems = _dbContext.TransferredItems.ToList();
            else
                productionItems = _dbContext.TransferredItems
                    .Where(tr => !tr.IsSentToExternalSystem)
                    .ToList();
            try
            {
                if (productionItems.Count > 0)
                {
                    foreach (var item in productionItems)
                    {
                        var producedItem = allItems.FirstOrDefault(i => i.ProducedItemId == item.ProducedItemId);
                        production.Add(new ProductionViewModel { ProducedItem = producedItem, ProductionItem = item });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return production;
        }

        /// <summary>
        /// Change transfer status
        /// </summary>
        /// <param name="productionId"></param>
        /// <param name="packageNumber"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public async Task<ServiceMessage> ChangeTransferStatus(int[] productionIds, int packageNumber, int documentType, bool swapStatus)
        {
            try
            {
                await _dbContext.TransferredItems
                      .Where(i => productionIds.Contains(i.ProductionItemId))
                      .ForEachAsync(n =>
                      {
                          n.IsSentToExternalSystem = true;
                          n.SentToExternalSystem = DateTime.Now;
                          n.DocumentType = n.TransferType > 0 ? n.TransferType : documentType;
                          n.PackageNumber = packageNumber;
                          n.TransferType = !swapStatus ? -3 : n.TransferType;
                      }).ConfigureAwait(false);
                _dbContext.SaveChanges();
                return new ServiceMessage { IsError = false, ItemId = 0, Message = "Status zmieniony" };
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsError = true, ItemId = 0, Message = ex.Message };
            }
        }

        /// <summary>
        /// Change swap status
        /// </summary>
        /// <param name="packageNumber"></param>
        /// <returns></returns>
        public async Task<ServiceMessage> ChangeSwapStatus(int packageNumber)
        {
            try
            {
                await _dbContext.TransferredItems
                      .Where(i => i.TransferType == -2)
                      .ForEachAsync(n =>
                      {
                          n.TransferType = packageNumber;
                      }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync();
                return new ServiceMessage { IsError = false, ItemId = 0, Message = "Status przeniesienia zmieniony" };
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsError = true, ItemId = 0, Message = ex.Message };
            }
        }

        /// <summary>
        /// Removes production from database
        /// </summary>
        /// <param name="productionId">ProductionItem.ProductionItemId</param>
        /// <returns></returns>
        public ServiceMessage RemoveProduction(int productionId)
        {
            try
            {
                var entitytoRemove = _dbContext.TransferredItems.Find(productionId);
                if (!entitytoRemove.IsSentToExternalSystem)
                {
                    _dbContext.TransferredItems.Remove(entitytoRemove);
                    _dbContext.SaveChanges();
                    return new ServiceMessage { IsError = false, ItemId = productionId, Message = "Removed" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsError = true, ItemId = productionId, Message = ex.Message };
            }
            return new ServiceMessage { IsError = true, ItemId = productionId, Message = "Usunięcie niemożliwe. Pozycja wysłana do systemu zewnętrznego." };
        }


    }
}
