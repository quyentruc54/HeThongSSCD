using NovaAlert.Entities.ViewModel;
using System;

namespace NovaAlert.Bll.Controller
{
    public interface ICallController
    {        
        void Dial();
        void PutOnHold();
        bool IsHolding { get; }
        void Resume();
        void Finalise();
        DateTime StartTime { get; }
        DateTime? FinalisedDate { get; }
        eCallStatus CallStatus { get; }
        bool IsRecording { get; }
        bool AutoRecording { get; }
        string GetDialingNumber();
        bool CanHold();
    }

    public interface IConferenceMember
    {
        DateTime StartTime { get; }
        UnitPhoneViewModel Unit { get; }
        HostPhoneViewModel Channel { get; }
        DateTime? FinalisedDate { get; }
        ClientAppViewModel ClientApp { get; }
        bool AutoRecording { get; }
    }
}
