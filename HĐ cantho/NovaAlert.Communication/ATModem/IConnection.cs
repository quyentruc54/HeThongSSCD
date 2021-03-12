namespace NovaAlert.Communication.ATModem
{
    public interface IConnection
    {
        IModem Modem { get; }

        //Stream InputStream { get; }

        //Stream OutputStream { get; }

        void Disconnect();

        void ExitDataMode();

        void ReenterDataMode();

        bool Connected { get; }

        bool OnlineDataMode { get; }

        bool OnlineCommandMode { get; }
        void SendData(string data);
        //void ReceiveData(string data);
    }
}
