using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private ProductionService productionService { get; set; }
        private ObservableCollection<ProductionViewModel> productionViewItems { get; set; }
        private delegate void GetDataDelegate();
        private readonly GetDataDelegate GetDataDelegateMethod;
        public MainWindow()
        {
            GetDataDelegateMethod = GetData;
            InitializeComponent();
            //BackButton.Style = this.FindResource("RoundCorner") as Style;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            configService = new ConfigService(new CryptoService());
            if (configService.Message != null && configService.Message.Contains("Error"))
            {
                MessageBox.Show(configService.Message, "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(999);
            }
            InitializeSystem();
            CheckingConnection(true);
            productionService = new ProductionService(appDbContext);

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
            config = configService.GetConfig();
            if (config == null)
            {
                configService.InitializeConfig();
                config = configService.GetConfig();
            }
        }

        private void CheckingConnection(bool systemStart)
        {
            CheckConnection checkConnection = new CheckConnection(dbService, configService, systemStart);
            checkConnection.ShowDialog();
            RenewConfiguration();
        }

        private void RenewConfiguration()
        {
            config = configService.GetConfig();
            dbService = new DbService(config);
            appDbContext = new AppDbContext(dbService.GetConnectionString());
        }

        private void GetData()
        {
            if (appDbContext.ProductGroups.Count() == 0)
            {
                appDbContext.ProductGroups.AddRange(ConstData.productGroups);
                appDbContext.SaveChanges();
            }
            producedItems = appDbContext.ProducedItems.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
            productGroups = appDbContext.ProductGroups.Where(x => x.IsActive || x.ProductGroupId == 1).ToList();
            AddButtons(producedItems);
            AddGroupButtons(productGroups);
            GetCurrentProduction();
        }

        /// <summary>
        /// Getting current production
        /// </summary>
        private void GetCurrentProduction()
        {
            var production = productionService.GetProduction(false);
            productionViewItems.Clear();
            foreach (var product in production)
                productionViewItems.Add(product);
        }

        private bool LogIn()
        {
            LoginWindow loginWindow = new LoginWindow(config);
            loginWindow.Owner = this;
            loginWindow.ShowDialog();
            if (!loginWindow.LoginOk)
                return false;
            return true;
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            if (!LogIn())
                return;
            ConfigWindow configPage = new ConfigWindow();
            configPage.Owner = this;
            configPage.ShowDialog();
            if (configPage.IsSaved)
            {
                config = configService.GetConfig();
                RenewConfiguration();
                CheckingConnection(false);
            }
        }

        private void Production_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var productId = int.Parse(btn.Name.Split('_')[1]);
            var item = producedItems.FirstOrDefault(x => x.ProducedItemId == productId);
            var measurementWindow = new MeasurementWindow(item.Name, config);
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
                ConfirmWindow confirmWindow = new ConfirmWindow(
                    "Tak",
                    "Nie",
                    "Na pewno chcesz usunąc?" +
                    $"\n{productToRemove.ProducedItem.Name}" +
                    $"\t{productToRemove.ProductionItem.Quantity} {productToRemove.ProducedItem.UnitOfMesure}");
                confirmWindow.Owner = this;
                confirmWindow.ShowDialog();
                if (confirmWindow.IsCanacel)
                    return;
                if (productToRemove != null)
                {
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
            ConfirmWindow confirmWindow = new ConfirmWindow();
            confirmWindow.Owner = this;
            confirmWindow.ShowDialog();
            if (!confirmWindow.IsCanacel)
                Close();
        }

        private void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            if (config.OnPasswordProduction)
                if (!LogIn())
                    return;
            ProductionWindow productionWindow = new ProductionWindow(dbService, appDbContext, config);
            productionWindow.Owner = this;
            productionWindow.ShowDialog();

            GetDataDelegateMethod.Invoke();
        }

        private void GetProductsFromEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (config.OnPasswordProductsImport)
                if (!LogIn())
                    return;
            ProductsWindow productsWindow = new ProductsWindow(this, appDbContext, config);
            productsWindow.ShowDialog();
            GetData();
        }

    }
}
