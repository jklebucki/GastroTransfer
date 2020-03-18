using GastroTransfer.Data;
using GastroTransfer.LsiService;
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
            //Test data
            producedItems = ConstData.producedItems;
            productGroups = ConstData.productGroups;
            producedItems = producedItems.OrderBy(x => x.Name).ToList();

            InitializeSystem();
            InitializeComponent();
            productionView = new List<ProductionViewModel>();
            GetData();
            PositionsListGrid.DataContext = productionView;
            PositionsListGrid.ItemsSource = productionView;
            float scale = (float)SystemParameters.PrimaryScreenWidth / 1920f;
            ProductName.FontSize = 22 * scale;
            UnitOfMesure.FontSize = 22 * scale;
            Quantity.FontSize = 22 * scale;

            AddButtons(producedItems);
            AddGroupButtons(productGroups);
            BackButton.Style = this.FindResource("RoundCorner") as Style;
            ConfigButton.Style = this.FindResource("RoundCorner") as Style;
            CloseButton.Style = this.FindResource("RoundCorner") as Style;
            SoapTestButton.Style = this.FindResource("RoundCorner") as Style;

        }

        private void InitializeSystem()
        {
            //read or initialize config
            configService = new ConfigService(new CryptoService());
            config = configService.GetConfig();
            if (config == null)
            {
                configService.InitializeConfig();
                config = configService.GetConfig();
            }
        }

        private void GetData()
        {
            //check or initialize database
            dbService = new DbService(config);

            while (!dbService.CheckConnection())
            {
                MessageBox.Show("Brak połączenia!" + dbService.ErrorMessage);
                var configWindow = new ConfigWindow(this.FindResource("RoundCorner") as Style);
                configWindow.ShowDialog();
                config = configService.GetConfig();
                dbService = new DbService(config);
            }
            appDbContext = new AppDbContext(dbService.GetConnectionString());
            if (appDbContext.ProductGroups.Count() == 0)
            {
                appDbContext.ProducedItems.AddRange(producedItems);
                appDbContext.ProductGroups.AddRange(productGroups);
                appDbContext.SaveChanges();
            }
            producedItems = appDbContext.ProducedItems.Where(x => x.IsActive).ToList();
            productGroups = appDbContext.ProductGroups.ToList();
            GetCurrentProduction();
        }

        private void GetCurrentProduction()
        {
            //get last production
            ProductionService productionService = new ProductionService(appDbContext);
            var currentProduction = productionService.GetProduction(false);
            foreach (var item in currentProduction)
            {
                productionView.Add(item);
            }
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configPage = new ConfigWindow(this.FindResource("RoundCorner") as Style);
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
            var items = producedItems.Where(x => x.ProductGroupId == groupId).ToList();
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

        private async void SoapTestButton_Click(object sender, RoutedEventArgs e)
        {
            LsiEndpointSupport.Info service = new LsiEndpointSupport.Info();
            var message = await service.GetInfo();
            string infoMessage = $"Lokal: {message.EndpointName}\nAdres usługi LSI: {message.EndpointUrl}\nMagazyny:";
            foreach (var item in message.Warehouses)
            {
                infoMessage += $"\n{item.Symbol}\t{item.Name}";
            }
            MessageBox.Show(infoMessage, "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
