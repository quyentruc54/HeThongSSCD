using System;
using System.Globalization;
using System.Linq;
using System.Text;
using NovaAlert.Entities;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Utils;
using NovaAlert.Config.ViewModels;
using NovaAlert.Common;
using NovaAlert.Bll.Controller;
using NovaAlert.Config;
using NovaAlert.Common.Setting;
using NovaAlert.Entities.ViewModel;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;

namespace NovaAlert.Bll
{
    public partial class ClientAppViewModel : ViewModelBase, IClientApp
    {
        #region Members & Properties        
        public INovaAlertService Service { get; private set; }
        protected CultureInfo _cultureInfo = new CultureInfo("vi-VN");

        public string RestrictedAreaCodes { get; private set; }

        DateTime _curDateTime;
        public DateTime CurDateTime
        {
            get { return _curDateTime; }
            set
            {
                _curDateTime = value;
                OnPropertyChanged("CurDateTime");
                OnPropertyChanged("DayOfWeek");
            }
        }

        public string DayOfWeek
        {
            get
            {
                return _cultureInfo.DateTimeFormat.GetDayName(_curDateTime.DayOfWeek);
            }
        }

        string _officeName;
        public string OfficeName
        {
            get { return _officeName; }
            set { _officeName = value; OnPropertyChanged("OfficeName"); }
        }

        public eClientType ClientType { get { return eClientType.ControlPanel; } }

        public byte ClientId { get; private set; }
        public byte POId { get; private set; }

        eWorkingMode _workingMode;
        public eWorkingMode WorkingMode
        {
            get { return _workingMode; }
            set
            {
                if (_workingMode != value)
                {
                    _workingMode = value;
                    OnPropertyChanged("WorkingMode");
                    
                }
                OnWorkingModeChanged();
            }
        }

        public int GroupId
        {
            get 
            {
                if (this.WorkingMode == eWorkingMode.Alert)
                {
                    return ClientSetting.Instance.AlertGroupId;
                }
                return ClientSetting.Instance.MultiDestGroupId;
            }
            set 
            {
                if (this.WorkingMode == eWorkingMode.Alert)
                {
                    ClientSetting.Instance.AlertGroupId = value;
                }
                else
                {
                    ClientSetting.Instance.MultiDestGroupId = value;
                }
                OnPropertyChanged("GroupId");
                OnWorkingModeChanged();
            }
        }

        public string Title
        {
            get 
            {
                if (this.Menu != null && this.Menu.IsInConfigMode)
                {
                    var vm = this.Menu.Config.NavBar.ViewSelected;
                    if (vm is SearchLogViewModel || vm is CallLogSearchViewModel)
                        return "NHẬT KÝ GHI ÂM & THAO TÁC";
                    else
                        return "CHẾ ĐỘ KHAI BÁO THÔNG SỐ HỆ THỐNG";
                }
                else return EnumEx.GetEnumDescription(_workingMode); 
            }
        }

        ViewModelBase __option;
        public ViewModelBase Option
        {
            get { return __option; }
            set { __option = value; OnPropertyChanged("Option"); }
        }

        ViewModelBase _control;
        public ViewModelBase Control
        {
            get { return _control; }
            set { _control = value; OnPropertyChanged("Control"); }
        }

        UnitPhoneListViewModel _units;
        public UnitPhoneListViewModel Units
        {
            get { return _units; }
            set 
            {
                var oldValue = _units;
                if (oldValue != null)
                {
                    oldValue.OnItemClicked -= this.UnitClicked;
                }

                _units = value;
                if (_units != null)
                {
                    _units.OnItemClicked += this.UnitClicked;
                }
                OnPropertyChanged("Units"); 
            }
        }

        PhoneListViewModel<HostPhoneViewModel> _channels;
        public PhoneListViewModel<HostPhoneViewModel> Channels
        {
            get { return _channels; }
            set 
            {
                var oldValue = _channels;
                if (oldValue != null)
                {
                    oldValue.OnItemClicked -= this.ChannelClicked;
                }

                _channels = value;
                if (_channels != null)
                {
                    _channels.OnItemClicked += this.ChannelClicked;
                }
                OnPropertyChanged("Channels"); 
            }
        }

        HelpViewModel _help;
        public HelpViewModel Help
        {
            get { return _help; }
            set { _help = value; OnPropertyChanged("Help"); }
        }

        MenuViewModel _menu;
        public MenuViewModel Menu
        {
            get { return _menu; }
            set { _menu = value; OnPropertyChanged("Menu"); }
        }

        SoundVolumnViewModel _volumnControl;
        public SoundVolumnViewModel VolumnControl
        {
            get { return _volumnControl; }
            set { _volumnControl = value; OnPropertyChanged("VolumnControl"); }
        }

        public bool AlertOptionVisible
        {
            get 
            { 
                return (_workingMode == eWorkingMode.Alert || _workingMode == eWorkingMode.CCPK_Alert  || _workingMode == eWorkingMode.TSL_Alert) && 
                    this.Menu.IsInConfigMode == false &&
                    this.MainController.HasConference != true; 
            }
        }

        public bool IsMultiDestMode
        {
            get { return _workingMode == eWorkingMode.MultiDest; }
        }

        public bool IsInfoVisible
        {
            get { return _workingMode == eWorkingMode.MultiDest || this.Menu.IsInConfigMode; }
        }

        public bool IsAlertMode
        {
            get { return _workingMode == eWorkingMode.Alert; }
        }

        public bool IsCCPKAlertMode
        {
            get { return _workingMode == eWorkingMode.CCPK_Alert; }
        }

        public bool IsAlarmMode
        {
            get { return _workingMode == eWorkingMode.Alarm; }
        }
        
        System.Threading.Timer _timer;

        public MainController MainController { get; private set; }

        PrivateAlarmViewModel _alarm;
        public PrivateAlarmViewModel Alarm
        {
            get { return _alarm; }
            set { _alarm = value; OnPropertyChanged("Alarm"); }
        }

        public IMediaPlayer MediaPlayer { get; private set; }
        bool _isRinging = false;
        public bool IsRinging
        {
            get { return _isRinging; }
            set
            {
                lock (this.MediaPlayer)
                {
                    if (_isRinging != value)
                    {
                        _isRinging = value;
                        var file = ClientSetting.Instance.RingTone;
                        if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
                        {
                            if (_isRinging)
                            {
                                // switch back to main view if it's in config mode
                                if (this.Menu.IsInConfigMode) this.Menu.IsInConfigMode = false;

                                this.MediaPlayer.EnableLooping = true;
                                this.MediaPlayer.FileName = file;
                                this.MediaPlayer.PlayCommand.Execute(null);                                
                            }
                            else
                            {
                                this.MediaPlayer.StopCommand.Execute(null);
                                //this.MediaPlayer.FileName = null;
                            }
                        }

                        if(!_isRinging)
                        {
                            foreach(var unit in this.Units.Items.Where(u => u.Status == ePhoneStatus.Ring))
                            {
                                if(this.MainController.IsFreeUnit(unit)) unit.Status = ePhoneStatus.Free;
                            }

                            //foreach(var ch in this.Channels.Items.Where(c => !string.IsNullOrEmpty(c.CallerId)))
                            //{
                            //    ch.CallerId = null;
                            //}
                        }
                    }
                }                
            }
        }

        bool _isSwitchConnected;
        public bool IsSwitchConnected
        {
            get { return _isSwitchConnected; }
            private set
            {
                if(_isSwitchConnected != value)
                {
                    _isSwitchConnected = value;
                    OnPropertyChanged("IsSwitchConnected");
                    OnDeviceStatusChanged();
                }
            }
        }

        public bool IsDeviceReady
        {
            get { return this.IsSwitchConnected && WcfProxy.Instance.IsConnected; }
        }

        public string DeviceStatusText
        {
            get
            {
                var sb = new StringBuilder();
                if (!this.IsSwitchConnected)
                {
                    sb.AppendLine("Mất kết nối với khối chuyển mạch.");
                }
                if (!WcfProxy.Instance.IsConnected)
                {
                    sb.AppendLine("Mất kết nối với khối xử lý trung tâm.");
                }
                return sb.ToString();
            }
        }

        TSL_AlertController _tslController;

        public TSL_AlertController TSLController
        {
            get 
            { 
                if(_tslController == null)
                {
                    _tslController = new TSL_AlertController(this);
                }
                return _tslController; 
            }
        }
        #endregion
        
        #region Ctor
        public ClientAppViewModel()
        {
            LoadConfig();
                 
            //ConfigServices.OnSystemDateTimeChangedHandler += (sender, e) => this.Service.UpdateSystemDateTime(e.DateTime);
            _timer = new System.Threading.Timer(OnTimer, this, 1000, 1000);

            this.Service = WcfProxy.Instance;
            WcfProxy.Instance.OnConnectSucceed += (sender, e) => SetupWorkspace();
            WcfProxy.Instance.Init(this);
            this.IsSwitchConnected = true;

            WcfProxy.Instance.PropertyChanged += WcfProxy_Instance_PropertyChanged;
        }
        #endregion

        #region Helpers

        void OnDeviceStatusChanged()
        {
            OnPropertyChanged(nameof(IsDeviceReady));
            OnPropertyChanged(nameof(DeviceStatusText));
        }
        void WcfProxy_Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsConnected") OnDeviceStatusChanged();
        }
        private void LoadConfig()
        {
            var setting = ClientSetting.Instance;
            ReleaseMediaPlayer();
            this.MediaPlayer = new ClientMediaPlayerViewModel(setting.LocalSoundId);
            this.ClientId = setting.ClientId;
            _workingMode = (eWorkingMode)setting.CurrentMode;
            this.POId = setting.POId;
        }

        void SetupWorkspace()
        {
            try
            {
                InitCommonComponent();
                OnWorkingModeChanged();
            }
            catch(Exception ex)
            {
                LogService.Logger.Error("SetupWorkspace", ex);
                System.Windows.MessageBox.Show(ex.Message, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        void InitCommonComponent()
        {
            try
            {
                Service.Subscribe(this.ClientId, this.ClientType);
            }
            catch
            {
            }

            this.OfficeName = Service.GetParameterValue(eGlobalParameter.OfficeName); // "SỞ CHỈ HUY - QUÂN KHU 7";
            this.CurDateTime = Service.GetDateTime();
            this.Help = new HelpViewModel();
            this.VolumnControl = new SoundVolumnViewModel();
            this.VolumnControl.PropertyChanged += VolumnControl_PropertyChanged;
        }

        void OnWorkingModeChanged()
        {
            CleanUp();

            if (_workingMode == eWorkingMode.Alert)
            {   
                this.Alarm = null;
                this.Units = new UnitPhoneListViewModel(this);// { ShowTask = true, ShowResult = true };
                this.Units.Refesh();
                this.Channels = new ChannelListViewModel(this);

                this.Menu = new AlertMenuViewModel(this);
                this.Menu.PropertyChanged += Menu_PropertyChanged;

                var ctrl = new AlertController(this);
                this.MainController = ctrl;
                this.Option = new AlertOptionViewModel(ctrl);
                this.Control = new AlertControlViewModel(ctrl, eTaskType.CTT);
                this.MainController.PropertyChanged += MainController_PropertyChanged;
                this.AddLog("Chuyển sang chế độ BĐ CCT-SSCĐ");
            }
            else if(_workingMode == eWorkingMode.TSL_Alert)
            {
                this.Alarm = null;
                this.Units = this.TSLController.Units; 
                this.Units.Refesh();
                this.Channels = this.TSLController.Channels; 

                this.Menu = new AlertMenuViewModel(this, Entities.Enums.eAlertMode.Transfer);
                this.Menu.PropertyChanged += Menu_PropertyChanged;

                this.MainController = this.TSLController;
                this.Option = this.TSLController.OptionVM;
                this.Control = this.TSLController.ControlVM;
                this.MainController.PropertyChanged += MainController_PropertyChanged;
                this.AddLog("Chuyển sang chế độ BĐ CCT-SSCĐ - Truyền số liệu");
            }
            else if (_workingMode == eWorkingMode.CCPK_Alert)
            {
                this.Alarm = null;
                this.Units = new UnitPhoneListViewModel(this);
                this.Units.Refesh();
                this.Channels = new ChannelListViewModel(this);

                this.Menu = new AlertMenuViewModel(this);
                this.Menu.PropertyChanged += Menu_PropertyChanged;

                var ctrl = new CCPK_AlertController(this);
                this.MainController = ctrl;
                this.Option = new CCPK_AlertOptionViewModel(ctrl);
                this.Control = new AlertControlViewModel(ctrl, eTaskType.CCPK);
                this.MainController.PropertyChanged += MainController_PropertyChanged;
                this.AddLog("Chuyển sang chế độ BĐ CCPK");
            }
            else if (_workingMode == eWorkingMode.MultiDest)
            {                
                this.Alarm = null;
                this.Option = null;               

                this.Units = new UnitPhoneListViewModel(this);
                this.Units.Refesh();
                this.Channels = new ChannelListViewModel(this);

                this.Menu = new MenuViewModel(this);
                this.Menu.PropertyChanged += Menu_PropertyChanged;                

                var ctrl = new MultiDestController(this);
                this.MainController = ctrl;
                this.Control = new MultiDestControlViewModel(ctrl);
                this.AddLog("Chuyển sang chế độ Liên Lạc Đa Hướng");
            }
            else if(_workingMode == eWorkingMode.Alarm)
            {
                this.Option = null;
                this.Units = null;
                
                this.Channels = new ChannelListViewModel(this);
                this.Menu = new MenuViewModel(this);
                this.Menu.PropertyChanged += Menu_PropertyChanged;                  
                      
                this.Control = null;
                this.Alarm = new PrivateAlarmViewModel(this);
                this.MainController = new PrivateAlarmController(this);
                this.AddLog("Chuyển sang chế độ BH-BĐ Nội Bộ");
            }

            this.RestrictedAreaCodes = this.Service.GetParameterValue(eGlobalParameter.RestrictedAreaCode);
            this.Menu.IsInConfigMode = false;
            
            RefeshUI();

            this.Service.UpdateAllStatus();
            ShowMessage(string.Empty, string.Empty, false);
        }

        void MainController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasConference") RefeshUI();
        }

        void Menu_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefeshUI();
        }        
        void RefeshUI()
        {
            OnPropertyChanged("Title");
            OnPropertyChanged("AlertOptionVisible");
            OnPropertyChanged("IsMultiDestMode");
            OnPropertyChanged("IsAlertMode");
            OnPropertyChanged("IsAlarmMode");
            OnPropertyChanged("IsInfoVisible");
        }   
     
        public void RefeshTitle()
        {
            OnPropertyChanged("Title");
        }

        int _counter1 = 0;
        void OnTimer(object state)
        {
            _counter1++;
            if(_counter1 >= 5)
            {
                _counter1 = 0;                
                try
                {
                    this.CurDateTime = Service.GetDateTime();
                }
                catch
                {
                } 
            }            
        }

        void CleanUp()
        {
            if(this.Units != null) this.Units.OnItemClicked -= UnitClicked;
            if(this.Channels != null) this.Channels.OnItemClicked -= ChannelClicked;
            ReleaseMediaPlayer();

            if (this.MainController != null)
            {
                this.MainController.ReleaseAllFreeChannels();
                this.MainController.ReleaseAllFreeUnits();
            }

            if(this.Menu != null)
            {
                this.Menu.PropertyChanged -= Menu_PropertyChanged;
            }
        }

        void VolumnControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Volumn")
            {
            }
        }        

        public void OnGroupSelect()
        {
            AddLog("Chọn nhóm");
            var vm = new GroupListViewModel(this) { ShowButtons = false };
            var view = new NovaAlert.Config.Views.GroupListView() { DataContext = vm, Width = 400, Height = 300 };
            if (NovaAlert.Common.Wpf.ModalDialog.ShowControl(view, "Chọn nhóm đơn vị") == true)
            {
                if(vm.ItemSelected != null)
                {
                    var groupId = vm.ItemSelected.GroupObj.Id;
                    if(groupId != this.GroupId)
                    {
                        this.GroupId = groupId;
                        AddLog(string.Format("Chọn nhóm {0}", this.GroupId));
                        ClientSetting.Instance.Save();
                    }
                }
            }
        }
        #endregion
 
        #region Override
        protected override void OnDispose()
        {
            this.MainController.Dispose();
            this.Service.UnSubscribe(this.ClientId, this.ClientType);
        }
        #endregion

        public void Reload()
        {
            LoadConfig();
            SetupWorkspace();
        }

        void ReleaseMediaPlayer()
        {
            if(this.MediaPlayer != null && this.MediaPlayer.IsPlaying)
            {
                this.MediaPlayer.StopCommand.Execute(null);
            }

            var player = this.MediaPlayer as IDisposable;
            if(player != null)
            {                
                player.Dispose();
            }
        }

        public void OnSwitchStatusChanged(bool isConnected)
        {
            this.IsSwitchConnected = isConnected;            
        }

        public void AddLog(string info, bool showInfo = false)
        {
            AsyncHelper.RunAsync<int, int, string>(this.Service.AddLog, this.ClientId, 0, info);
            if (showInfo)
            {
                ShowInfo(info);
            }
        }

        public void UpdateTaskAsync(int unitId, Task task, long? duration, eTaskType taskType)
        {
            AsyncHelper.RunAsync<int, Task, long?, eTaskType>(this.Service.UpdateTask, unitId, task, duration, taskType);
        }

        public void ShowMessage(string title, string text, bool isWarning = false)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => this.Help.HelpItem = new HelpItem(isWarning) { Title = title, Content = text }));
        }

        public void ShowError(string msg)
        {
            ShowMessage("LỖI", msg, true);
        }

        public void ShowHelp(string msg)
        {
            ShowMessage("HƯỚNG DẪN SỬ DỤNG", msg, true);
        }

        public void ShowInfo(string msg)
        {
            ShowMessage("Thông báo", msg);
        }

        public void ClearInfo()
        {
            ShowMessage(string.Empty, string.Empty);
        }

        public new void Refesh()
        {
            if (_tslController != null)
            {
                _tslController.Refesh();
            }
            OnWorkingModeChanged();
        }

        public bool IsChannelUsable(HostPhoneViewModel c)
        {
            return (this.WorkingMode == eWorkingMode.Alert && c.HostPhone.AlertEnabled)
                || (this.WorkingMode == eWorkingMode.MultiDest && c.HostPhone.MultiDestEnabled)
                || (this.WorkingMode == eWorkingMode.CCPK_Alert && c.HostPhone.CCPKEnabled);
        }

        public IEnumerable<UnitPhoneViewModel> GetAllUnits()
        {
            return this.Units.Items;
        }

        public IEnumerable<UnitPhoneViewModel> GetUnits(Func<UnitPhoneViewModel, bool> filter)
        {
            return GetAllUnits().Where(filter);
        }

        public IEnumerable<UnitPhoneViewModel> GetSelectedUnits()
        {
            return GetUnits(u => u.SelectedPanelId == this.ClientId);
        }

        public IEnumerable<UnitPhoneViewModel> GetSelectedUnits(Func<UnitPhoneViewModel, bool> filter)
        {
            return GetSelectedUnits().Where(filter);
        }

        public IEnumerable<HostPhoneViewModel> GetAllChannels()
        {
            return this.Channels.Items;
        }

        public IEnumerable<HostPhoneViewModel> GetChannels(Func<HostPhoneViewModel, bool> filter)
        {
            return GetAllChannels().Where(filter);
        }

        public IEnumerable<HostPhoneViewModel> GetSelectedChannels()
        {
            return GetChannels(channel => channel.SelectedPanelId == this.ClientId);
        }
    }
}
