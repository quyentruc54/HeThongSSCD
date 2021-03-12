namespace NovaAlert.Entities
{
    public interface IClientApp
    {
        byte ClientId { get; }
        eClientType ClientType { get; }
        eWorkingMode WorkingMode { get; set; }
        bool IsMultiDestMode { get; }
        bool IsAlertMode { get; }
        bool IsAlarmMode { get; }
        INovaAlertService Service { get; }

        IMediaPlayer MediaPlayer { get; }
        void Reload();
        void AddLog(string info, bool showInfo);
        void ShowMessage(string title, string text, bool isWarning);
        void ShowInfo(string msg);
        void ShowError(string msg);
        void ShowHelp(string msg);
        void Refesh();
        void RefeshTitle();
    }    
}
