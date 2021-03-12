using System;
using NovaAlert.Entities;

namespace NovaAlert.Service
{
    public class ClientAction
    {
        public DateTime Time { get; set; }
        public Client Client { get; set; }
        public eAction Action { get; set; }
        public eResourceType? ResourceType { get; set; }
        public int? ResourceId { get; set; }
    }
}
