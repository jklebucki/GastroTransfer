using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
