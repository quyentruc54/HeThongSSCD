using NovaAlert.Common.Setting;

namespace NovaAlert.Entities.ViewModel
{
    public class UnitPhoneViewModel: PhoneViewModel
    {
        #region Properties
        public UnitPhone UnitPhone { get { return (UnitPhone)this.Phone; } }

        public TaskViewModel Task { get; private set; }

        public int ListOrder
        {
            get { return this.UnitPhone.ListOrder; }
            set { this.UnitPhone.ListOrder = value; OnPropertyChanged("ListOrder"); }
        }

        public string Password
        {
            get { return this.UnitPhone.Password; }
            set { this.UnitPhone.Password = value; OnPropertyChanged("Password"); }
        }

        public int PhoneNumberId { get { return this.UnitPhone.PhoneNumberId; } }

        private bool _isTaskChanged;
        public bool IsTaskChanged
        {
            get { return _isTaskChanged; }
            protected set { _isTaskChanged = value; OnPropertyChanged("IsTaskChanged"); }
        }        

        public virtual string TextTask { get { return null; } }
        public virtual string TextResult { get { return null; } }

        #endregion

        #region Ctor
        public UnitPhoneViewModel(UnitPhone phone)
            : base(phone)
        {
            this.Task = new TaskViewModel(phone.Task);
            this.Task.PropertyChanged += Task_PropertyChanged;
        } 
        #endregion

        #region Helpers
        protected override void OnSelectedPanelChanged()
        {
            if (this.SelectedPanelId.HasValue)
            {
                if (this.SelectedPanelId.Value == ClientSetting.Instance.ClientId)
                {
                    if (this.Status == ePhoneStatus.Free)
                    {
                        this.Status = ePhoneStatus.Selected;
                    }
                }
                else
                {
                    this.Status = ePhoneStatus.Occupied;
                }
            }
            else
            {
                this.Status = ePhoneStatus.Free;
            }
        }

        void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {            
            if (e.PropertyName == "TaskObj")
            {
                this.UnitPhone.Task = this.Task.TaskObj;
                this.IsTaskChanged = false;
                OnPropertyChanged("Task");
                OnPropertyChanged("TextTask");
                OnPropertyChanged("TextResult");
            }
            else if(e.PropertyName == "CurrentTask" || e.PropertyName == "Level") // || e.PropertyName == "Result")
            {
                this.IsTaskChanged = true;
                OnPropertyChanged("TextTask");
                OnPropertyChanged("TextResult");
            }                
        }

        public void RefeshTask()
        {
            OnPropertyChanged("Task");
            OnPropertyChanged("TextTask");
            OnPropertyChanged("TextResult");
        } 

        public bool CanReceive()
        {
            return this.Task.IsValid && (this.Task.Result == eTaskResult.Connected ||
                    (this.Task.Result != eTaskResult.NL && this.Task.Result != eTaskResult.CTT));
        }

        public bool CanReceiveCCPK()
        {
            return (this.Task.Level == eTaskLevel.TC || this.Task.Level == eTaskLevel.CA) && 
                (this.Task.Result == eTaskResult.Connected ||
                    (this.Task.Result != eTaskResult.NL && this.Task.Result != eTaskResult.CTT));
        }
        #endregion        
    }
}
