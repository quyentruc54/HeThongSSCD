namespace NovaAlert.Communication.ATModem
{
    public interface IRingListener
    {
        ICallHandler Ring(IModem modem);
    }
}
