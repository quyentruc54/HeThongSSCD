using NovaAlert.Entities;
using NovaAlert.SwitchIC;
using System;
using System.Collections.Generic;

namespace NovaAlert.Service
{
    public interface ISwitchComm
    {
        void AdjustConferenceVolumn(NovaAlert.Entities.ConferenceConnection conn);
        void AdjustConnectionEndVolumn(NovaAlert.Entities.SwitchConnectionEnd switchCN);
        void ClearTone(byte lineId, bool clear);
        void Dial(byte lineId, string number);
        event EventHandler<NovaAlert.Entities.ChannelStatusChangedEventArgs> OnChannelStatusChanged;
        void OnDialCompleted(NovaAlert.SwitchIC.PresentationLayer.SimpleMessage msg);
        event EventHandler<NovaAlert.Entities.ChannelEventArgs> OnDialCompletedHandler;
        event EventHandler<NovaAlert.Entities.POStatusChangedEventArgs> OnPOStatusChanged;
        void SendConferenceConnection(NovaAlert.Entities.ConferenceConnection conn, bool isFirstOne = false);
        void SendConnection(NovaAlert.Entities.SwitchConnection conn);
        void SendExtRingPower(bool ring);
        void SendLoopStatus();
        void SetConference(bool enable);

        void SendSimpleMessage(eControl ctrl, byte? data = null);
        void SendAlarmConfig(bool enable);
        void SendDayTypes(List<DayTypeConfig> list);
        void SendAlarms(List<Alarm> list, List<RadioTime> rtList);
        void SendDateTime(DateTime dt);
        void SendAmplyPower(bool on);
        DateTime LastReceived { get; }
        void OnSystemDateTimeChanged(DateTime dt);

        string DataLinkStatus { get; }
    }
}
