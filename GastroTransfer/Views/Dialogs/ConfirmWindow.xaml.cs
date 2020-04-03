using System.Windows;

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public bool IsCanacel { get; protected set; }
        public ConfirmWindow()
        {
            InitializeComponent();
        }

        public ConfirmWindow(string confirmButtonText, string cancelButtonText, string queryText)
        {
            InitializeComponent();
            QueryText.Text = queryText;
            CancelButton.Content = cancelButtonText;
            CloseButton.Content = confirmButtonText;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            IsCanacel = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCanacel = true;
            Close();
        }
    }
}
