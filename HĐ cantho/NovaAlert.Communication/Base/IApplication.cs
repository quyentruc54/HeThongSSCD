using System;

namespace NovaAlert.Communication.Base
{
    public interface IApplication
    {
        IPresentation Presentation { get; }
        event EventHandler<ApplicationEventArgs> OnResultReceive;
    }
}
