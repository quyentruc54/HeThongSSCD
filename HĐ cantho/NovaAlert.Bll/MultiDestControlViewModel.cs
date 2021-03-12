using System.Linq;
using NovaAlert.Bll.Controller;
using NovaAlert.Common.Mvvm;

namespace NovaAlert.Bll
{
    public class MultiDestControlViewModel: ViewModelBase
    {
        private bool _canRecord;
        public RelayCommand CallGroupCommand { get; set; }
        public RelayCommand HoldCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand RecordCommand { get; set; }        
        public RelayCommand GroupSelectCommand { get; set; }

        public MultiDestController Controller { get; private set; }

        public bool CanRecord
        {
            get { return _canRecord; }
            set { _canRecord = value; OnPropertyChanged(nameof(CanRecord)); }
        }

        public MultiDestControlViewModel(MultiDestController controller)
        {            
            this.Controller = controller;
            _canRecord = this.Controller.ClientApp.Service.CanRecord();

            this.HoldCommand = new RelayCommand(p => this.Controller.OnPutOnHold(), p => CanClear());
            this.ClearCommand = new RelayCommand(p => OnClear(), p => CanClear());
            this.CallGroupCommand = new RelayCommand(p => this.Controller.OnCallGroup(), p => this.Controller.CanConnect());
            this.RecordCommand = new RelayCommand(p => this.Controller.OnRecord(), p => CanRecord);
            this.GroupSelectCommand = new RelayCommand(p => OnGroupSelect(), p => CanChangeGroup());
        }

        bool CanChangeGroup()
        {
            return this.Controller.ClientApp.Units.Items.All(u => u.SelectedPanelId != this.Controller.ClientApp.ClientId) &&
                this.Controller.Controllers.Count == 0;
        }

        private void OnGroupSelect()
        {
            this.Controller.ClientApp.OnGroupSelect();
        }

        bool CanClear()
        {
            var controller = this.Controller.ActiveController as ICallController;
            return controller != null && controller.CanHold();
        }        

        void OnClear()
        {
            this.Controller.ClientApp.AddLog("Xóa");
            var controller = this.Controller.ActiveController as ICallController;
            if (controller != null && !controller.IsHolding)
            {
                if (controller is ConferenceController)
                {
                    ((ConferenceController)controller).Clear();
                }
                else
                {
                    controller.Finalise();
                }
            }
        }
    }
}
