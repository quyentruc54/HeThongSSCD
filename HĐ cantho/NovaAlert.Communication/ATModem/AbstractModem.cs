using log4net;
using NovaAlert.Common;
using NovaAlert.Communication.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NovaAlert.Communication.ATModem
{
    public abstract class AbstractModem : IModem, IDisposable
    {
        #region Members & Properties
        private static readonly ILog log = ServiceContainer.Instance.GetService<ILog>();        
        public IConnection Connection { get; private set; }
        public Parser Parser { get; private set; }
        private string _lastEcho;
        private ResultCodeToken _lastResultCode;
        public string[] InitStrings { get; set; }
        private const int _defaultWaitTime = 2000;
        private const int _defaultTrys = 3;
        private const int _defaultDialWaitTime = 30000;
        private CallState _callState = CallState.Idle;
        private ICallHandler WaitingOutgoingCall { get; set; }
        public ICallHandler IncommingCallHandler { get; set; }                
        public readonly object _promptLock = new object();
        public IRingListener RingListener { get; set; }
        public int DefaultTrys { get { return _defaultTrys; } }
        public int DefaultWaitTime { get { return _defaultWaitTime; }}
        public IDataLink DataLink { get; set; }

        public event EventHandler<DataLinkEventArg> OnDataReceived;

        #region Extenstion
        public FaxExtention FaxExtention { get; set; }        
        public GsmExtention GsmExtention { get; set; }
        public VoiceExtention VoiceExtention { get; set; }
        public bool isGsmExtention()
        {
            return GsmExtention != null;
        }
        public bool isVoiceExtention()
        {
            return VoiceExtention != null;
        }
        public bool isFaxExtention()
        {
            return FaxExtention != null;
        } 
        #endregion
        #endregion

        #region Ctor
        public AbstractModem(IDataLink dataLink)
        {
            this.DataLink = dataLink;
            Parser = CreateParser();
        } 
        #endregion

        protected virtual Parser CreateParser()
        {
            return new Parser(this);
        }

        private void WaitForPrompt()
        {
            lock (_promptLock)
            {
                Monitor.Wait(_promptLock, _defaultWaitTime);
            }
        }
        private void ResetParser()
        {
            Parser.ResetParser();
            _lastEcho = null;
            _lastResultCode = null;
        }

        public bool SendCommandLineWithPrompt(string command, string data)
        {
            SendCommandLine(command);
            WaitForPrompt();
            return SendRawData(data + Modem_Fields.EOF, _defaultWaitTime * 10);
        }

        protected internal virtual bool WaitForOK(int waitTime)
        {
            long t = DateTimeHelperClass.CurrentUnixTimeMillis();
            ResultCodeToken e = WaitForResult(waitTime);
            if (e == null || !e.Ok)
            {
                log.InfoFormat("Timeout waiting ({0} ms) for OK resCode=\"{1}\"", new object[] { DateTimeHelperClass.CurrentUnixTimeMillis() - t, e != null ? e.ResultCode : "e == null" });
            }
            else
            {
                log.InfoFormat("Waited {0} ms for OK result: {1} ", new object[] { DateTimeHelperClass.CurrentUnixTimeMillis() - t, e.ResultCode, e.ResultCode });
                return e.Ok;
            }
            return (e == null) ? false : e.Ok;
        }

        public bool Reset()
        {
            return SendAndWaitForOK("ATZ");
        }

        public bool TurnEchoOff()
        {
            return SendAndWaitForOK("ATE0");
        }

        public string AutomaticAnswer
        {
            get
            {
                return SendAndExtractData("ATS0?")[0];
            }
        }

        public string CommandLineTerminationCharacter
        {
            get
            {
                return SendAndExtractData("ATS3?")[0];
            }
        }

        public string ResponseFormattingChar
        {
            get
            {
                return SendAndExtractData("ATS4?")[0];
            }
        }

        public string CommandLineEditingChar
        {
            get
            {
                return SendAndExtractData("ATS5?")[0];
            }
        }

        public string ModulationSelection
        {
            get
            {
                return SendAndExtractData("AT+MS?")[0];
            }
        }

        public IConnection Dial(string number)
        {
            if (_callState != CallState.Idle)
            {
                throw new Exception("Line busy: " + _callState);
            }
            _callState = CallState.DialPending;
            for (int i = 0; i < _defaultTrys; i++)
            {
                if (_callState != CallState.DialPending)
                {
                    break;
                }
                ResetParser();
                string commandline = "ATD " + number;
                SendCommandLine(commandline);
                Thread.Sleep(_defaultWaitTime);
                string buffer = Parser.GetBuffer();
                //if (buffer == null || !buffer.Contains(commandline)) // !buffer.Contains(commandline + Parser.CR))
                //{
                //    log.InfoFormat("Retransmitt CMD! \"{0}\"", buffer);
                //    SendCommandLine(commandline);
                //}
                ResultCodeToken e = WaitForResult(_defaultDialWaitTime); // 30 s             
                if (e == null)
                {
                }
                else if (Parser.IsOnlineDataMode())
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Caller: connected {0}", DateTime.Now));
                    _callState = CallState.callEstablished;
                    return this.Connection;
                }
            }
            throw new Exception("NOT Connected");
        }
                
        public bool HangUp()
        {
            bool result = false;
            Thread.Sleep(1000);
            if (Parser.IsOnlineDataMode())
            {
                Parser.PrepareOnlineHangup();
                result = SendAndWaitForOK("+++", _defaultTrys, _defaultWaitTime * 3);
            }

            if (result)
            {
                for (int i = 0; i < _defaultTrys; i++)
                {
                    ResetParser();
                    SendCommandLine("ATH0");
                    var e = WaitForResult(_defaultWaitTime * 3);
                    if (e != null && e.ResultCode == ResultCodeToken.NO_CARRIER)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            if (result)
            {
                Connection = null;
                _callState = CallState.Idle;
            }

            return result;
        }
        protected void SendCommandLine(string commandline)
        {
            log.InfoFormat("Send: \"{0}\"", commandline);
            
            this.DataLink.SendData(commandline);

            if (commandline != "+++") this.DataLink.SendControl((byte)Parser.CR);
        }
        private bool SendRawData(string rawData, int timeout)
        {
            ResetParser();
            this.DataLink.SendData(rawData);
            long t = DateTimeHelperClass.CurrentUnixTimeMillis();
            return WaitForOK(timeout);
        }
        public bool SendAndWaitForOK(string cmdLine)
        {
            return SendAndWaitForOK(cmdLine + Parser.TokenCrLf, _defaultTrys, _defaultWaitTime);
        }
        public bool SendAndWaitForOK(string cmdLine, int trys, int waitTime)
        {
            for (int i = 0; i < trys; i++)
            {
                ResetParser();
                SendCommandLine(cmdLine);
                if (WaitForOK(waitTime))
                {
                    return true;
                }
            }
            log.Debug("Error during " + cmdLine);
            return false;
        }
        public List<string> SendAndExtractData(string cmdLine)
        {
            return SendAndExtractData(cmdLine, _defaultTrys, _defaultWaitTime);
        }
        public List<string> SendAndExtractData(string cmdLine, int trys, int waitTime)
        {
            for (int i = 0; i < trys; i++)
            {
                ResetParser();
                SendCommandLine(cmdLine);
                ResultCodeToken e = WaitForResult(waitTime);
                if (e == null || !e.Ok)
                {
                }
                else
                {
                    return e.Data;
                }
            }
            throw new Exception("Error during " + cmdLine);
        }
        public bool Init()
        {
            bool result = false;
            if(this.InitStrings != null)
            {
                foreach (string s in this.InitStrings)
                {
                    result = SendAndWaitForOK(s);
                    if (!result)
                    {
                        return result;
                    }
                }
            }
            
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private ResultCodeToken WaitForResult(int waitTime)
        {
            long t = DateTimeHelperClass.CurrentUnixTimeMillis();
            if (this._lastResultCode == null)
            {
                Monitor.Wait(this, TimeSpan.FromMilliseconds(waitTime));
            }
            ResultCodeToken result = _lastResultCode;
            _lastResultCode = null;
            if (result == null)
            {
                log.InfoFormat("{0} ms waited > NO RESULT: \"{1}\"", new object[] { DateTimeHelperClass.CurrentUnixTimeMillis() - t, Parser.GetBuffer() });
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ParsedResultCode(Parser p, ResultCodeToken r)
        {
            _lastResultCode = r;
            if (r.Connect)
            {
                Connection = new DefaultConnection(this);
                if (_callState == CallState.DialPending && WaitingOutgoingCall != null)
                {
                    WaitingOutgoingCall.Connect(this, Connection);
                }
                else if (_callState == CallState.AnswerPending && this.IncommingCallHandler != null)
                {
                    this.IncommingCallHandler.Connect(this, Connection);
                }
                Monitor.PulseAll(this);
                _callState = CallState.callEstablished;
            }
            else
            {
                if (_callState == CallState.DialPending && WaitingOutgoingCall != null)
                {
                    WaitingOutgoingCall.ParsedResultCode(this, r);
                }
                else if (_callState == CallState.AnswerPending && this.IncommingCallHandler != null)
                {
                    this.IncommingCallHandler.ParsedResultCode(this, r);
                }
                Monitor.PulseAll(this);
                _callState = CallState.Idle;
                this.Connection = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ParsedRing(Parser p)
        {
            if (this.IncommingCallHandler == null && this.RingListener != null)
            {
                this.IncommingCallHandler = this.RingListener.Ring(this);
            }

            if (this.IncommingCallHandler != null && (_callState == CallState.Idle || _callState == CallState.AnswerPending))
            {
                try
                {
                    if (this.IncommingCallHandler.Ring(this))
                    {
                        SendCommandLine("ATA");
                        this.IncommingCallHandler.Answered(this);
                    }
                    _callState = CallState.AnswerPending;
                }
                catch (IOException)
                {
                    log.Info("Error answering call");
                }
            }
            else
            {
                SendCommandLine("ATA");
                System.Diagnostics.Debug.WriteLine(string.Format("Receiver: reply ATA {0}", DateTime.Now));

                _callState = CallState.callEstablished;
                Monitor.PulseAll(this);
                OnAccept();
            }            
        }

        protected virtual void OnAccept()
        {

        }

        public void RemoveHandler(ICallHandler handler)
        {
            if (this.IncommingCallHandler == handler)
            {
                this.IncommingCallHandler = null;
            }
            if (WaitingOutgoingCall == handler)
            {
                WaitingOutgoingCall = null;
            }
        }
        protected string ExtractLineData(List<string> strings)
        {
            return ExtractData(strings[0]);
        }
        public void GarbageCollected(Parser p, string s)
        {
            log.InfoFormat("Garbage collected: {0}", s);
        }
        public void ParsedPrompt(Parser p)
        {
            lock (_promptLock)
            {
                Monitor.PulseAll(_promptLock);
            }
        }
        public string ExtractData(string data)
        {
            int start = data.IndexOf(':') + 1;
            for (int i = start; i < data.Length; i++)
            {
                if (data[i] != ' ')
                {
                    start = i;
                    break;
                }
            }
            int end = data.Length - 1;
            for (int i = end; i > start; i--)
            {
                if (data[i] != ' ')
                {
                    end = i + 1;
                    break;
                }
            }
            try
            {
                return data.Substring(start, end - start);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(data + " start: " + start + " end: " + end);
                return data;
            }
        }                
        public bool Idle
        {
            get { return _callState == CallState.Idle; }
        }

        public void RemoveIncommingCallHandler(ICallHandler callHandler)
        {
            if (this.IncommingCallHandler == callHandler) this.IncommingCallHandler = null;
        }

        #region Let subclasses implement
                
        public virtual bool Close()
        {
            throw new System.NotSupportedException("Not supported yet.");
        }
        public virtual void ReenterDataMode()
        {
            throw new NotImplementedException();
        }
        public virtual void SetSpeaker()
        {
            throw new System.NotSupportedException("Not yet implemented");
        }
        public abstract string ManufacturerId { get; }
        public abstract string ModelId { get; }
        public abstract string RevisionId { get; }
        public abstract string SerialNumber { get; }
        public abstract string ObjectId { get; }
        public abstract ModemCapabilities[] Capabilities { get; }
        public abstract string CountryOfInstallation { get; } 
        #endregion

        public virtual void Dispose()
        {

        }

        public void RaiseDataReceivedEvent(string data)
        {
            if (this.OnDataReceived != null) OnDataReceived(this, new DataLinkEventArg(data));
        }
    }
}
