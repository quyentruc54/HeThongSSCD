using System.Collections.Generic;

namespace NovaAlert.Communication.ATModem
{
    public class ResultCodeToken
    {
        public const string BUSY = "BUSY";
        public const string OK = "OK";
        public const string CONNECT = "CONNECT";
        public const string NO_ANSWER = "NO ANSWER";
        public const string NO_DIALTONE = "NO DIALTONE";
        public const string NO_CARRIER = "NO CARRIER";
        public const string ERROR = "ERROR";
        public const string RING = "RING";

        private readonly string resultCode;
        private readonly List<string> data;
        private readonly string echo;

        public ResultCodeToken(string resultCode, List<string> data, string echo)
        {
            this.resultCode = resultCode;
            this.data = data;
            this.echo = echo;
        }

        public virtual List<string> Data
        {
            get
            {
                return data;
            }
        }

        public virtual bool Connect
        {
            get
            {
                return CONNECT.Equals(resultCode);
            }
        }

        public virtual bool Ok
        {
            get
            {
                return OK.Equals(resultCode);
            }
        }

        /// <returns> the resultCode </returns>
        public virtual string ResultCode
        {
            get
            {
                return resultCode;
            }
        }

        /// <returns> the echo </returns>
        public virtual string Echo
        {
            get
            {
                return echo;
            }
        }
    }
}
