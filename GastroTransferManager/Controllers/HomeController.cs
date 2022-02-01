using GastroTransferManager.Models;
using GastroTransferManager.Models.Interfaces;
using LsiEndpointSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GastroTransferManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string lsiEndpoinAddress;

        public HomeController(ILogger<HomeController> logger, IAddressesConfig addressesConfig)
        {
            lsiEndpoinAddress = addressesConfig.LsiEndpointAddress;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SystemInfo()
        {
            Info info = new Info(lsiEndpoinAddress);
            ViewBag.Message = await info.GetInfo();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
