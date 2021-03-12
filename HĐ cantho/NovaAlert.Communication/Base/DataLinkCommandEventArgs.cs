using System;

namespace NovaAlert.Communication.Base
{
    public class DataLinkCommandEventArgs : EventArgs
    {
        public byte Command { get; private set; }

        public DataLinkCommandEventArgs(byte cmd)
            : base()
        {
            this.Command = cmd;
        }

    }
}
