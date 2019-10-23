using System;
using AngelProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTestAngelProtocol
    {
        [TestMethod]
        public void TestMethod2()
        {
            WeightCommonInterface weight = new WeightCommonInterface(new ComPortSettings
            {
                BaudRate = 9600,
                PortName = "COM3",
                StopBits = 1,
                DataBits = 8,
                Parity = 0
            });

            var test = weight.GetWeihgt();
            Assert.IsTrue(test > 0);

        }
    }
}
