using System;
using System.Collections.Generic;
using System.Linq;
using NovaAlert.Common.Mvvm;
using NovaAlert.Entities.ViewModel;
using NovaAlert.Common;
using System.Collections.Specialized;

namespace NovaAlert.Bll
{
    public abstract class PhoneListViewModel<T>: ViewModelBase where T:PhoneViewModel
    {
        public POViewModel PO { get; protected set; }
        public bool HasPO
        {
            get { return this.PO != null; }
        }

        public ClientAppViewModel App { get; private set; }
        public MTObservableCollection<T> Items { get; protected set; }
        public event EventHandler<PhoneEventArgs<T>> OnItemClicked;

        public IEnumerable<T> GetSelectedItems()
        {
            return Items.Where(it => it.SelectedPanelId == this.App.ClientId);
        }

        public PhoneListViewModel(IEnumerable<T> items)
        {
            this.PO = null;
            this.Items = new MTObservableCollection<T>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }

        public PhoneListViewModel(ClientAppViewModel app)
        {
            this.App = app;
            this.Items = new MTObservableCollection<T>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            GetItemsFromServer();
        }

        void Item_OnClickHandler(object sender, EventArgs e)
        {
            var item = sender as T;
            if(OnItemClicked != null) OnItemClicked(this, new PhoneEventArgs<T>(item));
        }

        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(var item in e.NewItems.OfType<T>())
                    {
                        item.OnClickHandler += Item_OnClickHandler;
                    }                        
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<T>())
                    {
                        item.OnClickHandler -= Item_OnClickHandler;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        protected abstract void GetItemsFromServer();
    }
}
