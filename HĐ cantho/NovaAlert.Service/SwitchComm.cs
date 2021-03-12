using System;
using System.Collections.Generic;
using System.Linq;
using NovaAlert.Entities;
using NovaAlert.SwitchIC;
using NovaAlert.SwitchIC.PresentationLayer;
using NovaAlert.Communication.Base;
using NovaAlert.Common;
using NovaAlert.Common.Setting;

namespace NovaAlert.Service
{
    class SwitchComm : NovaAlert.SwitchIC.AppilcationLayer.Application, NovaAlert.Service.ISwitchComm
    {
        #region Events
        public event EventHandler<POStatusChangedEventArgs> OnPOStatusChanged;
        public event EventHandler<ChannelStatusChangedEventArgs> OnChannelStatusChanged;
        public event EventHandler<ChannelEventArgs> OnDialCompletedHandler;
        #endregion

        public string DataLinkStatus
        {
            get { return DataLinkBase.GetStatusText(this.Presentation.DataLink.Status); }
        }

        #region Ctor
        public SwitchComm(Presentation presentation)
            : base(presentation)
        {
            if(ClientSetting.Instance.IsInDebugMode)
            {
                this.Presentation.DataLink.OnRawDataReceive += DataLink_OnRawDataReceive;
                this.Presentation.DataLink.OnRawDataSend += DataLink_OnRawDataSend;
            }
        }
        #endregion

        #region Helpers
        public void SendConnection(SwitchConnection conn)
        {
            var scr = new ConnectionEnd()
            {
                Id = conn.Source.Address,
                Volumn = (byte)conn.Source.Volumn,
                Status = conn.Source.IsConnected ? eStatus.On : eStatus.Off
            };

            var dest = new ConnectionEnd()
            {
                Id = conn.Dest.Address,
                Volumn = (byte)conn.Dest.Volumn,
                Status = conn.Dest.IsConnected ? eStatus.On : eStatus.Off
            };

            var msg = new ConnectionMessage()
            {
                Source = scr,
                Dest = dest
            };
            this.Presentation.SendData(msg);
        }

        public void AdjustConnectionEndVolumn(SwitchConnectionEnd switchCN)
        {            
            var msg = new AdjVolMessage()
            {
                Id = switchCN.Address,
                Volumn = (byte)switchCN.Volumn
            };
            this.Presentation.SendData(msg);
        }

        protected override void OnLineStatusChanged(LineStatusMessageBase msg)
        {
            if (msg.Type == eLineStatusType.PO_Status || msg.Type == eLineStatusType.PO_Tone)
            {
                var e = new POStatusChangedEventArgs(msg.Address);
                if (msg.Type == eLineStatusType.PO_Status)
                {
                    e.Status = (((POStatusMessage)msg).Status == eStatus.On) ? ePOStatus.OffHook : ePOStatus.OnHook;
                }
                else
                {
                    e.Tone = ((NumberReceivedMessage)msg).Number;
                }

                if (this.OnPOStatusChanged != null)
                {
                    OnPOStatusChanged(this, e);
                }
            }
            else
            {
                var e = new ChannelStatusChangedEventArgs(msg.Address);

                switch (msg.Type)
                {
                    case eLineStatusType.Line:
                        var lmsg = msg as LineStatusMessage;
                        e.Status = lmsg.Status;
                        break;

                    case eLineStatusType.Revert:                        
                        break;

                    case eLineStatusType.Tone:
                    case eLineStatusType.CallerId:
                        var nmsg = msg as NumberReceivedMessage;
                        if (msg.Type == eLineStatusType.Tone)
                        {
                            e.Tone = nmsg.Number;
                        }
                        else
                        {
                            e.CallerId = nmsg.Number;
                        }
                        break;
                }

                if (this.OnChannelStatusChanged != null) OnChannelStatusChanged(this, e);
            }
        }

        public override void OnDialCompleted(SimpleMessage msg)
        {
            if (this.OnDialCompletedHandler != null)
            {
                OnDialCompletedHandler(this, new ChannelEventArgs(msg.Address));
            }
        }

        public void SendLoopStatus()
        {
            var msg = new LoopStatusMessage();
            foreach (var channel in CommonResource.Instance.Channels)
            {
                msg.Status[channel.Id - 1] = (channel.SelectedPanelId.HasValue) ? eStatus.On : eStatus.Off;
            }
            this.Presentation.SendData(msg);
        }

        public void Dial(byte lineId, string number)
        {
            var msg = new DialMessage(lineId, number);
            this.Presentation.SendData(msg);
        }

        public void ClearTone(byte lineId, bool clear)
        {
            var msg = new SimpleMessage(eControl.DeleteTone);
            msg.Data = (byte?)(clear ? 0x01 : 0x00);            
            this.Presentation.SendData(msg);
        }
        #endregion

        #region Conference
        public void SetConference(bool enable)
        {
            var msg = new SimpleMessage(eControl.Conference);
            msg.Data = (byte?)(enable ? 0x01 : 0x00);            
            this.Presentation.SendData(msg);
        }

        public void SendConferenceConnection(ConferenceConnection conn, bool isFirstOne = false)
        {
            var msg = new ConAddMessage();
            msg.AddOrRemove = conn.IsConnected ? eStatus.On : eStatus.Off;
            msg.Address = conn.Device.Address;
            msg.ConId = conn.ConferenceId;
            msg.InVolumn = (byte)conn.InVolumn;
            msg.OutVolumn = (byte)conn.OutVolumn;
            msg.IsFirstOne = isFirstOne ? eStatus.On : eStatus.Off;
            this.Presentation.SendData(msg);
        }

        public void AdjustConferenceVolumn(ConferenceConnection conn)
        {
            var msg = new ConAdjVolMessage();
            msg.Address = conn.Address;
            msg.InVolumn = (byte)conn.InVolumn;
            msg.OutVolumn = (byte)conn.OutVolumn;
            this.Presentation.SendData(msg);
        }        
        #endregion

        #region Ext Ring
        public void SendExtRingPower(bool ring)
        {
            var msg = new ExtRingMessage();
            msg.SetAllStatus(ring ? eStatus.On : eStatus.Off);
            this.Presentation.SendData(msg);
        } 
        #endregion

        #region Alarm
        public void SendAlarmConfig(bool enable)
        {
            //this.SendSimpleMessage(eControl.EnableAlarm, (byte?)(enable ? 1 : 0));
        }

        public void SendDayTypes(List<DayTypeConfig> list)
        {            
            var msg = new DayTypeDataMessage();
            for(int i = 0; i < msg.Days.Length; i++)
            {
                msg.Days[i] = new KeyValuePair<byte, byte>((byte)(list[i].DayOfWeek), list[i].DayType);
            }
            this.Presentation.SendData(msg);
        }

        public void SendAlarms(List<Alarm> list, List<RadioTime> rtList)
        {
            var groups = list.GroupBy(item => item.DayType).ToList();
            foreach(var g in groups)
            {
                var msg = new AlarmDataMessage();
                msg.Address = g.Key;
                for(int i = 0; i < msg.AlarmList.Length; i++)
                {
                    var alarm = g.Where(item => item.AlarmType == i + 1).FirstOrDefault();
                    if (alarm != null)
                    {
                        msg.AlarmList[i] = new AlarmData()
                        {
                            Period = (byte)(i + 1),                            
                            AlarmType = alarm.AlarmType,
                            Count = alarm.TimesOfPlaying,
                            Time = alarm.IsEnabled ? (DateTime?)alarm.Time : null
                        };
                    }
                }

                foreach(var item in rtList.Where(r => r.DayType == g.Key).OrderBy(r => r.ListOrder).ToList())
                {
                    if(item.IsEnabled)
                    {
                        msg.RadioTimes[item.ListOrder - 1].Start = item.StartTime;
                        msg.RadioTimes[item.ListOrder - 1].End = item.EndTime;
                    }
                    else
                    {
                        msg.RadioTimes[item.ListOrder - 1].Start = null;
                        msg.RadioTimes[item.ListOrder - 1].End = null;
                    }
                }

                this.Presentation.SendData(msg);
                System.Threading.Thread.Sleep(800);
            }
        }

        public void SendDateTime(DateTime dt)
        {
            var msg = new DateTimeMessage() { DateTime = dt };
            this.Presentation.SendData(msg);
        }
        #endregion

        public void SendAmplyPower(bool on)
        {
            SendSimpleMessage(eControl.AmplyPower, (byte)(on ? 1 : 0));
        }

        public void OnSystemDateTimeChanged(DateTime dt)
        {
            this.LastReceived = dt;
        }

        protected override void ProcessPresentationEvent(PresentationEventArgs e)
        {
            var msg = e.Data as MessageBase;

            switch (msg.Control)
            {
                case eControl.LineStatus:
                    OnLineStatusChanged(msg as LineStatusMessageBase);
                    break;

                case eControl.AllStatus:
                    OnUpdateAllStatus(msg as AllStatusMessage);
                    break;

                case eControl.DialCompleted:
                    OnDialCompleted(msg as SimpleMessage);
                    break;

                // Led Panel
                case eControl.LP_ACK:
                    break;

                case eControl.LP_NAK:
                    break;

                case eControl.LP_Update:
                    var ack = new SimpleMessage(eControl.LP_ACK) { TypeDest = eDevice.LedPanel, Address = msg.Address };
                    this.Presentation.SendData(ack);

                    System.Threading.Thread.Sleep(50);
                    SerialComm.SendResults(this.Presentation, msg.Address, eTaskType.CTT);
                    break;
            }

            this.LastReceived = DateTime.Now;
        }

        #region LogData
        private void DataLink_OnRawDataSend(object sender, DataLinkEventArg e)
        {
            Mediator.NotifyColleagues(Mediator_Message.SwitchComm_Send, e.Data);
        }

        private void DataLink_OnRawDataReceive(object sender, DataLinkEventArg e)
        {
            Mediator.NotifyColleagues(Mediator_Message.SwitchComm_Rcv, e.Data);
        }
        #endregion
    }
}
