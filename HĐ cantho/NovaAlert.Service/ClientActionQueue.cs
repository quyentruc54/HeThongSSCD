using System;
using System.ComponentModel;
using System.Linq;
using NovaAlert.Common;
using NovaAlert.Entities;

namespace NovaAlert.Service
{
    public class ClientActionQueue: ObservableQueue<ClientAction>, INotifyPropertyChanged
    {
        object SyncObj = new object();
        public int MaxCount { get; private set; }

        ClientAction _selectedAction;
        public ClientAction SelectedAction
        {
            get { return _selectedAction; }
            set { _selectedAction = value; OnPropertyChanged("SelectedAction"); }
        }

        public ClientActionQueue(int max = 100)
        {
            this.MaxCount = max;
        }

        public override void Enqueue(ClientAction item)
        {
            lock (SyncObj)
            {
                if (this.Count() > MaxCount)
                {
                    this.Clear();
                    //this.Dequeue();
                }

                base.Enqueue(item);
                this.SelectedAction = item;
            }            
        }

        public void Add(Client client, eAction action, eResourceType? resourceType = null, int? resourceId = null)
        {
            // temporary comment to avoid bug where Dequeue
            lock (SyncObj)
            {
                this.Enqueue(new ClientAction() { Client = client, Action = action, ResourceType = resourceType, ResourceId = resourceId, Time = DateTime.Now });
            }            
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
