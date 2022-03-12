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
        public void CreateFilterButtonTest()
        {
            var mainWindow = typeof(MainWindow);
            var obj = new PrivateObject(mainWindow);
            var retVal = obj.Invoke("CreateFilterButton", new ProductGroup());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }

        [TestMethod]
        public void CreateProductButtonTest()
        {
            MainWindow mainWindow = new MainWindow();
            var obj = new PrivateObject(mainWindow);
            var retVal = obj.Invoke("CreateProductButton", new ProducedItem());
            Assert.AreEqual(typeof(Button), retVal.GetType());
        }
    }

}
