using NovaAlert.Entities;
using System.ServiceModel;

namespace NovaAlert.Config
{
    public static class Proxy
    {
        public static INovaAlertConfigService CreateProxy()
        {            
            var cf = new ChannelFactory<INovaAlertConfigService>("NovaAlertConfigService_NetTcp");
            return cf.CreateChannel();
        }

    }
}
