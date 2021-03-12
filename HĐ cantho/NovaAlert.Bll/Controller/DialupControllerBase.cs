using NovaAlert.Common;
using NovaAlert.Communication.ATModem;
using System;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho chế độ quay số
    /// </summary>
    public abstract class DialupControllerBase: IDialupController
    {
        #region Members & 
        public IModem Modem { get; protected set; }
        public TSL_ALertUnitPhoneViewModel Unit { get; protected set; }
        public IConnection Connection { get; protected set; }

        public eDialupStatus Status { get; protected set; }
        public bool IsBusy { get; protected set; }
        public int Trys { get; protected set; }
        #endregion

        #region Ctor
        public DialupControllerBase(IModem modem, TSL_ALertUnitPhoneViewModel unit)
        {
            if (modem == null)
            {
                throw new ArgumentNullException(nameof(modem));
            }
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            this.Modem = modem;
            this.Unit = unit;
            Status = eDialupStatus.None;
            this.Trys = 0;
        }
        #endregion

        public void Dial()
        {
            try
            {
                this.Connection = this.Modem.Dial(Unit.GetFullNumber());
                Status = eDialupStatus.Connected;
            }
            catch (Exception ex)
            {
                this.Connection = null;
                Status = eDialupStatus.FailToConnect;
                LogService.Logger.Error(ex);
            }
        }

        protected void CloseConnection()
        {
            if (this.Connection != null)
            {
                this.Modem.HangUp();
                this.Connection = null;
            }
        }

        public virtual void Start()
        {
            this.Trys++;
            Dial();
            if (this.Connection != null)
            {
                // Send command

                // wait for respose
            }
        }

        public virtual void Cancel()
        {
            this.Status = eDialupStatus.Canceled;
            CloseConnection();
        }
    }

    public class PrepareController: DialupControllerBase
    {
        public PrepareController(IModem modem, TSL_ALertUnitPhoneViewModel unit)
            :base(modem, unit)
        {
        }
    }
}
