using GastroTransfer.Models;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    interface IProductService
    {
        ServiceMessage CreateProduct(ProducedItem producedItem);
        ServiceMessage RemoveProduct(ProducedItem producedItem);
        ServiceMessage UpdateProduct(ProducedItem producedItem);
        Task<ServiceMessage> ChangeActiveStatus(int producedItemId, bool isActive);
    }
}
