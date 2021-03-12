using System;

namespace NovaAlert.Communication.ATModem
{
    public interface IModemListener
    {
        //bool ring(ModemEvent @event);
        //bool connected(ModemEvent @event, Connection conn);
        //void dialFailed(ModemEvent @event);
        //void modemAnswerTimeout(ModemEvent @event);

        event EventHandler<ModemEventArgs> Ring;
        event EventHandler<ModemEventArgs> Connected;
        event EventHandler<ModemEventArgs> DialFailed;
        event EventHandler<ModemEventArgs> AnswerTimeout;
    }

    public class ModemEventArgs: EventArgs
    {
        public IConnection Connection { get; private set; }
        public ModemEventArgs(IConnection con)
        {
            this.Connection = con;
        }
    }
}
