using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductionWindow.xaml
    /// </summary>
    public partial class ProductionWindow : Window
    {
        private readonly DbService _dbService;
        private readonly AppDbContext _appDbContext;
        private readonly Config _config;
        private readonly Logger _logger;
        public ProductionWindow(DbService dbService, AppDbContext appDbContext, Config config, Logger logger)
        {
            _dbService = dbService;
            _appDbContext = appDbContext;
            _config = config;
            _logger = logger;
            InitializeComponent();
            DocumentType.Text += config.ProductionDocumentSymbol;
            WarehouseSymbol.Text += config.WarehouseSymbol;
            ProductionDate.SelectedDate = DateTime.Now;
            ProductionDate.DisplayDateEnd = DateTime.Now;
            ProductionDate.SelectedDateChanged += ProductionDate_SelectedDateChanged;
        }

        private void ProductionDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = (DatePicker)sender;
            if (selectedDate.SelectedDate == null || selectedDate.SelectedDate > DateTime.Now)
                ProductionDate.SelectedDate = DateTime.Now;
        }


        private async void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionButton.IsEnabled = false;
            ProductionTransferService productionTransferService = new ProductionTransferService(_dbService, _appDbContext, _config, _logger);
            var message = await productionTransferService.StartProduction((DateTime)ProductionDate.SelectedDate, (bool)SwapProduction.IsChecked);
            Info.Text = message.Message;
            if (message.IsError)
                Info.Foreground = Brushes.Red;
            Info.FontWeight = FontWeights.Bold;
            ProductionButton.IsEnabled = true;
        }

        private void SwapProduction_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UncheckedInfo == null)
                return;
            var item = (CheckBox)sender;
            if (!(bool)item.IsChecked)
                UncheckedInfo.Text = "Spowoduje to nieowracalne usunięcie nierozliczonych zwrotów.";
            else
                UncheckedInfo.Text = "";
        }
    }
}
