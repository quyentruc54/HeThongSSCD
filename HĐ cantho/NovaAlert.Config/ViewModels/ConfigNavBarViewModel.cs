using System;
using System.Collections.Generic;
using System.Linq;
using NovaAlert.Common.Mvvm;
using System.ComponentModel;
using NovaAlert.Entities;
using NovaAlert.Common.Setting;
namespace NovaAlert.Config.ViewModels
{
    public enum eConfigView
    {
        None = 0,
        [Description("Thời gian")]
        Date = 1,
        [Description("Báo giờ")]
        DayType =2,
        [Description("Danh bạ")]
        Contact = 3,
        [Description("Âm hiệu")]
        AlarmType = 4,
        [Description("Nhóm")]
        Group = 5,
        [Description("Kênh")]
        Channel = 6,
        [Description("Đơn vị")]
        Unit = 7,
        [Description("Báo động nội bộ")]
        PrivateAlarm = 8,
        CallLog = 9,
        ControlPanel = 10,
        DbLog = 11,
        Result = 12
    }
    public class ConfigNavBarViewModel : NovaAlert.Common.Mvvm.ViewModelBase
    {
        NavBarItem _privateAlarmNavBar;
        public IClientApp App { get; private set; } 

        public List<NavBarGroup> NavBarGroups { get; set; }

        NavBarGroup _selectedGroup;
        public NavBarGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; OnPropertyChanged("SelectedGroup"); }
        }

        ConfigViewModelBase _viewSelected;
        public ConfigViewModelBase ViewSelected { get { return _viewSelected; } set { _viewSelected = value; OnPropertyChanged("ViewSelected"); } }

        bool _isAllGroupCollapsed;
        public bool IsAllGroupCollapsed
        {
            get { return _isAllGroupCollapsed; }
            set { _isAllGroupCollapsed = value; OnPropertyChanged("IsAllGroupCollapsed"); }
        }

        public ConfigNavBarViewModel(IClientApp app)
        {
            this.App = app;
            CreateNavBar();
        }

        private void CreateNavBar()
        {
            this.NavBarGroups = new List<NavBarGroup>();
            var g = new NavBarGroup() { GroupName = "Chức năng" };
            g.Items.Add(new NavBarItem() { ItemName = "Báo động CTT", Command = new RelayCommand(p => SwitchWorkingMode(eWorkingMode.Alert)) });

            if(ClientSetting.Instance.Menu_CCPK_Visible == true)
            {
                g.Items.Add(new NavBarItem() { ItemName = "Báo động CCPK", Command = new RelayCommand(p => SwitchWorkingMode(eWorkingMode.CCPK_Alert)) });
            }            

            g.Items.Add(new NavBarItem() { ItemName = "Đa hướng", Command = new RelayCommand(p => SwitchWorkingMode(eWorkingMode.MultiDest)) });

            _privateAlarmNavBar = new NavBarItem() { ItemName = "Báo động nội bộ", Command = new RelayCommand(p => SwitchWorkingMode(eWorkingMode.Alarm)) };
            g.Items.Add(_privateAlarmNavBar);            

            this.NavBarGroups.Add(g);

            g = new NavBarGroup() { GroupName = "Nhật ký" };
            g.Items.Add(new NavBarItem() { ItemName = "Ghi âm", Command = new RelayCommand(p => SetSelectView(eConfigView.CallLog)) });
            g.Items.Add(new NavBarItem() { ItemName = "Thao tác", Command = new RelayCommand(p => SetSelectView(eConfigView.DbLog)) });
            this.NavBarGroups.Add(g);
            
            // if (Proxy.CreateProxy().GetParameterValue(eGlobalParameter.Menu_DisplayConfig_Visible) == "True")
            if (ClientSetting.Instance.Menu_DisplayConfig_Visible== true)
            {
                g = new NavBarGroup() { GroupName = "Hiển thị" };
                g.Items.Add(new NavBarItem() { ItemName = "Kết quả", Command = new RelayCommand(p => SetSelectView(eConfigView.Result)) });
                this.NavBarGroups.Add(g);
            }            

            g = new NavBarGroup() { GroupName = "Khai báo" };
            g.Items.Add(new NavBarItem() { ItemName = "Thời gian", Command = new RelayCommand(p => SetSelectView(eConfigView.Date)) });
            g.Items.Add(new NavBarItem() { ItemName = "Báo giờ", Command = new RelayCommand(p => SetSelectView(eConfigView.DayType)) });
            g.Items.Add(new NavBarItem() { ItemName = "Danh bạ", Command = new RelayCommand(p => SetSelectView(eConfigView.Contact)) });
            this.NavBarGroups.Add(g);

            g = new NavBarGroup() { GroupName = "Cài đặt" };
            g.Items.Add(new NavBarItem() { ItemName = "Âm hiệu", Command = new RelayCommand(p => SetSelectView(eConfigView.AlarmType)) });
            g.Items.Add(new NavBarItem() { ItemName = "Kênh thông tin", Command = new RelayCommand(p => SetSelectView(eConfigView.Channel)) });
            g.Items.Add(new NavBarItem() { ItemName = "Nhóm Đơn vị", Command = new RelayCommand(p => SetSelectView(eConfigView.Group)) });
            g.Items.Add(new NavBarItem() { ItemName = "Bàn điều khiển", Command = new RelayCommand(p => SetSelectView(eConfigView.ControlPanel)) });
            this.NavBarGroups.Add(g);
        }

        void SwitchWorkingMode(eWorkingMode mode)
        {
            SetSelectView(eConfigView.None);

            if (this.App != null)
            {
                this.App.WorkingMode = mode;
                ClearSelection();
            }
        }

        private void ClearSelection()
        {            
            foreach(var g in this.NavBarGroups)
            {
                foreach(var item in g.Items)
                {
                    item.IsSelected = false;
                }
            }
        }

        public void SetSelectView(eConfigView view)
        {
            var oldVM = ViewSelected;
            
            switch (view)
            {
                case eConfigView.Date:
                    ViewSelected = new DateConfigViewModel(this.App) { DisplayName = "KHAI BÁO THỜI GIAN" };
                    break;
                case eConfigView.DayType:
                    ViewSelected = new DayTypeConfigViewModel(this.App) { DisplayName = "KHAI BÁO BÁO GIỜ" };
                    break;
                case eConfigView.Contact:
                    ViewSelected = new ContactViewModel(this.App) { DisplayName = "KHAI BÁO DANH BẠ" };
                    break;
                case eConfigView.AlarmType:
                    ViewSelected = new DayTypeAlarmViewModel(this.App) { DisplayName = "KHAI BÁO ÂM HIỆU" };
                    break;
                case eConfigView.Group:
                    this.App.AddLog("Khai báo nhóm đơn vị", false);
                    ViewSelected = new GroupListViewModel(this.App) { DisplayName = "KHAI BÁO NHÓM ĐƠN VỊ" };
                    break;
                case eConfigView.Channel:
                    ViewSelected = new ChannelViewModel(this.App) { DisplayName = "KHAI BÁO KÊNH THÔNG TIN" };
                    break;
                case eConfigView.Unit:
                    //ViewSelected = new UnitListViewModel();
                    break;
                case eConfigView.PrivateAlarm:
                    //ViewSelected = new PrivateAlarmViewModel();
                    break;

                case eConfigView.CallLog:
                    ViewSelected = new CallLogSearchViewModel(this.App) { DisplayName = "NHẬT KÝ GHI ÂM" };
                    break;

                case eConfigView.DbLog:
                    ViewSelected = new SearchLogViewModel(this.App) { DisplayName = "NHẬT KÝ THAO TÁC" };
                    break;

                case eConfigView.ControlPanel:
                    ViewSelected = new ClientSettingViewModel(this.App) { DisplayName = "KHAI BÁO BÀN ĐIỀU KHIỂN" };
                    break;

                case eConfigView.Result:
                    ViewSelected = new DisplayDataViewModel(this.App) { DisplayName = "KHAI BÁO HIỂN THỊ KẾT QUẢ" };
                    break;                

                default:
                    ViewSelected = null;
                    break;
            }

            if (oldVM != null && oldVM is IDisposable) ((IDisposable)oldVM).Dispose();

            this.App.RefeshTitle();
        }

        public void Reset()
        {
            this.SelectedGroup = this.NavBarGroups.First();

            if(_privateAlarmNavBar != null)
            {
                var cfgService = Proxy.CreateProxy();
                _privateAlarmNavBar.IsVisible = !cfgService.IsAlartOnSwitch();
            }            
        }
    }

    public class NavBarItem : ViewModelBase
    {
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public RelayCommand Command { get; set; }
        bool _isSelected;
        public bool IsSelected
        { 
            get 
            { 
                return _isSelected; 
            } 
            set 
            { 
                _isSelected = value; 
                OnPropertyChanged("IsSelected"); 
            } 
        }

        bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; OnPropertyChanged("IsVisible"); }
        }

        public NavBarItem()
        {
            _isVisible = true;
        }
    }

    public class NavBarGroup: ViewModelBase
    {
        public string GroupName { get; set; }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        public List<NavBarItem> Items { get; set; }

        public NavBarGroup()
        {
            this.Items = new List<NavBarItem>();
        }
    }
}
