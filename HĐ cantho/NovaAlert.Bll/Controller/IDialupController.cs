using NovaAlert.Communication.ATModem;

namespace NovaAlert.Bll.Controller
{
    public enum eDialupStatus
    {
        None,
        Connected,
        FailToConnect,
        NotResponse,
        Canceled,
        Done
    }
    public interface IDialupController
    {
        eDialupStatus Status { get; }
        IModem Modem { get; }
        TSL_ALertUnitPhoneViewModel Unit { get; }
        bool IsBusy { get; }
        void Start();
        void Cancel();
        //event EventHandler OnCompleted;
        int Trys { get; }
    }
}
