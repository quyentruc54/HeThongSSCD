using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovaAlert.Entities;

namespace NovaAlert.SwitchIC.PresentationLayer
{
    public abstract class MessageBase
    {
        public eDevice TypeSource { get; set; }
        public eDevice TypeDest { get; set; }
        public byte Address { get; set; }

        public eControl Control { get; protected set; }
        public MessageBase(eControl ctrl)
        {
            this.TypeSource = eDevice.PC;
            this.TypeDest = eDevice.Switch;
            this.Address = 0;
            this.Control = ctrl;
        }

        public override string ToString()
        {
            return ByteArrayToString(this.ToBytes());
        }

        public virtual byte[] ToBytes()
        {
            return new byte[] { (byte)TypeDest, (byte)TypeSource, Address, (byte)Control };
        }

        public static MessageBase Parse(string data)
        {            
            //var allControls = Enum.GetValues(typeof(eControl)).OfType<byte>().ToList();
            //if(!allControls.Contains((byte)data[3])) return null;

            var ctrl = (eControl)(byte)data[3];
            MessageBase msg = null;

            switch (ctrl)
            {
                case eControl.ACK:                    
                case eControl.NAK:
                case eControl.DialCompleted:
                case eControl.LP_Update:
                case eControl.LP_ACK:
                case eControl.LP_NAK:
                //case eControl.TSL_ENQ:
                //case eControl.TSL_EOT:
                //case eControl.TSL_Prepare:
                //case eControl.TSL_ResultEnd:
                //case eControl.TSL_ResultRequest:
                    msg = new SimpleMessage(ctrl);
                    break;                

                case eControl.LineStatus:
                    msg = LineStatusMessageBase.Parse(data);
                    break;

                case eControl.AllStatus:
                    var allStatus = new AllStatusMessage();                    
                    msg = allStatus;
                    break;

                case eControl.StartAlarm:
                case eControl.EndAlarm:
                    msg = StartAlarmMessage.Parse(data);                    
                    break;

                //case eControl.TSL_ResultData:
                //    msg = TSL_ResultMessage.Parse(data);
                //    break;
            }

            if(msg != null) msg.Address = (byte)data[2];
            return msg;
        }

        public static string ByteArrayToString(byte[] arr)
        {            
            //return System.Text.Encoding.ASCII.GetString(arr);
            return System.Text.Encoding.UTF8.GetString(arr);            
        }

        public static byte ByteToHex(byte num)
        {
            return (byte)((num / 10)*16 +(num % 10));
        }
    }

    public class SimpleMessage: MessageBase
    {
        public byte? Data { get; set; }
        public SimpleMessage(eControl ctrl, byte? data = null):base(ctrl)
        {
            this.Data = data;
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            if (this.Data == null) return b;
            else
            {
                var arr = new byte[b.Length + 1];
                b.CopyTo(arr, 0);
                arr[b.Length] = this.Data.Value;
                return arr;
            }
        }        
    }

    public class LoopStatusMessage: MessageBase
    {
        public eStatus[] Status { get; private set; }
        public LoopStatusMessage():base(eControl.LoopStatus)
        {
            this.Status = new eStatus[20];
            for (int i = 0; i < this.Status.Length; i++)
                this.Status[i] = eStatus.Off;
        }

        public override byte[] ToBytes()
        {
            //var b = base.ToBytes();
            //var arr = new byte[b.Length + this.Status.Length];
            //b.CopyTo(arr, 0);
            //this.Status.Select(s => (byte)s).ToArray().CopyTo(arr, b.Length);            
            //return arr;

            var b = base.ToBytes();
            var arr = new byte[b.Length + this.Status.Length * 2];
            b.CopyTo(arr, 0);

            for (int i = 0; i < this.Status.Length; i++)
            {
                arr[b.Length + i * 2] = (byte)(i + 1);
                arr[b.Length + i * 2 + 1] = (byte)this.Status[i];
            }            
            return arr;
        }
    }    

    public class DialMessage: MessageBase
    {
        public byte LineId { get; private set; }
        public byte[] Dtmf { get; private set; }

        const int MaxDtmf = 13;
        public DialMessage(byte lineId, string number):base(eControl.Dial)
        {
            if (string.IsNullOrEmpty(number)) throw new InvalidOperationException();

            this.LineId = lineId;

            // get dtmf            
            this.Dtmf = new byte[MaxDtmf];
            for (int i = 0; i < this.Dtmf.Length; i++) this.Dtmf[i] = 0x0F;

            int count = MaxDtmf;
            if(number.Length < count) count = number.Length;
            for (int i = MaxDtmf - count; i < MaxDtmf; i++)
            {
                this.Dtmf[i] = (byte)((byte)number[i - MaxDtmf + count] & 0x0F);
            }
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + this.Dtmf.Length + 2];
            b.CopyTo(arr, 0);
            arr[b.Length] = this.LineId;
            arr[b.Length + 1] = 0x01;

            //this.Dtmf.Select(n => (byte)(n & 0x0F)).ToArray().CopyTo(arr, b.Length + 1);
            this.Dtmf.CopyTo(arr, b.Length + 2);
            return arr;
        }
    }

    public class ConnectionEnd
    {
        public byte Id { get; set; }
        public byte Volumn { get; set; }
        public eStatus Status { get; set; }
        public byte[] ToBytes()
        {
            return new byte[] { Id, Volumn, (byte)Status };
        }
    }

    public class ConnectionMessage: MessageBase
    {
        public ConnectionEnd Source { get; set; }
        public ConnectionEnd Dest { get; set; }

        public ConnectionMessage():base(eControl.Connect)
        {
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr1 = this.Source.ToBytes();
            var arr2 = this.Dest.ToBytes();

            var arr = new byte[b.Length + arr1.Length + arr2.Length];
            b.CopyTo(arr, 0);            
            arr1.CopyTo(arr, b.Length);
            arr2.CopyTo(arr, b.Length + arr1.Length);
            return arr;
        }
    }

    public class AdjVolMessage: MessageBase
    {
        public byte Id { get; set; }
        public byte Volumn { get; set; }
        public AdjVolMessage():base(eControl.AdjVol)
        {
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + 2];
            b.CopyTo(arr, 0);
            arr[b.Length] = this.Id;
            arr[b.Length + 1] = this.Volumn;
            return arr;
        }
    }

    public class ConAddMessage: MessageBase
    {
        public byte ConId { get; set; }
        //public byte Id { get; set; }
        public byte InVolumn { get; set; }
        public byte OutVolumn { get; set; }
        public eStatus AddOrRemove { get; set; }
        public eStatus IsFirstOne { get; set; }

        public ConAddMessage():base(eControl.ConAdd)
        {
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + 6];
            b.CopyTo(arr, 0);
            arr[b.Length] = this.ConId;
            arr[b.Length + 1] = this.Address; // this.Id;
            arr[b.Length + 2] = this.InVolumn;
            arr[b.Length + 3] = this.OutVolumn;
            arr[b.Length + 4] = (byte)this.AddOrRemove;
            arr[b.Length + 5] = (byte)this.IsFirstOne;
            return arr;
        }
    }

    public class ConAdjVolMessage: MessageBase
    {
        //public byte Id { get; set; }
        public byte InVolumn { get; set; }
        public byte OutVolumn { get; set; }
        public ConAdjVolMessage():base(eControl.ConAdjVol)
        {
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + 3];
            b.CopyTo(arr, 0);
            arr[b.Length] = this.Address; // this.Id;
            arr[b.Length + 1] = this.InVolumn;
            arr[b.Length + 2] = this.OutVolumn;            
            return arr;
        }
    }

    // Receivable Message
    public abstract class LineStatusMessageBase: MessageBase
    {
        public eLineStatusType Type { get; protected set; }
        public LineStatusMessageBase()
            : base(eControl.LineStatus)
        {
        }    
    
        public static new LineStatusMessageBase Parse(string s)
        {
            LineStatusMessageBase msg = null;
            var ctrl = (eLineStatusType)(byte)s[4];
            switch (ctrl)
            {
            	case eLineStatusType.Line:
                    var lineStatus = new LineStatusMessage();
                    lineStatus.Status = (eLineStatus)(byte)s[5];
                    msg = lineStatus;
            		break;

                case eLineStatusType.Revert:
                    var revertStatus = new RevertStatusMessage();
                    revertStatus.Status = (eStatus)(byte)s[5];
                    msg = revertStatus;
            		break;

                case eLineStatusType.PO_Status:
                    var poStatus = new POStatusMessage();
                    poStatus.Status = (eStatus)(byte)s[5];
                    msg = poStatus;
                    break;

                case eLineStatusType.Tone:                    
                case eLineStatusType.CallerId:
                case eLineStatusType.PO_Tone:
                    var numberStatus = new NumberReceivedMessage(ctrl);
                    numberStatus.Number = ConvertAsciiNumberToString(s.Substring(5, 13));
                    msg = numberStatus;
            		break;
            }

            if (msg != null) msg.Address = (byte)s[2];
            return msg;
        }

        public static string ConvertAsciiNumberToString(string s)
        {
            var arr = s.Select(c => (byte)c).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < arr.Count; i++)
            {                
                if (arr[i] < 10) sb.AppendFormat("{0}", arr[i].ToString());
                else if (arr[i] == 0x0B) sb.Append("*");
                else if (arr[i] == 0x0C) sb.Append("#");
            }

            return sb.ToString();
        }
    }

    public class LineStatusMessage:LineStatusMessageBase
    {
        public eLineStatus Status { get; set; }
        public LineStatusMessage()
        {
            this.Type = eLineStatusType.Line;
        }
    }

    public class RevertStatusMessage: LineStatusMessageBase
    {
        public eStatus Status { get; set; }
        public RevertStatusMessage()
        {
            this.Type = eLineStatusType.Revert;
        }
    }

    public class POStatusMessage : LineStatusMessageBase
    {
        public eStatus Status { get; set; }
        public POStatusMessage()
        {
            this.Type = eLineStatusType.PO_Status;
        }
    }

    public class NumberReceivedMessage: LineStatusMessageBase
    {
        //public byte[] Number { get; set; }        
        public string Number { get; set; }
        public NumberReceivedMessage(eLineStatusType type)
        {
            this.Type = type;            
        }
    }

    public class AllStatusMessage : MessageBase
    {
        public eStatus[] Status { get; private set; }
        public AllStatusMessage():base(eControl.AllStatus)
        {
            this.Status = new eStatus[24];
        }
    }

    public class ExtRingMessage: MessageBase
    {
        const int MaxExtRing = 4;
        public eStatus[] Status { get; private set; }
        public ExtRingMessage():base(eControl.ExtRingPower)
        {
            this.Status = new eStatus[MaxExtRing];
            SetAllStatus(eStatus.On);
        }

        public void SetAllStatus(eStatus status)
        {
            for (int i = 0; i < this.Status.Length; i++)
                this.Status[i] = status;
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + MaxExtRing*2];
            b.CopyTo(arr, 0);
            for (byte i = 0; i < MaxExtRing; i++)
            {
                arr[b.Length + i*2] = (byte)(i + 1); 
                arr[b.Length + i*2 + 1] = (byte)this.Status[i];
            }

            return arr;
        }
    }

    #region Alarm Message
    public class TimeRange
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public TimeRange()
        {
            this.Start = null;
            this.End = null;
        }
    }

    public class AlarmData
    {
        public byte Period { get; set; }
        public DateTime? Time { get; set; }
        public byte AlarmType { get; set; }
        public byte Count { get; set; }

        public byte[] ToBytes()
        {
            var arr = new byte[5];
            arr[0] = this.Period;
            var t = AlarmDataMessage.TimeToArray(this.Time);
            arr[1] = t[0];
            arr[2] = t[1];
            arr[3] = this.AlarmType;
            arr[4] = this.Count;
            return arr;
        }
    }

    public class AlarmDataMessage : MessageBase
    {        
        public AlarmData[] AlarmList { get; private set; }

        public TimeRange[] RadioTimes { get; private set; }

        public AlarmDataMessage():base(eControl.AlarmData)
        {
            this.TypeDest = eDevice.TB09;
            this.AlarmList = new AlarmData[13];
            for (int i = 0; i < this.AlarmList.Length; i++)
            {
                this.AlarmList[i] = new AlarmData();
            }            
            
            this.RadioTimes = new TimeRange[5];
            for(int i = 0; i < 5; i++)
            {
                this.RadioTimes[i] = new TimeRange();
            }
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + this.AlarmList.Length * 5 + this.RadioTimes.Length * 5];
            b.CopyTo(arr, 0);

            for (byte i = 0; i < this.AlarmList.Length; i++)
            {
                var d=  this.AlarmList[i];
                d.ToBytes().CopyTo(arr, b.Length + i * 5);
            }

            int start = b.Length + this.AlarmList.Length * 5;

            for (int i = 0; i < this.RadioTimes.Length; i++)
            {
                var rt = this.RadioTimes[i];
                arr[start + i * 5] = (byte)(i + 1);

                var t = TimeToArray(this.RadioTimes[i].Start);
                arr[start + i * 5 + 1] = t[0];
                arr[start + i * 5 + 2] = t[1];

                t = TimeToArray(this.RadioTimes[i].End);
                arr[start + i * 5 + 3] = t[0];
                arr[start + i * 5 + 4] = t[1];                
            }
            
            return arr;
        }

        public static byte[] TimeToArray(DateTime? time)
        {
            if (time.HasValue)
            {
                return new byte[2]
                {
                    ByteToHex((byte)time.Value.Hour), 
                    ByteToHex((byte)time.Value.Minute)
                };
            }
            else 
                return new byte[2] { 0xFF, 0xFF };
            //{
            //    return null;
            //}
        }
    }

    public class DayTypeDataMessage:MessageBase
    {
        public KeyValuePair<byte, byte>[] Days { get; private set; }
        public DayTypeDataMessage():base(eControl.DayTypeData)
        {
            this.TypeDest = eDevice.TB09;
            this.Days = new KeyValuePair<byte, byte>[7];            
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + this.Days.Length * 2];
            b.CopyTo(arr, 0);

            for (byte i = 0; i < this.Days.Length; i++)
            {                
                var day = this.Days[i];
                arr[b.Length + i * 2] = (byte)(day.Key + 1);
                arr[b.Length + i * 2 + 1] = day.Value;
            }
            return arr;
        }
    }

    public class DateTimeMessage:MessageBase
    {
        public DateTime DateTime { get; set; }
        public DateTimeMessage():base(eControl.DateTimeData)
        {
            this.TypeDest = eDevice.TB09;
        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + 8];
            b.CopyTo(arr, 0);
            int i = 0;
            arr[b.Length + i++] = ByteToHex((byte)((byte)DateTime.DayOfWeek + 1));
            arr[b.Length + i++] = ByteToHex((byte)this.DateTime.Day);
            arr[b.Length + i++] = ByteToHex((byte)this.DateTime.Month);
            arr[b.Length + i++] = ByteToHex((byte)(this.DateTime.Year / 100));
            arr[b.Length + i++] = ByteToHex((byte)(this.DateTime.Year % 100));
            arr[b.Length + i++] = ByteToHex((byte)this.DateTime.Hour);
            arr[b.Length + i++] = ByteToHex((byte)this.DateTime.Minute);
            arr[b.Length + i++] = ByteToHex((byte)this.DateTime.Second);
            return arr;
        }
    }

    public class StartAlarmMessage : MessageBase
    {
        public byte Period { get; set; }
        public byte Hour { get; set; }
        public byte Minute { get; set; }
        public byte Times { get; set; }

        public StartAlarmMessage()
            : base(eControl.StartAlarm)
        {
        }

        public static new StartAlarmMessage Parse(string data)
        {
            StartAlarmMessage msg = null;
            if ((eControl)(byte)data[3] == eControl.StartAlarm)
                msg = new StartAlarmMessage();
            else if ((eControl)(byte)data[3] == eControl.EndAlarm)
                msg = new EndAlarmMessage();

            if (msg == null) return null;

            msg.Period = (byte)data[4];
            msg.Hour = (byte)data[5];
            msg.Times = (byte)data[6];
            return msg;
        }
    }

    public class EndAlarmMessage: StartAlarmMessage
    {
        public EndAlarmMessage():base()
        {
            this.Control = eControl.EndAlarm;
        }
    }
    #endregion

    #region Led Panel
    public class ResultData
    {
        public byte Alert { get; set; }
        public byte Level { get; set; }
        public byte Result { get; set; }
    }
    //public class LP_ResultMessage: MessageBase
    //{
    //    public List<ResultData> Results { get; set; }
    //    public LP_ResultMessage():base(eControl.LP_Data)
    //    {

    //    }

    //    public override byte[] ToBytes()
    //    {
    //        var b = base.ToBytes();
    //        var arr = new byte[b.Length + this.Results.Count * 4];
    //        b.CopyTo(arr, 0);
    //        for (byte i = 0; i < this.Results.Count; i++)
    //        {
    //            arr[b.Length + i * 4] = (byte)(i + 1);
    //            arr[b.Length + i * 4 + 1] = this.Results[i].Alert;
    //            arr[b.Length + i * 4 + 2] = this.Results[i].Level;
    //            arr[b.Length + i * 4 + 3] = this.Results[i].Result;
    //        }
    //        return arr;
    //    }
    //}

    public class LP_ResultMessage : MessageBase
    {
        public byte Id { get; set; }
        public byte Alert { get; set; }
        public byte Level { get; set; }
        public byte Result { get; set; }
        

        public LP_ResultMessage()
            : base(eControl.LP_Data)
        {

        }

        public override byte[] ToBytes()
        {
            var b = base.ToBytes();
            var arr = new byte[b.Length + 4];
            b.CopyTo(arr, 0);
            arr[b.Length] = this.Id;
            arr[b.Length + 1] = this.Alert;
            arr[b.Length + 2] = this.Level;
            arr[b.Length + 3] = this.Result;
            return arr;
        }
    }    
    #endregion
}
