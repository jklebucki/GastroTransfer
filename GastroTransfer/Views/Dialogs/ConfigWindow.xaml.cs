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
        private ConfigService configService;
        private string[] ports { get; set; }
        public bool IsSaved { get; protected set; }
        public ConfigWindow(Style style)
        {
            IsSaved = false;
            try
            {
                Owner = Application.Current.MainWindow;
                ports = System.IO.Ports.SerialPort.GetPortNames();
            }
            catch (Exception)
            {
                //Ignore
            }

            Icon = null;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            configService = new ConfigService(new CryptoService());
            InitializeComponent();
            CloseButton.Style = style;
            SaveConfigButton.Style = style;
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs args)
        {
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
            WeightComPortName.Text = config.WeightComPortName;
            WeightComBoudRate.Text = config.WeightComBaudRate.ToString();
            WeightComIsConnected.IsChecked = config.WeightComIsConnected;
            WeightComDataBits.Text = config.WeightComDataBits.ToString();
            WeightComStopBits.SelectedItem = Enum.GetValues(typeof(System.IO.Ports.StopBits)).GetValue(config.WeightComStopBits);
            WeightComParity.SelectedItem = Enum.GetValues(typeof(System.IO.Ports.Parity)).GetValue(config.WeightComParity);
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
                    WeightComIsConnected = (bool)WeightComIsConnected.IsChecked,
                    WeightComPortName = WeightComPortName.Text,
                    WeightComBaudRate = int.Parse(WeightComBoudRate.Text),
                    WeightComDataBits = int.Parse(WeightComDataBits.Text),
                    WeightComStopBits = (int)(System.IO.Ports.StopBits)WeightComStopBits.SelectedItem,
                    WeightComParity = (int)(System.IO.Ports.Parity)WeightComParity.SelectedItem
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
                Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
