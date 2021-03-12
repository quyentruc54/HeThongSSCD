using NovaAlert.Entities;
namespace NovaAlert.Service.TSL
{
    public class TslResult
    {
        public bool Connected { get; set; }
        public bool Responsed { get; set; }

        public TslResult()
        {
            this.Connected = false;
            this.Responsed = false;
        }

        public eTslStatus TslStatus
        {
            get
            {
                eTslStatus status = eTslStatus.None;
                if (this.Connected)
                {
                    status = this.Responsed ? eTslStatus.Ready : eTslStatus.NotResponsed;
                }
                else
                {
                    status = eTslStatus.CanNotConnected;
                }
                return status;
            }            
        }
    }

    interface IServerModem
    {
        TslResult SendAndReceiveResult(UnitPhone unit);
        TslResult SendPrepareCommand(UnitPhone unit);
        void CancelWaitingTask();
        bool IsCanceled { get; }
    }

    class FakeServerModem : IServerModem
    {
        public TslResult SendAndReceiveResult(UnitPhone unit)
        {
            System.Threading.Thread.Sleep(5000);
            return new TslResult() { Connected = true };
        }

        public TslResult SendPrepareCommand(UnitPhone unit)
        {
            System.Threading.Thread.Sleep(5000);
            return new TslResult() { Connected = true, Responsed = true };
        }

        public void CancelWaitingTask() { }
        public bool IsCanceled { get { return false; } }
    }
}
