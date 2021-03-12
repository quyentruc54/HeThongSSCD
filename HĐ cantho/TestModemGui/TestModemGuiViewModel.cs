using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Wpf;
using NovaAlert.Communication.ATModem;
using NovaAlert.Dal;
using NovaAlert.Service.TSL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;

namespace TestModemGui
{
    class TestModemGuiViewModel: ViewModelBase
    {
        string _selectedPort;
        public string SelectedPort
        {
            get { return _selectedPort; }
            set { _selectedPort = value; OnPropertyChanged("SelectedPort"); }
        }
        public List<string> Comports { get; set; }
        public string NumberToCall { get; set; }                

        public RelayCommand OpenCommand { get; set; }
        public RelayCommand CloseModemCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand DialCommand { get; set; }
        public RelayCommand SwitchModeCommand { get; set; }
        public RelayCommand HangUpCommand { get; set; }
        public RelayCommand SendDataCommand { get; set; }
        public RelayCommand EchoOffCommand { get; set; }
        public RelayCommand SendResultCommand { get; set; }
        public RelayCommand PrepareCommand { get; set; }
        public RelayCommand GetResultCommand { get; set; }
        public string Data { get; set; }

        StringBuilder _sb = new StringBuilder();

        TSL_Modem _modem;

        public string LogText { get { return _sb.ToString(); } }

        public TestModemGuiViewModel()
        {
            //this.Comports = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            this.Comports = ModemHelper.GetAllComportWithModem();

            this.OpenCommand = new RelayCommand(p => OnOpen(), p => _modem == null);
            this.CloseModemCommand = new RelayCommand(p => OnCloseModem(), p => _modem != null);

            this.ResetCommand = new RelayCommand(p => _modem.Reset(), p => _modem != null);
            this.DialCommand = new RelayCommand(p => Dial(), p => _modem != null && !string.IsNullOrEmpty(NumberToCall));
            this.SendDataCommand = new RelayCommand(p => _modem.DataLink.SendData(this.Data), p => _modem != null && !string.IsNullOrEmpty(this.Data));

            this.HangUpCommand = new RelayCommand(p => _modem.HangUp(), p => _modem != null && _modem.Connection != null);

            this.EchoOffCommand = new RelayCommand(p => _modem.TurnEchoOff(), p => _modem != null);
            this.SendResultCommand = new RelayCommand(p => OnSendResult(), p => _modem != null);
            this.PrepareCommand = new RelayCommand(p => OnPrepare(), p => _modem != null);
            this.GetResultCommand = new RelayCommand(p => OnGetResult(), p => _modem != null);
        }

        private void Dial()
        {
            try
            {
                _modem.Dial(this.NumberToCall);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void OnOpen()
        {
            //var port = new SerialPort(this.SelectedPort, 9600, Parity.None, 8, StopBits.One);
            //var dl = new DialupDataLink(port);
            //_modem = new TSL_Modem(dl);
            _modem = TSL_Modem.CreateModem(this.SelectedPort);
            //dl.OnRawDataReceive += dl_OnRawDataReceive;
            _modem.OnDataReceived += _modem_OnDataReceived;
            _modem.OnAcceptHandler += _modem_OnAcceptHandler;
        }

        void _modem_OnAcceptHandler(object sender, EventArgs e)
        {
            //var thread = new Thread(new ThreadStart(ThreadStartPoint));
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;
            //thread.Start();
        }

        void ThreadStartPoint()
        {
            System.Threading.Thread.Sleep(10000);
            var client = new ClientModem(_modem);
            client.WaitAndReplyCommandFromServer(ClientDoPrepare);
        }

        bool ClientDoPrepare()
        {
            var msg = "Yêu cầu chuẩn bị nhận lệnh từ chỉ huy.";
            var vm = new MsgBoxWithTimeoutViewModel(msg, timeout: 10);
            var view = new MsgBoxWithTimeoutView() { DataContext = vm };
            view.ShowDialog();
            return vm.DialogResult ?? false;
        }

        void _modem_OnDataReceived(object sender, NovaAlert.Communication.Base.DataLinkEventArg e)
        {
            if (_sb.Length > 2000) _sb.Clear();
            _sb.AppendLine(e.Data);
            OnPropertyChanged("LogText");
        }

        void dl_OnRawDataReceive(object sender, NovaAlert.Communication.Base.DataLinkEventArg e)
        {
            if (_sb.Length > 2000) _sb.Clear();
            _sb.AppendLine(e.Data);
        }

        void OnCloseModem()
        {
            if(_modem != null)
            {
                var im = _modem as IDisposable;
                if(im != null)
                {
                    im.Dispose();
                }

                _modem = null;
            }
        }

        void OnSendResult()
        {
            //var msg = new TSL_ResultMessage()
            //{
            //    Id = 1,
            //    Name = "Đơn vị 1",
            //    Task = 1,
            //    Level = 1,
            //    Result = 1
            //};

            //var s = JsonConvert.SerializeObject(msg);
            ////var arr = System.Text.UTF8Encoding.Default.GetBytes(s);
            ////_modem.DataLink.SendData(arr, false);
            //_modem.DataLink.SendData(s);
        }

        void OnPrepare()
        {
            Action act = new Action(DoPrepare);
            act.BeginInvoke(null, null);

            
        }

        private void DoPrepare()
        {
            _modem.ClearBuffer();
            var server = new ServerModem(_modem);
            try
            {
                server.SendPrepareCommand(this.NumberToCall);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void OnGetResult()
        {
            _modem.ClearBuffer();
            var server = new ServerModem(_modem);
            try
            {
                server.SendAndReceiveResult(this.NumberToCall, 0, NovaAlertCommon.SaveSubResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
