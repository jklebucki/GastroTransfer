using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GastroTransfer.Services
{
    class ProductGroupsService : IProductGroupsService
    {
        private AppDbContext appDbContext { get; set; }
        public ProductGroupsService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public List<ProductGroup> GetGroups()
        {
            return appDbContext.ProductGroups.ToList();
        }

        public void AddGroup(ProductGroup group)
        {
            if (group != null)
            {
                var groupPresent = appDbContext.ProductGroups.FirstOrDefault(i => i.ExternalGroupId == group.ExternalGroupId);
                if (groupPresent == null)
                {
                    try
                    {
                        appDbContext.ProductGroups.Add(group);
                        appDbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    UpdateGroup(group);
                }
            }
            else
            {
                throw new Exception("Group parameter is null");
            }
        }

        public void UpdateGroup(ProductGroup group)
        {
            if (group != null)
            {
                var groupToUpdate = appDbContext.ProductGroups.FirstOrDefault(id => id.ExternalGroupId == group.ExternalGroupId);
                if (groupToUpdate != null)
                {
                    try
                    {
                        groupToUpdate.GroupName = group.GroupName;
                        groupToUpdate.IsActive = group.IsActive;
                        appDbContext.Entry(groupToUpdate).State = System.Data.Entity.EntityState.Modified;
                        appDbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                    throw new Exception("Group not found");
            }
            else
                throw new Exception("Group parameter is null");
        }
    }
}
