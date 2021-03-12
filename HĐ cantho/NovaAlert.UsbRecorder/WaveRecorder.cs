using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public class WaveRecorder : NovaAlert.UsbRecorder.IWaveRecorder //, INotifyPropertyChanged
    {
        #region Constants
        public int NumOfChannel { get; private set; } // new data format, only recording from PO //22;
        const int DataLength = 5;
        const byte Begin1 = 0x0D;
        const byte Begin2 = 0x0A;        
        #endregion

        #region Properties
        CircularBuffer<byte> _ringBuffer;
        public RecordChannel[] Channels { get; private set; }
        public IUsbReader UsbReader { get; private set; }

        public string RootDir { get; private set; }
        public string UsbDeviceSerialNumber { get; set; }

        object _syncObj = new object();
        public DateTime LastReceived { get; private set; }
        public DateTime? LastResetTime { get; private set; }

        bool _isTimeOut;
        public bool IsTimeOut
        {
            get { return _isTimeOut; }
            private set
            {
                if(_isTimeOut != value)
                {
                    _isTimeOut = value;
                    OnPropertyChanged("IsTimeOut");
                }
            }
        }

        System.Threading.Timer _timer;
        #endregion

        #region Ctor
        public WaveRecorder(string rootDir, int n = 20)
        {
            this.RootDir = rootDir;
            _ringBuffer = new CircularBuffer<byte>(1024 * 8, true);

            this.NumOfChannel = n;
            this.Channels = new RecordChannel[this.NumOfChannel];
            for (int i = 0; i < this.NumOfChannel; i++)
                this.Channels[i] = new RecordChannel(i, this.RootDir);

            this.LastReceived = DateTime.Now;
            this.LastResetTime = null;
            _timer = new System.Threading.Timer(OnTimer, this, 1000, 2000);
        } 
        #endregion

        #region Helpers

        public void Start()
        {
            _ringBuffer.Clear();
            //if (string.IsNullOrEmpty(this.UsbDeviceSerialNumber)) this.UsbReader = new UsbReader();
            //else this.UsbReader = new UsbReader(this.UsbDeviceSerialNumber);

            ResetReader();
        }

        void ResetReader()
        {
            if(this.UsbReader != null)
            {
                this.UsbReader.OnDataReceived -= UsbReader_OnDataReceived;
                this.UsbReader.Stop();
                this.UsbReader.Dispose();
            }

            this.UsbReader = new UsbReader2();

            this.UsbReader.OnDataReceived += UsbReader_OnDataReceived;
            this.UsbReader.Start();
        }

        public void Stop(bool closeCurrentRecording)
        {
            if(this.UsbReader != null)
            {
                this.UsbReader.OnDataReceived -= UsbReader_OnDataReceived;
                this.UsbReader.Stop();                
                this.UsbReader = null;

                if(closeCurrentRecording)
                {
                    foreach (var channel in this.Channels.Where(c => c.IsRecording))
                        channel.StopRecord();
                }                
            }
        }

        void UsbReader_OnDataReceived(object sender, UsbDataReceiveEventArgs e)
        {
            this.LastReceived = DateTime.Now;
            if (this.Channels.Any(c => c.IsRecording))
            {
                Action<UsbDataReceiveEventArgs> act = new Action<UsbDataReceiveEventArgs>(ProcessRecordData);
                act.BeginInvoke(e, null, null);
                //ProcessRecordData(e);
            }
            else if (_ringBuffer.Count() > 0) _ringBuffer.Clear();            
        }

        private void ProcessRecordData(UsbDataReceiveEventArgs e)
        {
            lock (_ringBuffer)
            {
                _ringBuffer.Put(e.Data, 0, e.Data.Length);

                bool found = true;
                int start = Find(Begin1);

                ArrayList arr = new ArrayList();
                while (found)
                {
                    int count = _ringBuffer.Count();
                    found = start >= 0 &&
                        count > start + 2 + DataLength &&
                        _ringBuffer.ElementAt(start + 1) == Begin2;

                    if (found)
                    {
                        var len = start + DataLength + 2;
                        var data = new byte[len];
                        _ringBuffer.Get(data, 0, len);

                        foreach (var channel in this.Channels.Where(c => c.IsRecording))
                        {
                            //channel.AddData(data[start + 2 + channel.Index]);
                            channel.AddData(data[start + 2 + channel.CurrentPOId]);
                        }
                        
                        start = Find(Begin1);
                    }
                }
            }
        }

        int Find(byte data)
        {
            var count = _ringBuffer.Count();
            for (int i = 0; i < count; i++)
            {
                if (_ringBuffer.ElementAt(i) == data) return i;
            }
            return -1;
        }
        
        public bool IsStarted
        {
            get { return this.UsbReader != null; }
        }

        public bool IsRecording()
        {
            return this.UsbReader != null &&
                this.Channels.Any(c => c.IsRecording);
        }

        RecordChannel GetChannelById(byte id)
        {
            return this.Channels[id - 1];
        }

        public string StartRecord(byte id, byte poId)
        {
            var channel = GetChannelById(id);
            lock(channel)
            {
                if (channel.IsRecording && channel.CurrentPOId != poId) channel.StopRecord();
                if (!channel.IsRecording) channel.StartRecord(poId);
                return channel.RecordingFileName;
            }            
        }

        public void StopRecord(byte id)
        {
            var channel = GetChannelById(id);
            lock(channel)
            {
                if (channel.IsRecording) channel.StopRecord();
            }            
        }

        public void PauseRecord(byte id, byte type)
        {
            var channel = GetChannelById(id);
            lock(channel)
            {
                channel.Pause(type);
            }            
        }

        public void ResumeRecord(byte id)
        {
            var channel = GetChannelById(id);
            lock(channel)
            {
                channel.Resume();
            }            
        } 
        #endregion

        #region Timer
        void OnTimer(object state)
        {
            try
            {
                if (!this.UsbReader.IsReading) return;

                var interval = (DateTime.Now - LastReceived).TotalSeconds;

                if (interval > 2)
                {
                    this.IsTimeOut = true;

                    // after 5 seconds
                    if (interval > 7)
                    {
                        if (this.LastResetTime == null ||
                            ((DateTime.Now - this.LastResetTime.Value).TotalSeconds > 5) && (DateTime.Now - this.LastResetTime.Value).TotalSeconds < 30)
                        {
                            ResetReader();
                            this.LastResetTime = DateTime.Now;
                        }
                    }

                }
                else
                {
                    this.IsTimeOut = false;
                    if (this.LastResetTime.HasValue) this.LastResetTime = null;
                }            
            }
            catch(Exception ex)
            {
                NovaAlert.Common.LogService.Logger.Error("WaveRecorder.OnTimer", ex);
            }
        }

        public void OnSystemDateTimeChanged(DateTime dt)
        {
            this.LastReceived = DateTime.Now;
            this.LastResetTime = null;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        } 
        #endregion
    }
}
