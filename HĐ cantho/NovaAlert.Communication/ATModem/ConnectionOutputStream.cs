using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NovaAlert.Communication.ATModem
{
    //public class ConnectionOutputStream : System.IO.StreamWriter
    //{
    //    private readonly DefaultConnection _conn;

    //    public ConnectionOutputStream(DefaultConnection conn, Stream str):base(str)
    //    {
    //        this._conn = conn;
    //    }

    //    //public override void Close()
    //    //{
    //    //    outerInstance.disconnect();
    //    //}        

    //    public override void Write(char b)
    //    {
    //        if (_conn.OnlineDataMode)
    //        {
    //            _conn.Modem.ModemOutputStream.WriteByte((byte)b);
    //        }
    //        else
    //        {
    //            throw new IOException("Stream closed");
    //        }
    //    }

    //    public override Encoding Encoding
    //    {
    //        get { return System.Text.Encoding.ASCII; }
    //    }
    //}
}
