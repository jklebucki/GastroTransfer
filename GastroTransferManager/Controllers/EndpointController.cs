using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsiEndpointSupport;

namespace GastroTransferManager.Controllers
{
    public class EndpointController : Controller
    {
        private LsiEndpointService service { get; set; }
        public EndpointController()
        {
            service = new LsiEndpointService("http://192.168.81.70:8089/icws.asmx");
        }
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> GetProducts()
        {
            var groups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var groupId = groups.FirstOrDefault(x => x.Nazwa.Contains("SUROWCE KOMPLETACJA MOP")).ID;
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            return Json(await service.GetProducts(groupId, warehouseId));
        }

        public async Task<IActionResult> GetMeals()
        {
            var groups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var groupId = groups.FirstOrDefault(x => x.Nazwa.Contains("RECEPTURY")).ID;
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            return Json(await service.GetMeals(groupId, warehouseId));
        }

        public async Task<IActionResult> GetProductsGroups()
        {
            return Json(await service.GetProductsGroups());
        }
    }
}