using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductionReportWF
{
    public partial class EditForm : Form
    {
        public decimal Quantity { get; protected set; }
        public bool IsChanged { get; protected set; }
        private DataGridViewRow _row { get; set; }
        public EditForm(DataGridViewRow row)
        {
            InitializeComponent();
            _row = row;
            IsChanged = false;
            quantity.Select();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            productNameLabel.Text = _row.Cells[0].Value.ToString();
            quantity.Text = _row.Cells[2].Value.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            decimal quantityValue;
            var isParsed = decimal.TryParse(quantity.Text, out quantityValue);
            if (isParsed)
            {
                Quantity = quantityValue;
                IsChanged = true;
                Close();
            }
            else
            {
                quantity.Text = _row.Cells[2].Value.ToString();
                errorLabel.Text = "Podaj prawidłową ilość";
            }
        }

        private void quantity_Enter(object sender, EventArgs e)
        {
            errorLabel.Text = "";
        }
    }
}
