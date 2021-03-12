using System;

namespace NovaAlert.Communication.Base
{
    public abstract class PresentationBase : IPresentation
    {
        public IDataLink DataLink { get; private set; }
        public PresentationBase(IDataLink dataLink)
        {
            if (dataLink == null)
            {
                throw new InvalidOperationException();
            }

            this.DataLink = dataLink;
            this.DataLink.OnDataReceive += DataLink_OnDataReceive;

            this.DataLink.OnCommandReceive += new EventHandler<DataLinkCommandEventArgs>(DataLink_OnCommandReceive);
        }

        void DataLink_OnCommandReceive(object sender, DataLinkCommandEventArgs e)
        {
            ProcessDataLinkCommand(e.Command);
        }

        public virtual void SendData(object data, bool queue = false)
        {
            throw new NotImplementedException();
        }

        private void DataLink_OnDataReceive(object sender, DataLinkEventArg e)
        {
            ProcessDataLinkEvent(e);
        }

        protected void RaiseEvent(PresentationEventArgs e)
        {
            if (OnDataReceive != null)
                OnDataReceive(this, e);
        }

        protected virtual void ProcessDataLinkEvent(DataLinkEventArg e)
        {
            throw new NotImplementedException();
        }

        protected virtual void ProcessDataLinkCommand(byte p)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<PresentationEventArgs> OnDataReceive;
    }
}
