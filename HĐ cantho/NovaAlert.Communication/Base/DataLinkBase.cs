using System;

namespace NovaAlert.Communication.Base
{
    public abstract class DataLinkBase: IDataLink
    {
        #region Members & Properties
        protected string _buffer;
        public eDataLinkStatus Status { get; protected set; }
        public event EventHandler<DataLinkEventArg> OnDataReceive;
        public event EventHandler<DataLinkCommandEventArgs> OnCommandReceive;
        public event EventHandler<DataLinkEventArg> OnRawDataReceive;
        public event EventHandler<DataLinkEventArg> OnRawDataSend; 
        #endregion

        #region Ctor
        public DataLinkBase()
        {
            _buffer = string.Empty;
            this.Status = eDataLinkStatus.Disconnected;
        } 
        #endregion

        #region Helpers
        protected void RaiseRawDataEvent(string data, bool rcv)
        {
            if(rcv)
            {
                if (OnRawDataReceive != null) OnRawDataReceive(this, new DataLinkEventArg(data));
            }
            else
            {
                if (OnRawDataSend != null) OnRawDataSend(this, new DataLinkEventArg(data));
            }
        }
        public virtual void SendControl(byte c)
        {
            throw new NotImplementedException();
        }

        public virtual void SendData(string data, bool package = false)
        {
            throw new NotImplementedException();
        }

        public virtual void SendData(object data, bool package = false)
        {
            throw new NotImplementedException();
        }

        protected void RaiseEvent(DataLinkEventArg e)
        {
            if (OnDataReceive != null) OnDataReceive(this, e);
        }

        protected virtual void ProcessBuffer()
        {
            var msg = GetNextCommand(ref _buffer);

            while (msg != null)
            {
                RaiseEvent(new DataLinkEventArg(msg));
                msg = GetNextCommand(ref _buffer);
            }
        }

        protected abstract string GetNextCommand(ref string data); 
        #endregion
        
        public static string GetStatusText(eDataLinkStatus status)
        {
            var s = string.Empty;
            switch (status)
            {
                case eDataLinkStatus.Disconnected:
                    s = "Chưa kết nối.";
                    break;
                case eDataLinkStatus.Connected:
                    s = "Đã kết nối.";
                    break;
                case eDataLinkStatus.FailToConnect:
                    s = "Lỗi, không thể kết nối";
                    break;
                default:
                    break;
            }

            return s;
        }
    }
}
