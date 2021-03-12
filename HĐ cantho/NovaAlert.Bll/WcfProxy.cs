using NovaAlert.Common;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows;

namespace NovaAlert.Bll
{
    public class ServiceException : Exception
    {
        public ServiceException()
        //    : base("Không thể kết nối tới máy chủ")
        {
        }
    }
    public class WcfProxy : INovaAlertService, INotifyPropertyChanged
    {
        private DuplexChannelFactory<INovaAlertService> _channelFactory;
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        static WcfProxy _instance;

        public static WcfProxy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WcfProxy();
                }

                return _instance;
            }
        }

        INovaAlertService _realProxy;
        public object Implementation { get; private set; }

        public event EventHandler OnConnectSucceed;
        bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    if (_isConnected && OnConnectSucceed != null) OnConnectSucceed(this, System.EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Ctor
        private WcfProxy()
        {
            _isConnected = false;
        }
        #endregion

        #region Helpers
        public void Init(object implementation)
        {
            this.Implementation = implementation;
            CheckConnection();
        }

        bool CheckConnection()
        {
            if (!IsConnected)
            {
                try
                {
                    bool isReconnected = false;
                    if (_channelFactory != null)
                    {
                        _channelFactory.Abort();
                        isReconnected = true;
                    }

                    var ctx = new InstanceContext(this.Implementation);
                    _channelFactory = new DuplexChannelFactory<INovaAlertService>(ctx, "NovaAlertService_NetTcp");
                    _realProxy = _channelFactory.CreateChannel();
                    if (isReconnected)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    _realProxy.Ping();
                    Application.Current.Dispatcher.Invoke(new Action<bool>(UpdateConnectionInfo), true);
                }
                catch(Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action<bool>(UpdateConnectionInfo), false);
                    LogService.Logger.Error(ex);
                }
            }

            return this.IsConnected;
        }

        void OnException(Exception ex)
        {
            this.IsConnected = false;
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        void UpdateConnectionInfo(bool isConnected)
        {
            this.IsConnected = isConnected;
        }
        #endregion

        #region Service Wrapper
        void INovaAlertService.Subscribe(int clientId, eClientType type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.Subscribe(clientId, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.UnSubscribe(int clientId, eClientType type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UnSubscribe(clientId, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.Request(eResourceType resourceType, int resourceId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.Request(resourceType, resourceId);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.Release(eResourceType resourceType, int resourceId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.Release(resourceType, resourceId);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SendConnection(Entities.SwitchConnection conn)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendConnection(conn);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        List<Entities.UnitPhone> INovaAlertService.GetUnitPhones(int? groupId, eTaskType taskType, bool includeTask)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetUnitPhones(groupId, taskType, includeTask);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        List<Entities.UnitPhone> INovaAlertService.GetTSLUnitPhones()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetTSLUnitPhones();
            }
            catch (Exception ex)
            {
                OnException(ex);
                 throw new ServiceException();
            }
        }

        List<Entities.HostPhone> INovaAlertService.GetAllChannels()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetAllChannels();
            }
            catch (Exception ex)
            {
                OnException(ex);
               throw new ServiceException();
            }
        }

        List<Entities.BaseEntity> INovaAlertService.GetAllGroups()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetAllGroups();
            }
            catch (Exception ex)
            {
                OnException(ex);
                 throw new ServiceException();
            }
        }

        Panel INovaAlertService.GetPanelInfo(int panelId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetPanelInfo(panelId);
            }
            catch (Exception ex)
            {
                OnException(ex);
               throw new ServiceException();
            }
        }

        byte INovaAlertService.GetPOAddress(byte id)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetPOAddress(id);
            }
            catch (Exception ex)
            {
                OnException(ex);
                 throw new ServiceException();
            }
        }

        void INovaAlertService.SendDeleteTone(byte address, bool clear)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendDeleteTone(address, clear);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SendDial(byte address, string number)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendDial(address, number);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SetConference(bool enable)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SetConference(enable);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SendConferenceConnection(Entities.ConferenceConnection conn, bool isFirstOne)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendConferenceConnection(conn, isFirstOne);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.AdjustConferenceVolumn(Entities.ConferenceConnection conn)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.AdjustConferenceVolumn(conn);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.Ping()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.Ping();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SendExtRingPower(bool ring)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendExtRingPower(ring);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.UpdateAllStatus()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UpdateAllStatus();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.PlayAlertSound(byte channelId, byte soundId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.PlayAlertSound(channelId, soundId);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.PlaySound(byte channelId, int pos, string filename)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.PlaySound(channelId, pos, filename);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.StopSound(byte channelId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.StopSound(channelId);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.UpdateTask(int unitId, Entities.Task task, long? duration, eTaskType taskType)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UpdateTask(unitId, task, duration, taskType);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.RefeshTask(int unitId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.RefeshTask(unitId);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.AdjustConnectionEndVolumn(Entities.SwitchConnectionEnd switchCN)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.AdjustConnectionEndVolumn(switchCN);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.UpdateSystemDateTime(DateTime datetime)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UpdateSystemDateTime(datetime);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.UpdateAlertSoundStatus(int clientId, Entities.eAlertSoundStatus status)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UpdateAlertSoundStatus(clientId, status);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SaveCallLog(Entities.CallLog en)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SaveCallLog(en);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.SaveCallLogDetail(Entities.CallLogDetail en)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SaveCallLogDetail(en);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        string INovaAlertService.StartRecord(byte id, byte poId)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.StartRecord(id, poId);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        void INovaAlertService.StopRecord(byte id)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.StopRecord(id);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.PauseRecord(byte id, byte type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.PauseRecord(id, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        void INovaAlertService.ResumeRecord(byte id)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.ResumeRecord(id);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        string INovaAlertService.GetRecordFolder()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetRecordFolder();
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        string INovaAlertService.GetSoundPath()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetSoundPath();
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        void INovaAlertService.SendAmplyPower(bool on)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.SendAmplyPower(on);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        List<ResultData> INovaAlertService.GetResults(eTaskType taskType)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetResults(taskType);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }


        void INovaAlertService.AddLog(int clientId, int userId, string info)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.AddLog(clientId, userId, info);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        UnitPhone INovaAlertService.GetUnitPhone(int id, eTaskType taskType)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetUnitPhone(id, taskType);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        string INovaAlertService.GetParameterValue(eGlobalParameter name)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetParameterValue(name);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        void INovaAlertService.ReplyFromRepareRequest(int id)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.ReplyFromRepareRequest(id);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }
        #endregion


        public TslStatus GetLatestTslStatus(int phoneNumberId, eTslStatusType type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetLatestTslStatus(phoneNumberId, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public void UpdateTslStatus(int phoneNumberId, eTslStatusType type, eTslStatus status)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.UpdateTslStatus(phoneNumberId, type, status);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public void CancelTslTask()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.CancelTslTask();
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public void DoTslTask(int phoneNumberId, eTslStatusType type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                _realProxy.DoTslTask(phoneNumberId, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public List<ResultData> GetSubResults(int phoneNumberId, eTaskType type)
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetSubResults(phoneNumberId, type);
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public bool CanRecord()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.CanRecord();
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }

        public DateTime GetDateTime()
        {
            try
            {
                if (CheckConnection() == false) throw new ServiceException();
                return _realProxy.GetDateTime();
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw new ServiceException();
            }
        }
    }
}
