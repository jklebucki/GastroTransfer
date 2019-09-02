using GastroTransfer.Data;
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

namespace GastroTransfer.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : Window
    {
        private AppDbContext context { get; set; }
        public ConfigPage(AppDbContext context)
        {
            this.context = context;
            InitializeComponent();
        }
    }
}
