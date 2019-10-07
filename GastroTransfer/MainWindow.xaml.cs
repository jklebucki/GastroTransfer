using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            producedItems = new List<ProducedItem>()
            {
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Schab w sosie gzybowym",
                    ProducedItemId = 1,
                    ProductGroupId = 2

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Karkówka z grilla",
                    ProducedItemId = 2,
                    ProductGroupId = 2

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Flaki",
                    ProducedItemId = 3,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa gulaszowa",
                    ProducedItemId = 4,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Żurek",
                    ProducedItemId = 5,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa grochowa",
                    ProducedItemId = 6,
                    ProductGroupId = 3

                }
            };

            productGroups = new List<ProductGroup>()
            {
                new ProductGroup
                {
                    GroupName = "Wszystkie",
                    ProductGroupId = 1,

                },
                new ProductGroup
                {
                    GroupName = "Zupy",
                    ProductGroupId = 3,

                },
                new ProductGroup
                {
                    GroupName = "Dania",
                    ProductGroupId = 2,

                },
                new ProductGroup
                {
                    GroupName= "Śniadania",
                    ProductGroupId = 4,

                }
            };

            producedItems = producedItems.OrderBy(x => x.Name).ToList();
            InitializeSystem();
            InitializeComponent();
            GetData();
            AddButtons(producedItems);
            AddGroupButtons(productGroups);
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

            if (!dbService.CheckConnection())
            {
                MessageBox.Show("Brak połączenia!" + dbService.ErrorMessage);
            }
            appDbContext = new AppDbContext(dbService.GetConnectionString());
            //appDbContext.ProducedItems.AddRange(producedItems);
            //appDbContext.ProductGroups.AddRange(productGroups);
            //appDbContext.SaveChanges();
            producedItems = appDbContext.ProducedItems.Where(x => x.IsActive).ToList();
            productGroups = appDbContext.ProductGroups.ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configPage = new ConfigWindow();
            configPage.ShowDialog();
        }

        private void Button_Click_Test(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            MessageBox.Show(btn.Name.Split('_')[1]);
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
                    Width = 240,
                };

                TextBlock text = new TextBlock
                {
                    Text = $"{item.Name}",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Width = 240,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                box.Child = text;

                Button button = new Button()
                {
                    Name = $"N_{item.ProducedItemId}",
                    Content = box,
                    Tag = item.ProductGroupId,
                    Height = 150,
                    Width = 250,
                    Margin = new Thickness(5, 5, 5, 5),
                    FontSize = 24,
                    Style = this.FindResource("RoundCorner") as Style
                };
                button.Click += new RoutedEventHandler(Button_Click_Test);

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
                    Width = 240,
                };

                TextBlock text = new TextBlock
                {
                    Text = $"{item.GroupName}",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Width = 240,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                box.Child = text;

                Button button = new Button()
                {
                    Name = $"N_{item.ProductGroupId}",
                    Content = box,
                    Tag = item.GroupName,
                    Height = 50,
                    Width = 250,
                    Margin = new Thickness(5, 5, 5, 5),
                    FontSize = 24,
                    Style = this.FindResource("RoundCorner") as Style
                };
                button.Click += new RoutedEventHandler(Button_Click_Filter);

                this.GroupButtons.Children.Add(button);
            }
        }
    }
}
