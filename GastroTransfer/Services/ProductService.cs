﻿using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    internal class ProductService : IProductService
    {
        private AppDbContext dbContext { get; set; }
        public ProductService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                dbContext.ProducedItems.Add(producedItem);
                serviceMessage.ItemId = dbContext.SaveChanges();
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
                dbContext.ProducedItems.Remove(producedItem);
                serviceMessage.ItemId = dbContext.SaveChanges();
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
                var itemToChange = dbContext.ProducedItems.FirstOrDefault(x => x.ExternalId == producedItem.ExternalId);
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
                    dbContext.Entry(itemToChange).State = EntityState.Modified;
                    serviceMessage.ItemId = dbContext.SaveChanges();
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

        public async Task<ServiceMessage> ChangeActiveStatus(List<int> externalGroupsIds, bool isActive)
        {
            var serviceMessage = new ServiceMessage()
            {
                IsError = false,
                Message = string.Empty,
                ItemId = 0
            };

            try
            {
                await dbContext.ProducedItems.Where(x => externalGroupsIds.Contains(x.ExternalGroupId)).ForEachAsync(x => x.IsActive = isActive);
                dbContext.SaveChanges();

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
