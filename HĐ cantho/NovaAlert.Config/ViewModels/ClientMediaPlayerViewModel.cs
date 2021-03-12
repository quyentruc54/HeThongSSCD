using NAudio.Wave;
using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;
using System.Runtime.Remoting.Contexts;

namespace NovaAlert.Config.ViewModels
{
    [Synchronization()]
    public class ClientMediaPlayerViewModel: ViewModelBase, IMediaPlayer
    {
        #region Properties       
        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }

        public int MaxVolume { get; private set; }
        public int MinVolume { get; private set; }

        int _volume;
        public int Volume
        {
            get { return _volume; }
            set 
            { 
                _volume = value;
                if (this.WaveIn != null)
                {
                    this.WaveIn.Volume = (float)_volume / 10;
                }
                OnPropertyChanged("Volume"); 
            }
        }

        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set 
            { 
                //if(_fileName != value)
                {
                    _fileName = value;
                    OnCurrentFileChanged();
                    OnPropertyChanged("FileName"); 
                }                
            }
        }
        
        public long Position
        {
            get 
            {
                if (this.WaveIn != null) return this.WaveIn.Position;
                else return 0;
            }
            set 
            {
                if (this.WaveIn != null)
                {                    
                    this.WaveIn.Position = value;
                    OnPropertyChanged("Position");                    
                }
            }
        }
        
        public long Duration
        {
            get 
            {
                if (this.WaveIn != null)
                {
                    return this.WaveIn.Length;
                }
                return 0;
            }
        }
        
        public System.Windows.Threading.DispatcherTimer Timer { get; private set; }

        public WaveOutEvent Player { get; private set; }
        public AudioFileReader WaveIn { get; private set; }
        public bool EnableLooping { get; set; }

        public bool IsPlaying
        {
            get
            {
                return this.Player.PlaybackState == PlaybackState.Playing;
            }
        }
        #endregion

        #region Ctor
        public ClientMediaPlayerViewModel(int deviceId)
        {
            this.MinVolume = 0;
            this.MaxVolume = 10;
            this.Volume = 5;

            this.PlayCommand = new RelayCommand(p => OnPlay(), p => CanPlay());
            this.StopCommand = new RelayCommand(p => OnStop(), p => CanStop());
            this.PauseCommand = new RelayCommand(p => OnPause(), p => CanPause());

            this.Timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            this.Timer.Interval = new TimeSpan(0, 0, 1);
            this.Timer.Tick += OnTimer;            

            this.Player = new WaveOutEvent();
            if (deviceId < WaveOut.DeviceCount)
            {
                this.Player.DeviceNumber = deviceId;
            }
            
            this.Player.PlaybackStopped += Player_PlaybackStopped;
        }

           
        #endregion

        #region Helpers

        void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (Timer.IsEnabled)
            {
                Timer.IsEnabled = false;
                this.Position = 0;
                if (this.WaveIn != null)
                {
                    this.Player.Init(this.WaveIn);
                }
                UpdateGUI();
            }
        }

        void OnCurrentFileChanged()
        {
            Reset();
            try
            {
                if (!string.IsNullOrEmpty(this.FileName) && System.IO.File.Exists(this.FileName))
                {                    
                    this.WaveIn = new AudioFileReader(this.FileName);
                    this.WaveIn.Volume = (float)_volume / 10;
                    if (!EnableLooping)
                    {
                        this.Player.Init(this.WaveIn);
                    }
                    else
                    {
                        var loop = new LoopStream(this.WaveIn);
                        this.Player.Init(loop);
                    }
                }
            }
            catch
            {
                this.WaveIn = null;
            }
            UpdateGUI();
        }

        private void Reset()
        {
            this.Player.Stop();
            if (this.WaveIn != null)
            {
                this.WaveIn.Dispose();
                this.WaveIn = null;
            }
        }

        void OnTimer(object sender, EventArgs e)
        {
            UpdateGUI();
        } 

        bool CanPlay()
        {
            return this.Player.PlaybackState != PlaybackState.Playing && this.WaveIn != null && this.WaveIn.CanRead;
        }

        void OnPlay()
        {   
            this.Player.Play();
            this.Timer.IsEnabled = true;
        }

        bool CanPause()
        {
            return this.Player.PlaybackState == PlaybackState.Playing;
        }

        void OnPause()
        {
            this.Player.Pause();
            this.Timer.IsEnabled = false;            
        }

        bool CanStop()
        {
            return this.Player.PlaybackState == PlaybackState.Playing || this.Player.PlaybackState == PlaybackState.Paused;
        }

        void OnStop()
        {
            this.Player.Stop();            
        } 

        void UpdateGUI()
        {
            OnPropertyChanged("Duration");
            OnPropertyChanged("Position");
        }
        #endregion
    }
}
