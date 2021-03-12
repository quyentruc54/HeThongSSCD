using NovaAlert.Common;
using System;
using System.IO.Ports;

namespace NovaAlert.Communication.Base
{
    public abstract class SerialPortDataLink: DataLinkBase, IDisposable
    {
        public SerialPort Port { get; protected set; }

        public SerialPortDataLink(SerialPort port):base()
        {
            try
            {
                Port = port;
                Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);

                if (Port.IsOpen == false)
                {
                    Port.Open();
                    this.Status = eDataLinkStatus.Connected;
                }
            }
            catch(Exception ex)
            {
                this.Status = eDataLinkStatus.FailToConnect; // "Không thể mở cổng kết nối";
                LogService.Logger.Error(ex);                
            }
        }

        protected virtual void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var rcvData = Port.ReadExisting();
            _buffer += rcvData;
            ProcessBuffer();

            // Live log
            RaiseRawDataEvent(rcvData, true);
        }

        public void Dispose()
        {
            if (Port != null && Port.IsOpen)
            {
                Port.Close();
                Port.Dispose();
            }
        }
    }
}
