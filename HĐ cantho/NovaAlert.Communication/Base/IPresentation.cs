using System;

namespace NovaAlert.Communication.Base
{
    public interface IPresentation
    {
        IDataLink DataLink { get; }
        event EventHandler<PresentationEventArgs> OnDataReceive;
        void SendData(object data, bool queue = true);
    }
}
