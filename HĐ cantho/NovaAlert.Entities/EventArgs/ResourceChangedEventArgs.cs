using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class ResourceChangedEventArgs: BaseEventArgs
    {        
        [DataMember]
        public eResourceType ResourceType { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public byte? PanelId { get; set; }

        public ResourceChangedEventArgs(eResourceType type, int id, byte? panelId, Task task)
        {
            this.Handled = false;
            this.ResourceType = type;
            this.Id = id;
            this.PanelId = panelId;
        }
    }
}
