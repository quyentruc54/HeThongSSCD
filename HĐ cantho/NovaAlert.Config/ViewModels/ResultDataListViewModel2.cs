using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;

namespace NovaAlert.Config.ViewModels
{
    public class ResultDataListViewModelBase: ViewModelBase
    {
        #region Properties
        public ObservableCollection<ResultDataViewModel> Results { get; set; }
        ResultDataViewModel _selectedResult;
        public ResultDataViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if(_selectedResult != value)
                {
                    _selectedResult = value;
                    OnPropertyChanged("SelectedResult");
                    OnPropertyChanged("ShowBackButton");
                    OnPropertyChanged("Title");
                    OnPropertyChanged("IsSubResults");
                    OnPropertyChanged("IsHostResults");

                    this.Results.Clear();
                    UpdateDataFromServer();
                }                
            }
        }

        public int TimerInterval
        {
            get { return ClientSetting.Instance.ResultViewer_Interval; }
            set
            {
                if (value > 10) ClientSetting.Instance.ResultViewer_Interval = value;                    
                OnPropertyChanged("TimerInterval");
            }
        }

        public int DataRefeshInterval
        {
            get { return ClientSetting.Instance.ResultViewer_DataRefeshInterval; }
            set
            {
                if (value >= 10) ClientSetting.Instance.ResultViewer_DataRefeshInterval = value;
                OnPropertyChanged("DataRefeshInterval");
            }
        }

        public string Title
        {            
            get 
            {
                if (_selectedResult != null) return _selectedResult.UnitName;
                else return this.HostName;
            }
        }

        public bool IsSubResults { get { return _selectedResult != null; } }
        public bool IsHostResults { get { return !this.IsSubResults; } }

        public bool ShowSubResult
        {
            get { return ClientSetting.Instance.ResultViewer_ShowSubResult; }
            set
            {
                ClientSetting.Instance.ResultViewer_ShowSubResult = value;
                OnPropertyChanged("ShowSubResult");
            }
        }

        public RelayCommand ShowSubResultCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }
        public string HostName { get; private set; }
        public bool ShowBackButton { get { return _selectedResult != null; } }
        public DateTime LastUpdate { get; private set; }
        #endregion

        #region ctor
        public ResultDataListViewModelBase()
        {
            _selectedResult = null;
            var service = Proxy.CreateProxy();
            this.HostName = service.GetParameterValue(eGlobalParameter.OfficeName);

            var st = ClientSetting.Instance;
            UpdateDataFromServer();
            this.ShowSubResultCommand = new RelayCommand(p => OnShowSubResult((string)p), p => true);
            this.BackCommand = new RelayCommand(p => this.SelectedResult = null, p => _selectedResult != null);
        }
        #endregion

        #region Helpers

        void OnShowSubResult(string displayId)
        {
            var re = this.Results.Where(r => r.DisplayId == displayId).FirstOrDefault();
            this.SelectedResult = re;

        }
        public void UpdateDataFromServer()
        {
            this.LastUpdate = DateTime.Now;
            Func<List<ResultDataViewModel>> func = new Func<List<ResultDataViewModel>>(LoadData);
            func.BeginInvoke(LoadDataCompleted, null);
        }

        List<ResultDataViewModel> LoadData()
        {
            var results = new List<ResultDataViewModel>();

            try
            {
                var service = Proxy.CreateProxy();                

                if(this.SelectedResult == null)
                {
                    var cttResults = service.GetResults(eTaskType.CTT);

                    foreach (var item in cttResults)
                    {
                        ResultDataViewModel r = new ResultDataViewModel() { CTT = item };
                        results.Add(r);
                    }

                    cttResults = service.GetResults(eTaskType.CCPK);
                    foreach (var cttResult in cttResults)
                    {
                        var vm = results.FirstOrDefault(r => r.CTT != null && r.CTT.DisplayId == cttResult.DisplayId);
                        if (vm != null)
                        {
                            vm.CCPK = cttResult;
                        }
                    }
                }
                else
                {
                    var subResults = service.GetSubResults(this.SelectedResult.CTT.PhoneNumberId, eTaskType.CTT);
                    if (subResults != null)
                    {
                        foreach (var sr in subResults)
                        {
                            results.Add(new ResultDataViewModel() { CTT = sr });
                        }
                    }

                    if(this.SelectedResult.CCPK != null)
                    {
                        var ccpkResults = service.GetSubResults(this.SelectedResult.CCPK.PhoneNumberId, eTaskType.CCPK);
                        if (ccpkResults != null)
                        {
                            foreach (var ccpkResult in ccpkResults)
                            {
                                var vm = results.Where(r => r.CTT != null && r.CTT.DisplayId == ccpkResult.DisplayId).FirstOrDefault();
                                if (vm != null)
                                {
                                    vm.CCPK = ccpkResult;
                                }
                            }
                        }
                    }                    
                }
            }            
            catch
            {                
            }

            return results;
        }

        void LoadDataCompleted(IAsyncResult ar)
        {
            var ars = ar as AsyncResult;
            if (ars == null && ars.IsCompleted == false) return;

            var f = ars.AsyncDelegate as Func<List<ResultDataViewModel>>;
            if (f != null)
            {
                var list = f.EndInvoke(ars);
                var act = new Action<List<ResultDataViewModel>>(UpdateData);
                Application.Current.Dispatcher.BeginInvoke(act, list);
            }
        }

        protected virtual void UpdateData(List<ResultDataViewModel> list)
        {
            if (list == null) return;

            bool resetRequired = false;
            if (this.Results == null)
            {
                this.Results = new ObservableCollection<ResultDataViewModel>(list);
                resetRequired = true;
            }
            else
            {
                // remove if not exist in new results
                var removedItems = this.Results.Where(it => !list.Any(n => n.DisplayId == it.DisplayId)).ToList();
                resetRequired = removedItems.Count > 0;
                foreach(var item in removedItems)
                {
                    this.Results.Remove(item);
                }

                // update
                foreach(var item in list)
                {
                    var existItem = this.Results.Where(it => it.DisplayId == item.DisplayId).FirstOrDefault();
                    if(existItem == null)
                    {
                        this.Results.Add(item);
                        resetRequired = true;
                    }
                    else
                    {
                        existItem.UpdateData(item);
                    }
                }
            }

            if(resetRequired) OnPropertyChanged("Results");
        }

        object _syncRefeshData = new object();
        public void CheckRefeshData()
        {
            lock (_syncRefeshData)
            {
                var now = DateTime.Now;
                if ((now - this.LastUpdate).TotalSeconds > this.DataRefeshInterval)
                {
                    UpdateDataFromServer();
                }
            }
        }
        #endregion
    }

    public class ResultDataListViewModel2: ResultDataListViewModelBase
    {
        public double ScrollDistance
        {
            get { return ClientSetting.Instance.ResultViewer_ScrollDistance; }
            set
            {
                if (value > 0) ClientSetting.Instance.ResultViewer_ScrollDistance = value;
                OnPropertyChanged("ScrollDistance");
            }
        }

        public bool ShowCTT
        {
            get { return ClientSetting.Instance.ResultViewer_ShowCTT; }
            set
            {
                ClientSetting.Instance.ResultViewer_ShowCTT = value;
                OnPropertyChanged("ShowCTT");
            }
        }
        public bool ShowPK
        {
            get { return ClientSetting.Instance.ResultViewer_ShowPK; }
            set
            {
                ClientSetting.Instance.ResultViewer_ShowPK = value;
                OnPropertyChanged("ShowPK");
            }
        }

        public bool ShowVerticalLines
        {
            get { return ClientSetting.Instance.ResultViewer_ShowVerticalLines; }
            set
            {
                ClientSetting.Instance.ResultViewer_ShowVerticalLines = value;
                OnPropertyChanged("ShowVerticalLines");
            }
        }

        public bool AutoScroll
        {
            get { return ClientSetting.Instance.ResultViewer_AutoScroll; }
            set
            {
                ClientSetting.Instance.ResultViewer_AutoScroll = value;
                OnPropertyChanged("AutoScroll");
            }
        }
    }

    public class ResultDataListViewModel3: ResultDataListViewModelBase
    {
        public List<ResultDataViewModel> LeftResults 
        {
            get
            {
                if (this.Results == null)
                {
                    return null;
                }

                var median = GetMedian();
                return this.Results.Where(r => r.CTT.DisplayId <= median).ToList();
            }
        }

        public List<ResultDataViewModel> RightResults
        {
            get
            {
                if (this.Results == null)
                {
                    return null;
                }
                var median = GetMedian();
                return this.Results.Where(r => r.CTT.DisplayId > median).ToList();
            }
        }

        private int GetMedian()
        {
            var median = this.Results.Count / 2 + this.Results.Count % 2;
            return median;
        }

        protected override void UpdateData(List<ResultDataViewModel> list)
        {
            base.UpdateData(list);
            OnPropertyChanged("LeftResults");
            OnPropertyChanged("RightResults");
        }
    }
}