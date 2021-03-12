namespace NovaAlert.Communication.ATModem
{
    public interface IExtension
    {
        IModem Modem { get; set; }
    }

    public class GsmExtention: IExtension
    {
        public IModem Modem { get; set; }
    }

    public class FaxExtention : IExtension
    {
        public IModem Modem { get; set; }
    }

    public class VoiceExtention : IExtension
    {
        public IModem Modem { get; set; }
    }
}
