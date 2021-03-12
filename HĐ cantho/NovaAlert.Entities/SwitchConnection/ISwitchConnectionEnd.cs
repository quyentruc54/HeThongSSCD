namespace NovaAlert.Entities
{
    public interface ISwitchConnectionEnd
    {
        byte Address { get; }
        eVolumn Volumn { get; set; }
        bool IsConnected { get; set; }
        void Update(eVolumn volumn, bool isConnected);
    }
}
