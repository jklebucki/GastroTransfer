using GastroTransfer.Models;
using GastroTransfer.Services;
using System;
using System.Linq;
using System.Windows;


namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private readonly ConfigService configService;
        private string[] ports { get; set; }
        public bool IsSaved { get; protected set; }
        public ConfigWindow()
        {
            IsSaved = false;
            try
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ports = System.IO.Ports.SerialPort.GetPortNames();
            }
            catch (Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            Icon = null;
            configService = new ConfigService(new CryptoService());
            InitializeComponent();
            var style = this.FindResource("RoundCorner") as Style;
            CloseButton.Style = style;
            SaveConfigButton.Style = style;
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs args)
        {
            ServerAddress.Focus();
            var stopBits = Enum.GetValues(typeof(System.IO.Ports.StopBits)).OfType<System.IO.Ports.StopBits>().ToList();
            stopBits.RemoveAt(0);
            WeightComPortName.ItemsSource = ports;
            WeightComStopBits.ItemsSource = stopBits;
            WeightComParity.ItemsSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            var config = configService.GetConfig();
            ServerAddress.Text = config.ServerAddress;
            DatabaseName.Text = config.DatabaseName;
            UserName.Text = config.UserName;
            Password.Password = config.Password;
            IsTrustedConnection.IsChecked = config.IsTrustedConnection;
            AdditionalConnectionStringDirective.Text = config.AdditionalConnectionStringDirective;
            ExternalDbServerAddress.Text = config.ExternalDbServerAddress;
            ExternalDbDatabaseName.Text = config.ExternalDbDatabaseName;
            ExternalDbUserName.Text = config.ExternalDbUserName;
            ExternalDbPassword.Password = config.ExternalDbPassword;
            ExternalDbIsTrustedConnection.IsChecked = config.ExternalDbIsTrustedConnection;
            ExternalDbAdditionalConnectionStringDirective.Text = config.ExternalDbAdditionalConnectionStringDirective;
            WeightComPortName.Text = config.WeightComPortName;
            WeightComBoudRate.Text = config.WeightComBaudRate.ToString();
            WeightComIsConnected.IsChecked = config.WeightComIsConnected;
            WeightComDataBits.Text = config.WeightComDataBits.ToString();
            WeightComStopBits.SelectedItem = Enum.GetValues(typeof(System.IO.Ports.StopBits)).GetValue(config.WeightComStopBits);
            WeightComParity.SelectedItem = Enum.GetValues(typeof(System.IO.Ports.Parity)).GetValue(config.WeightComParity);
            DocumentTypeSymbol.Text = config.ProductionDocumentSymbol;
            WarehouseId.Text = config.WarehouseSymbol;
            EndpointUrl.Text = config.EndpointUrl;
            SystemPassword.Password = config.SystemPassword;
            ProductionPassword.Password = config.ProductionPassword;
            OnPasswordProduction.IsChecked = config.OnPasswordProduction;
            OnPasswordProductsImport.IsChecked = config.OnPasswordProductsImport;
        }

        private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new Config();
            try
            {
                cfg = new Config
                {
                    ServerAddress = ServerAddress.Text,
                    DatabaseName = DatabaseName.Text,
                    UserName = UserName.Text,
                    Password = Password.Password,
                    IsTrustedConnection = (bool)IsTrustedConnection.IsChecked,
                    AdditionalConnectionStringDirective = AdditionalConnectionStringDirective.Text,
                    ExternalDbServerAddress = ExternalDbServerAddress.Text,
                    ExternalDbDatabaseName = ExternalDbDatabaseName.Text,
                    ExternalDbUserName = ExternalDbUserName.Text,
                    ExternalDbPassword = ExternalDbPassword.Password,
                    ExternalDbIsTrustedConnection = (bool)ExternalDbIsTrustedConnection.IsChecked,
                    ExternalDbAdditionalConnectionStringDirective = ExternalDbAdditionalConnectionStringDirective.Text,
                    WeightComIsConnected = (bool)WeightComIsConnected.IsChecked,
                    WeightComPortName = WeightComPortName.Text != null ? WeightComPortName.Text : "",
                    WeightComBaudRate = int.Parse(WeightComBoudRate.Text),
                    WeightComDataBits = int.Parse(WeightComDataBits.Text),
                    WeightComStopBits = (int)(System.IO.Ports.StopBits)WeightComStopBits.SelectedItem,
                    WeightComParity = (int)(System.IO.Ports.Parity)WeightComParity.SelectedItem,
                    EndpointUrl = EndpointUrl.Text,
                    WarehouseSymbol = WarehouseId.Text,
                    ProductionDocumentSymbol = DocumentTypeSymbol.Text,
                    SystemPassword = SystemPassword.Password,
                    ProductionPassword = ProductionPassword.Password,
                    OnPasswordProduction = (bool)OnPasswordProduction.IsChecked,
                    OnPasswordProductsImport = (bool)OnPasswordProductsImport.IsChecked
                };
                var isSaved = configService.SaveConfig(cfg);
                if (isSaved)
                {
                    IsSaved = true;
                    Close();
                }
                else
                {
                    ShowErrorMsg(configService.Message);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ex.Message);
            }

        }

        private void ShowErrorMsg(string message)
        {
            var choice = MessageBox.Show($"Błąd zapisu konfiguracji\n{message}\n\nAnuluj żeby wrócić do edycji.", "Błąd", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (choice == MessageBoxResult.OK)
                this.Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
