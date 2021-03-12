using NovaAlert.Communication.ATModem;
using NovaAlert.SwitchIC;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NovaAlert.Common;

namespace NovaAlert.Service.TSL
{
    public class TSL_Parser: Parser
    {
        string _buffer = string.Empty;
        const string _pattern = @"({.*?})";        

        public TSL_Parser(IModem modem):base(modem)
        {

        }
        protected override void ParseReceiveData(Communication.Base.DataLinkEventArg e)
        {
            LogService.Logger.Info(string.Format("Receive data: {0}", e.Data));

            _buffer += e.Data;
            var msg = GetNextCommand(ref _buffer);

            while (msg != null)
            {
                var msgObj = Parse(msg);
                if(msgObj != null)
                {
                    ProcessMessage(msgObj);
                }
                    
                msg = GetNextCommand(ref _buffer);
            }
        }

        protected virtual void ProcessMessage(object msgObj)
        {            
            var m = this.Modem as TSL_Modem;
            if (m != null) m.ParseTSLMessage(msgObj);
        }

        protected string GetNextCommand(ref string data)
        {
            var match = Regex.Match(data, _pattern);
            if (!match.Success) return null;

            var next = match.Index + match.Length + 1;
            if (data.Length < next) data = string.Empty;
            else data = data.Substring(next);
            return match.Value;            
        }

        object Parse(string s)
        {
            object msg = null;

            try
            {
                if (s.IndexOf("UnitName") >= 0 && s.IndexOf("Task") >= 0 && s.IndexOf("Level") >= 0)
                {
                    msg = JsonConvert.DeserializeObject<Entities.ResultData>(s);
                }
                else if(s.IndexOf("Control") >= 0)
                {
                    msg = JsonConvert.DeserializeObject<TSL_Message>(s);
                }
            }
            catch 
            {
            }
            
            return msg;
        }

        public override void ClearBuffer()
        {
            base.ClearBuffer();
            _buffer = string.Empty;
        }
    }
}
