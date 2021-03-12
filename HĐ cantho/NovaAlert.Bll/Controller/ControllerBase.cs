using System;
using System.Collections.Generic;
using System.ComponentModel;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Abstract base class for all controller
    /// </summary>
    public abstract class ControllerBase : IController
    {
        #region Members & Properties        
        public abstract IEnumerable<ISwitchConnection> GetConnections();
        public abstract IEnumerable<UnitPhoneViewModel> GetUnits();
        public ClientAppViewModel ClientApp { get; private set; }        
        public DateTime? FinalisedDate { get; protected set; }
        public event EventHandler OnFinalised;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Action<ISwitchConnection, bool> _internalSendConnection;
        #endregion

        #region Ctor
        public ControllerBase(ClientAppViewModel app)
        {
            this.ClientApp = app;
            this.ClientApp.ClearInfo();
            _internalSendConnection = new Action<ISwitchConnection, bool>(InternalSendConnection);
        }        
        #endregion

        #region INovaAlertServiceCallback
        public virtual void OnResourceChanged(ResourceChangedEventArgs e)
        {            
        }

        public virtual void OnPOStatusChanged(POStatusChangedEventArgs e)
        {   
        }

        public virtual void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {            
        }

        public virtual void OnDialCompleted(ChannelEventArgs e)
        {

        }
        public virtual void OnSystemDateTimeChanged(SystemDateTimeChangedEventArgs e)
        {

        }

        public virtual void OnTaskChanged(TaskChangedEventArgs e)
        {

        }

        public virtual void OnRequestPrepare()
        {

        }
        #endregion

        #region User Interactions
        public virtual void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {            
        }

        public virtual void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {            
        }
        #endregion

        #region IDisposable
        public virtual void Dispose()
        {            
        }        
        #endregion

        #region Helpers
        public virtual bool CanFinalise()
        {
            return true;
        }

        public virtual void Finalise()
        {
            this.FinalisedDate = ClientApp.Service.GetDateTime();
            if (this.OnFinalised != null)
            {
                OnFinalised(this, EventArgs.Empty);
            }
        }

        protected void SendConnection(ISwitchConnection conn, bool isFirstOne = false)
        {
            _internalSendConnection.BeginInvoke(conn, isFirstOne, null, null);            
        }

        private void InternalSendConnection(ISwitchConnection conn, bool isFirstOne)
        {
            try
            {
                if (conn is SwitchConnection)
                {
                    this.ClientApp.Service.SendConnection((SwitchConnection)conn);
                }
                else
                {
                    if (conn is ConferenceConnection)
                    {
                        this.ClientApp.Service.SendConferenceConnection((ConferenceConnection)conn, isFirstOne);
                    }
                }
            }
            catch
            {
            }
        }

        protected void Disconnect(ISwitchConnection conn)
        {
            if(conn != null && conn.IsConnected)
            {
                conn.IsConnected = false;
                SendConnection(conn);
            }
        }
        #endregion

        
        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        void INovaAlertServiceCallback.OnSwitchStatusChanged(bool isConnected)
        {

        }

        void INovaAlertServiceCallback.OnServerMessage(eServerMessageType messageType, string msg)
        {
        }

        void INovaAlertServiceCallback.Reload()
        {
            
        }

        public virtual void OnTslStatusChanged(int phoneNumberId, eTslStatusType type, eTslStatus result, int? hostClientId)
        {

        }
    }
}
