using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using NovaAlert.Common;
using NovaAlert.Dal;
using NovaAlert.Entities;
using NovaAlert.SwitchIC;
using System.IO;
using NovaAlert.Common.Utils;
using NovaAlert.UsbRecorder;
using NovaAlert.Common.Setting;
using System.Threading;

namespace NovaAlert.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class NovaAlertService : INovaAlertService, IDisposable
    {
        #region Members & Properties
        private CommonResource _resources = CommonResource.Instance;
        private System.Threading.Timer _timer;
        private string _soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sound");
        private string _recordDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Record");

        internal ISwitchComm Switch { get; private set; }
        public IWaveRecorder Recorder { get { return RecorderManager.Recorder; } }
        private bool _isRinging = false;
        public bool IsRinging
        {
            get
            {
                return _isRinging;
            }
            set
            {
                if (value != _isRinging)
                {
                    _isRinging = value;
                    if (!_isRinging) Switch.SendExtRingPower(false);
                }
            }
        }

        AlarmManager _alarmManager;
        object _syncObj = new object();
        #endregion

        #region Ctor
        public NovaAlertService()
        {
            _resources.OnResourceChanged += _resources_OnResourceChanged;
            InitSwitchComm();

            _alarmManager = AlarmManager.Instance;
            _timer = new System.Threading.Timer(OnTimer, this, 1000, 1000);

            this.Recorder.PropertyChanged += Recorder_PropertyChanged;
            LogService.Logger.Info("NovaAlert initialize component success");
        }
        #endregion

        #region Service Operations
        void INovaAlertService.Subscribe(int clientId, eClientType type)
        {
            // Check if exist then unsubscribe
            ((INovaAlertService)this).UnSubscribe(clientId, type);

            // Register new client
            INovaAlertServiceCallback callback = OperationContext.Current.GetCallbackChannel<INovaAlertServiceCallback>();
            var client = new Client(clientId) { Type = type, Callback = callback, LastAction = DateTime.Now };
            _resources.Clients.Add(client);
            _resources.Actions.Add(client, eAction.Subscribe);

            client.PropertyChanged += client_PropertyChanged;

            AddLog(client, "Kết nối");
        }

        void INovaAlertService.UnSubscribe(int clientId, eClientType type)
        {
            var existClient = _resources.Clients.FirstOrDefault(c => c.Type == type && c.Id == clientId);
            if (existClient != null)
            {
                _resources.Actions.Add(existClient, eAction.Unsubscribe);
                OnClientDisconnected(existClient);
                AddLog(existClient, "Ngắt kết nối");
            }
        }

        void INovaAlertService.UpdateAlertSoundStatus(int clientId, eAlertSoundStatus status)
        {
            var client = _resources.GetActiveClients().FirstOrDefault(c => c.Id == clientId);
            if (client != null)
            {
                client.AlertSoundStatus = status;
            }
        }

        void INovaAlertService.Request(eResourceType resourceType, int resourceId)
        {
            var client = GetClient();
            if (client == null)
            {
                return;
            }

            lock (_syncObj)
            {
                foreach (var re in _resources.GetResource(resourceType, resourceId))
                {
                    if (re.SelectedPanelId == null)
                    {
                        re.SelectedPanelId = (byte)client.Id;
                        client.LastAction = DateTime.Now;
                    }

                    if (re.ResourceType == eResourceType.Channel)
                    {
                        Switch.SendLoopStatus();
                    }
                }
            }
        }

        void INovaAlertService.Release(eResourceType resourceType, int resourceId)
        {
            var client = GetClient();

            lock (_syncObj)
            {
                var list = _resources.GetResource(resourceType, resourceId);

                if (client != null)
                {
                    list = list.Where(r => r.SelectedPanelId == (byte)client.Id).ToList();
                }

                foreach (var re in list)
                {
                    re.SelectedPanelId = null;
                    if (client != null)
                    {
                        client.LastAction = DateTime.Now;
                    }

                    if (re.ResourceType == eResourceType.Channel)
                    {
                        Switch.SendLoopStatus();
                    }
                }
            }
        }

        List<UnitPhone> INovaAlertService.GetUnitPhones(int? groupId, eTaskType taskType, bool includeTask)
        {
            if (groupId.HasValue)
            {
                List<UnitPhone> list = new List<UnitPhone>();
                var ids = NovaAlertCommon.GetUnitsInGroup(groupId.Value);
                foreach (var u in _resources.UnitPhones)
                {
                    var g = ids.FirstOrDefault(gu => gu.UnitId == u.Id);
                    if (g != null)
                    {
                        var up = new UnitPhone(u.Id, u.Name)
                        {
                            PhoneNumberId = u.PhoneNumberId,
                            AreaCode = u.AreaCode,
                            Number = u.Number,
                            Password = u.Password,
                            ListOrder = g.ListOrder,
                        };

                        if (includeTask)
                        {
                            up.Task = NovaAlertCommon.GetLastestTask(u.PhoneNumberId, taskType);
                        }

                        list.Add(up);
                    }
                }
                return list;
            }
            else
            {
                return _resources.UnitPhones.Select(vm => vm.Phone).OfType<UnitPhone>().ToList();
            }
        }

        List<UnitPhone> INovaAlertService.GetTSLUnitPhones()
        {
            List<UnitPhone> list = new List<UnitPhone>();

            foreach (var u in _resources.UnitPhones.Where(uu => !string.IsNullOrEmpty(uu.UnitPhone.TSLNumber)))
            {
                list.Add(new UnitPhone(u.Id, u.Name)
                {
                    PhoneNumberId = u.PhoneNumberId,
                    AreaCode = u.AreaCode,
                    Number = u.Number,
                    Password = u.Password
                });
            }
            return list;
        }

        UnitPhone INovaAlertService.GetUnitPhone(int id, eTaskType taskType)
        {
            var item = _resources.UnitPhones.FirstOrDefault(u => u.Id == id);
            if (item != null)
            {
                var unit = item.UnitPhone;
                return new UnitPhone(unit.Id, unit.Name)
                {
                    PhoneNumberId = unit.PhoneNumberId,
                    AreaCode = unit.AreaCode,
                    Number = unit.Number,
                    Password = unit.Password,
                    ListOrder = unit.ListOrder,
                    Task = NovaAlertCommon.GetLastestTask(unit.PhoneNumberId, taskType)
                };
            }

            return null;
        }

        List<HostPhone> INovaAlertService.GetAllChannels()
        {
            return _resources.Channels.Select(vm => vm.Phone).OfType<HostPhone>().ToList();
        }

        List<BaseEntity> INovaAlertService.GetAllGroups()
        {
            var list = NovaAlertCommon.GetAllGroups();
            return list.Select(g => new BaseEntity(g.GroupId, g.Name)).ToList();
        }

        Panel INovaAlertService.GetPanelInfo(int panelId)
        {
            return NovaAlertCommon.Instance.GetPanelInfo(panelId);
        }

        byte INovaAlertService.GetPOAddress(byte id)
        {
            var po = _resources.POes.FirstOrDefault(p => p.Id == id);
            if (po != null) return po.PO.Address;
            else return 0;
        }

        void INovaAlertService.SendConnection(SwitchConnection conn)
        {
            try
            {
                Switch.SendConnection(conn);

                var existConn = _resources.Connetions
                    .FirstOrDefault(c => c.Dest.Address == conn.Dest.Address && c.Source.Address == conn.Source.Address);

                if (existConn == null)
                {
                    if (conn.IsConnected)
                    {
                        _resources.Connetions.Add(conn);
                    }
                }
                else
                {
                    if (conn.IsConnected)
                    {
                        existConn.Source.Update(conn.Source.Volumn, conn.Source.IsConnected);
                        existConn.Dest.Update(conn.Dest.Volumn, conn.Dest.IsConnected);
                    }
                    else
                    {
                        _resources.Connetions.Remove(existConn);
                    }
                }
            }
            catch
            {
                // supress exception here to avoid application crash
            }
        }

        void INovaAlertService.AdjustConnectionEndVolumn(SwitchConnectionEnd switchCN)
        {
            Switch.AdjustConnectionEndVolumn(switchCN);
        }

        void INovaAlertService.SendDeleteTone(byte address, bool clear)
        {
            Switch.ClearTone(address, clear);
        }

        void INovaAlertService.SendDial(byte address, string number)
        {
            Switch.Dial(address, number);
        }

        void INovaAlertService.SetConference(bool enable)
        {
            Switch.SetConference(enable);
        }

        void INovaAlertService.SendConferenceConnection(ConferenceConnection conn, bool isFirstOne)
        {
            Switch.SendConferenceConnection(conn, isFirstOne);
        }

        void INovaAlertService.AdjustConferenceVolumn(ConferenceConnection conn)
        {
            Switch.AdjustConferenceVolumn(conn);
        }

        void INovaAlertService.Ping()
        {
            var client = GetClient();
            if (client != null) client.LastAction = DateTime.Now;
        }

        void INovaAlertService.SendExtRingPower(bool ring)
        {
            Switch.SendExtRingPower(ring);
        }

        void INovaAlertService.UpdateAllStatus()
        {
            if (Switch == null)
            {
                return;
            }

            Switch.SendSimpleMessage(eControl.UpdateStatus);

            foreach (var u in _resources.UnitPhones)
            {
                var ev = new ResourceChangedEventArgs(u.ResourceType, u.Id, u.SelectedPanelId, null);
                _resources.NotifyClient(ev);
            }
        }

        void INovaAlertService.UpdateSystemDateTime(DateTime datetime)
        {
            try
            {
                StopTimer();
                SystemDateHelper.SetSystemTime(datetime);

                this.Switch.OnSystemDateTimeChanged(datetime);
                this.Recorder.OnSystemDateTimeChanged(datetime);
                foreach (var c in CommonResource.Instance.Clients)
                {
                    c.LastAction = datetime;
                }
                this.Switch.SendDateTime(datetime);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error(ex);
            }
            finally
            {
                StartTimer();
            }

            //foreach (var c in _resources.GetActiveClients())
            //{
            //    try
            //    {
            //        c.Callback.OnSystemDateTimeChanged(new SystemDateTimeChangedEventArgs(datetime));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogService.Logger.Error("UpdateSystemDateTime", ex);
            //    }
            //}
        }

        void INovaAlertService.SendAmplyPower(bool on)
        {
            if (Switch == null)
            {
                return;
            }
            this.Switch.SendAmplyPower(on);
        }
        #endregion

        #region Sound
        void INovaAlertService.PlayAlertSound(byte channelId, byte soundId)
        {
            try
            {
               
                var filename = Path.Combine(_soundDir, string.Format(@"CAU{0}.WAV", soundId));
                //System.Windows.Forms.MessageBox.Show(filename, "address");
                var soundChannel = _resources.SoundChannels[channelId];
                if (soundChannel.IsPlaying)
                {
                    if (soundChannel.CurrentFile == filename) return;
                    else soundChannel.Stop();
                }

                _resources.SoundChannels[channelId].PlayFile(filename, 0, true);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error(string.Format("PlayAlertSound({0}, {1})", channelId, soundId), ex);
            }
        }

        void INovaAlertService.PlaySound(byte channelId, int pos, string filename)
        {
            var fullName = Path.Combine(_soundDir, filename);

            try
            {
                var soundChannel = _resources.SoundChannels[channelId];
                if (soundChannel.IsPlaying)
                {
                    soundChannel.Stop();
                }

                _resources.SoundChannels[channelId].PlayFile(fullName, pos, true);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error(string.Format("PlaySound({0}, {1})", channelId, fullName), ex);
            }
        }

        void INovaAlertService.StopSound(byte channelId)
        {
            try
            {
                var sc = _resources.SoundChannels[channelId];
                if (sc.IsPlaying)
                {
                    sc.Stop();
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error(string.Format("StopSound({0})", channelId), ex);
            }
        }

        string INovaAlertService.GetRecordFolder()
        {
            return PathHelper.ConvertLocalFolderPathToIPBasedUNCPath(_recordDir);
        }

        string INovaAlertService.GetSoundPath()
        {
            return PathHelper.ConvertLocalFolderPathToIPBasedUNCPath(_soundDir);
        }
        #endregion

        #region Alert Task
        void INovaAlertService.UpdateTask(int unitId, Task task, long? duration, eTaskType taskType)
        {
            var unit = _resources.UnitPhones.FirstOrDefault(u => u.Id == unitId);
           
            if (unit != null)
            {
                unit.Task.TaskObj = task;
               
                NovaAlertCommon.UpdateTask(unit.UnitPhone, duration, taskType);

                // Cap nhat bang den
                if (GlobalSetting.Instance.UseSwitchPortForLP)
                {
                    SerialComm.SendAllResultBySwitchComm(unit.UnitPhone.PhoneNumberId);
                }
                else
                {
                    SerialComm.LPComm.SendAllResults(unit.UnitPhone.PhoneNumberId);
                }

                foreach (var c in _resources.GetActiveClients())
                {
                    try
                    {
                        c.Callback.OnTaskChanged(new TaskChangedEventArgs(unitId, task));
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        void INovaAlertService.RefeshTask(int unitId)
        {
            var unit = _resources.UnitPhones.FirstOrDefault(u => u.Id == unitId);
            if (unit != null)
            {
                unit.RefeshTask();
            }
        }

        List<ResultData> INovaAlertService.GetResults(eTaskType taskType)
        {
            return NovaAlertCommon.GetResults(taskType);
        }

        void INovaAlertService.AddLog(int clientId, int userId, string info)
        {
            DbLogger.Instance.AddLog((byte)clientId, userId, info);
        }
        #endregion

        #region CallLog
        void INovaAlertService.SaveCallLog(CallLog en)
        {
            CallLogDal.Instance.SaveCallLog(en);
        }

        void INovaAlertService.SaveCallLogDetail(CallLogDetail en)
        {
            CallLogDal.Instance.SaveCallLogDetail(en);
        }
        #endregion

        #region Record
        string INovaAlertService.StartRecord(byte id, byte poId)
        {
            var filename = this.Recorder.StartRecord(id, poId);
            LogService.Logger.Debug(string.Format("Channel {0}, PO {1} start record, file {2}", id, poId, filename));
            return filename;
        }

        void INovaAlertService.StopRecord(byte id)
        {
            this.Recorder.StopRecord(id);
            LogService.Logger.Debug(string.Format("Channel {0} stop record", id));
        }

        void INovaAlertService.PauseRecord(byte id, byte type)
        {
            this.Recorder.PauseRecord(id, type);
            LogService.Logger.Debug(string.Format("Channel {0} pause record", id));
        }

        void INovaAlertService.ResumeRecord(byte id)
        {
            this.Recorder.ResumeRecord(id);
            LogService.Logger.Debug(string.Format("Channel {0} resume record", id));
        }
        #endregion

        #region Helpers
        void client_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AlertSoundStatus")
            {
                UpdateAlertSound();
            }
        }

        private void InitSwitchComm()
        {
            if (Switch != null)
            {
                return;
            }

            try
            {
                this.Switch = SerialComm.SwitchComm;
                this.Switch.SendSimpleMessage(eControl.ResetSwitch);

                this.Switch.OnPOStatusChanged += Switch_OnPOStatusChanged;
                this.Switch.OnChannelStatusChanged += Switch_OnChannelStatusChanged;
                this.Switch.OnDialCompletedHandler += Switch_OnDialCompletedHandler;

                Action act = new Action(SendInitialData);
                act.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("Init SwitchComm error", ex);
            }
        }

        private static void SendInitialData()
        {
            try
            {
                LogService.Logger.Debug("Start sending init data.");
                System.Threading.Thread.Sleep(10000);

                SerialComm.SwitchComm.SendSimpleMessage(eControl.UpdateStatus);

                SerialComm.SwitchComm.SendDateTime(DateTime.Now);
                System.Threading.Thread.Sleep(800);

                if (GlobalSetting.Instance.AlarmOnSwitch)
                {
                    SerialComm.SendDayTypes();
                    System.Threading.Thread.Sleep(800);

                    SerialComm.SendAlarms();
                }

                if (GlobalSetting.Instance.UseSwitchPortForLP)
                {
                    SerialComm.SendAllResultBySwitchComm();
                }

                LogService.Logger.Debug("Finish sending init data.");
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("Send init data error", ex);
            }
        }

        void _resources_OnResourceChanged(object sender, ResourceChangedEventArgs ev)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
                {
                    var disconnectedClients = new List<Client>();
                    foreach (var c in _resources.GetActiveClients())
                    {
                        try
                        {
                            c.Callback.OnResourceChanged(ev);
                        }
                        catch
                        {                            
                            disconnectedClients.Add(c);
                        }
                    }

                //    LogService.Logger.Debug("Disconnect clients: " + disconnectedClients.Count);
                    disconnectedClients.ForEach(OnClientDisconnected);
                }));
        }

        private Client GetClient()
        {
            if (OperationContext.Current == null)
            {
                return null;
            }

            var callback = OperationContext.Current.GetCallbackChannel<INovaAlertServiceCallback>();
            if (callback != null)
            {
                return _resources.Clients.FirstOrDefault(c => c.Callback == callback);
            }
            return null;
        }

        private void UpdateAlertSound()
        {
            var ithis = ((INovaAlertService)this);
            var soundStatuses = _resources.GetActiveClients().Select(c => c.AlertSoundStatus);

            if (soundStatuses.Any(s => s.HasFlag(eAlertSoundStatus.WaitForKeycode)))
            {
                ithis.PlayAlertSound(1, 1);
            }
            else
            {
                ithis.StopSound(1);
            }

            if (soundStatuses.Any(s => s.HasFlag(eAlertSoundStatus.WaitForCommand)))
            {
                ithis.PlayAlertSound(2, 2);    // Cau 2            
            }
            else
            {
                ithis.StopSound(2);
            }

            if (soundStatuses.Any(s => s.HasFlag(eAlertSoundStatus.WaitForReport)))
            {
                ithis.PlayAlertSound(3, 3);    // Cau 3            
            }
            else
            {
                ithis.StopSound(3);
            }
        }

        void OnClientDisconnected(Client c)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => Internal_OnClientDisconnected(c)));
        }

        private void Internal_OnClientDisconnected(Client c)
        {
            var ithis = ((INovaAlertService)this);

            // Release resource
            // ...
            if (!_resources.Clients.Contains(c)) return;

            _resources.Clients.Remove(c);

            if (c.Type == eClientType.ControlPanel)
            {
                foreach (var channel in _resources.Channels.Where(ch => ch.SelectedPanelId == c.Id))
                {
                    ithis.Release(eResourceType.Channel, channel.Id);
                }

                foreach (var unit in _resources.UnitPhones.Where(u => u.SelectedPanelId == c.Id))
                {
                    ithis.Release(eResourceType.UnitPhone, unit.Id);
                }
            }

            c.PropertyChanged -= client_PropertyChanged;
            UpdateAlertSound();
        }

        TimeSpan GetWaveDuration(string fileName)
        {
            try
            {
                var player = new System.Windows.Media.MediaPlayer();
                player.Open(new Uri(fileName, UriKind.Absolute));
                var duration = player.NaturalDuration;
                player.Close();

                if (duration.HasTimeSpan)
                {
                    return duration.TimeSpan;
                }
            }
            catch
            {
            }
            return TimeSpan.Zero;
        }

        void AddLog(Client client, string info)
        {
            DbLogger.Instance.AddLog((byte)client.Id, 0, info);
        }

        public void OnSystemChanged()
        {
            CommonResource.Instance.LoadPhones();
        }
        #endregion

        #region Switch event handlers
        void Switch_OnDialCompletedHandler(object sender, ChannelEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
                {
                    var channel = _resources.Channels.FirstOrDefault(c => c.HostPhone.Address == e.Address);
                    if (channel == null)
                    {
                        return;
                    }
                    var disconnectedClients = new List<Client>();
                    foreach (var client in _resources.GetActiveClients())
                    {
                        try
                        {
                            client.Callback.OnDialCompleted(e);
                        }
                        catch
                        {
                            disconnectedClients.Add(client);
                        }
                    }

                    disconnectedClients.ForEach(OnClientDisconnected);
                }));
        }

        void Switch_OnPOStatusChanged(object sender, POStatusChangedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
                {
                    var po = _resources.POes.FirstOrDefault(p => p.Id == e.Address);
                    if (po == null || (po.Status == e.Status && po.Tone == e.Tone))
                    {
                        return;
                    }

                    po.ApplyChanges(e);
                    var disconnectedClients = new List<Client>();
                    foreach (var client in _resources.GetActiveClients())
                    {
                        try
                        {
                            client.Callback.OnPOStatusChanged(e);
                        }
                        catch
                        {
                            disconnectedClients.Add(client);
                        }
                    }

                    disconnectedClients.ForEach(OnClientDisconnected);
                }));
        }

        void Switch_OnChannelStatusChanged(object sender, ChannelStatusChangedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                var channel = _resources.Channels.FirstOrDefault(c => c.Id == e.Address);
                if (channel == null)
                {
                    return;
                }
                channel.ApplyChanges(e);
                var disconnectedClients = new List<Client>();
                foreach (var client in _resources.GetActiveClients())
                {
                    try
                    {
                        client.Callback.OnChannelStatusChanged(e);
                    }
                    catch
                    {
                        disconnectedClients.Add(client);
                    }
                }

                disconnectedClients.ForEach(OnClientDisconnected);

                // ktra do chuong
                this.IsRinging = _resources.Channels.Any(c => (c.HostPhone.MultiDestEnabled || c.HostPhone.AlertEnabled) && c.LineStatus == eLineStatus.Ring);
            }));
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (this.Recorder != null && this.Recorder.UsbReader != null)
            {
                this.Recorder.UsbReader.Dispose();
            }
        }
        #endregion

        string INovaAlertService.GetParameterValue(eGlobalParameter name)
        {
            return GlobalSetting.Instance.GetParamByName(name.ToString());
        }

        bool INovaAlertService.CanRecord()
        {
            return !this.Recorder.IsTimeOut;
        }

        DateTime INovaAlertService.GetDateTime()
        {
            var client = GetClient();
            DateTime now = DateTime.Now;
            if (client != null)
            {
                client.LastAction = now;
            }
            return now;
        }
    }
}
