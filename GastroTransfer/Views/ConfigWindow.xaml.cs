using GastroTransfer.Data;
using GastroTransfer.Models;
using GastroTransfer.Services;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace GastroTransfer.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private ConfigService configService;
        public ConfigWindow()
        {
            Owner = Application.Current.MainWindow;
            Icon = null;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            configService = new ConfigService(new CryptoService());
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs args)
        {

            var config = configService.GetConfig();
            ServerAddress.Text = config.ServerAddress;
            DatabaseName.Text = config.DatabaseName;
            UserName.Text = config.UserName;
            Password.Password = config.Password;
            IsTrustedConnection.IsChecked = config.IsTrustedConnection;
            AdditionalConnectionStringDirective.Text = config.AdditionalConnectionStringDirective;
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var isSaved = configService.SaveConfig(new Config
            {
                ServerAddress = ServerAddress.Text,
                DatabaseName = DatabaseName.Text,
                UserName = UserName.Text,
                Password = Password.Password,
                IsTrustedConnection = (bool)IsTrustedConnection.IsChecked,
                AdditionalConnectionStringDirective = AdditionalConnectionStringDirective.Text
            });
            if (isSaved)
                Close();
            else
            {
                var choice = MessageBox.Show("Błąd zapisu konfiguracji", "Błąd", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (choice == MessageBoxResult.OK)
                    Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
