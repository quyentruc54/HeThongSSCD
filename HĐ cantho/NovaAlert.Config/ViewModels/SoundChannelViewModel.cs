using NovaAlert.Common.Mvvm;
using NovaAlert.Sound;
using System;

namespace NovaAlert.Config.ViewModels
{
    public class SoundChannelViewModel: SoundChannel
    {
        public new int MinVolume { get { return SoundChannel.MinVolume; } }
        public new int MaxVolume { get { return SoundChannel.MaxVolume; } }
        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public System.Windows.Threading.DispatcherTimer Timer { get; private set; }

        public SoundChannelViewModel(IntPtr owner, int deviceId, eSpeakerPan pan):base(owner, deviceId, pan, 0)
        {
            this.PlayCommand = new RelayCommand(p => Play(), p => this.CanPlay);
            this.StopCommand = new RelayCommand(p => StopAndReset(), p => this.IsPlaying);
            this.PauseCommand = new RelayCommand(p => Stop(), p => this.IsPlaying);

            this.Timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            this.Timer.Interval = new TimeSpan(0, 0, 1);
            this.Timer.Tick += OnTimer;
            this.Timer.IsEnabled = true;
        }

        void StopAndReset()
        {
            this.Stop();
            this.Position = 0;
        }

        void OnTimer(object sender, EventArgs e)
        {
            OnPropertyChanged("Position");
        }

        public override void Dispose()
        {
            this.Timer.IsEnabled = false;
            base.Dispose();
        }
    }
}
