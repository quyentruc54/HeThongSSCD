using System.ComponentModel;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class BindableEntity : INotifyPropertyChanged 
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
