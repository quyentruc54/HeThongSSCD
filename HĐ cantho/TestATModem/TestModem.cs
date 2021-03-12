using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NovaAlert.Communication.ATModem;
using NovaAlert.Common;
using log4net;
using System.IO.Ports;

namespace TestATModem
{
    [TestClass]
    public class TestModem
    {
        [TestInitialize]
        public void Init()
        {
            //ServiceContainer.Instance.AddService<ILog>(new log4net.Fakes.StubILog());
        }

        
        [TestMethod]
        public void TestRing()
        {
            var dl = new FakeDataLink();
            var modem = new ItuV250Modem(dl);

            dl.Receive(Parser.BuildToken(ResultCodeToken.RING, '\r', '\n'));
        }

        [TestMethod]
        public void TestDial()
        {
            //var dl = new FakeDataLink();
            var port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            var dl = new DialupDataLink(port);
            var modem = new ItuV250Modem(dl);

            //var mid = modem.ManufacturerId;
            IConnection conn;
            try
            {
                modem.TurnEchoOff();
                conn = modem.Dial("229");
                conn.Modem.DataLink.SendData("Hello world");
                conn.Modem.DataLink.SendData("Hello world");
                conn.Disconnect();
            }
            catch(Exception ex)
            {
            }
        }

        [TestMethod]
        public void TestReceive()
        {
            var port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            var dl = new DialupDataLink(port);
            var modem = new ItuV250Modem(dl);
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
