using NovaAlert.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for AppView.xaml
    /// </summary>
    public partial class AppView : UserControl
    {
        public AppView()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                LogService.Logger.Error(ex);
            }
        }
    }
}
