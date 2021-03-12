using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NovaAlert.UsbRecorder
{
    public class UsbReader : IUsbReader
    {
        #region Properties
        public FTDI FtdiDevice { get; private set; }

        EventWaitHandle _waitHandler;
        Thread _readerThread;

        public event EventHandler<UsbDataReceiveEventArgs> OnDataReceived;

        public string SerialNumber { get; private set; }

        public bool IsReading { get; private set; }
        public bool LogData { get; set; }
        public int ReadDelayInterval { get; set; }
        #endregion

        #region Ctor
        public UsbReader():this(null)
        {
        }

        public UsbReader(string serialNumber)
        {
            this.IsReading = false;
            this.SerialNumber = serialNumber;
            this.FtdiDevice = new FTDI();
        } 
        #endregion

        #region Helpers
        void SetDeviceParameter(FTDI dev)
        {
            var ftStatus = dev.SetBaudRate(3000000);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");

            ftStatus = dev.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");

            ftStatus = dev.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");

            ftStatus = dev.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");
        }

        public void Start()
        {
            FTDI.FT_STATUS ftStatus;
            if(string.IsNullOrEmpty(this.SerialNumber)) ftStatus = this.FtdiDevice.OpenByIndex(0);
            else ftStatus = this.FtdiDevice.OpenBySerialNumber(this.SerialNumber);

            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");

            _waitHandler = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
            ftStatus = this.FtdiDevice.SetEventNotification(FTD2XX_NET.FTDI.FT_EVENTS.FT_EVENT_RXCHAR, _waitHandler);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to SetEventNotification (error " + ftStatus.ToString() + ")");

            SetDeviceParameter(this.FtdiDevice);

            this.IsReading = true;
            _readerThread = new Thread(ReaderFromDevice, 0);
            _readerThread.Start();
        }

        public void Stop()
        {
            this.FtdiDevice.Close();
            this.IsReading = false;            
            if(_waitHandler != null)
            {
                _waitHandler.Set();
            }

            if (_readerThread != null) _readerThread.Abort();
            if (_waitHandler != null) _waitHandler.Dispose();
        }

        void ReaderFromDevice()
        {
            while (IsReading)
            {
                _waitHandler.WaitOne();
                if (!IsReading) break;

                // Do reading
                uint readCount = 0;
                this.FtdiDevice.GetRxBytesAvailable(ref readCount);

                if (readCount > 0)
                {
                    var buff = new byte[readCount];
                    uint receiveCount = 0;
                    this.FtdiDevice.Read(buff, readCount, ref receiveCount);

                    // Continue setting event monitor                    
                    var ftStatus = this.FtdiDevice.SetEventNotification(FTD2XX_NET.FTDI.FT_EVENTS.FT_EVENT_RXCHAR, _waitHandler);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to SetEventNotification (error " + ftStatus.ToString() + ")");

                    if (receiveCount < readCount) buff = buff.Where((d, i) => i < receiveCount).ToArray();
                    if (buff.Length > 0 && this.OnDataReceived != null) OnDataReceived(this, new UsbDataReceiveEventArgs(buff));
                }
            }
        } 

        public static List<KeyValuePair<string, string>> GetUsbDeviceList()
        {
            const int maxCount = 255;
            var dev = new FTDI();
            FTDI.FT_DEVICE_INFO_NODE[] arr = new FTDI.FT_DEVICE_INFO_NODE[maxCount];
            dev.GetDeviceList(arr);

            int i = 0;
            var info = arr[i];
            var list = new List<KeyValuePair<string, string>>();
            while(info != null & i < maxCount)
            {
                list.Add(new KeyValuePair<string, string>(info.SerialNumber, info.Description));
                i++;
                info = arr[i];
            }

            return list;
        }
        #endregion

        #region Disposing Pattern
        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Free any other managed objects here. 
                Stop();
            }

            // Free any unmanaged objects here. 
            try
            {
                if (this.FtdiDevice != null && this.FtdiDevice.IsOpen) this.FtdiDevice.Close();
            }
            catch { }

            disposed = true;
        } 
        #endregion
    }
}
