using System.Collections.Generic;

namespace NovaAlert.Entities
{
    public interface ISwitchConnection
    {
        IEnumerable<ISwitchAddress> Devices { get; }
        bool IsConnected { get; set; }
    }
}
