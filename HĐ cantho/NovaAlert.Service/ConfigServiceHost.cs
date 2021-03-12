using System;
using System.ServiceModel;

namespace NovaAlert.Service
{
    public class ConfigServiceHost: ServiceHost
    {
        public event EventHandler SystemChanged;
        public void RaiseSystemChangedEvent()
        {
            if (this.SystemChanged != null) this.SystemChanged(this, EventArgs.Empty);
        }

        public ConfigServiceHost(Type serviceType, params Uri[] baseAddresses):base(serviceType, baseAddresses)
        {

        }
    }
}
