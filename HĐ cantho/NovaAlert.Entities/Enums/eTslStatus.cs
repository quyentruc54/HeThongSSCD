namespace NovaAlert.Entities
{
    public enum eTslStatus
    {
        None = 0,
        InProgress = 1,
        Ready = 2,
        CanNotConnected = 3,
        NotResponsed = 4,
        Canceled
    }

    public enum eTslStatusType
    {
        Prepare = 1,
        Result = 2
    }

    public enum eServerMessageType
    {
        Notify,
        Error,
        RecorderError,
        RecorderResume
    }
}
