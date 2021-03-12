using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using NovaAlert.Common.Utils;
using System;

namespace NovaAlert.Entities.ViewModel
{
    public abstract class PhoneViewModel: ViewModelBase, IResource
    {
        #region Properties
        public Phone Phone { get; private set; }

        public int Id { get { return this.Phone.Id; } }

        public string AreaCode
        {
            get { return this.Phone.AreaCode; }
            set { this.Phone.AreaCode = value; OnPropertyChanged("AreaCode"); }
        }

        public string Number
        {
            get { return this.Phone.Number; }
            set { this.Phone.Number = value; OnPropertyChanged("Number"); }
        }

        public string FullNumber
        {
            get
            {
                return GeneralUtils.FormatPhoneNumber(this.AreaCode, this.Number);
            }
        }

        public bool IsRestricted
        {
            get { return this.Phone.IsRestricted; }
        }

        public string Name
        {
            get { return this.Phone.Name; }
            set { this.Phone.Name = value; OnPropertyChanged("Name"); }
        }

        ePhoneStatus _status;
        public ePhoneStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        eLineStatus _lineStatus;
        public eLineStatus LineStatus
        {
            get { return _lineStatus; }
            set
            {
                if (_lineStatus != value)
                {
                    _lineStatus = value;
                    OnPropertyChanged("LineStatus");
                    OnLineStatusChanged();
                }
            }
        }

        public new bool IsBusy
        {
            get { return this.Status != ePhoneStatus.Free; }
        }

        string _tone;
        public string Tone
        {
            get { return _tone; }
            set { _tone = value; OnPropertyChanged("Tone"); }
        }

        string _callerId;
        public string CallerId
        {
            get { return _callerId; }
            set { _callerId = value; OnPropertyChanged("CallerId"); }
        }

        public virtual eResourceType ResourceType
        {
            get { return this.Phone.ResourceType; }
        }

        byte? _selectedPanelId;
        public byte? SelectedPanelId
        {
            get { return _selectedPanelId; }
            set 
            {
                _selectedPanelId = value;
                OnPropertyChanged("SelectedPanelId");
                OnSelectedPanelChanged();               
            }
        }

        private bool _isExternalOffHook;
        public bool IsExternalOffHook
        {
            get { return _isExternalOffHook; }
            set
            {
                if(_isExternalOffHook != value)
                {
                    _isExternalOffHook = value;
                    OnPropertyChanged(nameof(IsExternalOffHook));
                }                
            }
        }

        public DateTime LastClicked { get; set; }

        public RelayCommand OnClickCommand { get; private set; }
        public event EventHandler OnClickHandler; 
        #endregion

        #region Ctor
        public PhoneViewModel(Phone phone)
        {
            this.Phone = phone;
            this.LastClicked = DateTime.Now;
            _status = ePhoneStatus.Free;
            _lineStatus = eLineStatus.Good;
            
            this.OnClickCommand = new RelayCommand(p => OnClick());
        } 
        #endregion

        #region Helpers
        public virtual void OnClick()
        {
            LastClicked = DateTime.Now;
            if (this.OnClickHandler != null)
            {
                this.OnClickHandler(this, System.EventArgs.Empty);
            }
        }

        public virtual void ApplyChanges(ChannelStatusChangedEventArgs e, bool applyTone = true)
        {
            if (e.Status.HasValue)
            {
                this.LineStatus = e.Status.Value;
            }

            if (applyTone && !string.IsNullOrEmpty(e.Tone))
            {
                this.Tone = e.Tone;
            }

            if (!string.IsNullOrEmpty(e.CallerId))
            {
                this.CallerId = e.CallerId;
            }
        }

        public string GetFullNumber()
        {
            return string.Format("{0}{1}", this.AreaCode, this.Number);
        }

        private void OnLineStatusChanged()
        {
            switch (this.LineStatus)
            {
                case eLineStatus.Disconnect:
                    this.Status = ePhoneStatus.Disconnect;
                    break;

                case eLineStatus.Occupied:
                    if (this.SelectedPanelId == ClientSetting.Instance.ClientId)
                    {
                        if (this.Status == ePhoneStatus.Free || this.Status == ePhoneStatus.Ring)
                        {
                            this.Status = ePhoneStatus.Selected;
                        }
                    }
                    else
                    {
                        this.Status = ePhoneStatus.Occupied;
                    }                        
                    break;

                case eLineStatus.Good:
                    this.Status = ePhoneStatus.Free;
                    break;

                case eLineStatus.Ring:
                    this.Status = ePhoneStatus.Ring;
                    break;

                case eLineStatus.ExtOffHook:
                    this.Status = ePhoneStatus.Occupied;
                    break;

                default:                    
                    this.Status = ePhoneStatus.Free;
                    break;
            }
        }

        protected virtual void OnSelectedPanelChanged()
        {
        }
        #endregion
    }
}
