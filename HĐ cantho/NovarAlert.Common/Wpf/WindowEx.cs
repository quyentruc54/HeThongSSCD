using System.Linq;
using System.Windows;

namespace NovaAlert.Common.Wpf
{
    public static class WindowEx
    {
        public static Window GetActiveWindow(this Application currentApp)        
        {
            if (Application.Current != null)
            {
                return Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            }

            return null;
        }
    }
}
