using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    internal class ProductService : IProductService
    {
        private AppDbContext _dbContext { get; set; }
        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ServiceMessage CreateProduct(ProducedItem producedItem)
        {
            var serviceMessage = new ServiceMessage()
            {
                IsError = false,
                Message = string.Empty,
                ItemId = 0
            };

            try
            {
                _dbContext.ProducedItems.Add(producedItem);
                serviceMessage.ItemId = _dbContext.SaveChanges();
                return serviceMessage;
            }
            catch (DbUpdateException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (Exception ex)
            {
                serviceMessage.Message = ex.Message;
            }
            serviceMessage.IsError = true;
            return serviceMessage;
        }

        public ServiceMessage RemoveProduct(ProducedItem producedItem)
        {
            var serviceMessage = new ServiceMessage()
            {
                IsError = false,
                Message = string.Empty,
                ItemId = 0
            };

            try
            {
                _dbContext.ProducedItems.Remove(producedItem);
                serviceMessage.ItemId = _dbContext.SaveChanges();
                return serviceMessage;
            }
            catch (DbUpdateException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (Exception ex)
            {
                serviceMessage.Message = ex.Message;
            }
            serviceMessage.IsError = true;
            return serviceMessage;
        }

        public ServiceMessage UpdateProduct(ProducedItem producedItem)
        {
            var serviceMessage = new ServiceMessage()
            {
                IsError = false,
                Message = string.Empty,
                ItemId = 0
            };

            try
            {
                var itemToChange = _dbContext.ProducedItems.FirstOrDefault(x => x.ExternalId == producedItem.ExternalId);
                if (itemToChange != null)
                {
                    itemToChange.Name = producedItem.Name;
                    itemToChange.IsActive = producedItem.IsActive;
                    itemToChange.UnitOfMesure = producedItem.UnitOfMesure;
                    itemToChange.ConversionRate = producedItem.ConversionRate;
                    itemToChange.ExternalId = producedItem.ExternalId;
                    itemToChange.ExternalIndex = producedItem.ExternalIndex;
                    itemToChange.ExternalName = producedItem.ExternalName;
                    itemToChange.ExternalUnitOfMesure = producedItem.ExternalUnitOfMesure;
                    itemToChange.ExternalGroupId = producedItem.ExternalGroupId;
                    _dbContext.Entry(itemToChange).State = EntityState.Modified;
                    serviceMessage.ItemId = _dbContext.SaveChanges();
                }
                return serviceMessage;
            }
            catch (DbUpdateException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (Exception ex)
            {
                serviceMessage.Message = ex.Message;
            }
            serviceMessage.IsError = true;
            return serviceMessage;
        }

        public async Task<ServiceMessage> ChangeActiveStatus(int externalGroupsId, bool isActive)
        {
            var serviceMessage = new ServiceMessage()
            {
                IsError = false,
                Message = string.Empty,
                ItemId = 0
            };

            try
            {
                await _dbContext.ProducedItems.Where(x => x.ExternalGroupId == externalGroupsId).ForEachAsync(x => x.IsActive = isActive);
                _dbContext.SaveChanges();

                return serviceMessage;
            }
            catch (DbUpdateException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                serviceMessage.Message = ex.Message;
            }
            catch (Exception ex)
            {
                serviceMessage.Message = ex.Message;
            }
            serviceMessage.IsError = true;
            return serviceMessage;
        }
    }
}
