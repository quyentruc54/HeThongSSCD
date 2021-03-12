using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Service;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using NovaAlert.Common.Utils;
using NovaAlert.Common.Setting;
using NovaAlert.Server.Views;
using NovaAlert.Common.Wpf;
using System.Runtime.Remoting.Messaging;
using System.Windows;

namespace NovaAlert.Server
{
    public class MainWindowViewModel: ViewModelBase
    {
        #region Properties
        ServiceHost _host;
        ConfigServiceHost _configHost;
        public DateTime TestTime { get; set; }
        public byte SelectedLedPanelId { get; set; }
        public RelayCommand TestAlarmCommand { get; set; }
        public RelayCommand StartCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand SendDataToLedPanelCommand { get; private set; }
        public RelayCommand PollLedPanelCommand { get; private set; }
        public RelayCommand DeleteSmallFileCommand { get; private set; }
        public RelayCommand ResetRecorderCommand { get; private set; }
        public RelayCommand ShowGlobalSettingCommand { get; set; }
        public RelayCommand ShowDbConfigCommand { get; set; }

        public RelayCommand TestTSLPrepareCommand { get; set; }
        public RelayCommand ResetModemCommand { get; set; }

        public SoundCardConfigViewModel SoundConfigVM
        {
            get { return CommonResource.Instance.SoundConfigVM; }
        }

        public ObservableCollection<Client> Clients { get { return CommonResource.Instance.Clients; } }

        public int MinRecordSize
        {
            get { return GlobalSetting.Instance.MinRecordSize; }
        }

        public double FreeSpace { get; private set; }

        System.Threading.Timer _timer;
        int _counter;
        object _syncObj = new object();

        NovaAlertService _service;

        public bool IsInDebugMode
        {
            get { return ClientSetting.Instance.IsInDebugMode; }
        }
        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            this.TestTime = DateTime.Now;
            this.SelectedLedPanelId = 1;
            this.StartCommand = new RelayCommand(p => StartService());
            this.StopCommand = new RelayCommand(p => StopService());
            this.TestAlarmCommand = new RelayCommand(p => DoTestAlarm());
            this.DeleteSmallFileCommand = new RelayCommand(p => DeleteSmallFile());
            ResetRecorderCommand = new RelayCommand(p => OnResetRecorder());
            ShowDbConfigCommand = new RelayCommand(p => OnShowDbConfig());
            this.ShowGlobalSettingCommand = new RelayCommand(p => OnShowGlobalSetting());            
            UpdateFreeSpace();

            _counter = 0;
            _timer = new System.Threading.Timer(OnTimer, null, 5000, 60000);

            this.TestTSLPrepareCommand = new RelayCommand(p => OnTestTSLPrepare());
            this.ResetModemCommand = new RelayCommand(p => CommonResource.Instance.Modem.SelectedPanelId = null);

            var st = GlobalSetting.Instance;
            StartService();
        } 
        #endregion

        #region Helpers
        void OnShowGlobalSetting()
        {
            var vm = new GlobalSettingViewModel();
            var wnd = new GlobalSettingView() { DataContext = vm };
            
            var owner = Application.Current.GetActiveWindow();            
            if (owner != null)
            {
                wnd.Owner = owner;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            if(wnd.ShowDialog() == true)
            {
                //App.Restart();
            }
        }

        void OnShowDbConfig()
        {
            var vm = new ConfigDatabaseViewModel();
            var wnd = new ConfigDatabaseView() { DataContext = vm };

            var owner = Application.Current.GetActiveWindow();
            if(owner != null)
            {
                wnd.Owner = owner;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            if (wnd.ShowDialog() == true)
            {
                
            }
        }
        void StartService()
        {
            try
            {
                _configHost = new ConfigServiceHost(typeof(NovaAlertConfigService));
                _configHost.Open();
                _configHost.SystemChanged += _configHost_SystemChanged;

                _service = new NovaAlertService();
                _host = new ServiceHost(_service);
                _host.Open();
                _host.Faulted += OnHostFaulted;                
            }
            catch (Exception ex)
            {
                ServiceContainer.Instance.GetService<IMessageBoxService>().ShowError(ex.Message);
            }
            finally
            {
                OnPropertyChanged("HostInfo");
            }
        }

        void StopService()
        {
            if (_host != null && _host.State == CommunicationState.Opened)
            {
                _host.Close();
                if(_service != null)
                {
                    _service.Dispose();
                    _service = null;
                }
            }

            if (_configHost != null && _configHost.State == CommunicationState.Opened)
            {
                _configHost.Close();
                _configHost.SystemChanged -= _configHost_SystemChanged;
            }

            OnPropertyChanged("HostInfo");
        }

        void OnHostFaulted(object sender, EventArgs e)
        {
            OnPropertyChanged("HostInfo");
        }

        public string HostInfo
        {
            get
            {
                var sb = new StringBuilder();
                if (_host != null)
                {
                    sb.Append(GetHostInfo(_host) + System.Environment.NewLine);
                }

                if (_configHost != null)
                {
                    sb.Append(GetHostInfo(_configHost));
                }

                return sb.ToString();
            }
        }

        string GetHostInfo(ServiceHost h)
        {
            return string.Format("Address: {0}, State: {1}", h.BaseAddresses[0].ToString(), h.State);
        }

        void DoTestAlarm()
        {
            AlarmManager.Instance.CheckAlarm(this.TestTime, true);
        }

        void UpdateFreeSpace()
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory; // ServiceSettings.RecordFolder;
            this.FreeSpace = 0;

            if (!string.IsNullOrEmpty(folder))            
            {
                DriveInfo drv = new DriveInfo(folder[0].ToString());
                this.FreeSpace = drv.AvailableFreeSpace / (1024 * 1024);
            }

            OnPropertyChanged("FreeSpace");            
        }

        void DeleteSmallFile()
        {
            var min = GlobalSetting.Instance.MinRecordSize;
            if (min == 0)
            {
                return;
            }

            var localPath = PathHelper.GetLocalPath(GlobalSetting.Instance.RecordFolder);
            var rootDir = new DirectoryInfo(localPath);
            foreach(var dir in rootDir.GetDirectories())
            {
                var smallFiles = dir.GetFiles().Where(f => f.Length < 1024 * min).ToList();
                if (smallFiles.Count < 0)
                {
                    return;
                }

                foreach (var item in smallFiles)
                {
                    try
                    {
                        LogService.Logger.Info(string.Format("Deleting {0}", item.FullName));
                        File.Delete(item.FullName);
                    }
                    catch (Exception ex)
                    {
                        LogService.Logger.Error(ex);
                    }
                }
            }            
        }

        void OnTimer(object obj)
        {
            try
            {
                UpdateFreeSpace();
                _counter++;
                if (_counter > 60)
                {
                    Action act = new Action(DeleteSmallFile);
                    act.BeginInvoke(null, null);
                    _counter = 0;
                }
            }
            catch(Exception ex)
            {
                LogService.Logger.Error("MainWindowViewModel.Timer", ex);
            }
        }

        void OnResetRecorder()
        {
            RecorderManager.Recorder.Stop(true);
            RecorderManager.Recorder.Start();
        }

        void _configHost_SystemChanged(object sender, EventArgs e)
        {
            if(_service != null)
            {
                _service.OnSystemChanged();
            }
        }
        #endregion

        void OnTestTSLPrepare()
        {
        }

        void OnComplete(IAsyncResult ar)
        {
            var ars = ar as AsyncResult;
            if(ars != null)
            {
                var f = ars.AsyncDelegate as Func<bool>;
                var r = f.EndInvoke(ar);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (r) GetService<IMessageBoxService>().ShowInfo("Success");
                        else GetService<IMessageBoxService>().ShowInfo("Fail");
                    }));                
            }
        }

        #region Debug Mode
        public int Recorder_ReadDelayInterval
        {
            get { return RecorderManager.Recorder.UsbReader.ReadDelayInterval; }
            set
            {
                RecorderManager.Recorder.UsbReader.ReadDelayInterval = value;
                OnPropertyChanged("Recorder_ReadDelayInterval");
            }
        }

        public bool Recorder_LogData
        {
            get { return RecorderManager.Recorder.UsbReader.LogData; }
            set
            {
                RecorderManager.Recorder.UsbReader.LogData = value;
                OnPropertyChanged("Recorder_LogData");
            }
        } 
        #endregion
    }
}
