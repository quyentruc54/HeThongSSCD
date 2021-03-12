using System.Linq;
using NovaAlert.Common.Mvvm;
using NovaAlert.Bll.Controller;
using NovaAlert.Config;
using NovaAlert.Entities;

namespace NovaAlert.Bll
{
    public class AlertControlViewModel: ViewModelBase
    {        
        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand IssueCommand { get; private set; }
        public RelayCommand ReceiveCommand { get; private set; }
        public RelayCommand ChangeStatusCommand { get; private set; }
        public RelayCommand ResultCommand { get; private set; }
        public RelayCommand GroupSelectCommand { get; private set; }
        
        public AlertController Controller { get; private set; }
        public eTaskType TaskType { get; private set; }

        public string FinishButtonCaption { get; private set; }

        public AlertControlViewModel(AlertController controller, eTaskType taskType)
        {
            this.TaskType = taskType;
            this.Controller = controller;
            this.ConnectCommand = new RelayCommand(p => this.Controller.OnConnect(), p => this.Controller.CanConnect());

            this.IssueCommand = new RelayCommand(p => this.Controller.OnIssue(), p => this.Controller.CanIssue());

            this.ReceiveCommand = new RelayCommand(p => this.Controller.OnReceive(), p => this.Controller.CanReceive());

            this.ChangeStatusCommand = new RelayCommand(p => this.Controller.OnChangeStatus(), p => this.Controller.CanChangeStatus());

            this.ResultCommand = new RelayCommand(p => OnViewResult());

            this.GroupSelectCommand = new RelayCommand(p => OnGroupSelect(), p => CanChangeGroup());

            if (this.TaskType == eTaskType.CTT)
            {
                this.FinishButtonCaption = "ĐÃ CTT";
            }
            else
            {
                this.FinishButtonCaption = "ĐÃ CC";
            }
        }

        private void OnGroupSelect()
        {
            this.Controller.ClientApp.OnGroupSelect();
        }

        private void OnViewResult()
        {
            var results = this.Controller.ClientApp.Service.GetResults(this.TaskType);            
            ConfigServices.ShowResults(results, this.TaskType, this.Controller.ClientApp.OfficeName);    
        }

        bool CanChangeGroup()
        {
            return !this.Controller.ClientApp.GetSelectedUnits().Any();
        }
    }
}
