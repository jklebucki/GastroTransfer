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

        public MainWindow()
        {
            InitializeSystem();
            InitializeComponent();
            AddButtons(new List<ProducedItem>()
            {
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Schab w sosie gzybowym",
                    ProducedItemId = 1,

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Karkówka z grilla",
                    ProducedItemId = 2,

                }
            });
            AddGroupButtons(new List<ProducedItem>()
            {
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupy",
                    ProducedItemId = 1,

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Dania",
                    ProducedItemId = 2,

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Robaki",
                    ProducedItemId = 3,

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Glisty",
                    ProducedItemId = 4,

                }
            });

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

            //check or initialize database
            dbService = new DbService(config);

            //while (!dbService.CheckConnection())
            //{
            //    //open config form, after 
            //    MessageBox.Show("Brak połączenia!" + dbService.ErrorMessage);
            //}
        }

        private void GetButtons()
        {


        }

        private void GetData()
        {

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

        private void AddButtons(List<ProducedItem> producedItems)
        {
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
                    Tag = item.ProductGroup,
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
        private void AddGroupButtons(List<ProducedItem> producedItems)
        {
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
                    Tag = item.ProductGroup,
                    Height = 50,
                    Width = 250,
                    Margin = new Thickness(5, 5, 5, 5),
                    FontSize = 24,
                    Style = this.FindResource("RoundCorner") as Style
                };
                button.Click += new RoutedEventHandler(Button_Click_Test);

                this.GroupButtons.Children.Add(button);
            }
        }
    }
}
