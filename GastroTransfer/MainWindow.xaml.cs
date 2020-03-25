using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using LsiEndpointSupport;

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
        private List<ProductionViewModel> productionView { get; set; }

        public MainWindow()
        {
            var cultureInfo = new CultureInfo("pl-PL");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
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
            productionView = new List<ProductionViewModel>();
            //Getting products and products groups data  
            GetData();
            PositionsListGrid.DataContext = productionView;
            PositionsListGrid.ItemsSource = productionView;
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
                //appDbContext.ProducedItems.AddRange(producedItems);
                appDbContext.ProductGroups.AddRange(ConstData.productGroups);
                appDbContext.SaveChanges();
            }
            producedItems = appDbContext.ProducedItems.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
            productGroups = appDbContext.ProductGroups.ToList();
            GetCurrentProduction();
        }

        private void GetCurrentProduction()
        {
            //get last production
            ProductionService productionService = new ProductionService(appDbContext);
            var currentProduction = productionService.GetProduction(false);
            productionView.Clear();
            PositionsListGrid.Items.Refresh();
            foreach (var item in currentProduction)
            {
                productionView.Add(item);
            }
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
                        productionView.Add(productionViewModel);
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
            if (productionView.Count > 0)
            {
                var productToRemove = productionView.Last();
                if (productToRemove != null)
                {
                    ProductionService productionService = new ProductionService(appDbContext);
                    var messsge = productionService.RemoveProduction(productToRemove.ProductionItem.ProductionItemId);
                    if (!messsge.IsError)
                    {
                        productionView.Remove(productToRemove);
                        if (productionView.Count > 0)
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
            var groupId = int.Parse(btn.Name.Split('_')[1]);
            GetData();
            var items = producedItems.Where(x => x.ProductGroupId == groupId).OrderBy(n => n.Name).ToList();
            if (groupId == 1)
                AddButtons(producedItems);
            else
                AddButtons(items);
        }

        private void AddButtons(List<ProducedItem> producedItems)
        {
            this.WrapButtons.Children.Clear();
            foreach (var item in producedItems)
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
                    Tag = item.ProductGroupId,
                    Height = 90,
                    Width = 180,
                    Margin = new Thickness(5, 5, 5, 5),
                    FontSize = 24,
                    Style = this.FindResource("RoundCorner") as Style
                };
                button.Click += new RoutedEventHandler(Production_Button_Click);
                this.WrapButtons.Children.Add(button);
            }
        }

        private void AddGroupButtons(List<ProductGroup> productGroups)
        {
            this.GroupButtons.Children.Clear();
            foreach (var item in productGroups)
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
                    Name = $"N_{item.ProductGroupId}",
                    Content = box,
                    Tag = item.GroupName,
                    Height = 35,
                    Width = 180,
                    Margin = new Thickness(5, 5, 5, 5),
                    FontSize = 24,
                    Style = this.FindResource("RoundCorner") as Style
                };
                button.Click += new RoutedEventHandler(Button_Click_Filter);
                this.GroupButtons.Children.Add(button);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            var productionService = new ProductionService(appDbContext);
            var currentProduction = productionService.GetProduction(false);
            if (currentProduction.Count == 0)
            {
                MessageBox.Show("Nie ma nic do wyprodukowania.", "Komunikat", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var products = new LsiService.ArrayOfUtworzDokumentRozchodowyRequestProduktObject();
            var sum = currentProduction.GroupBy(i => i.ProductionItem.ProducedItemId).Select(r => new { Index = r.Select(s => s.ProducedItem.ExternalId).First(), Q = r.Sum(q => q.ProductionItem.Quantity) }).ToList();
            //return;
            foreach (var product in sum)
            {
                if (product.Q > 0)
                    products.Add(new LsiService.UtworzDokumentRozchodowyRequestProduktObject { Ilosc = product.Q, ProduktID = product.Index, Cena = 0 });
            }

            if (products.Count == 0)
            {
                MessageBox.Show("Nie ma nic do wyprodukowania.\nZerowy lub ujemny bilans pozycji.", "Komunikat", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int documentTypeId = 1140;
            Service service = new Service();
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            var response = await service.CreateDocument(documentTypeId, warehouseId, products);
            var docResp = response.Body.UtworzDokumentRozchodowyResult.Dokument;
            var respMessage = "";
            if (docResp != null)
            {
                if (docResp.ID > 0)
                    foreach (var product in currentProduction)
                    {
                        productionService.ChangeTransferStatus(product.ProductionItem.ProductionItemId, docResp.ID, documentTypeId);
                    }
                respMessage = $"Id dokumentu: {docResp.ID}\nNumer: {docResp.Numer}\nKod błędu: {docResp.KodBledu}\nOpis błędu: {docResp.OpisBledu}";
            }
            else
                respMessage = "Totalny błąd";

            MessageBox.Show(respMessage, "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            GetCurrentProduction();
        }

        private async void GetProductsFromEndpoint_Click(object sender, RoutedEventArgs e)
        {
            ProductService productService = new ProductService(appDbContext);
            Service service = new Service();
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var productGroupeId = productsGroups.FirstOrDefault(x => x.Nazwa.Contains("RECEPTURY")).ID;
            var warehouseId = warehouses.FirstOrDefault(x => x.Symbol.Contains("MT")).MagazynID;
            var productsLsi = await service.GetMeals(productGroupeId, warehouseId);
            var currentDbProducts = appDbContext.ProducedItems.ToList();
            foreach (var product in productsLsi)
            {
                if (currentDbProducts.FirstOrDefault(ex => ex.ExternalIndex.Contains(product.Indeks)) == null)
                {
                    productService.CreateProduct(new ProducedItem
                    {
                        ExternalId = product.ProduktID,
                        Name = product.Nazwa.Contains("r_") ? (product.Nazwa.Replace("r_", "").ToUpper()) : product.Nazwa,
                        ConversionRate = 1,
                        UnitOfMesure = product.JM,
                        ExternalUnitOfMesure = product.JM,
                        ExternalIndex = product.Indeks,
                        IsActive = true,
                        ProductGroupId = 1,
                        ExternalName = product.NazwaSkrocona,
                    });
                }
            }
            GetData();
            AddButtons(producedItems);
        }


    }
}
