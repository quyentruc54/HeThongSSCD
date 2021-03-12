using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using NovaAlert.Entities.Enums;

namespace NovaAlert.Bll
{
    public class AlertMenuViewModel: MenuViewModel
    {
        public RelayCommand SwitchModeCommand { get; private set; }

        public eAlertMode Mode { get; private set; }    
    
        public string ModeName
        {
            get
            {
                if (this.Mode == eAlertMode.Call)
                {
                    return "THOẠI";
                }
                return "TSL";
            }
        }

        public bool IsTransferMode { get { return this.App.WorkingMode == eWorkingMode.TSL_Alert; } }
        public bool IsVoiceMode { get { return this.App.WorkingMode == eWorkingMode.Alert; } }

        public bool ShowSwithModeButton { get; private set; }

        public AlertMenuViewModel(ClientAppViewModel app, eAlertMode mode = eAlertMode.Call):base(app)
        {
            this.Mode = mode;            
            this.ShowSwithModeButton = (this.App.WorkingMode == eWorkingMode.Alert || this.App.WorkingMode == eWorkingMode.TSL_Alert) && 
                string.Compare(app.Service.GetParameterValue(eGlobalParameter.ShowTSL), "true", true) == 0;
        }

        protected override void InitCommand()
        {
            base.InitCommand();
            this.SwitchModeCommand = new RelayCommand(p => OnSwitchMode());
        }

        private void OnSwitchMode()
        {
            if (this.App.WorkingMode == eWorkingMode.Alert)
            {
                this.App.WorkingMode = eWorkingMode.TSL_Alert;
            }
            else
            {
                this.App.WorkingMode = eWorkingMode.Alert;
            }
        }
    }
}
