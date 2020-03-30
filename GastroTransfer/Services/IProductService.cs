using GastroTransfer.Models;
using System.Collections.Generic;

namespace GastroTransfer.Services
{
    interface IProductService
    {
        ServiceMessage CreateProduct(ProducedItem producedItem);
        ServiceMessage RemoveProduct(ProducedItem producedItem);
        ServiceMessage UpdateProduct(ProducedItem producedItem);
        ServiceMessage ChangeActiveStatus(List<int> producedItemId, bool isActive);
    }
}
