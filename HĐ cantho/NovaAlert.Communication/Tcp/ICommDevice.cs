namespace NovaAlert.Communication.Tcp
{
    public interface ICommDevice
    {
        int Port { get; }
        void UpdateData(byte[] data);
    }
}
