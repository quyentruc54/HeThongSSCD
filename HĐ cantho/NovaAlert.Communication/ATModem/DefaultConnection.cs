using System;
using System.IO;

namespace NovaAlert.Communication.ATModem
{
    public class DefaultConnection : IConnection
    {
        private AbstractModem _modem;
        public IModem Modem { get { return _modem; } }

        //internal readonly Stream connectionInputStream;
        //internal readonly Stream connectionOutputStream;        
        
        public DefaultConnection(AbstractModem modem)
        {
            _modem = modem;
            
            //this.connectionInputStream = modem.Parser.OutputStream;
            //this.connectionOutputStream = modem.Parser.OutputStream;
        }
        //public Stream InputStream
        //{
        //    get
        //    {
        //        return connectionInputStream;
        //    }
        //}
        //public Stream OutputStream
        //{
        //    get
        //    {
        //        return connectionOutputStream;
        //    }
        //}

        public virtual void SendData(string data)
        {
            if(this.Connected)
            {
                this.Modem.DataLink.SendData(data);
            }
        }

        //public virtual void ReceiveData(string data)
        //{

        //}

        public void Disconnect()
        {
            try
            {
                _modem.HangUp();
            }            
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    //connectionInputStream.Close();
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }
        public  void ExitDataMode()
        {
            throw new System.NotSupportedException("Not supported yet.");
        }
        public  void ReenterDataMode()
        {
            throw new System.NotSupportedException("Not supported yet.");
        }
        public  bool Connected
        {
            get
            {
                return _modem.Connection == this;
            }
        }
        public  bool OnlineDataMode
        {
            get
            {
                return _modem.Parser.IsOnlineDataMode();
            }
        }
        public  bool OnlineCommandMode
        {
            get
            {
                throw new System.NotSupportedException("Not supported yet.");
            }
        }

        ~DefaultConnection()
        {
            if (Connected)
            {
                Disconnect();
            }
        }
    }
}
