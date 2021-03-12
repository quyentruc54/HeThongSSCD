using System;

namespace NovaAlert.Communication.Base
{
    public abstract class ApplicationBase : IApplication
    {
        public IPresentation Presentation
        {
            get;
            private set;
        }

        public event EventHandler<ApplicationEventArgs> OnResultReceive;

        public ApplicationBase(IPresentation presentation)
        {
            if (presentation == null)
                throw new InvalidOperationException();

            this.Presentation = presentation;
            this.Presentation.OnDataReceive += Presentation_OnDataReceive;
        }

        public ApplicationBase()
        {
        }

        private void Presentation_OnDataReceive(object sender, PresentationEventArgs e)
        {
            ProcessPresentationEvent(e);
        }

        protected void RaiseEvent(ApplicationEventArgs e)
        {
            if (OnResultReceive != null)
                OnResultReceive(this, e);
        }

        protected virtual void ProcessPresentationEvent(PresentationEventArgs e)
        {
        }
    }
}
