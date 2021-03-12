using System;
using System.Collections.Generic;

namespace NovaAlert.Service.Fake
{
    public class FakeSwitch: ISwitchComm
    {
        void ISwitchComm.AdjustConferenceVolumn(Entities.ConferenceConnection conn)
        {
            
        }

        void ISwitchComm.AdjustConnectionEndVolumn(Entities.SwitchConnectionEnd switchCN)
        {
            
        }

        void ISwitchComm.ClearTone(byte lineId, bool clear)
        {
            
        }

        void ISwitchComm.Dial(byte lineId, string number)
        {
            
        }

        event EventHandler<Entities.ChannelStatusChangedEventArgs> ISwitchComm.OnChannelStatusChanged
        {
            add {  }
            remove {  }
        }

        void ISwitchComm.OnDialCompleted(SwitchIC.PresentationLayer.SimpleMessage msg)
        {
            
        }

        event EventHandler<Entities.ChannelEventArgs> ISwitchComm.OnDialCompletedHandler
        {
            add {  }
            remove {  }
        }

        event EventHandler<Entities.POStatusChangedEventArgs> ISwitchComm.OnPOStatusChanged
        {
            add {  }
            remove {  }
        }

        void ISwitchComm.SendConferenceConnection(Entities.ConferenceConnection conn, bool isFirstOne)
        {
            
        }

        void ISwitchComm.SendConnection(Entities.SwitchConnection conn)
        {
            
        }

        void ISwitchComm.SendExtRingPower(bool ring)
        {
            
        }

        void ISwitchComm.SendLoopStatus()
        {            
        }

        void ISwitchComm.SetConference(bool enable)
        {            
        }

        void ISwitchComm.SendSimpleMessage(SwitchIC.eControl ctrl, byte? data)
        {            
        }
        void ISwitchComm.SendAlarmConfig(bool enable)
        {
        }


        void ISwitchComm.SendDayTypes(List<Entities.DayTypeConfig> list)
        {            
        }

        void ISwitchComm.SendAlarms(List<Entities.Alarm> list, List<Entities.RadioTime> rtList)
        {            
        }

        void ISwitchComm.SendDateTime(DateTime dt)
        {            
        }


        void ISwitchComm.SendAmplyPower(bool on)
        {
            //throw new NotImplementedException();
        }


        DateTime ISwitchComm.LastReceived
        {
            get { return DateTime.Now.AddSeconds(-10); }
        }

        void ISwitchComm.OnSystemDateTimeChanged(DateTime dt)
        {

        }
        
        string ISwitchComm.DataLinkStatus
        {
            get { return string.Empty; }
        }
    }

    public class FakeLP: ILPComm
    {

        void ILPComm.PollAll(bool forceSend, byte? id)
        {
            
        }

        void ILPComm.SendResults(byte address)
        {
            
        }

        string ILPComm.DataLinkStatus
        {
            get { return string.Empty; }
        }

        public void SendAllResults(int phoneNumberId) { }
    }
}
