using System;

namespace NovaAlert.Communication.Tcp
{
    public class StatusChangedEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public StatusChangedEventArgs(string status): base()
        {
            this.Status = status;
        }
    }
}
