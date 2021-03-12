using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace NovaAlert.Common.Mvvm
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; OnPropertyChanged("DisplayName"); }
        }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// The PropertyChanged event can indicate all properties on the object have changed by using either null or String.Empty as the property name in the PropertyChangedEventArgs
        /// </summary>
        public void Refesh()
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members

        #region Unity
        /// <summary>
        /// Retrieves a service object identified by <typeparamref name="TServiceContract"/>.
        /// </summary>
        /// <typeparam name="TServiceContract">The type identifier of the service.</typeparam>
        public TServiceContract GetService<TServiceContract>()
            where TServiceContract : class
        {
            return ServiceContainer.Instance.GetService<TServiceContract>();
        }
        #endregion

        #region OK and Close Command
        RelayCommand _okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(DoOk, CanDoOk);
                }
                return _okCommand;
            }
        }

        protected virtual void DoOk(object obj)
        {
            var win = obj as Window;
            if (win != null)
            {
                win.DialogResult = true;
                win.Close();
            }
        }

        bool CanDoOk(object win) { return win is Window; }

        RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(DoClose, p => CanClose());
                }
                return _closeCommand;
            }
        }


        private void DoClose(object obj)
        {
            var win = obj as Window;
            if (win != null)
            {
                win.DialogResult = false;
                win.Close();
            }
        }

        //protected virtual bool CanDoClose(object win) { return win is Window; } 
        public virtual bool CanClose()
        {
            return true;
        }
        #endregion

        bool? _closed;
        public bool? Closed
        {
            get { return _closed; }
            set
            {
                if (_closed != value)
                {
                    _closed = value;
                    OnPropertyChanged("Closed");
                }
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }
    }

    public abstract class DialogViewModelBase : ViewModelBase
    {
        bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; OnPropertyChanged("DialogResult"); }
        }

        public RelayCommand OKCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public DialogViewModelBase()
            : base()
        {
            OKCommand = new RelayCommand(p => OnOK(), p => CanOK());
            CancelCommand = new RelayCommand(p => OnCancel(), p => CanCancel());
        }

        protected virtual bool CanOK() { return true; }
        protected virtual void OnOK()
        {
            this.DialogResult = true;
        }

        protected virtual bool CanCancel() { return true; }
        protected virtual void OnCancel()
        {
            this.DialogResult = false;
        }

    }
}
