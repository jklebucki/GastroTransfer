using GastroTransfer.Models;
using System.Windows;
using System.Windows.Input;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Config config { get; set; }
        public bool LoginOk { get; protected set; }
        private LoginType _loginType { get; set; }
        public LoginWindow(Config config, LoginType loginType)
        {
            _loginType = loginType;
            LoginOk = false;
            this.config = config;
            InitializeComponent();
            switch (_loginType)
            {
                case LoginType.System:
                    QueryText.Text = "Podaj hasło administratora.";
                    break;
                case LoginType.Production:
                    QueryText.Text = "Podaj hasło do produkcji.";
                    break;
            }
            
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogIn();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Password.KeyDown += Password_KeyDown;
            Password.FontSize = 16.00;
            Password.Focus();
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter || e.Key == Key.Return)
                LogIn();
        }

        private void LogIn()
        {
            switch (_loginType)
            {
                case LoginType.System:
                    CheckCorrect(Password.Password, config.SystemPassword);
                    break;
                case LoginType.Production:
                    CheckCorrect(Password.Password, config.ProductionPassword);
                    break;
            }
        }

        private void CheckCorrect(string savedPassword, string enteredPassword)
        {
            if (savedPassword == enteredPassword)
            {
                LoginOk = true;
                Close();
            }
            else
            {
                Info.Text = "Hasło nieprawidłowe";
                LoginOk = false;
            }
        }
    }
}
