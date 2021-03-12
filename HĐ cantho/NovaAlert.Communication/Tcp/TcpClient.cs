using System;
using System.Net;
using System.Net.Sockets;
using NovaAlert.Common;

namespace NovaAlert.Communication.Tcp
{
    public class TcpClient : TcpBase
    {
        protected byte[] _rvcBuffer = new byte[MaxBufferSize];
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public TcpClient(ICommDevice recloser, string serverAddress, int serverPort)
            : base(recloser)
        {
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
        }

        public void Connect()
        {
            if (_socket == null)
            {
                _socket = CreateSocketConnection();
            }
        }

        private Socket CreateSocketConnection()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(this.ServerAddress), this.ServerPort));

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.BeginReceive(_rvcBuffer, 0, _rvcBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), socket);

            return socket;
        }

        public void Disconnect()
        {
            if (_socket != null)
            {
                _socket.BeginDisconnect(false, OnDisconnect, _socket);
            }
        }

        private void OnDisconnect(IAsyncResult ar)
        {
            Socket clientSocket = (Socket)ar.AsyncState;
            clientSocket.EndDisconnect(ar);

            if (ar.IsCompleted)
            {
                _socket = null;
            }
        }

        public override void Send(byte[] data)
        {
            if (_socket != null)
            {
                if (!_socket.Poll(-1, SelectMode.SelectWrite))
                {
                    _socket.Disconnect(false);
                    _socket.Dispose();

                    _socket = CreateSocketConnection();
                }
            }
            else
            {
                _socket = CreateSocketConnection();
            }

            base.Send(data);
        }

        protected override void OnReceive(IAsyncResult ar)
        {
            Socket clientSocket = (Socket)ar.AsyncState;
            int receivedBytes = clientSocket.EndReceive(ar);

            try
            {
                if (receivedBytes > 0)
                {
                    var data = new byte[receivedBytes];
                    Array.Copy(_rvcBuffer, 0, data, 0, receivedBytes);

                    // Raise event                    
                    RaiseDataReceiveEvent(data);

                    // Add data to ring buffer                    
                    ProcessRingBuffer(data);
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnReceive", ex);
            }
            finally
            {
                //continue receiving
                clientSocket.BeginReceive(_rvcBuffer, 0, _rvcBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
            }
        }
    }
}
