using NovaAlert.Common;
using NovaAlert.Dal;
using NovaAlert.Entities;
using NovaAlert.SwitchIC;
using System;

namespace NovaAlert.Service.TSL
{
    public class ServerModem: ModemBase, NovaAlert.Service.TSL.IServerModem
    {
        const string ModemIsBusy = "Modem đang bận.";
        const string ModemIsNull = "Modem không tồn tại.";

        public bool IsCanceled { get; private set; }
        
        public ServerModem(TSL_Modem modem):base(modem)
        {
            this.IsCanceled = false;
        }        

        public TslResult SendPrepareCommand(UnitPhone unit)
        {
            CheckModem();
            string number = string.Format("{0}{1}", unit.TSLAreaCode, unit.TSLNumber).Trim();
            return SendPrepareCommand(number);
        }

        public TslResult SendPrepareCommand(string number)
        {
            LogService.Logger.Debug(string.Format("SendPrepareCommand {0}", number));            
            var tslResult = new TslResult();

            try
            {
                this.Modem.Dial(number);

                if (this.Modem.Connection != null)
                {
                    if (!this.IsCanceled)
                    {
                        tslResult.Connected = true;
                        var msg = new TSL_Message() { Control = eControl.TSL_Prepare };

                        var ret = this.Modem.SendTSLMessageAndWaitForResult(msg, TSL_WaitTime);
                        tslResult.Responsed = ret != null && ret is TSL_Message && ((TSL_Message)ret).Control == eControl.ACK;
                    }                    

                    SendMessage(eControl.TSL_EOT);
                    this.Modem.HangUp();
                }
            }
            catch(Exception ex)
            {
                LogService.Logger.Error(ex);
            }

            return tslResult;
        }

        public TslResult SendAndReceiveResult(string number, int phoneNumberId, Action<int, ResultData> saveFunc)
        {
            var tslResult = new TslResult();

            try
            {
                this.Modem.Dial(number);

                if (this.Modem.Connection != null)
                {
                    if(!this.IsCanceled)
                    {
                        tslResult.Connected = true;
                        var done = false;
                        var msg = new TSL_Message() { Control = eControl.TSL_ResultRequest };
                        var ret = this.Modem.SendTSLMessageAndWaitForResult(msg, TSL_WaitTime);

                        while (ret != null && !done && !this.IsCanceled)
                        {
                            if (ret is TSL_Message)
                            {
                                var r_msg = ret as TSL_Message;

                                if (r_msg.Control == eControl.TSL_ResultEnd)
                                {
                                    SendMessage(eControl.TSL_EOT);
                                    done = true;
                                }
                            }
                            else if (ret is ResultData)
                            {
                                var result = ret as ResultData;
                                if (saveFunc != null)
                                {
                                    saveFunc(phoneNumberId, result);
                                }
                                ret = this.Modem.SendTSLMessageAndWaitForResult(msg, TSL_WaitTime);
                            }
                        }

                        tslResult.Responsed = done;
                    }                    
                }

                this.Modem.HangUp();
                this.Modem.ClearBuffer();
            }
            catch(Exception ex)
            {
                LogService.Logger.Error(ex);
            }

            return tslResult;
        }

        public TslResult SendAndReceiveResult(UnitPhone unit)
        {
            CheckModem();
            string number = string.Format("{0}{1}", unit.TSLAreaCode, unit.TSLNumber).Trim();
            return SendAndReceiveResult(number, unit.PhoneNumberId, NovaAlertCommon.SaveSubResult);
        }    
    
        void CheckModem()
        {
            if (!this.Available)
            {
                if (this.Modem == null)
                {
                    throw new InvalidOperationException(ModemIsNull);
                }
                else
                {
                    throw new InvalidOperationException(ModemIsBusy);
                }
            }            
        }

        public void CancelWaitingTask()
        {
            this.IsCanceled = true;
            this.Modem.Reset();
        }
    }
}
