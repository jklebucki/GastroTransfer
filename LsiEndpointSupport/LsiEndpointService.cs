using LsiService;
using System;
using System.Threading.Tasks;

namespace LsiEndpointSupport
{
    public class LsiEndpointService
    {
        private CWSSoapClient _lsiService;
        public string ErrorMessage { get; protected set; }
        public LsiEndpointService(string endpointUrl)
        {
            InitService(endpointUrl);
        }

        private void InitService(string endpointUrl)
        {
            try
            {
                _lsiService = new CWSSoapClient(CWSSoapClient.EndpointConfiguration.ICWSSoap, endpointUrl);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public async Task<ArrayOfPobierzProduktyProduktObject> GetProducts(int groupId, string magazynId)
        {
            ArrayOfPobierzProduktyProduktObject pobierzProduktyProduktObjects = new ArrayOfPobierzProduktyProduktObject();
            try
            {
                var response = await _lsiService.PobierzProduktyAsync(new PobierzProduktyRequest
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
                pobierzProduktyProduktObjects = response.Body.PobierzProduktyResult.Produkty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return pobierzProduktyProduktObjects;
        }

        public async Task<ArrayOfPobierzPotrawyProduktObject> GetMeals(int groupId, string magazynId)
        {
            ArrayOfPobierzPotrawyProduktObject pobierzPotrawyProduktObjects = new ArrayOfPobierzPotrawyProduktObject();
            try
            {
                var response = await _lsiService.PobierzPotrawyAsync(new PobierzPotrawyRequest
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
                pobierzPotrawyProduktObjects = response.Body.PobierzPotrawyResult.Produkty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }

            return pobierzPotrawyProduktObjects;
        }

        public async Task<ArrayOfPobierzGrupyTowaroweGrupaTowarowaObject> GetProductsGroups()
        {
            ArrayOfPobierzGrupyTowaroweGrupaTowarowaObject pobierzGrupyTowaroweGrupaTowarowaObjects = new ArrayOfPobierzGrupyTowaroweGrupaTowarowaObject();
            try
            {
                var pobierzGrupyTowaroweResponse = await _lsiService.PobierzGrupyTowaroweAsync(new PobierzGrupyTowaroweRequest { Body = new PobierzGrupyTowaroweRequestBody() });
                pobierzGrupyTowaroweGrupaTowarowaObjects = pobierzGrupyTowaroweResponse.Body.PobierzGrupyTowaroweResult.GrupyTowarowe;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return pobierzGrupyTowaroweGrupaTowarowaObjects;
        }

        public async Task<ArrayOfPobierzMagazynyMagazynObject> GetWarehouses()
        {
            ArrayOfPobierzMagazynyMagazynObject pobierzMagazynyMagazynObjects = new ArrayOfPobierzMagazynyMagazynObject();
            try
            {
                var response = await _lsiService.PobierzMagazynyAsync(new PobierzMagazynyRequest { Body = new PobierzMagazynyRequestBody { } });
                pobierzMagazynyMagazynObjects =  response.Body.PobierzMagazynyResult.Magazyny;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return pobierzMagazynyMagazynObjects;
        }

        public async Task<UtworzDokumentRozchodowyResponse> CreateDocument(int documentTypeId, string magazynID, ArrayOfUtworzDokumentRozchodowyRequestProduktObject products)
        {
            try
            {
                var request = new UtworzDokumentRozchodowyRequestDataRequestObject { TypDokumentuID = documentTypeId, MagazynID = magazynID, Produkty = products };
                var response = await _lsiService.UtworzDokumentRozchodowyAsync(new UtworzDokumentRozchodowyRequest
                {
                    Body = new UtworzDokumentRozchodowyRequestBody { RequestData = request }
                });
                return response;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(ErrorMessage);
            }
        }
    }
}
