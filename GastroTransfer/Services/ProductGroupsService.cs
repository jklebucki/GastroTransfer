using GastroTransfer.Data;
using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GastroTransfer.Services
{
    class ProductGroupsService : IProductGroupsService
    {
        private AppDbContext _appDbContext { get; set; }
        public ProductGroupsService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public List<ProductGroup> GetGroups()
        {
            return _appDbContext.ProductGroups.ToList();
        }

        public void AddGroup(ProductGroup group)
        {
            if (group != null)
            {
                var groupPresent = _appDbContext.ProductGroups.FirstOrDefault(i => i.ExternalGroupId == group.ExternalGroupId);
                if (groupPresent == null)
                {
                    try
                    {
                        _appDbContext.ProductGroups.Add(group);
                        _appDbContext.SaveChanges();
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
                var groupToUpdate = _appDbContext.ProductGroups.FirstOrDefault(id => id.ExternalGroupId == group.ExternalGroupId);
                if (groupToUpdate != null)
                {
                    try
                    {
                        groupToUpdate.GroupName = group.GroupName;
                        groupToUpdate.IsActive = group.IsActive;
                        _appDbContext.Entry(groupToUpdate).State = System.Data.Entity.EntityState.Modified;
                        _appDbContext.SaveChanges();
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
