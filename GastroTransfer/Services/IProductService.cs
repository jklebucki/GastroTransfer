using GastroTransfer.Models;

namespace GastroTransfer.Services
{
    interface IProductService
    {
        ServiceMessage CreateProduct(ProducedItem producedItem);
        ServiceMessage RemoveProduct(ProducedItem producedItem);
        ServiceMessage UpdateProduct(ProducedItem producedItem);
        ServiceMessage ChangeActiveStatus(int producedItemId);
    }
}
