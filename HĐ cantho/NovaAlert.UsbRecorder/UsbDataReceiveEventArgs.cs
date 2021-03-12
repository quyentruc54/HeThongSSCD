using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public class UsbDataReceiveEventArgs: EventArgs
    {
        public byte[] Data { get; private set; }
        public UsbDataReceiveEventArgs(byte[] data)
        {
            this.Data = data;
        }
    }
}
