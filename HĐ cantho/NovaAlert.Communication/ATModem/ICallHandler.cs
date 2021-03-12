namespace NovaAlert.Communication.ATModem
{
    public interface ICallHandler
    {
        void ParsedResultCode(IModem modem, ResultCodeToken r);

        void Connect(IModem modem, IConnection connection);

        bool Ring(IModem modem);

        void Dialed(IModem modem, string number);

        void Answered(IModem modem);
    }
}
