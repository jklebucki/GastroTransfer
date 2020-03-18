using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GastroTransfer.Data;
using GastroTransfer.Models;

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

        public ServiceMessage ChangeTransferStatus(int productionId)
        {
            throw new NotImplementedException();
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
                dbContext.TransferredItems.Remove(entitytoRemove);
                dbContext.SaveChanges();
                return new ServiceMessage { IsError = false, ItemId = productionId, Message = "Removed" };
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsError = true, ItemId = productionId, Message = ex.Message };
            }
        }

    }
}
