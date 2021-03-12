using NovaAlert.Communication.Base;
using System;
using System.Collections.Generic;

namespace NovaAlert.Communication.ATModem
{
    public interface IModem
    {
        int DefaultWaitTime { get; }
        int DefaultTrys { get; }
        bool Idle { get; }
        void GarbageCollected(Parser parser, string s);
        void ParsedResultCode(Parser parser, ResultCodeToken r);
        void ParsedPrompt(Parser parser);
        void ParsedRing(Parser parser);

        bool Init();

        //bool Init(params string[] initStrings);
        void RemoveIncommingCallHandler(ICallHandler callHandler);

        ICallHandler IncommingCallHandler { set; }

        bool SendAndWaitForOK(string cmdLine);

        bool SendAndWaitForOK(string cmdLine, int trys, int waitTime);

        bool SendCommandLineWithPrompt(string command, string data);

        List<string> SendAndExtractData(string cmdLine);

        List<string> SendAndExtractData(string cmdLine, int trys, int waitTime);
        IRingListener RingListener { set; }
        IDataLink DataLink { get; }
        IConnection Connection { get; }
        //Stream ModemInputStream { get; set; }
        //Stream ModemOutputStream { get; }
        //bool GsmExtention {get;set;}
        //bool VoiceExtention {get;set;}
        //bool FaxExtention {get;set;}

        GsmExtention GsmExtention { get; }
        VoiceExtention VoiceExtention { get; }
        FaxExtention FaxExtention { get; }

        IConnection Dial(string number);

        //void Dial(string number, ICallHandler waitingOutgoingCall);
        void SetSpeaker();

        bool Reset();
        bool TurnEchoOff();

        string AutomaticAnswer { get; }

        string CommandLineTerminationCharacter { get; }

        string ResponseFormattingChar { get; }

        string CommandLineEditingChar { get; }

        string ModulationSelection { get; }

        string ManufacturerId { get; }

        string ModelId { get; }

        string RevisionId { get; }

        string SerialNumber { get; }

        string ObjectId { get; }

        ModemCapabilities[] Capabilities { get; }

        string CountryOfInstallation { get; }
        string[] InitStrings { set; get; }

        bool HangUp();
        bool Close();
        //void ExitDataMode();
        void ReenterDataMode();
        string ExtractData(string data);

        event EventHandler<DataLinkEventArg> OnDataReceived;
        void RaiseDataReceivedEvent(string data);
    }

    public static class Modem_Fields
    {
        public static readonly char EOF = (char)0x1A;
    }

    [Flags]
    public enum ModemCapabilities
    {
        Data,
        Voice,
        Fax,
        Gsm
    }

    public enum CallState
    {
        Idle,
        DialPending,
        AnswerPending,
        callEstablished
    }
}

