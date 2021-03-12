using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NovaAlert.Bll;
using NovaAlert.Common;
using NovaAlert.Entities;
using System.Windows.Input;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientAppViewModel _app;
        public MainWindow()
        {
            try
            {
                
                InitializeComponent();

                var settings = NovaAlert.Common.Setting.ClientSetting.Instance;
                _app = new NovaAlert.Bll.ClientAppViewModel();
                DataContext = _app;
                this.Closed += MainWindow_Closed;

                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 1),
                    System.Windows.Threading.DispatcherPriority.Normal, OnTimer, this.Dispatcher);
            }
            catch(Exception ex)
            {
                LogService.Logger.Error(ex);
            }
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _app.Dispose();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
