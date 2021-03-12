namespace NovaAlert.Entities
{
    public interface IMediaPlayer
    {
        long Duration { get; }
        string FileName { get; set; }
        NovaAlert.Common.Mvvm.RelayCommand PauseCommand { get; }
        NovaAlert.Common.Mvvm.RelayCommand PlayCommand { get; }
        long Position { get; set; }
        NovaAlert.Common.Mvvm.RelayCommand StopCommand { get; }
        int Volume { get; set; }
        bool EnableLooping { get; set; }
        bool IsPlaying { get; }
    }
}
