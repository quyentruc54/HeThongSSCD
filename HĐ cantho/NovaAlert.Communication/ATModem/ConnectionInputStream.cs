using log4net;
using NovaAlert.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NovaAlert.Communication.ATModem
{
    //public class ConnectionInputStream : System.IO.StreamReader
    //{        
    //    //RingBuffer
    //    private readonly int[] _buffer = new int[2048];
    //    //ReadPointer
    //    private int _readPos = 0;
    //    //WritePointer
    //    private int _writePos = 0;

    //    public ConnectionInputStream(Stream str):base(str)
    //    {
    //    }

    //    public void ClearBuffer()
    //    {
    //        LogInfo("Clean Buffer start");
    //        lock (_buffer)
    //        {
    //            _readPos = 0;
    //            _writePos = 0;
    //        }
    //        LogInfo("Clean Buffer done");
    //    }
        
    //    public void PutByte(int b)
    //    {
    //        lock (_buffer)
    //        {
    //            if (_writePos == _buffer.Length - 1 && _readPos == 0)
    //            {
    //                throw new Exception("Buffer full no read");
    //            }
    //            else
    //            {
    //                if (_writePos == _readPos - 1)
    //                {
    //                    throw new Exception("Buffer Overrun at: " + _writePos);
    //                }
    //            }

    //            _buffer[_writePos++] = b;
    //            //wrap around
    //            if (_writePos == _buffer.Length)
    //            {
    //                _writePos = 0;
    //                LogInfo("Buffer write wrap");
    //            }

    //            Monitor.PulseAll(_buffer);
    //        }
    //    }
        
    //    public override int Read()
    //    {
    //        int result;
    //        lock (_buffer)
    //        {
    //            if (_writePos == _readPos)
    //            {
    //                try
    //                {
    //                    Monitor.Wait(_buffer);
    //                }
    //                catch (Exception)
    //                {
    //                    //TODO close???
    //                    return -1;
    //                }
    //            }
    //            result = _buffer[_readPos++];
    //            if (_readPos == _buffer.Length)
    //            {
    //                _readPos = 0;
    //                LogInfo("Buffer read wrap");
    //            }
    //        }
    //        return result;
    //    }

    //    public override void Close()
    //    {
    //        LogInfo("Buffer Close");
    //        PutByte(-1);
    //        ClearBuffer();
    //    }

    //    public void LogInfo(string s)
    //    {
    //        var log = ServiceContainer.Instance.GetService<ILog>();
    //        if(log != null) log.Info(s);
    //    }

    //}
}
