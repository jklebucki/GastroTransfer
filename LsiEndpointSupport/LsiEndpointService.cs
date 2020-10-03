using LsiService;
using System;
using System.Threading.Tasks;

namespace LsiEndpointSupport
{
    public class LsiEndpointService
    {
        private CWSSoapClient lsiService { get; set; }
        public LsiEndpointService(string endpointUrl)
        {
            lsiService = new CWSSoapClient(CWSSoapClient.EndpointConfiguration.ICWSSoap, endpointUrl);
        }

        public async Task<ArrayOfPobierzProduktyProduktObject> GetProducts(int groupId, string magazynId)
        {
            var getProducsGroups = await GetProductsGroups();
            var getWarehouses = await GetWarehouses();
            PobierzProduktyResponse response = new PobierzProduktyResponse();
            try
            {
                response = await lsiService.PobierzProduktyAsync(new PobierzProduktyRequest
                {
                    Body = new PobierzProduktyRequestBody
                    {
                        RequestData = new PobierzProduktyRequestDataRequestObject
                        {
                            GrupaTowID = groupId,
                            MagazynID = magazynId
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response.Body.PobierzProduktyResult.Produkty;
        }

        public async Task<ArrayOfPobierzPotrawyProduktObject> GetMeals(int groupId, string magazynId)
        {
            var getProducsGroups = await GetProductsGroups();
            var getWarehouses = await GetWarehouses();
            PobierzPotrawyResponse response = new PobierzPotrawyResponse();
            try
            {
                response = await lsiService.PobierzPotrawyAsync(new PobierzPotrawyRequest
                {
                    Body = new PobierzPotrawyRequestBody
                    {
                        RequestData = new PobierzPotrawyRequestDataRequestObject
                        {
                            GrupaTowID = groupId,
                            MagazynID = magazynId
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response.Body.PobierzPotrawyResult.Produkty;
        }

        public async Task<ArrayOfPobierzGrupyTowaroweGrupaTowarowaObject> GetProductsGroups()
        {
            try
            {
                var response = await lsiService.PobierzGrupyTowaroweAsync(new PobierzGrupyTowaroweRequest { Body = new PobierzGrupyTowaroweRequestBody() });
                return response.Body.PobierzGrupyTowaroweResult.GrupyTowarowe;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ArrayOfPobierzMagazynyMagazynObject> GetWarehouses()
        {
            var response = await lsiService.PobierzMagazynyAsync(new PobierzMagazynyRequest { Body = new PobierzMagazynyRequestBody { } });
            return response.Body.PobierzMagazynyResult.Magazyny;
        }

        public async Task<UtworzDokumentRozchodowyResponse> CreateDocument(int documentTypeId, string magazynID, ArrayOfUtworzDokumentRozchodowyRequestProduktObject products)
        {

            var request = new UtworzDokumentRozchodowyRequestDataRequestObject { TypDokumentuID = documentTypeId, MagazynID = magazynID, Produkty = products };
            var response = await lsiService.UtworzDokumentRozchodowyAsync(new UtworzDokumentRozchodowyRequest
            {
                Body = new UtworzDokumentRozchodowyRequestBody { RequestData = request }
            });
            return response;
        }
    }
}
