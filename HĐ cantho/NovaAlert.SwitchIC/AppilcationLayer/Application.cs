using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovaAlert.Communication.Base;
using NovaAlert.SwitchIC.PresentationLayer;

namespace NovaAlert.SwitchIC.AppilcationLayer
{
    public class Application: ApplicationBase
    {
        public DateTime LastReceived { get; protected set; }
        public Application(PresentationLayer.Presentation presentation):base(presentation)
        {
        }

        public void SendSimpleMessage(eControl ctrl, byte? data = null)
        {
            this.Presentation.SendData(new SimpleMessage(ctrl, data), true);
        }        

        protected override void ProcessPresentationEvent(PresentationEventArgs e)
        {
            var msg = e.Data as MessageBase;

            switch (msg.Control)
            {
                case eControl.LineStatus:
                    OnLineStatusChanged(msg as LineStatusMessageBase);
                    break;

                case eControl.AllStatus:
                    OnUpdateAllStatus(msg as AllStatusMessage);
                    break;

                case eControl.DialCompleted:
                    OnDialCompleted(msg as SimpleMessage);
                    break;
            }

            this.LastReceived = DateTime.Now;
        }

        protected virtual void OnLineStatusChanged(LineStatusMessageBase msg)
        {
            switch (msg.Type)
            {
                case eLineStatusType.Line:
                    break;

                case eLineStatusType.Revert:
                    break;

                case eLineStatusType.Tone:
                    break;

                case eLineStatusType.PO_Status:
                    break;

                case eLineStatusType.PO_Tone:
                    break;
            }            
        }

        protected virtual void OnUpdateAllStatus(AllStatusMessage msg)
        {
        }

        public virtual void OnDialCompleted(SimpleMessage msg)
        {
        }
    }
}
