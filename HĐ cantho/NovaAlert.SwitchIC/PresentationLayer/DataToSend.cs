using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.SwitchIC.PresentationLayer
{
    class DataToSend
    {
        public object Data { get; set; }
        public byte TryCount { get; set; }

        public DataToSend(object obj)
        {
            if (obj is MessageBase)
                this.Data = ((MessageBase)obj).ToBytes();
            else if (obj is byte[])
                this.Data = obj;
            else 
                this.Data = obj.ToString();

            this.TryCount = 0;
        }
    }
}
