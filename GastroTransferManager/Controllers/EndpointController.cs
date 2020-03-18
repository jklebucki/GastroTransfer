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
        private Service service { get; set; }
        public EndpointController()
        {
            service = new Service();
        }
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> GetProducts()
        {
            return Json(await service.GetProducts());
        }

        public async Task<IActionResult> GetMeals()
        {
            return Json(await service.GetMeals());
        }

        public async Task<IActionResult> GetProductsGroups()
        {
            return Json(await service.GetProductsGroups());
        }
    }
}