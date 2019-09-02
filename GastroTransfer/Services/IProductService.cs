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
        bool CreateProduct(ProducedItem producedItem);
        bool RemoveProduct(ProducedItem producedItem);
        bool UpdateProduct(ProducedItem producedItem);
        bool ChangeActiveStatus(int producedItemId);
    }
}
