using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using LsiEndpointSupport;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            GetGroups();
        }

        private void FillViewModel()
        {
            ViewModel.Products.Clear();
            ViewModel.ProductsGroups.Clear();
            var productsGroups = appDbContext.ProductGroups.Where(a => a.IsActive).ToList();
            foreach (var group in productsGroups)
            {
                ViewModel.ProductsGroups.Add(new ProductGroupView
                {
                    ProductGroupId = group.ProductGroupId,
                    ExternalGroupId = group.ExternalGroupId,
                    GroupName = group.GroupName,
                    IsActive = group.IsActive
                });
            }

            var products = appDbContext.ProducedItems.Where(a => a.IsActive).ToList();
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

        private async Task<ServiceMessage> GetProducts(int groupId, string warehouseSymbol)
        {
            if (string.IsNullOrEmpty(config.EndpointUrl))
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Endpoint address not set" };

            ProductService productService = new ProductService(appDbContext);
            LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouse = warehouses.FirstOrDefault(x => x.Symbol.Contains(warehouseSymbol));
            if (warehouse == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Warehouse is null" };
            var productsLsi = await service.GetMeals(groupId, warehouse.MagazynID);
            if (productsLsi == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Product list is null" };
            var currentDbProducts = appDbContext.ProducedItems.ToList();
            foreach (var product in productsLsi)
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

                if (currentDbProducts.FirstOrDefault(ex => ex.ExternalIndex.Contains(product.Indeks)) == null)
                {
                    productService.CreateProduct(item);
                }
                else
                {
                    productService.UpdateProduct(item);
                }
            }
            return new ServiceMessage { IsError = false, ItemId = 0, Message = "Products downloaded" };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void GetProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(config.EndpointUrl) && !string.IsNullOrEmpty(config.WarehouseSymbol))
            {
                var groups = appDbContext.ProductGroups.Where(a => a.IsActive).ToList();

                foreach (var group in groups)
                {
                    if (group.ExternalGroupId != 0)
                        await GetProducts(group.ExternalGroupId, config.WarehouseSymbol);
                }
                FillViewModel();
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

        private async Task<ServiceMessage> GetGroups()
        {
            if (string.IsNullOrEmpty(config.EndpointUrl))
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Endpoint URL not set" };
            LsiEndpointService endpointService = new LsiEndpointService(config.EndpointUrl);
            var groups = await endpointService.GetProductsGroups();
            if (groups.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Empty group list" };
            foreach (var group in groups)
            {
                var id = 1;
                ViewModel.DownloadedProductsGroups.Add(new ProductGroupView { ProductGroupId = id, ExternalGroupId = group.ID, GroupName = group.Nazwa, IsActive = false });
                id++;
            }
            return new ServiceMessage { IsError = false, ItemId = 0, Message = "Groups downloaded" };
        }

        private void GroupList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProductGroupsService productGroupsService = new ProductGroupsService(appDbContext);
            var selectedItem = (ProductGroupView)((ListBox)sender).SelectedItem;
            if (selectedItem == null)
                return;
            productGroupsService.AddGroup(
                new ProductGroup
                {
                    ExternalGroupId = selectedItem.ExternalGroupId,
                    GroupName = selectedItem.GroupName,
                    IsActive = true
                });
            var allAddedGroups = productGroupsService.GetGroups();
            ActivateProducts(allAddedGroups);
            ViewModel.ProductsGroups.Clear();
            foreach (var group in allAddedGroups.Where(a => a.IsActive).ToList())
                ViewModel.ProductsGroups.Add(
                    new ProductGroupView
                    {
                        ProductGroupId = group.ProductGroupId,
                        IsActive = group.IsActive,
                        ExternalGroupId = group.ExternalGroupId,
                        GroupName = group.GroupName
                    });
        }

        private void SelectedGroupList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProductGroupsService productGroupsService = new ProductGroupsService(appDbContext);
            var selectedItem = (ProductGroupView)((ListBox)sender).SelectedItem;
            productGroupsService.UpdateGroup(
                new ProductGroup
                {
                    ProductGroupId = selectedItem.ProductGroupId,
                    ExternalGroupId = selectedItem.ExternalGroupId,
                    GroupName = selectedItem.GroupName,
                    IsActive = false
                });
            var allAddedGroups = productGroupsService.GetGroups();
            DeactivateProducts(allAddedGroups);
            foreach (var group in allAddedGroups.Where(a => a.IsActive).ToList())
                ViewModel.ProductsGroups.Add(
                    new ProductGroupView
                    {
                        ProductGroupId = group.ProductGroupId,
                        IsActive = group.IsActive,
                        ExternalGroupId = group.ExternalGroupId,
                        GroupName = group.GroupName
                    });
            FillViewModel();
        }

        private void DeactivateProducts(List<ProductGroup> allAddedGroups)
        {
            ProductService productService = new ProductService(appDbContext);
            productService.ChangeActiveStatus(allAddedGroups.Where(a => !a.IsActive).Select(i => i.ExternalGroupId).ToList(), false);
        }

        private void ActivateProducts(List<ProductGroup> allAddedGroups)
        {
            ProductService productService = new ProductService(appDbContext);
            productService.ChangeActiveStatus(allAddedGroups.Where(a => a.IsActive).Select(i => i.ExternalGroupId).ToList(), true);
        }
    }
}
