using GastroTransfer.Helpers;
using GastroTransfer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Controls;

namespace UnitTests
{
    [TestClass]
    public class TestMainWindow
    {
        private void ButtonTestEventHendler(object sender, RoutedEventArgs e) { }

        [TestMethod]
        public void CreateFilterButtonTest()
        {
            var style = new System.Collections.Generic.Dictionary<string, Style>();
            style.Add("roundedButton", new Style());
            var retVal = CreateControls.CreateProductButton(style, ButtonTestEventHendler, new ProducedItem());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }

        [TestMethod]
        public void CreateProductButtonTest()
        {
            var style = new System.Collections.Generic.Dictionary<string, Style>();
            style.Add("roundedButton", new Style());
            var retVal = CreateControls.CreateFilterButton(style, ButtonTestEventHendler, new ProductGroup());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }
    }

}
