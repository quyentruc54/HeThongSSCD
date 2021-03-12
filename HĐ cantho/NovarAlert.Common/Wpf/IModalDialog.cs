using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NovaAlert.Common.Wpf
{
    public interface IModalDialog
    {
        void OnOK();
        void OnCancel();
    }
}
