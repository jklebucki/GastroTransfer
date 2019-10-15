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

namespace GastroTransfer.Views.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy MeasurementWindow.xaml
    /// </summary>
    public partial class MeasurementWindow : Window
    {
        public decimal Quantity { get; protected set; }
        public bool IsCanceled { get; protected set; }

        public MeasurementWindow(Style buttonStyle, string productName)
        {
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            SetButtonsStyle(buttonStyle);
            Quantity = 0;
            QuantityTextBox.Text = "0";
            ProductName.Text = productName;
        }

        private void keyboardEvent(object sender, ExecutedRoutedEventArgs e)
        {
            var sendedContent = ((RoutedUICommand)e.Command).Text;
            if (sendedContent != "ENTER")
                ImputsBehavior(sendedContent);
            else
                Confirm_Click(null, null);
        }

        private void NumericButton_Click(object sender, RoutedEventArgs e)
        {
            var sendedContent = (sender as Button).Content.ToString();
            ImputsBehavior(sendedContent);
        }

        private void ImputsBehavior(string sendedContent)
        {
            if (QuantityTextBox.Text == "0")
                QuantityTextBox.Text = "";
            if (sendedContent == "." && !QuantityTextBox.Text.Contains('.'))
                QuantityTextBox.Text += sendedContent;
            else if (sendedContent == "←")
            {
                if (QuantityTextBox.Text.Length <= 1)
                    QuantityTextBox.Text = "0";
                else if (QuantityTextBox.Text.Length > 1)
                    QuantityTextBox.Text = QuantityTextBox.Text.Substring(0, QuantityTextBox.Text.Length - 1);
            }
            else if (sendedContent != ".")
                QuantityTextBox.Text += sendedContent;
        }

        private void SetButtonsStyle(Style buttonStyle)
        {
            NumericButtonZero.Style = buttonStyle;
            NumericButtonOne.Style = buttonStyle;
            NumericButtonTwo.Style = buttonStyle;
            NumericButtonThree.Style = buttonStyle;
            NumericButtonFour.Style = buttonStyle;
            NumericButtonFive.Style = buttonStyle;
            NumericButtonSix.Style = buttonStyle;
            NumericButtonSeven.Style = buttonStyle;
            NumericButtonEight.Style = buttonStyle;
            NumericButtonNine.Style = buttonStyle;
            NumericButtonPoint.Style = buttonStyle;
            NumericButtonBack.Style = buttonStyle;
            Cancel.Style = buttonStyle;
            Confirm.Style = buttonStyle;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Quantity = Math.Round(decimal.Parse(QuantityTextBox.Text.Replace('.', ',')), 4);
            IsCanceled = false;
            Close();
        }

        private void QuantityTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyFunctions(e);

            if (e.Key == Key.Decimal || e.Key == Key.OemComma)
            {
                if ((sender as TextBox).Text.Replace('.', ',').Contains(","))
                    e.Handled = true;
            }
        }

        private void KeyFunctions(KeyEventArgs e)
        {
            if (!ConstData.numpadKeys.Contains(e.Key))
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                Cancel_Click(null, null);
            }
        }
    }
}
