using GastroTransfer.Data;
//using GastroTransfer.Migrations;
using GastroTransfer.Models;
using GastroTransfer.Pages;
using GastroTransfer.Services;
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
            appDbContext = new AppDbContext(dbService.GetConnectionString());
            var dbInit = appDbContext.ProducedItems.FirstOrDefault();

            while (!dbService.CheckConnection())
            {
                //open config form, after 
                MessageBox.Show("Brak połączenia!" + dbService.ErrorMessage);
            }
        }

        private void GetButtons()
        {


        }

        private void GetData()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigPage configPage = new ConfigPage();
            configPage.ShowDialog();
        }
    }
}
