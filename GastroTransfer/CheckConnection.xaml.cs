using GastroTransfer.Services;
using GastroTransfer.Views.Dialogs;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GastroTransfer
{
    /// <summary>
    /// Interaction logic for CheckConnection.xaml
    /// </summary>
    public partial class CheckConnection : Window
    {
        private string rtbMessage = "";
        private BackgroundWorker backgroundWorker;
        private DispatcherTimer timer;
        private DbService dbService;
        private readonly ConfigService configService;
        private bool connected;
        public CheckConnection(DbService dbService, ConfigService configService)
        {
            this.configService = configService;
            this.dbService = dbService;
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(999);
        }

        private void configButton_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigWindow();
            configWindow.Owner = this;
            configWindow.ShowDialog();
            if (configWindow.IsSaved)
            {
                closeButton.Visibility = Visibility.Hidden;
                configButton.Visibility = Visibility.Hidden;
                timer.Start();
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                closeButton.Visibility = Visibility.Visible;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeWorker();
            rtbInfo.Document.LineHeight = 2;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += timerTick;
            timer.Start();
        }

        public void timerTick(object sender, EventArgs e)
        {
            rtbInfo.Document.Blocks.Clear();
            rtbInfo.AppendText(rtbMessage);
            rtbInfo.ScrollToEnd();
        }

        private void initializeWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += doWork;
            backgroundWorker.RunWorkerCompleted += done;
            backgroundWorker.RunWorkerAsync();
        }

        private void doWork(object sender, DoWorkEventArgs e)
        {
            rtbMessage = "Sprawdzam połączenie z bazą danych...";
            //Check and initialize database 
            dbService = new DbService(configService.GetConfig());
            connected = dbService.CheckConnection();
            if (!connected)
            {
                rtbMessage = "Brak połączenia!";
                rtbMessage += $"\n{dbService.ErrorMessage}";
            }
            else
            {
                rtbMessage = "Połączono!";
                rtbMessage += "\nUruchamiam system...";
            }
            Thread.Sleep(100);
        }

        private void done(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!connected)
            {
                timer.Stop();
                closeButton.Visibility = Visibility.Visible;
                configButton.Visibility = Visibility.Visible;
            }
            else
            {
                timer.Stop();
                Thread.Sleep(2000);
                Close();
            }
        }
    }
}
