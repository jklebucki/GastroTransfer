using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using LsiEndpointSupport;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        private AppDbContext appDbContext { get; set; }
        private ProductsViewModel ViewModel { get; set; }
        private List<ProducedItemView> producedItemsToChange { get; set; }
        private Config config { get; set; }
        public ProductsWindow(Window window, AppDbContext appDbContext, Style style, Config config)
        {
            this.config = config;
            this.appDbContext = appDbContext;
            ViewModel = new ProductsViewModel();
            DataContext = ViewModel;
            FillViewModel();
            InitializeComponent();
            Owner = window;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            CloseButton.Style = style;
            GetProductsButton.Style = style;
        }

        private void FillViewModel()
        {
            var productsGroups = appDbContext.ProductGroups.ToList();
            foreach (var group in productsGroups)
            {
                ViewModel.ProductsGroups.Add(new ProductGroupView
                {
                    ProductGroupId = group.ProductGroupId,
                    ExternalGroupId = group.ExternalGroupId,
                    GroupName = group.GroupName
                });
            }

            var products = appDbContext.ProducedItems.ToList();
            foreach (var product in products)
            {
                var item = new ProducedItemView
                {
                    ProducedItemId = product.ProducedItemId,
                    Name = product.Name,
                    IsActive = product.IsActive,
                    UnitOfMesure = product.UnitOfMesure,
                    ConversionRate = product.ConversionRate,
                    ExternalId = product.ExternalId,
                    ExternalIndex = product.ExternalIndex,
                    ExternalName = product.ExternalName,
                    ExternalUnitOfMesure = product.ExternalUnitOfMesure,
                    ProductGroupId = product.ExternalGroupId
                };
                item.PropertyChanged += Item_PropertyChanged;
                ViewModel.Products.Add(item);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddProductToChange((ProducedItemView)sender);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            producedItemsToChange = new List<ProducedItemView>();
        }

        private async Task<bool> GetProducts(int groupId, string warehouseSymbol)
        {
            if (string.IsNullOrEmpty(config.EndpointUrl))
                return false;

            ProductService productService = new ProductService(appDbContext);
            LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouse = warehouses.FirstOrDefault(x => x.Symbol.Contains(warehouseSymbol));
            if (warehouse == null)
                return false;
            var productsLsi = await service.GetMeals(groupId, warehouse.MagazynID);
            var currentDbProducts = appDbContext.ProducedItems.ToList();
            foreach (var product in productsLsi)
            {
                if (currentDbProducts.FirstOrDefault(ex => ex.ExternalIndex.Contains(product.Indeks)) == null)
                {
                    var item = new ProducedItem
                    {
                        ExternalId = product.ProduktID,
                        Name = product.Nazwa.Contains("r_") ? (product.Nazwa.Replace("r_", "").ToUpper()) : product.Nazwa,
                        ConversionRate = 1,
                        UnitOfMesure = product.JM,
                        ExternalUnitOfMesure = product.JM,
                        ExternalIndex = product.Indeks,
                        IsActive = true,
                        ExternalGroupId = groupId,
                        ExternalName = product.NazwaSkrocona,
                    };
                    productService.CreateProduct(item);
                }
            }
            return true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void GetProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(config.EndpointUrl) && !string.IsNullOrEmpty(config.WarehouseSymbol))
            {
                var groups = appDbContext.ProductGroups.ToList();
                if (groups.Count == 1)
                {
                    await AddFirstGroup();
                    groups = appDbContext.ProductGroups.ToList();
                }

                foreach (var group in groups)
                {
                    if (group.ExternalGroupId != 0)
                        await GetProducts(group.ExternalGroupId, config.WarehouseSymbol);
                }
                //InitializeViewModel();
                MessageBox.Show("Pobieranie zakończone", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Niewłaściwa konfiguracja systemu.\nUstaw adres usługi oraz magazyn", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddProductToChange(ProducedItemView producedItem)
        {
            var item = producedItemsToChange.FirstOrDefault(id => id.ProducedItemId == producedItem.ProducedItemId);
            if (item == null)
                producedItemsToChange.Add(producedItem);
            else
            {
                producedItemsToChange.Remove(item);
                producedItemsToChange.Add(producedItem);
            }
        }

        private async Task<bool> AddFirstGroup()
        {
            if (string.IsNullOrEmpty(config.EndpointUrl))
                return false;
            LsiEndpointService endpointService = new LsiEndpointService(config.EndpointUrl);
            ProductGroupsService productGroupsService = new ProductGroupsService(appDbContext);
            var groups = await endpointService.GetProductsGroups();
            var group = groups.FirstOrDefault(x => x.Nazwa.Contains("RECEPTURY PROD"));
            if (group == null)
                return false;
            productGroupsService.AddGroup(new ProductGroup { ExternalGroupId = group.ID, GroupName = group.Nazwa });
            return true;
        }
    }
}
