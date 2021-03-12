using NovaAlert.Common.Mvvm;
using System;

namespace NovaAlert.Entities.ViewModel
{
    public class TaskViewModel: ViewModelBase
    {
        Task _taskObj;
        public Task TaskObj
        {
            get { return _taskObj; }
            set
            {
                _taskObj = value;
                OnPropertyChanged("TaskObj");
                Refesh();
            }
        }

        public eTask CurrentTask
        {
            get { return this.TaskObj.CurrentTask; }
            set { this.TaskObj.CurrentTask = value; OnPropertyChanged("CurrentTask"); }
        }

        public eTaskLevel Level
        {
            get { return this.TaskObj.Level; }
            set { this.TaskObj.Level = value; OnPropertyChanged("Level"); }
        }

        public eTaskResult Result
        {
            get { return this.TaskObj.Result; }
            set 
            { 
                this.TaskObj.Result = value;

                var now = DateTime.Now;
                this.LastDuration = (long)(now - this.TaskObj.CreatedDate).TotalSeconds;
                this.TaskObj.CreatedDate = now;                
                OnPropertyChanged("Result"); 
            }
        }

        public TaskViewModel(Task t)
        {
            _taskObj = t;
        }

        public bool IsValid
        {
            get
            {
                return this.CurrentTask != eTask.None && this.Level != eTaskLevel.None;
            }
        }
        
        public long? LastDuration { get; private set; }
    }
}
