using GastroTransferManager.Models.Interfaces;
using LsiEndpointSupport;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GastroTransferManager.Controllers
{
    public class EndpointController : Controller
    {
        private LsiEndpointService service { get; set; }
        public EndpointController(IAddressesConfig addressesConfig)
        {
            service = new LsiEndpointService(addressesConfig.LsiEndpointAddress);
        }
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> GetProducts()
        {
            var groups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var groupId = groups.FirstOrDefault(x => x.Nazwa.Contains("TOWARY MOP")).ID;
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            return Json(await service.GetProducts(groupId, warehouseId));
        }

        public async Task<IActionResult> GetMeals()
        {
            var groups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var groupId = groups.FirstOrDefault(x => x.Nazwa.Contains("RP")).ID;
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            return Json(await service.GetMeals(groupId, warehouseId));
        }


        public async Task<IActionResult> GetProductsGroups()
        {
            return Json(await service.GetProductsGroups());
        }
    }
}