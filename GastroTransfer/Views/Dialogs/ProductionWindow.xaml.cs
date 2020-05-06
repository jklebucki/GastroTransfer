using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using LsiEndpointSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductionWindow.xaml
    /// </summary>
    public partial class ProductionWindow : Window
    {
        private DbService dbService { get; set; }
        private AppDbContext appDbContext { get; set; }
        private Config config { get; set; }
        public ProductionWindow(DbService dbService, AppDbContext appDbContext, Config config)
        {
            this.dbService = dbService;
            this.appDbContext = appDbContext;
            this.config = config;
            InitializeComponent();
            DocumentType.Text += config.ProductionDocumentSymbol;
            WarehouseSymbol.Text += config.WarehouseSymbol;
            ProductionDate.SelectedDate = DateTime.Now;
            ProductionDate.DisplayDateEnd = DateTime.Now;
            ProductionDate.SelectedDateChanged += ProductionDate_SelectedDateChanged;
        }

        private void ProductionDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = (DatePicker)sender;
            if (selectedDate.SelectedDate == null || selectedDate.SelectedDate > DateTime.Now)
                ProductionDate.SelectedDate = DateTime.Now;
        }

        private async Task<ServiceMessage> StartProduction()
        {
            if (!dbService.CheckLsiConnection())
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak połączenia z bazą danych systemu LSI" };

            LsiDbService lsiDbService = new LsiDbService(dbService.GetLsiConnectionString());
            var documentType = lsiDbService.GetDocumentsTypes().FirstOrDefault(x => x.Symbol == config.ProductionDocumentSymbol);

            if (documentType == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = $"Niewłaściwy typ dokumentu produkcji - {config.ProductionDocumentSymbol}" };

            if (string.IsNullOrEmpty(config.EndpointUrl))
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak konfiguracji usługi LSI" };

            var productionService = new ProductionService(appDbContext);
            var selectedDate = (DateTime)ProductionDate.SelectedDate;
            var currentProduction = productionService.GetProduction(false).Where(d => d.ProductionItem.Registered <= new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 23, 59, 59)).ToList();
            if (currentProduction.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania." };

            var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
            var sum = currentProduction.GroupBy(i => i.ProductionItem.ProducedItemId).Select(r => new { Id = r.First().ProducedItem.ProducedItemId, Index = r.First().ProducedItem.ExternalId, Q = r.Sum(q => q.ProductionItem.Quantity) }).ToList();
            //return;
            var productsToSwap = new List<ProductionItem>();
            foreach (var product in sum)
            {
                if (product.Q > 0)
                    products.Add(new LsiService.UtworzDokumentRozchodowyRequestProduktObject { Ilosc = product.Q, ProduktID = product.Index, Cena = 0 });
                else if (product.Q < 0)
                    if ((bool)SwapProduction.IsChecked)
                        productsToSwap.Add(new ProductionItem { ProducedItemId = product.Id, Quantity = product.Q, TransferType = 5 });
            }

            if (products.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania.\nZerowy lub ujemny bilans pozycji." };

            LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            var response = await service.CreateDocument(documentType.DocumentTypeId, warehouseId, products);
            var docResp = response.Body.UtworzDokumentRozchodowyResult.Dokument;
            var respMessage = new ServiceMessage { IsError = false, ItemId = 0, Message = "" };
            if (docResp != null)
            {
                if (docResp.ID > 0)
                {
                    productionService.ChangeTransferStatus(
                        currentProduction.Select(i => i.ProductionItem.ProductionItemId).ToArray(),
                        docResp.ID,
                        documentType.DocumentTypeId);
                    lsiDbService.AddDocumentInfo(docResp.ID, $"Produkcja, Ilość razem: {sum.Sum(x => x.Q)}", (DateTime)ProductionDate.SelectedDate);
                }

                if (docResp.KodBledu != 0)
                {
                    respMessage.IsError = true;
                    respMessage.Message = $"Kod błędu: {docResp.KodBledu}\nOpis błędu: {docResp.OpisBledu}";
                }
                respMessage.Message = $"Dokument {config.ProductionDocumentSymbol} utworzony.";
            }
            else
            {
                respMessage.Message = "Totalny błąd";
                respMessage.IsError = true;
            }
            return respMessage;
        }

        private async void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionButton.IsEnabled = false;
            var message = await StartProduction();
            Info.Text = message.Message;
            if (message.IsError)
                Info.Foreground = Brushes.Red;
            Info.FontWeight = FontWeights.Bold;
            ProductionButton.IsEnabled = true;
        }

        private void SwapProduction_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UncheckedInfo == null)
                return;
            var item = (CheckBox)sender;
            if (!(bool)item.IsChecked)
                UncheckedInfo.Text = "Spowoduje to nieowracalne usunięcie nierozliczonych zwrotów.";
            else
                UncheckedInfo.Text = "";
        }
    }
}
