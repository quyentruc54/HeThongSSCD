using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Wpf;
using NovaAlert.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace NovaAlert.Config.ViewModels
{
    public class CallLogSearchViewModel: ConfigViewModelBase
    {
        #region Properties
        
        //INovaAlertConfigService _service = Proxy.CreateProxy();
        public SearchCallLogCriteria Criteria { get; private set; }
        public ObservableCollection<SearchCallLogResult> SearchResults { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public List<KeyValuePair<byte, string>> CallTypeList
        {
            get
            {
                var list = new List<KeyValuePair<byte, string>>();
                list.Add(new KeyValuePair<byte, string>(0, "Tất cả"));
                list.Add(new KeyValuePair<byte, string>(1, "Cuộc gọi đơn"));
                list.Add(new KeyValuePair<byte, string>(2, "Cuộc gọi đến"));
                list.Add(new KeyValuePair<byte, string>(3, "Cuộc gọi nhóm"));
                list.Add(new KeyValuePair<byte, string>(4, "BĐ-CTT"));
                if (Proxy.CreateProxy().GetParameterValue(eGlobalParameter.Menu_CCPK_Visible) == "True")
                {
                    list.Add(new KeyValuePair<byte, string>(5, "BĐ-CCPK"));
                }
                return list;
            }
        }
        public List<KeyValuePair<int?, string>> ChannelList { get; private set; }
        
        public IMediaPlayer Player { get; private set; }

        SearchCallLogResult _selectedCallLog;
        public SearchCallLogResult SelectedCallLog
        {
            get { return _selectedCallLog; }
            set
            {
                if(_selectedCallLog != value)
                {
                    _selectedCallLog = value;
                    OnPropertyChanged("SelectedCallLog");
                    OnSelectedCallLogChanged();
                }                
            }
        }

        CallLogDetail _selectedCallLogDetail;
        public CallLogDetail SelectedCallLogDetail
        {
            get { return _selectedCallLogDetail; }
            set 
            { 
                if(_selectedCallLogDetail != value)
                {
                    _selectedCallLogDetail = value;
                    OnPropertyChanged("SelectedCallLogDetail"); 
                    
                    if (_selectedCallLogDetail != null && !string.IsNullOrEmpty(_selectedCallLogDetail.Record))
                    {
                        //var fileName = System.IO.Path.Combine(this.RecordFolder, string.Format(@"Kenh {0}\{1}", _selectedCallLogDetail.ChannelId - 1, _selectedCallLogDetail.Record));
                        var fileName = System.IO.Path.Combine(this.RecordFolder, _selectedCallLogDetail.GetRecordFileName());
                        if (System.IO.File.Exists(fileName)) this.Player.FileName = fileName;
                    }
                    else
                        this.Player.FileName = null;

                }                
            }
        }

        public string RecordFolder { get; private set; }

        public CallLogSearchContract SearchContract { get; private set; }
        
        #endregion

        #region Ctor
        public CallLogSearchViewModel(IClientApp app):base(app)
        {
            this.DisplayName = "NHẬT KÝ GHI ÂM";
            this.App.AddLog("Xem nhật ký ghi âm", false);
            this.RecordFolder = this.App.Service.GetRecordFolder();
            this.Criteria = new SearchCallLogCriteria();
            this.SearchCommand = new RelayCommand(p => DoSearch());

            var list = Proxy.CreateProxy().GetAllChannels();
            this.ChannelList = new List<KeyValuePair<int?, string>>();
            this.ChannelList.Add(new KeyValuePair<int?, string>(null, "Tất cả"));
            foreach(var c in list)
            {
                this.ChannelList.Add(new KeyValuePair<int?, string>(c.ChannelId, c.Number));
            }

            this.Player = this.App.MediaPlayer;
            this.Player.Volume = 5;
            this.Player.FileName = null;
            this.Player.EnableLooping = false;

            this.DeleteCommand = new RelayCommand(p => OnDelete(), p => this.SelectedCallLog != null);

            DoSearch();
        } 
        #endregion

        private void DoSearch()
        {            
            this.SearchContract = new CallLogSearchContract(this.Criteria);            
            OnPropertyChanged("SearchContract");
        }

        protected virtual void ShowSearchResult(List<SearchCallLogResult> list)
        {
            this.SearchResults = new ObservableCollection<SearchCallLogResult>(list);
            OnPropertyChanged("SearchResults");
        }

        void OnSelectedCallLogChanged()
        {
            if(this.SelectedCallLog != null)
            {
                this.SelectedCallLogDetail = this.SelectedCallLog.Details.FirstOrDefault();
            }
        }

        protected override void OnDispose()
        {
            if (this.Player.IsPlaying) this.Player.StopCommand.Execute(null);
            base.OnDispose();
        }

        void OnDelete()
        {
            var selectedCallLogs = GetSelectedCallLogs();
            if (!selectedCallLogs.Any())
            {
                return;
            }

            string msg = string.Format("Bạn có chắc chắn xóa {0}cuộc gọi này không ?", selectedCallLogs.Skip(1).Any() ? selectedCallLogs.Count().ToString() + " " : "" );

            if (ServiceContainer.Instance.GetService<IMessageBoxService>().AskYesNo(msg) == MessageBoxResult.No)
            {
                return;
            }

            foreach(var item in selectedCallLogs)
            {
                if (item.Details.Contains(this.SelectedCallLogDetail))
                {
                    this.SelectedCallLogDetail = null;
                }
                Proxy.CreateProxy().DeleteCallLog(item);
            }
            
            DoSearch();
        }

        IEnumerable<SearchCallLogResult> GetSelectedCallLogs()
        {
            var selectedCallLogs = this.SearchContract.CurrentItems.Where(c => c.Selected);
            if (!selectedCallLogs.Any())
            {
                yield return this.SelectedCallLog;
            }
            else
            {
                foreach(var callLog in selectedCallLogs)
                {
                    yield return callLog;
                }                
            }
        }
    }

    public class CallLogSearchContract: IPageControlContract, INotifyPropertyChanged
    {
        public SearchCallLogCriteria Criteria { get; private set; }

        public CallLogSearchContract(SearchCallLogCriteria cr)
        {
            this.Criteria = cr;
        }

        public uint GetTotalCount()
        {
            return Proxy.CreateProxy().CountCallLog(this.Criteria);
        }

        public List<SearchCallLogResult> CurrentItems { get; private set; }

        public ICollection<object> GetRecordsBy(uint startingIndex, uint numberOfRecords, object filterTag)
        {
            this.Criteria.StartIndex = (int)startingIndex;
            this.Criteria.NumOfRecords = (int)numberOfRecords;
            this.CurrentItems = Proxy.CreateProxy().SearchCallLog(this.Criteria);
            OnPropertyChanged("CurrentItems");
            return this.CurrentItems.ToList<object>();
        }        
    
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
