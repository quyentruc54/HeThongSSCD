using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NovaAlert.Common;
using NovaAlert.Communication.Base;

namespace NovaAlert.Communication.Tcp
{
    public class TcpServer : TcpBase
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        TcpListener _tcpListener;

        public TcpServer(ICommDevice recloser)
            : base(recloser)
        {
        }

        public void StartListening()
        {
            if (_ringBuffer != null)
                _ringBuffer.Dispose();
            _ringBuffer = new RingBuffer(MaxBufferSize);

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _device.Port);
            _tcpListener = new TcpListener(ipEndPoint);
            _tcpListener.Start(100);

            _tcpListener.BeginAcceptSocket(new AsyncCallback(OnAcceptSocket), _tcpListener);
        }

        public void StopListening()
        {
            try
            {
                if (_tcpListener != null)
                {
                    _tcpListener.Stop();
                }

                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Disconnect(false);
                    _socket = null;
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("StopListening Error", ex);
            }
        }

        private void Restart()
        {
            StopListening();
            System.Threading.Thread.Sleep(100);
            StartListening();
        }

        protected override void OnReceive(IAsyncResult ar)
        {
            if (_socket == null)
            {
                return;
            }

            var state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.workSocket;

            try
            {
                int receivedBytes = clientSocket.EndReceive(ar);

                if (receivedBytes > 0)
                {
                    var data = new byte[receivedBytes];
                    Array.Copy(state.buffer, 0, data, 0, receivedBytes);

                    // Raise event
                    RaiseDataReceiveEvent(data);

                    // Add data to ring buffer                    
                    lock (_ringBuffer)
                    {
                        ProcessRingBuffer(data);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnReceive", ex);
            }

            //continue receiving
            try
            {
                clientSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), state);
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("Call BeginReceive Error", ex);
                LogService.Logger.Info("Try to restart");
                Restart();
            }
        }

        private void OnAcceptSocket(IAsyncResult ar)
        {
            try
            {
                var listener = ar.AsyncState as TcpListener;
                if (listener.Server != null && listener.Server.IsBound)
                {
                    _socket = listener.EndAcceptSocket(ar);
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    RaiseStatusChangedEvent(string.Format("Connected from {0}.", _socket.RemoteEndPoint));
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("OnAccept Error", ex);
            }

            // Start receiving and & continue listening
            try
            {
                if (_socket != null)
                {
                    StateObject state = new StateObject();
                    state.workSocket = _socket;
                    _socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), state);
                    _tcpListener.BeginAcceptSocket(new AsyncCallback(OnAcceptSocket), _tcpListener);
                }
            }
            catch (Exception ex)
            {
                LogService.Logger.Error("Call BeginAcceptSocket Error", ex);
            }
        }

        protected override void OnSocketDisconnected()
        {
            RaiseStatusChangedEvent(string.Format("Disconnected from {0}.", _socket.RemoteEndPoint));
            base.OnSocketDisconnected();
        }
    }    
}
