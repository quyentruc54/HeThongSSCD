using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Wpf;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovaAlert.Config.ViewModels
{
    public class SearchLogViewModel : ConfigViewModelBase
    {
        public List<LogItem> SearchResults { get; private set; }

        public RelayCommand SearchCommand { get; private set; }

        DateTime _searchDate;
        public DateTime SearchDate
        {
            get { return _searchDate; }
            set { _searchDate = value; OnPropertyChanged("SearchDate"); }
        }

        string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged("SearchText"); }
        }

        public List<KeyValuePair<byte, string>> PanelList
        {
            get
            {
                return new List<KeyValuePair<byte, string>>()
                {
                    new KeyValuePair<byte, string>(0, "Tất cả"),
                    new KeyValuePair<byte, string>(1, "BĐK 1"),
                    new KeyValuePair<byte, string>(2, "BĐK 2"),
                    new KeyValuePair<byte, string>(3, "BĐK 3"),
                    new KeyValuePair<byte, string>(4, "BĐK 4"),
                };
            }
        }

        byte _selectedPanelId;
        public byte SelectedPanelId
        {
            get { return _selectedPanelId; }
            set { _selectedPanelId = value; OnPropertyChanged("SelectedPanelId"); }
        }

        public SearchLogContract SearchContract { get; private set; }

        public SearchLogViewModel(IClientApp app):base(app)
        {
            _searchDate = DateTime.Now;
            _searchText = null;
            this.SearchCommand = new RelayCommand(p => OnSearch());
            this.App.AddLog("Xem nhật thao tác", false);
            OnSearch();
        }

        void OnSearch()
        {
            this.SearchContract = new SearchLogContract(this.SearchDate, this.SearchText, this.SelectedPanelId);
            OnPropertyChanged("SearchContract");
        }
    }

    public class SearchLogContract: IPageControlContract
    {
        public DateTime Date { get; private set; }
        public string Text { get; private set; }
        public byte PanelId { get; set; }

        public SearchLogContract(DateTime date, string text, byte panelId)
        {
            this.Date = date;
            this.Text = text;
            this.PanelId = panelId;
        }

        public uint GetTotalCount()
        {
            return Proxy.CreateProxy().CountLog(this.PanelId, this.Date, this.Text);
        }

        public ICollection<object> GetRecordsBy(uint startingIndex, uint numberOfRecords, object filterTag)
        {
            return Proxy.CreateProxy().SearchLog(this.PanelId, this.Date, this.Text, (int)startingIndex, (int)numberOfRecords).ToList<object>();
        }
    }
}
