using NovaAlert.Communication.Base;
using System.IO.Ports;
using System.Text;

namespace NovaAlert.Communication.ATModem
{
    public class DialupDataLink: SerialPortDataLink
    {
        public DialupDataLink(SerialPort port):base(port)
        {
            this.Port.Encoding = Encoding.UTF8;
        }

        protected override void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var s = Port.ReadExisting();
            RaiseEvent(new DataLinkEventArg(s));
        }

        protected override string GetNextCommand(ref string data)
        {
            return null;
        }

        public override void SendData(string data, bool package = false)
        {
            if (Port == null || Port.IsOpen == false) return;
            Port.Write(data);
        }

        public override void SendData(object data, bool package = false)
        {
            if (data is byte[])
            {
                var arr = (byte[])data;
                Port.Write(arr, 0, arr.Length);
            }
            else
            {
                SendData(data.ToString());
            }
        }

        public override void SendControl(byte c)
        {
            this.Port.Write(new byte[] { c }, 0, 1);
        }
    }
}
