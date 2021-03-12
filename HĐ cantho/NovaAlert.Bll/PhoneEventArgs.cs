using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll
{
    public class PhoneEventArgs<T>: BaseEventArgs where T:PhoneViewModel
    {
        public T Item { get; private set; }        
        public PhoneEventArgs(T t)
        {
            this.Handled = false;
            this.Item = t;
        }
    }
}
