using LsiEndpointSupport.Models;
using LsiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsiEndpointSupport
{
    public class Info
    {
        private Config config { get; set; }
        private CWSSoapClient lsiService { get; set; }
        private Endpoint endpoint { get; set; }
        public Info()
        {
            config = new Config();
            endpoint = config.Endpoints.FirstOrDefault(s => s.Selected == true);
            lsiService = new CWSSoapClient(CWSSoapClient.EndpointConfiguration.ICWSSoap, endpoint.Url);
        }
        public async Task<SystemInfo> GetInfo()
        {
            PobierzMagazynyResponse magazynyResponse = new PobierzMagazynyResponse();
            var systemInfo = new SystemInfo();
            systemInfo.EndpointUrl = lsiService.Endpoint.Address.Uri.ToString();
            systemInfo.EndpointName = endpoint.Name;
            systemInfo.Warehouses = new List<Warehouse>();
            try
            {
                magazynyResponse = await lsiService.PobierzMagazynyAsync(new PobierzMagazynyRequest { Body = new PobierzMagazynyRequestBody() });
                var magazyny = magazynyResponse.Body.PobierzMagazynyResult.Magazyny;
                List<Warehouse> warehouses = new List<Warehouse>();
                foreach (var magazyn in magazyny)
                {
                    warehouses.Add(new Warehouse { Id = magazyn.MagazynID, Name = magazyn.Opis, Symbol = magazyn.Symbol });
                }
                systemInfo.Warehouses = warehouses;
            }
            catch (Exception ex)
            {
                systemInfo.EndpointUrl = ex.Message;
            }

            return systemInfo;
        }
    }
}
