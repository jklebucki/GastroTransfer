using GastroTransfer.Models;
using System.Collections.Generic;

namespace GastroTransfer.Services
{
    interface IProductGroupsService
    {
        List<ProductGroup> GetGroups();
        void AddGroup(ProductGroup group);
        void UpdateGroup(ProductGroup group);
    }
}
