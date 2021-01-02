using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using LsiEndpointSupport;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
        private ProductService productService { get; set; }
        private ProductGroupsService productGroupsService { get; set; }
        public ProductsWindow(Window window, AppDbContext appDbContext, Config config)
        {
            productService = new ProductService(appDbContext);
            productGroupsService = new ProductGroupsService(appDbContext);
            this.config = config;
            this.appDbContext = appDbContext;
            ViewModel = new ProductsViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            Owner = window;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private async Task<ServiceMessage> FillViewModel()
        {
            ViewModel.Products.Clear();
            ViewModel.ProductsGroups.Clear();
            var productsGroups = await appDbContext.ProductGroups.Where(a => a.IsActive).ToListAsync();
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

            var products = await appDbContext.ProducedItems.Where(a => a.IsActive).ToListAsync();
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

            return new ServiceMessage { Message = "Pozycje załadowane.", IsError = false, ItemId = 0 };
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddProductToChange((ProducedItemView)sender);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            producedItemsToChange = new List<ProducedItemView>();
            Progress.Visibility = Visibility.Visible;
            Feedback.Text = (await FillViewModel()).Message;
            Feedback.Text = (await GetGroups()).Message;
            Progress.Visibility = Visibility.Hidden;
        }

        private async Task RepairProducts()
        {
            var doubleProductsExternalIds = await appDbContext.ProducedItems.GroupBy(g => g.ExternalId).Select(dp => new
            {
                ExternalId = dp.Key,
                CountDp = dp.Count()
            }).Where(c => c.CountDp > 1).Select(s => s.ExternalId).ToListAsync();

            if (doubleProductsExternalIds.Count() == 0)
                return;

            var doubleProducts = await appDbContext.ProducedItems.Where(p => doubleProductsExternalIds.Contains(p.ExternalId)).ToListAsync();
            var doubleProductsIds = doubleProducts.Select(i => i.ProducedItemId).ToArray();
            var production = await appDbContext.TransferredItems.Where(pr => doubleProductsIds.Contains(pr.ProducedItemId)).ToListAsync();
            foreach (var pr in production)
            {
                var exId = doubleProducts.FirstOrDefault(p => p.ProducedItemId == pr.ProducedItemId).ExternalId;
                var newId = doubleProducts.FirstOrDefault(p => p.ExternalId == exId).ProducedItemId;
                pr.ProducedItemId = newId;
                appDbContext.Entry(pr).State = EntityState.Modified;
            }
            await appDbContext.SaveChangesAsync();

            var productsIdsContainedInProduction = await appDbContext.TransferredItems.Where(pr => doubleProductsIds.Contains(pr.ProducedItemId)).Select(i => i.ProducedItemId).ToListAsync();
            var doubleProductsToRemove = doubleProducts.Where(p => !productsIdsContainedInProduction.Contains(p.ProducedItemId));
            appDbContext.ProducedItems.RemoveRange(doubleProductsToRemove);
            await appDbContext.SaveChangesAsync();
            return;
        }

        private async Task<ServiceMessage> GetProducts(int groupId, string warehouseSymbol)
        {
            await RepairProducts();
            if (string.IsNullOrEmpty(config.EndpointUrl))
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Ades usługi LSI nie został ustawiony." };

            LsiEndpointService service = new LsiEndpointService(config.EndpointUrl);
            var productsGroups = await service.GetProductsGroups();
            var warehouses = await service.GetWarehouses();
            var warehouse = warehouses.FirstOrDefault(x => x.Symbol.Contains(warehouseSymbol));
            if (warehouse == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Niepoprawny magazyn." };
            var productsLsi = await service.GetMeals(groupId, warehouse.MagazynID);
            if (productsLsi == null)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Lista produktów jest pusta." };
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

                if (currentDbProducts.FirstOrDefault(ex => ex.ExternalId == item.ExternalId) == null)
                {
                    productService.CreateProduct(item);
                }
                else
                {
                    productService.UpdateProduct(item);
                }
            }
            return new ServiceMessage { IsError = false, ItemId = 0, Message = "Produkty pobrane." };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Konfiguracja usługi LSI nieprawidłowa." };
            LsiEndpointService endpointService = new LsiEndpointService(config.EndpointUrl);
            var groups = await endpointService.GetProductsGroups();
            if (groups.Count == 0)
                return new ServiceMessage { IsError = true, ItemId = 0, Message = "Nie pobrano żadnej grupy. Sprawdź konfigurację systemu." };
            foreach (var group in groups.OrderBy(n => n.Nazwa))
            {
                var id = 1;
                ViewModel.DownloadedProductsGroups.Add(new ProductGroupView { ProductGroupId = id, ExternalGroupId = group.ID, GroupName = group.Nazwa, IsActive = false });
                id++;
            }
            return new ServiceMessage { IsError = false, ItemId = 0, Message = "Dostepne grupy pobrane z usługi LSI" };
        }

        private async void GroupList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
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
            Progress.Visibility = Visibility.Visible;
            Feedback.Text = "Pobieram dane...";
            Feedback.Text = (await GetProducts(selectedItem.ExternalGroupId, config.WarehouseSymbol)).Message;
            Progress.Visibility = Visibility.Hidden;
            await FillViewModel();
        }

        private async void SelectedGroupList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = (ProductGroupView)((ListBox)sender).SelectedItem;
            if (selectedItem == null)
                return;
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
            Feedback.Text = $"Usunieto grupę {selectedItem.GroupName}";
            await FillViewModel();
        }

        private void DeactivateProducts(List<ProductGroup> allAddedGroups)
        {
            productService.ChangeActiveStatus(allAddedGroups.Where(a => !a.IsActive).Select(i => i.ExternalGroupId).ToList(), false);
        }

        private void ActivateProducts(List<ProductGroup> allAddedGroups)
        {
            productService.ChangeActiveStatus(allAddedGroups.Where(a => a.IsActive).Select(i => i.ExternalGroupId).ToList(), true);
        }
    }
}
