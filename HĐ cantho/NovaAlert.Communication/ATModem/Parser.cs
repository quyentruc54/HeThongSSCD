using log4net;
using NovaAlert.Common;
using NovaAlert.Communication.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovaAlert.Communication.ATModem
{
    public enum eParserState 
    { 
        COLLECT_ALL, 
        COLLECT_CMD_ECHO, 
        COLLECT_DATA_OR_RESPONSE 
    }

    public class Parser
    {
        #region Members
        private static readonly ILog log = ServiceContainer.Instance.GetService<ILog>();
        
        public const string ONLINE_DATA_OMNLINE_COMMAND_INDICATOR = "+++";
        
        //private Action parserThread;
        public char CR { get; private set;} //= '\r';
        public char LF { get; private set; } // = '\n';
        public string TokenCrLf {get; private set; } //separator between data and resultcode and endmarker for resultcode     
        public string TokenCrCrLf { get; private set; } // = new char[] { '\r', '\r', '\n' }; //Echo before data and or resultcode after     
        public string TokenPrompt {get; private set; } // = ">"; // = new char[] { '>' }; //Prompt for GSM AT+CMGS=     
        public string TokenAT {get; private set; } // = "AT"; // = new char[] { 'A', 'T' };
        public string TokenOk { get; private set; }
        public string TokenConnect { get; private set; }
        public string TokenRing { get; private set; }
        public string TokenNoCarrier { get; private set; }
        public string TokenError { get; private set; }
        public string TokenNoDialtone { get; private set; }
        public string TokenBusy { get; private set; }
        public string TokenNoAnswer { get; private set; }
        public string TokenConnectWithText { get; private set; }

        private eModemState _state;
        public virtual eModemState State 
        { 
            get { return _state; }
            private set 
            { 
                _state = value;
                System.Diagnostics.Debug.WriteLine(string.Format("State changed to {0}", _state));
            }
        }

        private StringBuilder _sb = new StringBuilder();
        private int _onlineEscapeIndex;
        private String _parsedEcho;
        private List<string> _parsedData;
        
        public Boolean IsOnlineDataMode() { return eModemState.ONLINE_DATA_STATE == _state; }
        public void PrepareOnlineHangup() { this.State = eModemState.ONLINE_COMMAND_STATE; }
        
        public virtual System.IO.Stream OutputStream { get; set; }
        public IDataLink DataLink { get; private set; }
        private eParserState _parserState = eParserState.COLLECT_ALL;
        public virtual string Echo
        {
            get
            {
                return _parsedEcho;
            }
        }

        public IModem Modem { get; private set; }        
        #endregion

        #region Ctor
        public Parser(IModem modem, char cr = '\r', char lf = '\n')
        {
            TokenCrLf = BuildToken(CR, LF);

            this.Modem = modem;
            this.DataLink = this.Modem.DataLink;
            this.State = eModemState.COMMAND_STATE;
            SetLineChars(cr, lf);            
            this.DataLink.OnDataReceive += DataLink_OnDataReceive;
        }
        #endregion
                
        public static string BuildToken(string s, params char[] c)
        {
            StringBuilder sb1 = new StringBuilder(s);
            sb1.Append(c);
            return sb1.ToString();
        }
        public static string BuildToken(params char[] c)
        {
            return BuildToken(string.Empty, c);
        }

        private void MatchedGarbage(int end)
        {
            Modem.GarbageCollected(this, _sb.ToString().Substring(0, end));
            _sb.Remove(0, end);
        }

        private int FindLastToken(string token)
        {
            var s = _sb.ToString();
            return s.LastIndexOf(token);
        }

        public virtual void ResetParser()
        {
            ClearBuffer();
            _parserState = eParserState.COLLECT_ALL;
            _parsedData = new List<string>();
            _parsedEcho = null;
        }
        
        public virtual void ClearBuffer()
        { 
            _sb.Clear(); 
        }

        public virtual void SetLineChars(char cr, char lf)
        {
            this.CR = cr;
            this.LF = lf;
            this.TokenAT = "AT";
            this.TokenCrLf = BuildToken(cr, lf);
            this.TokenCrCrLf = BuildToken(cr, cr, lf);
            this.TokenPrompt = ">";
            this.TokenOk = BuildToken(ResultCodeToken.OK, cr, lf);
            this.TokenConnect = BuildToken(ResultCodeToken.CONNECT, cr, lf);
            this.TokenRing = BuildToken(ResultCodeToken.RING, cr, lf);
            this.TokenNoCarrier = BuildToken(ResultCodeToken.NO_CARRIER, cr, lf);
            this.TokenError = BuildToken(ResultCodeToken.ERROR, cr, lf);
            this.TokenNoDialtone = BuildToken(ResultCodeToken.NO_DIALTONE, cr, lf);
            this.TokenBusy = BuildToken(ResultCodeToken.BUSY, cr, lf);
            this.TokenNoAnswer = BuildToken(ResultCodeToken.NO_ANSWER, cr, lf);
            this.TokenConnectWithText = BuildToken(ResultCodeToken.CONNECT);
        }

        private void MatchedBusy()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.BUSY, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedConnect()
        {
            this.State = eModemState.ONLINE_DATA_STATE;
            //_connectionInputStream.ClearBuffer();

            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.CONNECT, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedConnectWithText()
        {            
            this.State = eModemState.ONLINE_DATA_STATE;
            //_connectionInputStream.ClearBuffer();
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.CONNECT, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedError()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.ERROR, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedNoAnswer()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.NO_ANSWER, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedNoCarrier()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.NO_CARRIER, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedNoDialtone()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.NO_DIALTONE, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedOk()
        {
            if (Modem != null)
            {
                Modem.ParsedResultCode(this, new ResultCodeToken(ResultCodeToken.OK, _parsedData, _parsedEcho));
            }
            ResetParser();
        }
        private void MatchedRing()
        {
            if (Modem != null)
            {
                Modem.ParsedRing(this);
            }
            if(_sb.Length > TokenRing.Length)
                _sb.Remove(_sb.Length - TokenRing.Length, TokenRing.Length);
        }
        private void MatchedPrompt()
        {
            _sb.Remove(0, 1);
            if (Modem != null)
            {
                Modem.ParsedPrompt(this);
            }
        }

        public String GetBuffer()
        {
            return _sb.ToString();
        }
        public void ParseChar(char c)
        {
            _sb.Append(c);
            
            var data = _sb.ToString();

            if (data.EndsWith(TokenRing))
            {
                MatchedRing();
            }
            switch (_parserState)
            {
                case eParserState.COLLECT_ALL:
                    if (data.EndsWith(TokenOk))
                    {
                        MatchedOk();
                    }
                    else if (data.EndsWith(TokenBusy))
                    {
                        MatchedBusy();
                    }
                    else if (data.EndsWith(TokenConnect))
                    {
                        MatchedConnect();
                    }                    
                    else if (data.EndsWith(TokenError))
                    {
                        MatchedError();
                    }
                    else if (data.EndsWith(TokenNoAnswer))
                    {
                        MatchedNoAnswer();
                    }
                    else if (data.EndsWith(TokenNoCarrier))
                    {
                        MatchedNoCarrier();
                    }
                    else if (data.EndsWith(TokenNoDialtone))
                    {
                        MatchedNoDialtone();
                    }
                    else if (data.EndsWith(TokenCrLf))
                    {
                        if (FindLastToken(TokenConnectWithText) >= 0)
                        {
                            MatchedConnectWithText();
                        }
                    }
                    else if (data.EndsWith(TokenAT))
                    {
                        _parserState = eParserState.COLLECT_CMD_ECHO;
                        if (_sb.Length > TokenAT.Length)
                        {
                            MatchedGarbage(_sb.Length - TokenAT.Length);
                        }
                        break;
                    }
                    //else
                    //{
                    //    if (this.Modem.Connection != null)
                    //    {
                    //        this.Modem.Connection.ReceiveData(_sb.ToString());
                    //        ClearBuffer();
                    //    }
                    //}
                    break;
                case eParserState.COLLECT_CMD_ECHO:
                    if (data.EndsWith(TokenCrCrLf))
                    {
                        _parsedEcho = _sb.ToString().Substring(0, _sb.Length - TokenCrCrLf.Length);
                        _parsedData = new List<string>();
                        _parserState = eParserState.COLLECT_DATA_OR_RESPONSE;
                        ClearBuffer();
                    }
                    break;
                case eParserState.COLLECT_DATA_OR_RESPONSE:
                    if (_parsedData.Count == 0 && data.EndsWith(TokenPrompt))
                    {
                        MatchedPrompt();
                    }
                    else if (data.EndsWith(TokenOk))
                    {
                        MatchedOk();
                    }
                    else if (data.EndsWith(TokenBusy))
                    {
                        MatchedBusy();
                    }
                    else if (data.EndsWith(TokenConnect))
                    {
                        MatchedConnect();
                    }
                    else if (data.EndsWith(TokenError))
                    {
                        MatchedError();
                    }
                    else if (data.EndsWith(TokenNoAnswer))
                    {
                        MatchedNoAnswer();
                    }
                    else if (data.EndsWith(TokenNoCarrier))
                    {
                        MatchedNoCarrier();
                    }
                    else if (data.EndsWith(TokenNoDialtone))
                    {
                        MatchedNoDialtone();
                    }
                    else if (data.EndsWith(TokenCrLf))
                    {
                        if (FindLastToken(TokenConnectWithText) >= 0)
                        {
                            MatchedConnectWithText();
                        }
                        else
                        {
                            _parsedData.Add(_sb.ToString().Substring(0, _sb.Length - TokenCrLf.Length));
                            ClearBuffer();
                        }
                    }
                    break;
                default:
                    throw new Exception("UNKNOWN PARSER STATE");
            }
        }        

        object _syncObj = new object();

        void DataLink_OnDataReceive(object sender, Base.DataLinkEventArg e)
        {
            lock(_syncObj)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("State = {0}, Rcv: {1}", this.State, e.Data.Replace(System.Environment.NewLine, string.Empty)));

                if (_state == eModemState.ONLINE_DATA_STATE)
                {
                    _sb.Append(e.Data);
                    var s = _sb.ToString();
                    if (s.EndsWith(TokenNoCarrier))
                    {
                        MatchedNoCarrier();
                        this.State = eModemState.COMMAND_STATE;
                    }
                    else
                    {
                        ParseReceiveData(e);
                    }
                }
                else
                {
                    for (int i = 0; i < e.Data.Length; i++)
                    {
                        ParseChar(e.Data[i]);
                    }
                }       
            }             
        }

        protected virtual void ParseReceiveData(Base.DataLinkEventArg e)
        {
            this.Modem.RaiseDataReceivedEvent(e.Data);
        }         
    }
}

