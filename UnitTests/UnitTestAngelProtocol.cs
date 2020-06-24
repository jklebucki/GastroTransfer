using AngelProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO.Ports;

namespace UnitTests
{
    [TestClass]
    public class UnitTestAngelProtocol
    {
        [TestMethod]
        public void InitializeSerialPort()
        {
            WeightCommonInterface weight = new WeightCommonInterface(new ComPortSettings
            {
                BaudRate = 9600,
                PortName = "COM3",
                StopBits = 1,
                DataBits = 8,
                Parity = 0
            });
            var obj = new PrivateObject(weight);
            var retVal = obj.Invoke("InitializeSerialPort");
            Trace.WriteLine(retVal);
            Assert.IsTrue(typeof(SerialPort) == retVal.GetType());

        }
        [TestMethod]
        public void GetWeight()
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
            Assert.IsTrue(test == 0);

        }
    }
}
