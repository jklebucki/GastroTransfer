using LsiEndpointSupport.Models;
using LsiService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LsiEndpointSupport
{
    public class Info
    {
        private CWSSoapClient _lsiService { get; set; }
        public Info(string endpointUrl)
        {
            _lsiService = new CWSSoapClient(CWSSoapClient.EndpointConfiguration.ICWSSoap, endpointUrl);
        }
        public async Task<SystemInfo> GetInfo()
        {
            PobierzMagazynyResponse magazynyResponse = new PobierzMagazynyResponse();
            var systemInfo = new SystemInfo();
            systemInfo.EndpointUrl = _lsiService.Endpoint.Address.Uri.ToString();
            systemInfo.EndpointName = "Serwis LSI";
            systemInfo.Warehouses = new List<Warehouse>();
            try
            {
                magazynyResponse = await _lsiService.PobierzMagazynyAsync(new PobierzMagazynyRequest { Body = new PobierzMagazynyRequestBody() });
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
