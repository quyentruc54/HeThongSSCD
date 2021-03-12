using Newtonsoft.Json;
using NovaAlert.Common;
using NovaAlert.Communication.ATModem;
using NovaAlert.Communication.Base;
using NovaAlert.Entities;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;

namespace NovaAlert.Service.TSL
{
    public class TSL_Modem : ItuV250Modem, IResource, INotifyPropertyChanged
    {
        #region Members & Properties
        protected object _syncObj;

        public event EventHandler OnAcceptHandler;
        public object LastTSLMessage { get; private set; }

        public string AreaCode { get; set; }
        public string Number { get; set; }
        #endregion

        #region ctor
        public TSL_Modem(IDataLink link)
            : base(link)
        {
            _syncObj = new object();
        } 
        #endregion

        #region Helpers
        protected override Parser CreateParser()
        {
            return new TSL_Parser(this);
        }

        public void ParseTSLMessage(object msgObj)
        {
            this.LastTSLMessage = msgObj;
            PulseAll();
        }

        public void PulseAll()
        {
            lock (_syncObj)
            {
                Monitor.PulseAll(_syncObj);
            }
        }

        public void SendMessage(object obj)
        {
            var s = JsonConvert.SerializeObject(obj);
            this.DataLink.SendData(s, false);
        }

        public object WaitForTSLResult(int waitTime)
        {
            lock (_syncObj)
            {
                if (this.LastTSLMessage == null)
                {
                    Monitor.Wait(_syncObj, TimeSpan.FromMilliseconds(waitTime));
                }
                var result = this.LastTSLMessage;
                this.LastTSLMessage = null;

                if (result == null)
                {
                    //log.InfoFormat("{0} ms waited > NO RESULT: \"{1}\"", DateTimeHelperClass.CurrentUnixTimeMillis() - t, Parser.GetBuffer());
                }
                return result;
            }
        }

        public object SendTSLMessageAndWaitForResult(object obj, int waitTime)
        {
            SendMessage(obj);
            return WaitForTSLResult(waitTime);
        }

        protected override void OnAccept()
        {
            if (this.OnAcceptHandler != null)
            {
                OnAcceptHandler(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ClearBuffer()
        {
            this.LastTSLMessage = null;
        }
        #endregion

        #region Factory
        public static TSL_Modem CreateModem(string portName)
        {
            try
            {
                var port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                port.DtrEnable = true;
                port.RtsEnable = true;

                var dl = new DialupDataLink(port);
                return new TSL_Modem(dl);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("Fail to initilize TSM modem", ex);
                return null;
            }
        } 
        #endregion

        #region IResource
        public int Id
        {
            get { return 0; }
        }

        public eResourceType ResourceType
        {
            get { return eResourceType.Modem; }
        }

        byte? _selectedPanelId;
        public byte? SelectedPanelId
        {
            get { return _selectedPanelId; }
            set
            {
                if(_selectedPanelId != value)
                {
                    _selectedPanelId = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedPanelId"));
                    }
                }
            }
        }

        public event EventHandler OnClickHandler;
        #endregion


    }
}
