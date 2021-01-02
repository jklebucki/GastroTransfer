using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GastroTransfer.Services
{
    class ProductionService : IProductionService
    {
        private AppDbContext dbContext { get; set; }
        public ProductionService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                dbContext.TransferredItems.Add(model.ProductionItem);
                dbContext.SaveChanges();
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
                dbContext.TransferredItems.Add(productionItem);
                dbContext.SaveChanges();
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
            var allItems = dbContext.ProducedItems.ToList();
            List<ProductionItem> productionItems = new List<ProductionItem>();
            if (fullData)
                productionItems = dbContext.TransferredItems.ToList();
            else
                productionItems = dbContext.TransferredItems.Where(tr => !tr.IsSentToExternalSystem).ToList();
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
        public ServiceMessage ChangeTransferStatus(int[] productionIds, int packageNumber, int documentType)
        {
            try
            {
                dbContext.TransferredItems
                    .Where(i => productionIds.Contains(i.ProductionItemId))
                    .ForEachAsync(n =>
                    {
                        n.IsSentToExternalSystem = true;
                        n.SentToExternalSystem = DateTime.Now;
                        n.DocumentType = documentType;
                        n.PackageNumber = packageNumber;
                    }).Wait();
                dbContext.SaveChanges();
                return new ServiceMessage { IsError = false, ItemId = 0, Message = "Status zmieniony" };
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
                var entitytoRemove = dbContext.TransferredItems.Find(productionId);
                if (!entitytoRemove.IsSentToExternalSystem)
                {
                    dbContext.TransferredItems.Remove(entitytoRemove);
                    dbContext.SaveChanges();
                    return new ServiceMessage { IsError = false, ItemId = productionId, Message = "Removed" };
                }
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsError = true, ItemId = productionId, Message = ex.Message };
            }
            return new ServiceMessage { IsError = true, ItemId = productionId, Message = "Usunięcie niemożliwe. Pozycja wysłana do systemu zewnętrnego." };
        }

    }
}
