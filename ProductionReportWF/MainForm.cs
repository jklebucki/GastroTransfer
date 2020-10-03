using ProductionReportWF.DbSettings;
using ProductionReportWF.DbSettings.Models;
using ProductionReportWF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ProductionReportWF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string ConnectionString()
        {
            return new SystemSettingsService(new CryptoService()).GetSystemSettings().ConnStr();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
            if (settings.SavePressed)
            {
                dgReport.Rows.Clear();
                dgReport.Refresh();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            dgReport.Rows.Clear();
            dgReport.Refresh();
            var prod = GetReportData();
            if (prod != null)
            {
                var viewData = new BindingList<ProductionView>(prod);
                dgReport.DataSource = viewData;
                if (viewData.Count > 0)
                {
                    SetColumns();
                }
            }
        }

        private List<ProductionView> GetReportData()
        {
            var dateFrom = new DateTime(dtFrom.Value.Year, dtFrom.Value.Month, dtFrom.Value.Day,
                timeFrom.Value.TimeOfDay.Hours, timeFrom.Value.TimeOfDay.Minutes, timeFrom.Value.TimeOfDay.Seconds);
            var dateTo = new DateTime(dtTo.Value.Year, dtTo.Value.Month, dtTo.Value.Day,
                timeTo.Value.TimeOfDay.Hours, timeTo.Value.TimeOfDay.Minutes, timeTo.Value.TimeOfDay.Seconds);
            try
            {
                using (var db = new DbGastroTransfer(ConnectionString()))
                {
                    var query = from p in db.Productions
                                join product in db.Products on p.ProducedItemId equals product.ProducedItemId
                                where p.ProducedItemId > 0 && (p.Registered >= dateFrom && p.Registered <= dateTo)
                                orderby p.Registered descending
                                select new ProductionView
                                {
                                    Nazwa = product.Name,
                                    Ilosc = p.Quantity,
                                    JM = product.UnitOfMesure,
                                    Wyslany = p.IsSentToExternalSystem,
                                    DataProdukcji = p.Registered,
                                    DataWyslaniaDoLSI = p.Registered != p.SentToExternalSystem ? p.SentToExternalSystem : (DateTime?)null
                                };
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            dgReport.AutoResizeColumns();
        }

        private void SetColumns()
        {
            dgReport.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[0].FillWeight = 35F;
            dgReport.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[1].FillWeight = 5F;
            dgReport.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[2].FillWeight = 5F;
            dgReport.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[3].FillWeight = 8F;
            dgReport.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[4].FillWeight = 15F;
            dgReport.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[5].FillWeight = 15F;
        }
    }
}
