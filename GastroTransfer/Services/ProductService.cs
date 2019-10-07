using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GastroTransfer.Data;
using GastroTransfer.Models;

namespace GastroTransfer.Services
{
    class ProductService : IProductService
    {
        public string ErrorMessage { get; protected set; }
        private AppDbContext dbContext { get; set; }
        public ProductService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool CreateProduct(ProducedItem producedItem)
        {
            try
            {
                dbContext.ProducedItems.Add(producedItem);
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }

        public bool RemoveProduct(ProducedItem producedItem)
        {
            try
            {
                dbContext.ProducedItems.Remove(producedItem);
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }

        public bool UpdateProduct(ProducedItem producedItem)
        {

            try
            {
                var itemToChange = dbContext.ProducedItems.Find(producedItem.ProducedItemId);

                itemToChange.Name = producedItem.Name;
                itemToChange.IsActive = producedItem.IsActive;
                itemToChange.UnitOfMesure = producedItem.UnitOfMesure;
                itemToChange.ConversionRate = producedItem.ConversionRate;
                itemToChange.ExternalId = producedItem.ExternalId;
                itemToChange.ExternalIndex = producedItem.ExternalIndex;
                itemToChange.ExternalName = producedItem.ExternalName;
                itemToChange.ExternalUnitOfMesure = producedItem.ExternalUnitOfMesure;
                itemToChange.ProductGroupId = producedItem.ProductGroupId;

                dbContext.Entry(itemToChange).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }

        public bool ChangeActiveStatus(int producedItemId)
        {
            try
            {
                var itemToChange = dbContext.ProducedItems.Find(producedItemId);

                itemToChange.IsActive = false;

                dbContext.Entry(itemToChange).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }
    }
}
