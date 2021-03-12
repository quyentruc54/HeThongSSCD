using NovaAlert.Common;
using NovaAlert.Common.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NovaAlert.Service
{
    public partial class NovaAlertService
    {
        const int MaxIdleInterval = 20;
        int _counter1 = 0;
        int _counter2 = 0;
        int _counter3 = 0;
        int _counter4 = 0;

        void StopTimer()
        {
            _timer.Change(0, 0);
        }

        void StartTimer()
        {
            _counter1 = 0;
            _counter2 = 0;
            _counter3 = 0;
            _counter4 = 0;
            _timer.Change(1000, 1000);
        }

        #region Timer        

        void OnTimer(object state)
        {
            try
            {
                if (this.IsRinging)
                {
                    if (_counter2 == 0)
                    {
                        Switch.SendExtRingPower(true);
                    }
                    else 
                    {
                        if (_counter2 == 3)
                        {
                            Switch.SendExtRingPower(false);
                        }                            
                    }

                    _counter2++;
                    if (_counter2 > 3)
                    {
                        _counter2 = 0;
                    }
                }
                else
                {
                    _counter2 = 0;
                }

                _counter3++;
                if (_counter3 > 10)
                {
                    // Poll bang den
                    if (GlobalSetting.Instance.UseSwitchPortForLP == false)
                    {
                        SerialComm.LPComm.PollAll(false, null);
                    }

                    if (GlobalSetting.Instance.AlarmOnSwitch == false)
                    {
                        lock (_alarmManager)
                        {
                            // ktra bao hieu
                            if (!_alarmManager.IsPlaying)
                            {
                                _alarmManager.CheckAlarm(DateTime.Now);
                            }
                        }
                    }

                    _counter3 = 0;
                }

                if (GlobalSetting.Instance.UseSwitchPortForLP == false)
                {
                    _counter4++;
                    if (_counter4 > 30)
                    {
                        // Cap nhat kq xuong bang den
                        SerialComm.LPComm.SendAllResults(0);
                        _counter4 = 0;
                    }
                }

                _counter1++;
                if (_counter1 >= MaxIdleInterval)
                {
                    _counter1 = 0;
                    var now = DateTime.Now;
                    CheckClientsTimeOut(now);
                    CheckSwitchConnection(now);
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnTimer", ex);
            }
        }

        private void CheckSwitchConnection(DateTime now)
        {
            bool isConnected = (now - this.Switch.LastReceived).TotalSeconds <= MaxIdleInterval;
            var disconnectedClients = new List<Client>();
            foreach (var c in CommonResource.Instance.Clients)
            {
                try
                {
                    c.Callback.OnSwitchStatusChanged(isConnected);
                }
                catch
                {
                    disconnectedClients.Add(c);
                }
            }

            disconnectedClients.ForEach(OnClientDisconnected);
        }

        private void CheckClientsTimeOut(DateTime now)
        {
            CommonResource.Instance.Clients
                .Where(c => c.LastAction == null || (now - c.LastAction.Value).TotalSeconds > MaxIdleInterval)
                .ToList().ForEach(OnClientDisconnected);            
        }
        #endregion

        #region Recorder checking
        bool _isResettingRecorder = false;

        void Recorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsTimeOut") return;

            try
            {
                if (this.Recorder.IsTimeOut)
                {
                    foreach (var c in CommonResource.Instance.Clients)
                    {
                        c.Callback.OnServerMessage(Entities.eServerMessageType.RecorderError, "Mất tín hiệu từ khối ghi âm.");
                    }
                    Action resetAction = new Action(ResetUSBRecorder);
                    resetAction.BeginInvoke(null, null);
                }
                else
                {
                    foreach (var c in CommonResource.Instance.Clients)
                    {
                        c.Callback.OnServerMessage(Entities.eServerMessageType.RecorderResume, "Đã nhận được tín hiệu từ khối ghi âm.");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        void ResetUSBRecorder()
        {
            if (_isResettingRecorder) return;

            _isResettingRecorder = true;
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    this.Switch.SendSimpleMessage(SwitchIC.eControl.ResetUSB);
                    System.Threading.Thread.Sleep(12000);
                    if (this.Recorder.IsTimeOut == false) break;
                }
            }
            catch
            {
            }
            finally
            {
                _isResettingRecorder = false;
            }
        }
        #endregion
    }
}
