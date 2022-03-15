using LinqToDB;
using ProductionReportWF.Common;
using ProductionReportWF.DbSettings;
using ProductionReportWF.DbSettings.Models;
using ProductionReportWF.Models;
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
        private int? _selectedIndex;
        public MainForm()
        {
            InitializeComponent();
        }

        private string ConnectionString()
        {
            return new SystemSettingsService(new CryptoService()).GetSystemSettings().ConnStr();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
            if (settings.SavePressed)
            {
                dgReport.Rows.Clear();
                dgReport.Refresh();
            }
        }

        private void BtnProductionReport_Click(object sender, EventArgs e)
        {
            GetReport(1);
        }
        private void BtnTrashReport_Click(object sender, EventArgs e)
        {
            GetReport(2);
        }

        private void GetReport(int? reportType)
        {
            dgReport.Rows.Clear();
            dgReport.Refresh();
            var prod = GetReportData(reportType);
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
        /// <summary>
        /// Operation type: 1 production, 2 trash
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private List<ProductionView> GetReportData(int? operationType)
        {
            int? operationTypeFirstCondition = operationType == 1 ? null : operationType;
            int? operationTypeSecondCondition = operationType;
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
                                where p.ProducedItemId > 0
                                    && (p.Registered >= dateFrom && p.Registered <= dateTo)
                                    && (p.OperationType == operationTypeFirstCondition || p.OperationType == operationType)
                                orderby p.Registered descending
                                select new ProductionView
                                {
                                    Nazwa = product.Name,
                                    Ilosc = p.Quantity,
                                    JM = product.UnitOfMesure,
                                    Wyslany = p.IsSentToExternalSystem,
                                    DataProdukcji = p.Registered,
                                    DataWyslaniaDoLSI = p.SentToExternalSystem,
                                    Id = p.ProductionItemId
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
            dgReport.Columns[2].FillWeight = 12F;
            dgReport.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[3].FillWeight = 8F;
            dgReport.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[4].FillWeight = 15F;
            dgReport.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgReport.Columns[5].FillWeight = 15F;
            dgReport.Columns[6].Visible = false;
            foreach (DataGridViewRow row in dgReport.Rows)
            {
                row.ContextMenuStrip = contextMenuRowEdit;
                if (!(bool)row.Cells[3].Value) row.Cells[5].Value = null;
            }
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((bool)dgReport.Rows[(int)_selectedIndex].Cells[3].Value)
            {
                MessageBox.Show(this, $"Edycja niedozwolona - pozycja przesłana do LSI", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedIndex == null)
            {
                MessageBox.Show(this, $"Wybierz pozycję", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var selectedRow = dgReport.Rows[(int)_selectedIndex];
                EditForm editForm = new EditForm(selectedRow);
                editForm.ShowDialog(this);
                if (editForm.IsChanged)
                {
                    dgReport.Rows[(int)_selectedIndex].Cells[2].Value = string.Format("{0:0.0000}", editForm.Quantity);
                    UpdateQuantityDb((int)selectedRow.Cells[6].Value, editForm.Quantity);
                }
            }
        }

        private void DgReport_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            _selectedIndex = e.RowIndex;
        }

        private void UpdateQuantityDb(int id, decimal quantity)
        {
            using (var db = new DbGastroTransfer(ConnectionString()))
            {
                var productionItem = db.Productions.FirstOrDefault(p => p.ProductionItemId == id);
                productionItem.Quantity = quantity;
                db.Update(productionItem);
            }
        }

        private SaveFileDialog InitSaveFileDialog(string fileName = "Dane")
        {
            return new SaveFileDialog
            {
                DefaultExt = "xlsx",
                FileName = fileName,
                Filter = "Pliki Excel (*.xlsx)|*.xlsx"
            };
        }

        private void BtnExportToExcelRaw_Click(object sender, EventArgs e)
        {
            if (dgReport.Rows.Count == 0)
            {
                MessageBox.Show(this, "Brak danych do eksportu", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saveFileDialog = InitSaveFileDialog("DaneProdukcji");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    infoLab.Text = "Eksportuję dane...";
                    Refresh();
                    ExportToExcel.dataGridToExcel(saveFileDialog.FileName, false, dgReport, "Produkcja");
                    MessageBox.Show(this, "Eksport do pliku zakończony", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    infoLab.Text = "";
                    Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nError 3", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private List<ReportRow> AggregateReportData(DataGridViewRowCollection viewRowCollection)
        {
            List<ReportRow> report = new List<ReportRow>();
            foreach (DataGridViewRow row in viewRowCollection)
            {
                report.Add(new ReportRow
                {
                    Product = row.Cells[0].Value.ToString(),
                    Unit = row.Cells[1].Value.ToString(),
                    Quantity = Convert.ToDecimal(row.Cells[2].Value),
                    Year = Convert.ToDateTime(row.Cells[4].Value).Year,
                    Month = Convert.ToDateTime(row.Cells[4].Value).Month,
                });
            }

            report = report.GroupBy(r => new { r.Product, r.Unit, r.Month, r.Year }).Select(d => new ReportRow
            {
                Product = d.Key.Product,
                Unit = d.Key.Unit,
                Month = d.Key.Month,
                Year = d.Key.Year,
                Quantity = d.Sum(s => s.Quantity)
            }).ToList();
            return report;
        }

        private void BtnExportToExcel_Click(object sender, EventArgs e)
        {
            if (dgReport.Rows.Count == 0)
            {
                MessageBox.Show(this, "Brak danych do eksportu", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var report = AggregateReportData(dgReport.Rows);
            var saveFileDialog = InitSaveFileDialog("DaneProdukcjiZagregowane");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    infoLab.Text = "Eksportuję dane...";
                    Refresh();
                    ExportToExcel.listToExcel(saveFileDialog.FileName, false, report, "Dane");
                    MessageBox.Show(this, "Eksport do pliku zakończony", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    infoLab.Text = "";
                    Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nError 3", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveDocumentInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                "Na pewno chcesz zresetować status wysyłki?",
                "Potwierdź",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (_selectedIndex == null)
            {
                MessageBox.Show(this, $"Wybierz pozycję", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if ((bool)dgReport.Rows[(int)_selectedIndex].Cells[3].Value
                && dgReport.Rows[(int)_selectedIndex].Cells[3].Value != null
                && (int)dgReport.Rows[(int)_selectedIndex].Cells[6].Value > 0)
            {
                ClearDocumentStatus((int)_selectedIndex);
            }
            else
            {
                MessageBox.Show(this, $"Pozycje jeszcze nie była przesłana do LSI", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void ClearDocumentStatus(int selectedIndex)
        {
            var positionsToClear = new List<ProductionView>();

            using (var db = new DbGastroTransfer(ConnectionString()))
            {
                var id = (int)dgReport.Rows[(int)_selectedIndex].Cells[6].Value;
                var packageNumber = db.Productions.FirstOrDefault(p => p.ProductionItemId == id).PackageNumber;

                var query = from p in db.Productions
                            join product in db.Products on p.ProducedItemId equals product.ProducedItemId
                            where p.ProducedItemId > 0 && p.PackageNumber == packageNumber
                            orderby p.Registered descending
                            select new ProductionView
                            {
                                Nazwa = product.Name,
                                Ilosc = p.Quantity,
                                JM = product.UnitOfMesure,
                                Wyslany = p.IsSentToExternalSystem,
                                DataProdukcji = p.Registered,
                                DataWyslaniaDoLSI = p.SentToExternalSystem,
                                Id = p.ProductionItemId
                            };
                positionsToClear = query.ToList();
            }

            if (positionsToClear.Count > 0)
            {
                using (var db = new DbGastroTransfer(ConnectionString()))
                {
                    var prod = db.Productions
                        .Where(p => positionsToClear.Select(r => r.Id).Contains(p.ProductionItemId))
                        .Set(p => p.IsSentToExternalSystem, false)
                        .Set(p => p.PackageNumber, (int?)null)
                        .Set(p => p.DocumentType, (int?)null)
                        .Set(p => p.SentToExternalSystem, (p => p.Registered))
                        .Update();
                }
            }
            BtnProductionReport_Click(null, null);
            dgReport.FirstDisplayedScrollingRowIndex = selectedIndex;
            dgReport.Rows[selectedIndex].Selected = true;
        }

    }
}
