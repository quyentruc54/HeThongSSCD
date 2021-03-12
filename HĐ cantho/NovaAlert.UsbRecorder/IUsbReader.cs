using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public interface IUsbReader: IDisposable
    {
        void Start();
        void Stop();

        event EventHandler<UsbDataReceiveEventArgs> OnDataReceived;
        bool IsReading { get; }
        bool LogData { get; set; }
        int ReadDelayInterval { get; set; }
    }
}
