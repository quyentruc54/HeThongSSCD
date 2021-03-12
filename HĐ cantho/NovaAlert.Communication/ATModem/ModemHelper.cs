using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace NovaAlert.Communication.ATModem
{
    public static class ModemHelper
    {
        public static bool CheckExistingModemOnComPort(SerialPort serialPort)
        {
            if ((serialPort == null) || !serialPort.IsOpen)
                return false;

            // Commands for modem checking
            string[] modemCommands = new string[] { "AT",       // Check connected modem. After 'AT' command some modems autobaud their speed.
                                            "ATQ0" };   // Switch on confirmations
            serialPort.DtrEnable = true;    // Set Data Terminal Ready (DTR) signal 
            serialPort.RtsEnable = true;    // Set Request to Send (RTS) signal

            string answer = "";
            bool retOk = false;
            for (int rtsInd = 0; rtsInd < 2; rtsInd++)
            {
                foreach (string command in modemCommands)
                {
                    serialPort.Write(command + '\r');
                    retOk = false;
                    answer = "";
                    int timeout = (command == "AT") ? 10 : 20;

                    // Waiting for response 1-2 sec
                    for (int i = 0; i < timeout; i++)
                    {
                        Thread.Sleep(100);
                        answer += serialPort.ReadExisting();
                        if (answer.IndexOf("OK") >= 0)
                        {
                            retOk = true;
                            break;
                        }
                    }
                }
                // If got responses, we found a modem
                if (retOk)
                    return true;

                // Trying to execute the commands without RTS
                serialPort.RtsEnable = false;
            }
            return false;
        }

        public static List<string> GetAllComportWithModem()
        {
            var list = new List<string>();
            var ports = SerialPort.GetPortNames();
            foreach(var portName in ports)
            {
                var p = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);

                try
                {                    
                    p.Open();
                    if (CheckExistingModemOnComPort(p)) list.Add(portName);
                    p.Close();
                }
                catch
                {
                }

                p.Dispose();
            }

            return list;
        }
    }
}
