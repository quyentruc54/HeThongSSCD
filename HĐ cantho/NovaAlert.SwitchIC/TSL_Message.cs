using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.SwitchIC
{
    public class TSL_Message
    {
        public eControl Control { get; set; }
    }

    public class TSL_ResultMessage
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte Task { get; set; }
        public byte Level { get; set; }
        public byte Result { get; set; }
        public int Duration { get; set; }
    }
}
