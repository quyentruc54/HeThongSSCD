using System;

namespace NovaAlert.Communication.Base
{
    public class PresentationEventArgs : EventArgs
    {
        public object Data { get; private set; }
        public PresentationEventArgs(object data)
        {
            this.Data = data;
        }
    }
}
