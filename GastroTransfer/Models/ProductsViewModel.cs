using System.Collections.ObjectModel;

namespace GastroTransfer.Models
{
    public class ProductsViewModel
    {
        public ObservableCollection<ProductGroupView> ProductsGroups { get; set; }
        public ObservableCollection<ProductGroupView> DownloadedProductsGroups { get; set; }
        public ObservableCollection<ProducedItemView> Products { get; set; }
        public ProductsViewModel()
        {
            ProductsGroups = new ObservableCollection<ProductGroupView>();
            DownloadedProductsGroups = new ObservableCollection<ProductGroupView>();
            Products = new ObservableCollection<ProducedItemView>();
        }
    }
}
