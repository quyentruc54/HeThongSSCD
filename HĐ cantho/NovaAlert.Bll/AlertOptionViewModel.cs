using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using NovaAlert.Bll.Controller;

namespace NovaAlert.Bll
{
    public abstract class AlertOptionViewModelBase: ViewModelBase
    {
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand ClearCommand { get; private set; }
        public AlertController Controller { get; private set; }

        public AlertOptionViewModelBase(AlertController controller)
        {
            this.Controller = controller;
            this.SaveCommand = new RelayCommand(p => this.Controller.CommitTaskChanges());
            this.ClearCommand = new RelayCommand(p => this.Controller.ClearTask());
        }
    }

    public class CCPK_AlertOptionViewModel : AlertOptionViewModelBase
    {
        public RelayCommand TC_Command { get; private set; }
        public RelayCommand CA_Command { get; private set; }

        public CCPK_AlertOptionViewModel(AlertController controller)
            : base(controller)
        {
            this.TC_Command = new RelayCommand(p => this.Controller.ChangeTaskLevel(eTaskLevel.TC));
            this.CA_Command = new RelayCommand(p => this.Controller.ChangeTaskLevel(eTaskLevel.CA));
        }
    }

    public class AlertOptionViewModel : AlertOptionViewModelBase
    {
        public RelayCommand A_Command { get; private set; }
        public RelayCommand A2_Command { get; private set; }
        public RelayCommand A3_Command { get; private set; }
        public RelayCommand A4_Command { get; private set; }
        public RelayCommand TC_Command { get; private set; }
        public RelayCommand CA_Command { get; private set; }
        public RelayCommand TB_Command { get; private set; }
        
        public AlertOptionViewModel(AlertController controller):base(controller)
        {

            this.A_Command = new RelayCommand(p => this.Controller.ChangeTask(eTask.A));
            this.A2_Command = new RelayCommand(p => this.Controller.ChangeTask(eTask.A2));
            this.A3_Command = new RelayCommand(p => this.Controller.ChangeTask(eTask.A3));
            this.A4_Command = new RelayCommand(p => this.Controller.ChangeTask(eTask.A4));

            this.TC_Command = new RelayCommand(p => this.Controller.ChangeTaskLevel(eTaskLevel.TC));
            this.CA_Command = new RelayCommand(p => this.Controller.ChangeTaskLevel(eTaskLevel.CA));
            this.TB_Command = new RelayCommand(p => this.Controller.ChangeTaskLevel(eTaskLevel.TB));
        }
    }
}
