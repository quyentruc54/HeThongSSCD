using NovaAlert.Communication.Base;
using NovaAlert.Entities;
using NovaAlert.SwitchIC;
using NovaAlert.SwitchIC.PresentationLayer;
using System;

namespace NovaAlert.Service
{
    public interface ILPComm
    {
        void PollAll(bool forceSend, byte? id);
        void SendResults(byte address);
        string DataLinkStatus { get; }
        void SendAllResults(int phoneNumberId);
    }

    public class LPComm : NovaAlert.SwitchIC.AppilcationLayer.Application, ILPComm
    {
        public const byte MaxPanelId = 1;
        public DateTime? LastSentTime { get; private set; }

        object _syncObj = new object();
        public string DataLinkStatus
        {
            get { return DataLinkBase.GetStatusText(this.Presentation.DataLink.Status); }
        }

        public LPComm(NovaAlert.SwitchIC.PresentationLayer.Presentation presentation)
            : base(presentation)
        {
            this.LastSentTime = null;
        }

        protected override void ProcessPresentationEvent(Communication.Base.PresentationEventArgs e)
        {
            var msg = e.Data as MessageBase;

            switch (msg.Control)
            {
                case eControl.LP_ACK:
                    break;

                case eControl.LP_NAK:
                    break;

                case eControl.LP_Update:                    
                    var ack = new SimpleMessage(eControl.LP_ACK) { TypeDest = eDevice.LedPanel, Address = msg.Address };
                    this.Presentation.SendData(ack);

                    System.Threading.Thread.Sleep(50);
                    SendResults(msg.Address);
                    break;
            }
        }

        public void PollAll(bool forceSend = false, byte? id = null)
        {
            if(forceSend || this.LastSentTime == null || (DateTime.Now - this.LastSentTime.Value).TotalSeconds > 10)
            {
                if (id.HasValue)
                {
                    SendPollMessage(id.Value);
                }
                else
                {
                    for (byte i = 0; i <= MaxPanelId; i++)
                    {
                        SendPollMessage((byte)(i + 1));
                        System.Threading.Thread.Sleep(50);
                    }
                }                
            }
        }
        
        void SendPollMessage(byte address)
        {
            var msg = new SimpleMessage(eControl.LP_Polling) { TypeDest = eDevice.LedPanel, Address = address };
            this.Presentation.SendData(msg);
        }

        public void SendResults(byte address)
        {
            SerialComm.SendResults(this.Presentation, address, eTaskType.CTT);
        }

        public void SendAllResults(int phoneNumberId)
        {
            Action<IPresentation, eTaskType, int> act = new Action<IPresentation, eTaskType, int>(SerialComm.SendAllResults);
            act.BeginInvoke(this.Presentation, eTaskType.CTT, phoneNumberId, null, null);
        }
    }
}
