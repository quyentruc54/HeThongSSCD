using NovaAlert.Communication.ATModem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestATModem
{
    public class FakeDataLink: DialupDataLink
    {
        public FakeDataLink():base(null)
        {            
        }

        public void Receive(string data)
        {
            RaiseEvent(new NovaAlert.Communication.Base.DataLinkEventArg(data));
        }

        public void Receive(char[] arr)
        {
            var s = new string(arr);
            Receive(s);
        }

        public override void SendControl(byte c)
        {
            //base.SendControl(c);
        }

        public override void SendData(string data, bool package = false)
        {
            //base.SendData(data, package);
        }
    }
}
