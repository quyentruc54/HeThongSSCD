using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovaAlert.Communication.Base;

namespace NovaAlert.SwitchIC.PresentationLayer
{
    public class Presentation: PresentationBase
    {
        const int TimerInterval = 10;
        System.Threading.Timer _timer;
        internal Queue<DataToSend> Queue { get; private set; }

        public Presentation(DataLinkLayer.DataLink dataLink):base(dataLink)
        {
            this.Queue = new Queue<DataToSend>();

            _timer = new System.Threading.Timer(OnTimer, this, 10, TimerInterval);
        }

        void OnTimer(object state)
        { 
            lock(this.Queue)
            {
                if(this.Queue.Count > 0)
                {
                    var data = this.Queue.Dequeue();
                    this.DataLink.SendData(data.Data, true);
                }
            }
        }

        //void SendNextMessage()
        //{
        //    if (this.Queue.Count == 0) return;
        //    var data = this.Queue.Peek();
        //    data.TryCount += 1;
        //    this.DataLink.SendData(data.Data, true);
        //}

        public override void SendData(object msg, bool queue = true)
        {
            if (queue)
            {
                lock (this.Queue)
                {
                    var data = new DataToSend(msg);
                    this.Queue.Enqueue(data);
                    //if (this.Queue.Count == 1) SendNextMessage();
                }
            }
            else
            {
                this.DataLink.SendData(msg, true);
            }
        }

        void OnDeviceReply(bool ack)
        {
            //if(this.Queue.Count == 0) return;
            //if (ack)
            //    this.Queue.Dequeue();
            //else
            //{
            //    var data = this.Queue.Peek();
            //    if (data.TryCount > 3) this.Queue.Dequeue();
            //}
            //SendNextMessage();
        }

        public void SendSimpleMessage(eControl ctrl, byte? data = null, bool queue = false)
        {
            SendData(new SimpleMessage(ctrl, data), queue);
        }

        protected override void ProcessDataLinkEvent(DataLinkEventArg e)
        {
            var msg = MessageBase.Parse(e.Data);
            if (msg == null)
            {                
                return;
            }

            if (msg is SimpleMessage)
            {
                var simpleMsg = msg as SimpleMessage;

                if (msg.Control == eControl.ACK) 
                    OnDeviceReply(true);
                else if (msg.Control == eControl.NAK) 
                    OnDeviceReply(false);
            }

            RaiseEvent(new PresentationEventArgs(msg));
        }
    }
}
