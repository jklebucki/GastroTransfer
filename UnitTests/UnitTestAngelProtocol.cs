using AngelProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class UnitTestAngelProtocol
    {
        [TestMethod]
        public void InitializeSerialPortTest()
        {
            WeightCommonObject weight = new WeightCommonObject(new ComPortSettings
            {
                BaudRate = 9600,
                PortName = "COM3",
                StopBits = 1,
                DataBits = 8,
                Parity = 0
            });

            var obj = new PrivateObject(weight);
            var testNumber = "NNNN0025,0";
            var arg = new object[1] { testNumber };
            var retVal = obj.Invoke("ParseResponseApProt", arg);
            Trace.WriteLine(retVal);
            Assert.IsTrue(typeof(decimal) == retVal.GetType() && (decimal)retVal == 25);

        }
        [TestMethod]
        public void GetWeightTest()
        {
            WeightCommonObject weight = new WeightCommonObject(new ComPortSettings
            {
                BaudRate = 9600,
                PortName = "COM3",
                StopBits = 1,
                DataBits = 8,
                Parity = 0
            });

            var test = weight.GetWeihgtApProt();
            Assert.IsTrue(test == 0);

        }
    }
}
