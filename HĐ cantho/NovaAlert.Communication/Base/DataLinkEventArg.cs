using System;

namespace NovaAlert.Communication.Base
{
    public class DataLinkEventArg : EventArgs
    {
        public string Data { get; private set; }

        public DataLinkEventArg(string data): base()
        {
            this.Data = data;
        }
    }
}
