using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using NovaAlert.Communication.Base;

namespace NovaAlert.SwitchIC.DataLinkLayer
{
    public class DataLink: SerialPortDataLink
    {
        const byte MinLength = 8;
        const byte MaxLength = 200;
        const char Begin1 = (char)0x0D;
        const char Begin2 = (char)0x0A;

        public DataLink(SerialPort port):base(port)
        {
        }

        public override void SendData(string data, bool package = false)
        {
            if (Port == null || Port.IsOpen == false) return;
            string sendData = data;

            if(package)
            {
                sendData = string.Format("{0}{1}{2}{3}{4}", (char)Begin1, (char)Begin2, (char)data.Length, data, (char)0);
            }

            Port.Write(sendData);
            RaiseRawDataEvent(sendData, false);

            //if (package)
            //    Port.Write(string.Format("{0}{1}{2}{3}{4}", (char)Begin1, (char)Begin2, (char)data.Length, data, (char)0)); //CalculateChecksum(data)));
            //else
            //    Port.Write(data);
        }

        public override void SendData(object data, bool package = false)
        {
            string sendData = null;

            if(data is byte[])
            {
                var arr = (byte[])data;
                if(package)
                {
                    var parr = new byte[arr.Length + 4];
                    parr[0] = (byte)Begin1;
                    parr[1] = (byte)Begin2;
                    parr[2] = (byte)arr.Length;
                    arr.CopyTo(parr, 3);
                    parr[parr.Length - 1] = 0;
                    Port.Write(parr, 0, parr.Length);

                    sendData = System.Text.Encoding.ASCII.GetString(parr);
                }
                else
                {
                    Port.Write(arr, 0, arr.Length);
                    sendData = System.Text.Encoding.ASCII.GetString(arr);
                }
            }
            else
            {
                sendData = data.ToString();
                SendData(sendData, package);                
            }

            RaiseRawDataEvent(sendData, false);
        }

        byte CalculateChecksum(string s)
        {
            byte cs = 0;
            for (int i = 0; i < s.Length; i++)
                cs += (byte)s[i];
            return cs;
        }

        protected override string GetNextCommand(ref string data)
        {
            var length = data.Length;
            if (length < MinLength) return null;

            var start = data.IndexOf(Begin1);
            if (start < 0 || length - start < MinLength || data[start + 1] != Begin2)
            {
                if (data[start + 1] != Begin2 && data.IndexOf(Begin1, start + 1) > start && data[data.IndexOf(Begin1, start + 1) + 1] == Begin2)
                    data = data.Remove(0, start+1);
                return null;
            }

            var msgLength = data[start + 2];
            var end = start + 3 + msgLength;

            if (msgLength > MaxLength || end > length) return null;

            var msg = data.Substring(start + 3, msgLength);
            if (end == length) data = string.Empty;
            else data = data.Substring(end + 1);

            return msg;
        }
    }
}
