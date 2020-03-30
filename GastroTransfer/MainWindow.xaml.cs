using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views.Dialogs;
using LsiEndpointSupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace GastroTransfer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config config { get; set; }
        private ConfigService configService { get; set; }
        private AppDbContext appDbContext { get; set; }
        private DbService dbService { get; set; }
        private List<ProducedItem> producedItems { get; set; }
        private List<ProductGroup> productGroups { get; set; }
        private ObservableCollection<ProductionViewModel> productionViewItems { get; set; }
        private delegate void GetDataDelegate();
        private GetDataDelegate GetDataDelegateMethod;
        public MainWindow()
        {
            GetDataDelegateMethod = GetData;
            InitializeComponent();
            BackButton.Style = this.FindResource("RoundCorner") as Style;
            ConfigButton.Style = this.FindResource("RoundCorner") as Style;
            CloseButton.Style = this.FindResource("RoundCorner") as Style;
            ProductionButton.Style = this.FindResource("RoundCorner") as Style;
            GetProducts.Style = this.FindResource("RoundCorner") as Style;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSystem();
            CheckingConnection();
            productionViewItems = new ObservableCollection<ProductionViewModel>();
            //Getting products and products groups data  
            GetData();
            PositionsListGrid.DataContext = productionViewItems;
            PositionsListGrid.ItemsSource = productionViewItems;
            float scale = (float)SystemParameters.PrimaryScreenWidth / 1920f;
            ProductName.FontSize = 22 * scale;
            UnitOfMesure.FontSize = 22 * scale;
            Quantity.FontSize = 22 * scale;

            AddButtons(producedItems);
            AddGroupButtons(productGroups);
        }

        private void InitializeSystem()
        {
            //read or initialize config
            configService = new ConfigService(new Services.CryptoService());
            config = configService.GetConfig();
            if (config == null)
            {
                configService.InitializeConfig();
                config = configService.GetConfig();
            }
        }

        private void CheckingConnection()
        {
            CheckConnection checkConnection = new CheckConnection(dbService, configService, this.FindResource("RoundCorner") as Style);
            checkConnection.ShowDialog();
        }

        private void GetData()
        {
            configService = new ConfigService(new Services.CryptoService());
            config = configService.GetConfig();
            dbService = new DbService(config);
            //Database available
            appDbContext = new AppDbContext(dbService.GetConnectionString());
            if (appDbContext.ProductGroups.Count() == 0)
            {
                appDbContext.ProductGroups.AddRange(ConstData.productGroups);
                appDbContext.SaveChanges();
            }
            producedItems = appDbContext.ProducedItems.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
            productGroups = appDbContext.ProductGroups.Where(x => x.IsActive || x.ProductGroupId == 1).OrderBy(x => x.GroupName).ToList();
            AddButtons(producedItems);
            AddGroupButtons(productGroups);
            GetCurrentProduction();
        }

        /// <summary>
        /// Getting current production
        /// </summary>
        private void GetCurrentProduction()
        {
            ProductionService productionService = new ProductionService(appDbContext);
            var production = productionService.GetProduction(false);
            productionViewItems.Clear();
            foreach (var product in production)
                productionViewItems.Add(product);
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configPage = new ConfigWindow(this.FindResource("RoundCorner") as Style, this);
            configPage.ShowDialog();
            if (configPage.IsSaved)
            {
                config = configService.GetConfig();
            }
        }

        private void Production_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var productId = int.Parse(btn.Name.Split('_')[1]);
            var item = producedItems.FirstOrDefault(x => x.ProducedItemId == productId);
            var measurementWindow = new MeasurementWindow(this.FindResource("RoundCorner") as Style, item.Name, config);
            measurementWindow.ShowDialog();
            if (!measurementWindow.IsCanceled)
            {
                try
                {
                    var productionViewModel = new ProductionViewModel
                    {
                        ProducedItem = item,
                        ProductionItem = new ProductionItem
                        {
                            Quantity = !(bool)ToggleIn.IsChecked ? measurementWindow.Quantity : measurementWindow.Quantity * -1,
                            Registered = DateTime.Now
                        }
                    };

                    ProductionService productionService = new ProductionService(appDbContext);
                    var message = productionService.AddProduction(productionViewModel);
                    if (!message.IsError)
                    {
                        productionViewModel.ProductionItem.ProductionItemId = message.ItemId;
                        productionViewItems.Add(productionViewModel);
                        PositionsListGrid.Items.Refresh();
                        PositionsListGrid.SelectedItem = PositionsListGrid.Items[PositionsListGrid.Items.Count - 1];
                        PositionsListGrid.ScrollIntoView(PositionsListGrid.Items[PositionsListGrid.Items.Count - 1]);
                    }
                    else
                    {
                        throw new Exception($"Item id:{message.ItemId}\nMessage: {message.Message}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (productionViewItems.Count > 0)
            {
                var productToRemove = productionViewItems.Last();
                if (productToRemove != null)
                {
                    ProductionService productionService = new ProductionService(appDbContext);
                    var messsge = productionService.RemoveProduction(productToRemove.ProductionItem.ProductionItemId);
                    if (!messsge.IsError)
                    {
                        productionViewItems.Remove(productToRemove);
                        if (productionViewItems.Count > 0)
                        {
                            PositionsListGrid.SelectedItem = PositionsListGrid.Items[PositionsListGrid.Items.Count - 1];
                            PositionsListGrid.ScrollIntoView(PositionsListGrid.Items[PositionsListGrid.Items.Count - 1]);
                        }
                    }
                }
            }
            PositionsListGrid.Items.Refresh();
        }

        private void Button_Click_Filter(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var groupId = int.Parse(btn.Tag.ToString());// btn.Name.Split('_')[1]);
            GetData();
            var items = producedItems.Where(x => x.ExternalGroupId == groupId).OrderBy(n => n.Name).ToList();
            if (groupId == 0)
                AddButtons(producedItems);
            else
                AddButtons(items);
        }

        private Button CreateProductButton(ProducedItem item)
        {
            Viewbox box = new Viewbox
            {
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 160,
            };

            TextBlock text = new TextBlock
            {
                Text = $"{item.Name}",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 160,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 5)
            };
            box.Child = text;

            Button button = new Button()
            {
                Name = $"N_{item.ProducedItemId}",
                Content = box,
                Tag = item.ExternalGroupId,
                Height = 90,
                Width = 180,
                Margin = new Thickness(5, 5, 5, 5),
                FontSize = 24,
                Style = this.FindResource("RoundCorner") as Style
            };
            button.Click += new RoutedEventHandler(Production_Button_Click);
            return button;
        }

        private void AddButtons(List<ProducedItem> producedItems)
        {
            this.WrapButtons.Children.Clear();
            foreach (var item in producedItems)
            {
                this.WrapButtons.Children.Add(CreateProductButton(item));
            }
        }

        private Button CreateFilterButton(ProductGroup item)
        {
            Viewbox box = new Viewbox
            {
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 160,
            };

            TextBlock text = new TextBlock
            {
                Text = $"{item.GroupName}",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 160,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 5)
            };
            box.Child = text;

            Button button = new Button()
            {
                Name = $"N_{item.ExternalGroupId}",
                Content = box,
                Tag = item.ExternalGroupId,
                Height = 35,
                Width = 180,
                Margin = new Thickness(5, 5, 5, 5),
                FontSize = 24,
                Style = this.FindResource("RoundCorner") as Style
            };
            button.Click += new RoutedEventHandler(Button_Click_Filter);
            return button;
        }

        private void AddGroupButtons(List<ProductGroup> productGroups)
        {
            this.GroupButtons.Children.Clear();
            foreach (var item in productGroups)
            {
                this.GroupButtons.Children.Add(CreateFilterButton(item));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            var message = await StartProduction();
            GetDataDelegateMethod.Invoke();
            MessageBox.Show(message.Message, "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task<ServiceMessage> StartProduction()
        {
            if (!dbService.CheckLsiConnection())
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak połączenia z bazą danych systemu LSI" };

            LsiDbService lsiDbService = new LsiDbService(dbService.GetLsiConnectionString());
            var documentType = lsiDbService.GetDocumentsTypes().FirstOrDefault(x => x.Symbol == config.ProductionDocumentSymbol);

            if (documentType == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Niewłaściwy typ dokumentu produkcji" };

            if (string.IsNullOrEmpty(config.EndpointUrl))
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Brak konfiduracji usług" };

            var productionService = new ProductionService(appDbContext);
            var currentProduction = productionService.GetProduction(false);
            if (currentProduction.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania." };

            var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
            var sum = currentProduction.GroupBy(i => i.ProductionItem.ProducedItemId).Select(r => new { Index = r.Select(s => s.ProducedItem.ExternalId).First(), Q = r.Sum(q => q.ProductionItem.Quantity) }).ToList();
            //return;
            foreach (var product in sum)
            {
                if (product.Q > 0)
                    products.Add(new LsiService.UtworzDokumentRozchodowyRequestProduktObject { Ilosc = product.Q, ProduktID = product.Index, Cena = 0 });
            }

            if (products.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie ma nic do wyprodukowania.\nZerowy lub ujemny bilans pozycji." };

            LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            var response = await service.CreateDocument(documentType.DocumentTypeId, warehouseId, products);
            var docResp = response.Body.UtworzDokumentRozchodowyResult.Dokument;
            var respMessage = "";
            if (docResp != null)
            {
                if (docResp.ID > 0)
                    foreach (var product in currentProduction)
                    {
                        productionService.ChangeTransferStatus(product.ProductionItem.ProductionItemId, docResp.ID, documentType.DocumentTypeId);
                    }
                respMessage = $"Id dokumentu: {docResp.ID}\nNumer: {docResp.Numer}\nKod błędu: {docResp.KodBledu}\nOpis błędu: {docResp.OpisBledu}";
            }
            else
                respMessage = "Totalny błąd";
            return new ServiceMessage { IsError = true, ItemId = 0, Message = respMessage };
        }

        private void GetProductsFromEndpoint_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow(this, appDbContext, this.FindResource("RoundCorner") as Style, config);
            productsWindow.ShowDialog();
            GetData();
        }

    }
}
