using System;

namespace NovaAlert.Communication.Base
{
    public interface IDataLink
    {        
        void SendControl(byte c);
        void SendData(string data, bool package = false);
        void SendData(object data, bool package = false);
        event EventHandler<DataLinkEventArg> OnDataReceive;
        event EventHandler<DataLinkCommandEventArgs> OnCommandReceive;
        event EventHandler<DataLinkEventArg> OnRawDataReceive;
        event EventHandler<DataLinkEventArg> OnRawDataSend;
        eDataLinkStatus Status { get; }        
    }

    public enum eDataLinkStatus
    {
        Disconnected,
        Connected,
        FailToConnect
    }
}
