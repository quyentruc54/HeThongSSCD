using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NovaAlert.Bll.Controller
{
    public class TSL_Task: ViewModelBase
    {
        const string NotSuccess = " --> không thành công.";
        const string Success = " --> xong.";

        public List<TSL_ALertUnitPhoneViewModel> Units { get; private set; }
        public eTslStatusType TaskType { get; private set; }
        public TSL_AlertController Controller { get; set; }

        bool _isCanceledByUser;
        public bool IsCanceledByUser
        {
            get { return _isCanceledByUser; }
            set 
            { 
                _isCanceledByUser = value;
                if (_isCanceledByUser)
                {
                    AddLog("Thao tao bị hủy bởi người sử dụng.");
                }
            }
        }

        bool _isCompleted;
        public bool IsCompleted
        {
            get { return _isCompleted; }
            private set { _isCompleted = value; OnPropertyChanged("IsCompleted"); }
        }

        int _retryCount;
        StringBuilder _sb;

        public string StatusText
        {
            get { return _sb.ToString(); }
        }

        public TSL_Task(TSL_AlertController controller, List<TSL_ALertUnitPhoneViewModel> units, eTslStatusType type)
        {
            _isCompleted = false;
            _isCanceledByUser = false;
            _sb = new StringBuilder();

            this.Controller = controller;
            this.Units = units;
            this.TaskType = type;

            var desc = (string.Format("Gởi y/c {0} đến {1}", type == eTslStatusType.Prepare ? "CBNL" : "nhận KQ", TSL_AlertController.TranslateToText(units)));
            this.Controller.ClientApp.AddLog(desc);
            this.AddLog(desc);

            string s = string.Empty;
            if (this.TaskType == eTslStatusType.Prepare)
            {
                s = this.Controller.ClientApp.Service.GetParameterValue(eGlobalParameter.TSL_PrepareTries);
            }
            else
            {
                s = this.Controller.ClientApp.Service.GetParameterValue(eGlobalParameter.TSL_ReceiveTries);
            }

            if (!int.TryParse(s, out _retryCount) || _retryCount < 1)
            {
                _retryCount = 1;
            }
        }

        public void Start()
        {
            Action act = new Action(WorkerThread);
            act.BeginInvoke(null, null);
        }

        public void WorkerThread()
        {
            lock (this.Controller)
            {
                AddLog("Yêu cầu khối xử lý trung tâm kết nối với Modem.");
                this.Controller.ClientApp.Service.Request(eResourceType.Modem, 0);
                var timeout = !Monitor.Wait(this.Controller, 2000);
                int count = 1;

                if (!timeout)
                {                    
                    while (count <= _retryCount && !IsCanceledByUser)
                    {
                        if (_retryCount > 1)
                        {
                            AddLog(string.Format("Kết nối lần {0}", count));
                        }

                        count++;
                        DoTask(this.Units);

                        if (count <= _retryCount)
                        {
                            Thread.Sleep(10000);
                        }
                    }
                }
                else
                {
                    AddLog(NotSuccess, false);
                }

                this.Controller.ClientApp.Service.Release(eResourceType.Modem, 0);
                this.IsCompleted = true;
            }
        }

        void DoTask(List<TSL_ALertUnitPhoneViewModel> units)
        {
            foreach(var u in units)
            {
                if (this.IsCanceledByUser)
                {
                    break;
                }

                AddLog(string.Format("Kết nối đến {0}", u.Name));
                this.Controller.ClientApp.Service.DoTslTask(u.PhoneNumberId, this.TaskType);
                var timeout = !Monitor.Wait(this.Controller, 60000);
                if (timeout)
                {
                    AddLog(NotSuccess, false);
                }
                else
                {
                    AddLog(Success, false);
                }
            }
        }

        void AddLog(string s, bool newline = true)
        {
            if (newline && _sb.Length > 0)
            {
                _sb.Append(System.Environment.NewLine);
            }
            _sb.Append(s);                   
            OnPropertyChanged("StatusText");
        }
    }
}
