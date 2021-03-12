using System;

namespace NovaAlert.Communication.Base
{
    public class ApplicationEventArgs : EventArgs
    {
        public object AppObj { get; private set; }
        public ApplicationEventArgs(object appObj)
        {
            AppObj = appObj;
        }
    }
}
