using System;

namespace NovaAlert.Server.Models
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }
    }

}
