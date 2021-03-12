using NovaAlert.Common;
using NovaAlert.Config;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho chế độ truyền số liệu
    /// </summary>
    public class TSL_AlertController : MainController
    {
        public TSL_AlertOptionViewModel OptionVM { get; private set; }
        public TSL_AlertControlViewModel ControlVM { get; private set; }
        public TSL_UnitPhoneListViewModel Units { get; private set; }
        public TSL_ChannelListViewModel Channels { get; set; }
        public ModemViewModel Modem { get; set; }

        TSL_Task _currentTask;
        public TSL_Task CurrentTask
        {
            get { return _currentTask; }
            set 
            { 
                _currentTask = value;
                if (_currentTask != null) _currentTask.Start();

                OnPropertyChanged("CurrentTask");
                OnPropertyChanged("IsPreparing");
                OnPropertyChanged("IsReceiving");
                OnPropertyChanged("IsBusy");
            }
        }

        public bool IsPreparing
        {
            get { return this.CurrentTask != null && this.CurrentTask.TaskType == eTslStatusType.Prepare; }
        }

        public bool IsReceiving
        {
            get { return this.CurrentTask != null && this.CurrentTask.TaskType == eTslStatusType.Result; }
        }

        public bool IsBusy
        {
            get { return IsPreparing || IsReceiving; }
        }

        public Queue<TSL_ALertUnitPhoneViewModel> Queue { get; private set; }

        public TSL_AlertController(ClientAppViewModel app)
            : base(app, false)
        {
            this.Units = new TSL_UnitPhoneListViewModel(app);// { ShowTask = true, ShowResult = true };
            this.Channels = new TSL_ChannelListViewModel(app);
            this.OptionVM = new TSL_AlertOptionViewModel(this);
            this.ControlVM = new TSL_AlertControlViewModel(this);

            this.Modem = this.Channels.Items.OfType<ModemViewModel>().First();
            this.Modem.PropertyChanged += Modem_PropertyChanged;
        }

        void Modem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedPanelId" && this.Modem.SelectedPanelId == this.ClientApp.ClientId)
            {
                lock(this)
                {
                    Monitor.PulseAll(this);
                }
            }
        }

        List<TSL_ALertUnitPhoneViewModel> GetSelectedUnits()
        {
            return this.Units.GetSelectedItems().OfType<TSL_ALertUnitPhoneViewModel>().ToList();
        }

        public bool CanPrepare()
        {
            if (this.Modem.SelectedPanelId.HasValue && this.Modem.SelectedPanelId.Value != this.ClientApp.ClientId)
            {
                return false;
            }

            if (this.IsReceiving)
            {
                return false;
            }

            if (this.IsPreparing)
            {
                return !this.CurrentTask.IsCanceledByUser;
            }

            var selectedUnits = this.GetSelectedUnits();
            return selectedUnits.Count > 0 && selectedUnits.All(u => u.PrepareStatus == eTslStatus.None);
        }

        public void OnDoTask(eTslStatusType type)
        {
            if (this.CurrentTask != null)
            {
                this.CurrentTask.IsCanceledByUser = true;
                this.ClientApp.Service.CancelTslTask();
                return;
            }
            else
            {
                var selectedUnits = this.GetSelectedUnits();
                var task = new TSL_Task(this, selectedUnits, type);
                task.PropertyChanged += task_PropertyChanged;
                this.CurrentTask = task;
            }
        }

        void task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var task = sender as TSL_Task;
            if (task == null) return;

            if(e.PropertyName == "StatusText")
            {
                if (this.ClientApp.WorkingMode == eWorkingMode.TSL_Alert)
                {
                    this.ClientApp.ShowInfo(task.StatusText);
                }
            }
            else if (e.PropertyName == "IsCompleted" && task.IsCompleted)
            {
                task.PropertyChanged -= task_PropertyChanged;
                this.CurrentTask = null;
            }
        }

        public bool CanReceiveResult()
        {
            if (this.Modem.SelectedPanelId.HasValue && this.Modem.SelectedPanelId.Value != this.ClientApp.ClientId)
            {
                return false;
            }

            if (this.IsPreparing)
            {
                return false;
            }

            if (this.IsReceiving)
            {
                return !this.CurrentTask.IsCanceledByUser;
            }

            var selectedUnits = this.GetSelectedUnits();
            return selectedUnits.Count > 0 && selectedUnits.All(u => u.ResultStatus == eTslStatus.None);
        }        

        public bool CanShowResult()
        {
            return this.GetSelectedUnits().Count == 1;
        }

        public void OnShowResult()
        {
            var unit = this.GetSelectedUnits().FirstOrDefault();
            if (unit == null)
            {
                return;
            }
            var results = this.ClientApp.Service.GetSubResults(unit.PhoneNumberId, eTaskType.CTT);
            ConfigServices.ShowResults(results, eTaskType.CTT, unit.Name);
        }

        public override void OnTslStatusChanged(int phoneNumberId, eTslStatusType type, eTslStatus result, int? hostClientId)
        {
            var unit = this.ClientApp.GetUnits(u => u.PhoneNumberId == phoneNumberId)
                .OfType<TSL_ALertUnitPhoneViewModel>().FirstOrDefault();            

            if (unit != null)
            {
                if (type == eTslStatusType.Prepare)
                {
                    unit.PrepareStatus = result;
                }
                else
                {
                    unit.ResultStatus = result;
                }
            }

            if (hostClientId == this.ClientApp.ClientId)
            {
                lock (this)
                {
                    Monitor.PulseAll(this);
                }
            }
        }

        public void ClearStatus(eTslStatusType type)
        {
            var units = GetSelectedUnits();
            if (units.Count == 0)
            {
                this.ClientApp.ShowError("Vui lòng chọn đơn vị cần xóa trạng thái.");
            }
            else
            {
                foreach (var u in units)
                {
                    this.ClientApp.Service.UpdateTslStatus(u.PhoneNumberId, type, eTslStatus.None);
                }

                this.ClientApp.AddLog(string.Format("Xóa trạng thái {0} của {1}",
                    type == eTslStatusType.Prepare ? "CBNL" : "nhận KQ", TranslateToText(units)));
            }
        }

        public static string TranslateToText(List<TSL_ALertUnitPhoneViewModel> units)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}đơn vị ", units.Count > 1 ? "các " : "");
            sb.Append(string.Join(", ", units.Select(u => u.Name)));
            return sb.ToString();
        }

        #region Client side
        public override void OnRequestPrepare()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var msg = "Yêu cầu chuẩn bị nhận lệnh từ chỉ huy.";
                    this.ClientApp.Service.ReplyFromRepareRequest(this.ClientApp.ClientId);
                    ServiceContainer.Instance.GetService<IMessageBoxService>().ShowInfo(msg, "Chuẩn bị nhận lệnh.");
                    this.ClientApp.AddLog("Nhận y/c nhận lệnh từ chỉ huy.");
                }));            
        }
        #endregion

        #region Override
        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
        }

        public override void ReleaseAllFreeChannels()
        {            
        }
        #endregion

        public void Refesh()
        {
            this.Units = new TSL_UnitPhoneListViewModel(this.ClientApp);
        }
    }
}
