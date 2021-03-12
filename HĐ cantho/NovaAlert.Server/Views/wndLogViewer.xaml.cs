using NovaAlert.Common;
using System;
using System.Windows;

namespace NovaAlert.Server.Views
{
    /// <summary>
    /// Interaction logic for wndLogViewer.xaml
    /// </summary>
    public partial class wndLogViewer : Window
    {
        public wndLogViewer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnOptionChanged(null, new DependencyPropertyChangedEventArgs());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }

        private void UnRegisterEvents()
        {
            Mediator.Unregister(Mediator_Message.SwitchComm_Rcv, OnCommRcv);
            Mediator.Unregister(Mediator_Message.SwitchComm_Send, OnCommSend);
        }

        void OnCommRcv(object obj)
        {
            var msg = (string)obj;
            rcvLogView.AddLog(msg);
        }

        void OnCommSend(object obj)
        {
            var msg = (string)obj;
            sendLogView.AddLog(msg);
        }

        void OnOptionChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnRegisterEvents();

            if (chkRcvData.IsChecked == true)
            {
                Mediator.Register(Mediator_Message.SwitchComm_Rcv, OnCommRcv);
            }


            if (chkLogSendData.IsChecked == true)
            {
                Mediator.Register(Mediator_Message.SwitchComm_Send, OnCommSend);
            }
        }

        //private void btSend_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(txData.Text))
        //    {
        //        Mediator.NotifyColleagues(Mediator_Message.SwitchComm_Send, txData.Text);
        //    }
        //}

        //private void btRcv_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(txData.Text))
        //    {
        //        Mediator.NotifyColleagues(Mediator_Message.SwitchComm_Rcv, txData.Text);
        //    }
        //}
    }
}
