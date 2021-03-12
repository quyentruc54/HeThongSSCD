using NovaAlert.Common;
using NovaAlert.Dal;
using NovaAlert.SwitchIC;
using System;
using System.Collections.Generic;

namespace NovaAlert.Service.TSL
{
    public class ClientModem: ModemBase
    {
        public ClientModem(TSL_Modem modem):base(modem)
        {
            
        }

        public void WaitAndReplyCommandFromServer(Func<bool> act)
        {
            this.Modem.SelectedPanelId = 255;

            LogService.Logger.Info("Client: start waiting for server");
            System.Diagnostics.Debug.WriteLine("Client start waiting command from server");
            var ret = this.Modem.WaitForTSLResult(TSL_WaitTime * 6);                                    
            bool done = false;            
            int curIndex = 0;

            while (ret != null && !done)
            {
                if (ret is TSL_Message)
                {
                    var r_msg = ret as TSL_Message;
                    LogService.Logger.Info(string.Format("Client: receive {0}", r_msg.Control));

                    if (r_msg.Control == eControl.TSL_ENQ)
                    {
                        ret = this.Modem.SendTSLMessageAndWaitForResult(new TSL_Message() { Control = eControl.ACK }, TSL_WaitTime);
                    }
                    else if (r_msg.Control == eControl.TSL_Prepare)
                    {
                        bool ack = (act != null) ? act() : false;                        

                        if (ack)
                        {
                            this.SendMessage(eControl.ACK);
                        }
                        else
                        {
                            this.SendMessage(eControl.NAK);
                        }
                    }
                    else if (r_msg.Control == eControl.TSL_ResultRequest)
                    {
                        // send result
                        List<Entities.ResultData> results = null;
                        if (results == null) results = NovaAlertCommon.GetResults(Entities.eTaskType.CTT);

                        if (curIndex < results.Count)
                        {
                            var obj = results[curIndex++];                            
                            this.Modem.SendMessage(obj);                            
                        }
                        else
                        {
                            this.SendMessage(eControl.TSL_ResultEnd);
                        }
                    }
                    else if (r_msg.Control == eControl.TSL_EOT)
                    {
                        done = true;
                    }                    

                    ret = this.Modem.WaitForTSLResult(TSL_WaitTime);
                }
                else
                {
                    LogService.Logger.Info("Client: receive undefined message");
                }
            }

            this.Modem.HangUp();
            this.Modem.ClearBuffer();
            this.Modem.SelectedPanelId = null;
        }
    }
}
