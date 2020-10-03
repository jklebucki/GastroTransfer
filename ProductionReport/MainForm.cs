using LinqToDB.Data;
using ProductionReport.DbSettings;
using ProductionReport.DbSettings.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ProductionReport
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.DataGridView dgReport;
        public MainForm()
        {
            InitializeComponent();
            DataGridInit();
            DataConnection.DefaultSettings = new DatabaseSettings();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {

            //MessageBox.Show($"Ilość produktów: {prod.Count}");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //MessageBox.Show($"{timeFrom.Value.TimeOfDay} -- {timeTo.Value.TimeOfDay}");
            var prod = All();
            var p = Prod();
            var viewData = new BindingList<Production>(p);
            dgReport.DataSource = viewData;

            //foreach (DataGridViewColumn col in dgReport.Columns)
            //{
            //    col.SortMode = DataGridViewColumnSortMode.Automatic;
            //}
        }

        public static List<Product> All()
        {
            using (var db = new DbGastroTransfer())
            {
                var query = from p in db.Products
                            where p.ProducedItemId > 0
                            orderby p.Name descending
                            select p;
                return query.ToList();
            }
        }

        public static List<Production> Prod()
        {
            using (var db = new DbGastroTransfer())
            {
                var query = from p in db.Productions
                            where p.ProducedItemId > 0
                            orderby p.Quantity descending
                            select p;
                return query.ToList();
            }
        }

        private void DataGridInit()
        {
            this.dgReport = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgReport)).BeginInit();
            this.SuspendLayout();
            // 
            // dgReport
            // 
            this.dgReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgReport.AllowUserToAddRows = false;
            dgReport.AllowUserToDeleteRows = false;
            dgReport.AllowUserToOrderColumns = true;
            dgReport.AllowDrop = false;
            dgReport.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgReport.Parent = this.splitContainer1.Panel2;
            this.dgReport.Dock = DockStyle.Fill;
            this.dgReport.Margin = new Padding(4, 3, 4, 3);
            this.dgReport.Name = "dgRaporty";
            this.dgReport.TabIndex = 0;
            //dgReport.ColumnHeaderMouseClick += DgReport_ColumnHeaderMouseClick;
            ((System.ComponentModel.ISupportInitialize)(this.dgReport)).EndInit();
        }

        private void DgReport_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //dgReport.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }
}
