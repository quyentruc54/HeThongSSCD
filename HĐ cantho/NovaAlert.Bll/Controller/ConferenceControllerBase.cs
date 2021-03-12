using System;
using System.Collections.Generic;
using System.Linq;
using NovaAlert.Common;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Class cơ sở cho gọi nhóm hoặc CTT
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConferenceControllerBase<T> : ComplexControllerBase, ICallController where T : CallControllerBase<ConferenceConnection>
    {
        #region Members & Properties
        public const byte MaxSpeaker = 2;
        public const eVolumn MaxInVolumn = eVolumn.Volumn_6;
        public const eVolumn ListenInVolumn = eVolumn.Volumn_2;

        private eCallStatus _callStatus;
        public eCallStatus CallStatus
        {
            get { return _callStatus; }
            protected set { _callStatus = value; OnPropertyChanged("CallStatus"); }
        }

        public CallLog CallLog { get; private set; }
        public DateTime StartTime { get; private set; }
        public virtual bool IsRecording
        {
            get
            {
                return this.GetAllMembers().Any(m => m.IsRecording);
            }
        }

        public virtual bool AutoRecording { get { return false; } }
        #endregion

        #region Ctor
        public ConferenceControllerBase(ClientAppViewModel app, IEnumerable<UnitPhoneViewModel> units)
            : base(app)
        {
            this.StartTime = app.Service.GetDateTime();
            CreateMembers(units);
            this.CallLog = CreateCallLog();
            SaveLog();
        }
        #endregion

        #region Override
        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (this.ClientApp.Channels.PO.Id == e.Address &&
                e.Status.HasValue && e.Status.Value == ePOStatus.OnHook && 
                this.IsHolding == false)
            {
                Finalise();
                this.ClientApp.Channels.PO.ApplyChanges(e);
                e.Handled = true;
            }
        }

        public override void Finalise()
        {            
            base.Finalise();
            SaveLog();
        }
        #endregion

        #region Helpers
        protected abstract void CreateMembers(IEnumerable<UnitPhoneViewModel> units);

        public IEnumerable<T> GetAllMembers()
        {
            return this.Controllers.OfType<T>();
        }
        #endregion

        #region ICallController
        public DateTime? DialCompletedTime
        {
            get { return null; }
        }

        public void Dial()
        {

        }

        public virtual void PutOnHold()
        {
            foreach (var mem in GetAllMembers())
            {
                mem.PutOnHold();
            }
        }

        public virtual bool IsHolding
        {
            get
            {
                return false;
            }
        }

        public virtual void Resume()
        {
            foreach (var mem in GetAllMembers())
            {
                mem.Resume();
            }
        }        
        #endregion

        public virtual bool NeedRecord { get { return false; } }

        protected virtual CallLog CreateCallLog()
        {
            var callLog = new Entities.CallLog() 
            {
                CallType = eCallType.Conference,
                StartTime = ClientApp.Service.GetDateTime(),
                POId = this.ClientApp.Channels.PO.Id                
            };

            return callLog;
        }

        protected virtual void SaveLog()
        {
            if (this.FinalisedDate.HasValue)
            {
                this.CallLog.EndTime = this.FinalisedDate.Value;                
            }

            AsyncHelper.RunAsync<CallLog>(this.ClientApp.Service.SaveCallLog, this.CallLog);
        }

        public string GetDialingNumber()
        {
            return "000";
        }

        public bool CanHold()
        {
            return true;
        }
    }
}
