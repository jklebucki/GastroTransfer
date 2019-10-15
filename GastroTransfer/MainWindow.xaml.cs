﻿using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using GastroTransfer.Views;
using GastroTransfer.Views.Dialogs;
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
            GetData();
            productionView = new List<ProductionViewModel>();
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
                var configWindow = new ConfigWindow();
                configWindow.ShowDialog();
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
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configPage = new ConfigWindow();
            configPage.ShowDialog();
        }

        private void Production_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var productId = int.Parse(btn.Name.Split('_')[1]);
            var item = producedItems.FirstOrDefault(x => x.ProducedItemId == productId);
            var measurementWindow = new MeasurementWindow(this.FindResource("RoundCorner") as Style, item.Name);
            measurementWindow.ShowDialog();
            if (!measurementWindow.IsCanceled)
            {
                productionView.Add(
                    new ProductionViewModel
                    {
                        ProducedItem = item,
                        TransferredItem = new TransferredItem
                        {
                            Quantity = measurementWindow.Quantity
                        }
                    });
                PositionsListGrid.Items.Refresh();
                PositionsListGrid.SelectedItem = PositionsListGrid.Items[PositionsListGrid.Items.Count - 1];
                PositionsListGrid.ScrollIntoView(PositionsListGrid.Items[PositionsListGrid.Items.Count - 1]);
            }
        }

        private void CurrencyListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (productionView.Count > 0)
            {
                productionView.Remove(productionView.Last());
                if (productionView.Count > 0)
                {
                    PositionsListGrid.SelectedItem = PositionsListGrid.Items[PositionsListGrid.Items.Count - 1];
                    PositionsListGrid.ScrollIntoView(PositionsListGrid.Items[PositionsListGrid.Items.Count - 1]);
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
