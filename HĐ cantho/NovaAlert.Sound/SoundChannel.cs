using Microsoft.DirectX.DirectSound;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NovaAlert.Sound
{
    public class SoundChannel : IDisposable, INotifyPropertyChanged, ISwitchAddress
    {
        #region Properties
        public const int MinVolume = -10000;
        public const int MaxVolume = 0;

        public eSpeakerPan SpeakerPan { get; private set; }
        public Device Device { get; private set; }
        public int DeviceId { get; private set; }
        public SecondaryBuffer Buffer { get; private set; }

        public byte Address { get; private set; }

        public bool IsPlaying
        {
            get
            {
                if (Buffer != null)
                {
                    var status = Buffer.Status;
                    return (status.Playing || status.Looping);
                }
                else
                    return false;
            }
        } 

        public bool CanPlay
        {
            get
            {
                return this.Buffer != null &&
                    Position >= 0 && Position < Duration;
            }
        }

        public int Position
        {
            get
            {
                if (this.Buffer != null)
                    return (int)(this.Buffer.PlayPosition / this.Buffer.Format.AverageBytesPerSecond);
                else
                    return -1;
            }
            set
            {
                if (value >= 0 && value <= Duration)
                {
                    this.Buffer.SetCurrentPosition(value * Buffer.Format.AverageBytesPerSecond);
                    OnPropertyChanged("Position");
                }
            }
        }

        public int Duration
        {
            get
            {
                if (this.Buffer != null)
                    return (int)(this.Buffer.Caps.BufferBytes / this.Buffer.Format.AverageBytesPerSecond);
                else
                    return -1;
            }
        }

        string _currentFile;
        public string CurrentFile
        {
            get { return _currentFile; }
            set 
            {
                try
                {
                    _currentFile = value;
                    OnCurrentFileChanged();
                    OnPropertyChanged("CurrentFile");                    
                }
                catch(Exception ex)
                {
                    _currentFile = null;
                    ClearBuffer();
                }

            }
        }

        public bool IsLoop { get; set; }

        int _volume;
        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value >= MinVolume && value <= MaxVolume)
                {
                    _volume = value;
                    if (this.Buffer != null) this.Buffer.Volume = value;
                    OnPropertyChanged("Volume");
                }                
            }
        }
        #endregion

        #region Ctor
        public SoundChannel(IntPtr owner, int devId, eSpeakerPan speakerPan, byte address)
        {
            _volume = MaxVolume;
            this.Address = address;
            this.SpeakerPan = speakerPan;
            this.DeviceId = devId;
            var devList = new DevicesCollection();
            this.Device = new Device(devList[this.DeviceId].DriverGuid);
            this.Device.SetCooperativeLevel(owner, CooperativeLevel.Priority);            
        } 
        #endregion

        #region Helpers
        void OnCurrentFileChanged()
        {
            ClearBuffer();
            this.Buffer = CreateNewBuffer(this.CurrentFile);
            Refesh();
        }

        private void Refesh()
        {
            OnPropertyChanged("Duration");
            OnPropertyChanged("Position");
            OnPropertyChanged("CanPlay");
        } 

        public virtual void Play()
        {
            if(CanPlay)
            {
                var flag = BufferPlayFlags.Default;
                if (this.IsLoop) flag |= BufferPlayFlags.Looping;
                this.Buffer.Play(0, flag);
                OnPropertyChanged("IsPlaying");
            }
        }

        public void PlayFile(string file, int pos = 0, bool looping = false)
        {
            this.CurrentFile = file;
            this.IsLoop = looping;
            if (pos > 0) this.Position = pos;
            Play();
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                this.Buffer.Stop();
                OnPropertyChanged("IsPlaying");
            }
        }

        public void ClearBuffer()
        {
            if (IsPlaying) Stop();

            if(this.Buffer != null)
            {
                this.Buffer.Dispose();
                this.Buffer = null;
            }
            Refesh();
        }

        SecondaryBuffer CreateNewBuffer(string file)
        {
            var bufferDesc = new BufferDescription()
            {
                Flags = BufferDescriptionFlags.ControlVolume | BufferDescriptionFlags.ControlFrequency |
                BufferDescriptionFlags.ControlPan | BufferDescriptionFlags.GlobalFocus
            };

            var buff = new SecondaryBuffer(file, bufferDesc, this.Device) { Volume = this.Volume };
            
            switch (this.SpeakerPan)
            {
                case eSpeakerPan.Left:
                    buff.Pan = -10000;
                    break;
                case eSpeakerPan.Right:
                    buff.Pan = 10000;
                    break;
                case eSpeakerPan.Stereo:
                    buff.Pan = 0;
                    break;
            }

            return buff;
        }
        
        #endregion

        #region Static
        public static int GetDeviceCount()
        {
            var devList = new DevicesCollection();
            return devList.Count;
        }

        public static List<KeyValuePair<byte, string>> GetSoundCards()
        {
            var list = new List<KeyValuePair<byte, string>>();
            var devList = new DevicesCollection();
            for (byte i = 0; i < devList.Count; i++)
                list.Add(new KeyValuePair<byte, string>(i, devList[i].Description));
            return list;
        } 
        #endregion

        #region IDisposable
        public virtual void Dispose()
        {
            Stop();
            ClearBuffer();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region ISwitchAddress
        public string Name
        {
            get { return string.Format("Sound {0} - {1}", this.DeviceId, this.SpeakerPan) ; }
        }
        #endregion
    }
}
