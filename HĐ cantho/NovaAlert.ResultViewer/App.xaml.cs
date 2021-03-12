using DevExpress.Xpf.Core;
using NovaAlert.Bll;
using NovaAlert.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace NovaAlert.ResultViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
      
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            ThemeManager.ApplicationThemeName = Theme.TouchlineDark.Name; // Theme.HybridApp.Name;

            Window wnd;
            NovaAlert.Config.ViewModels.ResultDataListViewModelBase vm;

            var st = NovaAlert.ResultViewer.Properties.Settings.Default;
            if (st.ViewStyle == 3)
            {
                wnd = new NovaAlert.Config.Views.ResultDataListView3();
                wnd.FontSize = st.FontSize_3;
                vm = new NovaAlert.Config.ViewModels.ResultDataListViewModel3();
            }
            else
            {
                wnd = new NovaAlert.Config.Views.ResultDataListView2();
                vm = new NovaAlert.Config.ViewModels.ResultDataListViewModel2();
            }

            wnd.DataContext = vm;

            if (Screen.AllScreens.Length > 0)
            {
                Screen s1 = Screen.AllScreens[Screen.AllScreens.Length - 1];
                Rectangle r1 = s1.WorkingArea;

                wnd.Top = r1.Top;
                wnd.Left = r1.Left;
                wnd.Width = r1.Width;
                wnd.Height = r1.Height;
            }
            else
            {
                wnd.WindowState = WindowState.Maximized;
            }
            wnd.Show();

        }
    }
}
