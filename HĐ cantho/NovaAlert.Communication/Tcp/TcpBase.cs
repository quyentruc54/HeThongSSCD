using System;
using System.Net.Sockets;
using NovaAlert.Common;
using NovaAlert.Communication.Base;

namespace NovaAlert.Communication.Tcp
{
    public class TcpBase
    {
        #region Members & Properties
        protected static readonly int MaxBufferSize = 1024;

        protected Socket _socket;
        public event EventHandler<StatusChangedEventArgs> OnStatusChanged;
        public event EventHandler<DataTransferEventArg> OnDataReceived;
        public event EventHandler<DataTransferEventArg> OnDataSent;

        protected RingBuffer _ringBuffer;
        public RingBuffer ReceiveBuffer
        {
            get { return _ringBuffer; }
        }

        public Func<Type, RingBuffer, byte[]> DoParse { get; set; }
        protected ICommDevice _device;

        private static int _instanceCount;

        public bool IsConnected
        {
            get
            {
                return _socket != null;
            }
        }
        #endregion

        #region Ctor()
        static TcpBase()
        {
            _instanceCount = 0;
        }

        public TcpBase(ICommDevice device)
        {
            InitilizeComponent(device);
        }
        #endregion

        #region Helpers
        protected void InitilizeComponent(ICommDevice device)
        {
            System.Threading.Interlocked.Increment(ref _instanceCount);
            _device = device;
            _ringBuffer = new RingBuffer(MaxBufferSize);
        }

        public virtual void Send(byte[] data)
        {
            if (_socket != null)
            {
                try
                {
                    if (Utility.IsConnected(_socket))
                    {
                        _socket.Send(data, 0, data.Length, SocketFlags.None);
                        if (this.OnDataSent != null)
                            this.OnDataSent(this, new DataTransferEventArg(data, _device.Port));
                    }
                    else
                    {
                        // _sockect is disconnect
                        OnSocketDisconnected();
                    }
                }
                catch (Exception ex)
                {
                    LogService.Logger.Error("Send", ex);
                }
            }
        }

        protected virtual void OnSocketDisconnected()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Disconnect(false);
                    _socket = null;
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnSocketDisconnected", ex);
            }
        }

        protected void ProcessRingBuffer(byte[] data)
        {
            try
            {
                if (_device == null) return;
                bool found = false;
                byte[] buff;
                bool dataAdded;

            Process:
                dataAdded = false;

                if (!_ringBuffer.IsFull)
                {
                    _ringBuffer.Write(data, 0, data.Length);
                    dataAdded = true;
                }

                if (DoParse != null)
                {
                    found = false;
                    buff = DoParse(_device.GetType(), _ringBuffer);

                    while (buff != null)
                    {
                        found = true;
                        _device.UpdateData(buff);

                        buff = DoParse(_device.GetType(), _ringBuffer);
                    }
                }

                // incase no data package found
                // Remove unidentified bytes from buffer to release spaces
                if (!found && _ringBuffer.IsFull)
                {
                    var byteToRelease = _ringBuffer.Capacity - 200;
                    if (byteToRelease < 0)
                    {
                        byteToRelease = _ringBuffer.Capacity - 100;
                    }

                    var unusedBytes = new byte[byteToRelease];

                    _ringBuffer.Read(unusedBytes, 0, byteToRelease);
                }

                if (!dataAdded) goto Process;
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("ProcessRingBuffer", ex);
            }
        }
        #endregion

        #region Async Result
        protected void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnSend", ex);
            }
        }

        protected virtual void OnReceive(IAsyncResult ar)
        {            
        }

        protected void RaiseDataReceiveEvent(byte[] data)
        {
            if (this.OnDataReceived != null)
            {
                this.OnDataReceived(this, new DataTransferEventArg(data, _device.Port));
            }
        }

        protected void RaiseStatusChangedEvent(string status)
        {
            if (this.OnStatusChanged != null)
            {
                OnStatusChanged(this, new StatusChangedEventArgs(status));
            }
        }
        #endregion
    }
}
