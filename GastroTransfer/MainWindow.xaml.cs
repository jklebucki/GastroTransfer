using GastroTransfer.Data;
using GastroTransfer.Helpers;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GastroTransfer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config _config;
        private ConfigService _configService;
        private AppDbContext _appDbContext;
        private DbService _dbService;
        private List<ProducedItem> _producedItems;
        private List<ProductGroup> _productGroups;
        private ProductionService _productionService;
        private ObservableCollection<ProductionViewModel> _productionViewItems;
        private delegate void GetDataDelegate();
        private readonly GetDataDelegate GetDataDelegateMethod;
        private readonly Dictionary<string, Style> _style;
        public MainWindow()
        {
            GetDataDelegateMethod = GetData;
            InitializeComponent();
            _style = new Dictionary<string, Style>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectStyle();
            _configService = new ConfigService(new CryptoService());
            if (_configService.Message != null && _configService.Message.Contains("Error"))
            {
                MessageBox.Show(_configService.Message, "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(999);
            }
            InitializeSystem();
            CheckingConnection(true);
            InitializeView();
            GetData();
            AddButtons(_producedItems);
            AddGroupButtons(_productGroups);
        }

        private void InitializeView()
        {
            _productionService = new ProductionService(_appDbContext);
            _productionViewItems = new ObservableCollection<ProductionViewModel>();
            PositionsListGrid.DataContext = _productionViewItems;
            PositionsListGrid.ItemsSource = _productionViewItems;
            float scale = (float)SystemParameters.PrimaryScreenWidth / 1920f;
            ProductName.FontSize = 22 * scale;
            UnitOfMesure.FontSize = 22 * scale;
            Quantity.FontSize = 22 * scale;
        }

        private void CollectStyle()
        {
            _style.Add("roundedButton", FindResource("RoundCorner") as Style);
        }

        private void InitializeSystem()
        {
            _config = _configService.GetConfig();
            if (_config == null)
            {
                _configService.InitializeConfig();
                _config = _configService.GetConfig();
            }
        }

        private void CheckingConnection(bool systemStart)
        {
            CheckConnection checkConnection = new CheckConnection(_dbService, _configService, systemStart);
            checkConnection.ShowDialog();
            RenewConfiguration();
        }

        private void RenewConfiguration()
        {
            _config = _configService.GetConfig();
            _dbService = new DbService(_config);
            _appDbContext = new AppDbContext(_dbService.GetConnectionString());
        }

        private void GetData()
        {
            if (_appDbContext.ProductGroups.Count() == 0)
            {
                _appDbContext.ProductGroups.AddRange(ConstData.productGroups);
                _appDbContext.SaveChanges();
            }
            _producedItems = _appDbContext.ProducedItems.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
            _productGroups = _appDbContext.ProductGroups.Where(x => x.IsActive || x.ProductGroupId == 1).ToList();
            AddButtons(_producedItems);
            AddGroupButtons(_productGroups);
            GetCurrentProduction();
        }

        /// <summary>
        /// Getting current production
        /// </summary>
        private void GetCurrentProduction()
        {
            List<ProductionViewModel> productionView = new List<ProductionViewModel>();
            if ((bool)ToggleIn.IsChecked || (bool)ToggleOut.IsChecked)
            {
                productionView = _productionService.GetProduction(false).Where(o => o.ProductionItem.OperationType == null || o.ProductionItem.OperationType == 1).ToList();
            }
            else
            {
                productionView = _productionService.GetProduction(false).Where(o => o.ProductionItem.OperationType != null && o.ProductionItem.OperationType == 2).ToList();
            }
            _productionViewItems.Clear();
            foreach (var product in productionView)
                _productionViewItems.Add(product);
        }

        private bool LogIn(LoginType loginType)
        {
            LoginWindow loginWindow = new LoginWindow(_config, loginType)
            {
                Owner = this
            };
            loginWindow.ShowDialog();
            if (!loginWindow.LoginOk)
                return false;
            return true;
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            if (!LogIn(LoginType.System))
                return;
            ConfigWindow configPage = new ConfigWindow
            {
                Owner = this
            };
            configPage.ShowDialog();
            if (configPage.IsSaved)
            {
                _config = _configService.GetConfig();
                RenewConfiguration();
                CheckingConnection(false);
            }
        }

        private void Production_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var productId = int.Parse(btn.Name.Split('_')[1]);
            var item = _producedItems.FirstOrDefault(x => x.ProducedItemId == productId);
            var measurementWindow = new MeasurementWindow(item.Name, _config);
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
                            Registered = DateTime.Now,
                            OperationType = (bool)ToggleTrash.IsChecked ? (int)OperationType.Trash : (int)OperationType.Production
                        }
                    };

                    var message = _productionService.AddProduction(productionViewModel);
                    if (!message.IsError)
                    {
                        productionViewModel.ProductionItem.ProductionItemId = message.ItemId;
                        _productionViewItems.Add(productionViewModel);
                        RefreshView();
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

        private void RefreshView()
        {
            PositionsListGrid.Items.Refresh();
            if (PositionsListGrid.Items.Count > 0)
            {
                PositionsListGrid.SelectedItem = PositionsListGrid.Items[PositionsListGrid.Items.Count - 1];
                PositionsListGrid.ScrollIntoView(PositionsListGrid.Items[PositionsListGrid.Items.Count - 1]);
            }
        }

        private void RemoveLastProductionEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_productionViewItems.Count > 0)
            {
                var productToRemove = _productionViewItems.Last();
                ConfirmWindow confirmWindow = new ConfirmWindow(
                    "Tak",
                    "Nie",
                    "Na pewno chcesz usunąc?" +
                    $"\n{productToRemove.ProducedItem.Name}" +
                    $"\t{productToRemove.ProductionItem.Quantity} {productToRemove.ProducedItem.UnitOfMesure}")
                {
                    Owner = this
                };
                confirmWindow.ShowDialog();
                if (confirmWindow.IsCanacel)
                    return;
                if (productToRemove != null)
                {
                    var messsge = _productionService.RemoveProduction(productToRemove.ProductionItem.ProductionItemId);
                    if (!messsge.IsError)
                    {
                        _productionViewItems.Remove(productToRemove);
                        RefreshView();
                    }
                    else
                    {
                        ConfirmWindow confirm = new ConfirmWindow("Rozumiem", messsge.Message)
                        {
                            Owner = this
                        };
                        confirm.ShowDialog();
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
            var items = _producedItems.Where(x => x.IsActive).OrderBy(n => n.Name);
            if (groupId == 0)
                AddButtons(items.ToList());
            else
                AddButtons(items.Where(x => x.ExternalGroupId == groupId).ToList());
        }

        private void AddButtons(List<ProducedItem> producedItems)
        {
            WrapButtons.Children.Clear();
            foreach (var item in producedItems)
            {
                WrapButtons.Children.Add(CreateControls.CreateProductButton(_style, Production_Button_Click, item));
            }
        }

        private void AddGroupButtons(List<ProductGroup> productGroups)
        {
            GroupButtons.Children.Clear();
            foreach (var item in productGroups)
            {
                GroupButtons.Children.Add(CreateControls.CreateFilterButton(_style, Button_Click_Filter, item));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow confirmWindow = new ConfirmWindow
            {
                Owner = this
            };
            confirmWindow.ShowDialog();
            if (!confirmWindow.IsCanacel)
                Close();
        }

        private void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_config.OnPasswordProduction)
                if (!LogIn(LoginType.Production))
                    return;
            ProductionWindow productionWindow = new ProductionWindow(_dbService, _appDbContext, _config)
            {
                Owner = this
            };
            productionWindow.ShowDialog();
            GetDataDelegateMethod.Invoke();
        }

        private void GenerateTrashDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_config.OnPasswordTrashDocument)
                if (!LogIn(LoginType.Production))
                    return;
            TrashDocumentWindow trashDocumentWindow = new TrashDocumentWindow(_dbService, _appDbContext, _config)
            {
                Owner = this
            };
            trashDocumentWindow.ShowDialog();
            GetDataDelegateMethod.Invoke();
        }

        private void GetProductsFromEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (_config.OnPasswordProductsImport)
                if (!LogIn(LoginType.System))
                    return;
            ProductsWindow productsWindow = new ProductsWindow(this, _appDbContext, _config);
            productsWindow.ShowDialog();
            GetData();
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentProduction();
            RefreshView();
        }
    }
}
