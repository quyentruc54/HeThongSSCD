using System;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using NovaAlert.Server.Models;

namespace NovaAlert.Server.Views
{
    /// <summary>
    /// Interaction logic for LiveLogViewCtrl.xaml
    /// </summary>
    public partial class LiveLogViewCtrl : UserControl
    {
        private bool _autoScroll = true;
        public ObservableCollection<LogEntry> LogEntries { get; private set; }        

        public LiveLogViewCtrl()
        {
            InitializeComponent();
            LogEntries = new ObservableCollection<LogEntry>();
            DataContext = LogEntries;

            //var timer = new Timer(OnTimer, null, 1000, 50);
        }

        public void AddLog(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;
            Dispatcher.BeginInvoke((Action)(() =>InternalAddLog(msg)));
        }

        private void InternalAddLog(string msg)
        {
            if(LogEntries.Count > 200)
            {
                LogEntries.Clear();
            }

            var entry = new LogEntry()
            {
                DateTime = DateTime.Now,
                Message = ByteArrayToString(System.Text.Encoding.ASCII.GetBytes(msg))
            };
            LogEntries.Add(entry);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:X2} ", b);
            }
            return hex.ToString();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    _autoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    _autoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (_autoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
            }
        }

        void OnTimer(object obj)
        {
            AddLog("test");
        }
    }
}
