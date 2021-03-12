using System;

namespace NovaAlert.Entities
{
    public interface IResource
    {
        int Id { get; }
        eResourceType ResourceType { get; }

        byte? SelectedPanelId { get; set; }
        event EventHandler OnClickHandler;
    }
}
