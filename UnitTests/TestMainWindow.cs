using GastroTransfer;
using GastroTransfer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;

namespace UnitTests
{
    [TestClass]
    public class TestMainWindow
    {

        [TestMethod]
        public void CreateFilterButton()
        {
            var obj = new PrivateObject(typeof(MainWindow));
            var retVal = obj.Invoke("CreateFilterButton", new ProductGroup());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }

        [TestMethod]
        public void CreateProductButton()
        {
            var obj = new PrivateObject(typeof(MainWindow));
            var retVal = obj.Invoke("CreateProductButton", new ProducedItem());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }
    }

}
