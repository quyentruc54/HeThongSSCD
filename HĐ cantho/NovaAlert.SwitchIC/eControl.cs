using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.SwitchIC
{
    public enum eControl: byte
    {
        Polling     =   0x30,
        ACK         =   0x31,
        NAK         =   0x32,
        LoopStatus  =   0x33,
        AmplyPower  =   0x34,
        ExtRingPower =  0x35,
        PlayDtmf    =   0x36,
        Connect     =   0x37,
        AdjVol      =   0x38,
        Conference  =   0x39,
        ConAdd      =   0x3A,
        ConAdjVol   =   0x3B,
        LineStatus  =   0x3C,
        POStatus    =   0x3D,
        UpdateStatus =  0x3E,
        DeleteTone  =   0x3F,
        Dial        =   0x40,
        DialCompleted = 0x41,
        ResetSwitch =   0x42,
        ResetUSB    =   0x48,
        AllStatus   =   0x49,

        //EnableAlarm = 0x70,
        DateTimeData = 0x70,
        AlarmData   = 0x71,
        DayTypeData = 0x72,
        //DateTimeData= 0x73,
        StartAlarm  = 0x74,
        EndAlarm    = 0x75,

        // Led Panel
        LP_Polling  = 0x50,
        LP_ACK      = 0x51,
        LP_NAK      = 0x52,
        LP_Data     = 0x53,
        LP_Update   = 0x54,

        // TSL
        TSL_ENQ = 0x60,
        TSL_Prepare = 0x61,
        TSL_ResultRequest = 0x62,
        TSL_ResultData = 0x63,
        TSL_ResultEnd = 0x64,
        TSL_EOT = 0x65
    }

    public enum eDevice: byte
    {
        Switch      = 0x03,
        PC          = 0x04,
        LedPanel    = 0x05,
        TB09        = 0x06,
        TCT_14A     = 0x08,
        TCT_14B     = 0x09
    }

    public enum eStatus: byte
    {
        Off = 0,
        On = 0x01        
    }

    public enum eLineStatusType: byte
    {
        Line        =   0x30,
        Revert      =   0x31,
        Tone        =   0x32,
        CallerId    =   0x33,
        // The following will be adjust in documents
        PO_Status   =   0x34,
        PO_Tone     =   0x35
    }
}
