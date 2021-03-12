using NovaAlert.Common;
using NovaAlert.Common.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NovaAlert.UsbRecorder
{
    /// <summary>
    /// Usb reader uses AID library
    /// </summary>
    public class UsbReader2: IUsbReader
    {
        public event EventHandler<UsbDataReceiveEventArgs> OnDataReceived;
        public bool IsReading { get; private set; }

        public bool LogData { get; set; }
        public int ReadDelayInterval { get; set; }

        Thread _thread;
        public UsbReader2()
        {
            this.LogData = false;
            this.ReadDelayInterval = ClientSetting.Instance.Recorder_ReadDelayInterval;
        }

        public void Start()
        {
            AID.FT_Open();
            IsReading = true;
            //Action readerThread = new Action(ReaderThread);
            //readerThread.BeginInvoke(null, null);

            ThreadStart ts = new ThreadStart(ReaderThread);
            _thread = new Thread(ts);
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            IsReading = false;
            AID.FT_Close();
        }        

        void ReaderThread()
        {
            while(this.IsReading)
            {
                ulong rxSize = 0;
                ulong txSize = 0;
                AID.FT_GetStatus(ref rxSize, ref txSize);
                if(rxSize > 0)
                {
                    var data = new byte[rxSize];
                    var readCount = AID.FT_Read(data, rxSize);
                    if (readCount < rxSize) data = data.Where((d, i) => i < readCount).ToArray();
                    if (data.Length > 0 && this.OnDataReceived != null) OnDataReceived(this, new UsbDataReceiveEventArgs(data));

                    if(this.LogData)
                    {
                        LogService.Logger.Debug(string.Format("{0}", readCount));
                    }
                }

                // Sleep here
                //Thread.Sleep(100);
                if (this.ReadDelayInterval > 0) Thread.Sleep(this.ReadDelayInterval);
            }
        }


        public void Dispose()
        {
            
        }
    }
}
