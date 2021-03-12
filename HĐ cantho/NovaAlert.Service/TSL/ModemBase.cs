using NovaAlert.SwitchIC;

namespace NovaAlert.Service.TSL
{
    public abstract class ModemBase
    {
        public const int TSL_WaitTime = 10000;
        public TSL_Modem Modem { get; private set; }
        public ModemBase(TSL_Modem modem)
        {
            this.Modem = modem;
            this.Modem.ClearBuffer();
        }

        public bool Available
        {
            get { return this.Modem != null && this.Modem.Idle; }
        }

        protected void SendMessage(eControl ctrl)
        {
            var msg = new TSL_Message() { Control = ctrl };
            this.Modem.SendMessage(msg);
        }
    }
}
