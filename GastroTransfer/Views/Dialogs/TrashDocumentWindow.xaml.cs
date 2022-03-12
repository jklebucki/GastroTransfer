using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for TrashDocumentWindow.xaml
    /// </summary>
    public partial class TrashDocumentWindow : Window
    {
        private DbService _dbService { get; set; }
        private AppDbContext _appDbContext { get; set; }
        private Config _config { get; set; }
        public TrashDocumentWindow(DbService dbService, AppDbContext appDbContext, Config config)
        {
            _dbService = dbService;
            _appDbContext = appDbContext;
            _config = config;
            InitializeComponent();
            SetValues();
        }
        private void SetValues()
        {
            DocumentType.Text += _config.TrashDocumentSymbol;
            WarehouseSymbol.Text += _config.WarehouseSymbol;
            ProductionDate.SelectedDate = DateTime.Now;
            ProductionDate.DisplayDateEnd = DateTime.Now;
            ProductionDate.SelectedDateChanged += Date_SelectedDateChanged;
        }
        private void Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = (DatePicker)sender;
            if (selectedDate.SelectedDate == null || selectedDate.SelectedDate > DateTime.Now)
                ProductionDate.SelectedDate = DateTime.Now;
        }
        private async void GenerateTrashDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            TrashDocumentTransferService trashDocumentTransferService = new TrashDocumentTransferService(_dbService, _appDbContext, _config);
            var message = await trashDocumentTransferService.TransferTrashDocument((DateTime)ProductionDate.SelectedDate);
            Info.Text = message.Message;
            if (message.IsError)
                Info.Foreground = Brushes.Red;
            Info.FontWeight = FontWeights.Bold;
        }
    }
}
