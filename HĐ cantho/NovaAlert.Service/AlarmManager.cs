using NovaAlert.Common;
using NovaAlert.Dal;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;

namespace NovaAlert.Service
{
    public class AlarmManager
    {
        #region Singleton
        static AlarmManager _instance;
        public static AlarmManager Instance
        {
            get
            {
                if (_instance == null) _instance = new AlarmManager();
                return _instance;
            }
        }

        private AlarmManager()
        {
            this.MainSpeakerConnection = new SwitchConnection(0, new SwitchConnectionEnd(OtherDevice.SoundChannels[SoundChannelId]), new SwitchConnectionEnd(OtherDevice.Amply));
            this.OtherSpeakerConnections = new SwitchConnection[OtherDevice.Speakers.Length];
            for (int i = 0; i < OtherDevice.Speakers.Length; i++)
            {
                this.OtherSpeakerConnections[i] = new SwitchConnection(0, new SwitchConnectionEnd(OtherDevice.SoundChannels[SoundChannelId]), new SwitchConnectionEnd(OtherDevice.Speakers[i]));
            }
        } 
        #endregion

        public SwitchConnection MainSpeakerConnection {get; private set;}
        public SwitchConnection[] OtherSpeakerConnections { get; private set; }
        const int SoundChannelId = 0;

        bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set 
            {
                if(_isPlaying != value)
                {
                    _isPlaying = value;
                    SetConnection(_isPlaying);
                }                
            }
        }

        public void CheckAlarm(DateTime now, bool testOnly = false)
        {
            if (this.IsPlaying)
            {
                return;
            }

            var dayOfWeek = (byte)(now.DayOfWeek + 1);
            var dt = NovaAlertCommon.Instance.GetDayType(dayOfWeek);
            if (dt == null)
            {
                return;
            }

            var list = NovaAlertCommon.Instance.AlarmList.Where(a => a.DayType == dt.DayType).ToList();

            var alarm = list.Where(a => a.IsEnabled && now.Hour == a.Time.Hour && now.Minute == a.Time.Minute && 
                Math.Abs(now.Second - a.Time.Second) < 10).FirstOrDefault();
            
            if(alarm != null && alarm.TimesOfPlaying > 0)
            {
                if (IsAlarmPlayed(alarm.AlarmType, now, testOnly))
                {
                    return;
                }

                AddAlarmLog(alarm.AlarmType, now, testOnly);
                var t = NovaAlertCommon.Instance.GetEnum("AlarmType", alarm.AlarmType);
                if(t != null)
                {
                    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sound", t.Desc);
                    if(File.Exists(fileName))
                    {
                        SetConnection(true);
                        Action<string, int> act = new Action<string, int>(StartAlarm);
                        act.BeginInvoke(fileName, alarm.TimesOfPlaying, OnFinishAlarm, null);
                    }
                    else
                    {
                        LogService.Logger.Error(string.Format("Khong tim thay file {0}", fileName));
                    }
                }
            }
        }

        void SetConnection(bool isConnected)
        {
            this.MainSpeakerConnection.Source.IsConnected = isConnected;
            this.MainSpeakerConnection.Dest.IsConnected = false;
            SerialComm.SwitchComm.SendConnection(this.MainSpeakerConnection);

            for (int i = 0; i < this.OtherSpeakerConnections.Length; i++ )
            {
                var conn = this.OtherSpeakerConnections[i];
                conn.Source.IsConnected = isConnected;
                conn.Dest.IsConnected = false;
                SerialComm.SwitchComm.SendConnection(conn);
            }
            
            SerialComm.SwitchComm.SendAmplyPower(isConnected);
        }

        void StartAlarm(string fileName, int count)
        {
            this.IsPlaying = true;

            // Cho bat amply, 10 giay
            System.Threading.Thread.Sleep(10000);

            var sc = CommonResource.Instance.SoundChannels[SoundChannelId];

            for (int i = 0; i < count; i++)
            {
                sc.PlayFile(fileName);

                while (sc.IsPlaying)
                {                    
                    System.Threading.Thread.Sleep(1000);
                }                
            }

            this.IsPlaying = false;
        }

        void OnFinishAlarm(IAsyncResult ar)
        {
            
        }

        bool IsAlarmPlayed(byte alarmType, DateTime date, bool testOnly)
        {
            if (testOnly)
            {
                return false;
            }

            using (var ctx = new NovaAlertContext())
            {
                return ctx.AlarmLogs.Where(a => a.AlarmType == alarmType &&
                    SqlFunctions.DateDiff("day", a.AlarmTime, date) == 0).Count() > 0;
            }
        }

        void AddAlarmLog(byte alarmType, DateTime date, bool testOnly)
        {
            if (testOnly)
            {
                LogService.Logger.Debug(string.Format("AddAlarm({0},{1})", alarmType, date.ToString("HH:mm:ss")));
                return;
            }

            using (var ctx = new NovaAlertContext())
            {
                var al = new AlarmLog() { AlarmType = alarmType, AlarmTime = date };
                ctx.AlarmLogs.Add(al);
                ctx.SaveChanges();
            }
        }
    }
}
