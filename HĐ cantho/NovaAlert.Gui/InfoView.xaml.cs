using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var wnd = new wndAbout();
            wnd.ShowInTaskbar = false;
            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }
    }
}
