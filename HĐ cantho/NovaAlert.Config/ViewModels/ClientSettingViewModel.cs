using Microsoft.Win32;
using NAudio.Wave;
using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using NovaAlert.Entities;
using System.Collections.Generic;

namespace NovaAlert.Config.ViewModels
{
    public class ClientSettingViewModel : ConfigViewModelBase
    {
        public List<KeyValuePair<byte, string>> SoundCardList 
        { 
            get 
            { 
                var list = new List<KeyValuePair<byte, string>>();

                for(byte i = 0; i < WaveOut.DeviceCount; i++)
                {
                    var cap = WaveOut.GetCapabilities(i);
                    list.Add(new KeyValuePair<byte, string>(i, cap.ProductName));
                }
                return list;
            } 
        }
        public ClientSetting Setting { get; private set; }

        public RelayCommand FileBrowseCommand { get; private set; }
        public RelayCommand TestRingToneCommand { get; private set; }
        public RelayCommand StopRingToneCommand { get; private set; }

        public bool NeedReload { get; private set; }               

        public byte ClientId 
        {
            get { return this.Setting.ClientId; }
            set 
            { 
                if(this.Setting.ClientId != value)
                {
                    this.Setting.ClientId = value;
                    OnPropertyChanged("ClientId");
                    this.NeedReload = true;
                }                
            }
        }

        public byte POId
        {
            get { return this.Setting.POId; }
            set 
            { 
                if(this.Setting.POId != value)
                {
                    this.Setting.POId = value;
                    OnPropertyChanged("POId");
                    this.NeedReload = true;
                }                
            }
        }

        public byte LocalSoundId
        {
            get { return this.Setting.LocalSoundId; }
            set { this.Setting.LocalSoundId = value; OnPropertyChanged("LocalSoundId"); }
        }

        public byte RingtoneVolumn
        {
            get { return this.Setting.RingtoneVolumn; }
            set { this.Setting.RingtoneVolumn = value; OnPropertyChanged("RingtoneVolumn"); }
        }

        public string RingTone
        {
            get { return this.Setting.RingTone; }
            set { this.Setting.RingTone = value; OnPropertyChanged("RingTone"); }
        }

        public ClientSettingViewModel(IClientApp app): base(app)
        {
            this.App.AddLog("Khai báo bàn điều khiển", false);
            this.Setting = ClientSetting.Instance;
            this.FileBrowseCommand = new RelayCommand(p => SelectRingTone());
            this.TestRingToneCommand = new RelayCommand(p => OnTestRingTone(), p => !string.IsNullOrEmpty(this.RingTone) && !this.App.MediaPlayer.IsPlaying);
            this.StopRingToneCommand = new RelayCommand(p => OnStopRingTone(), p => this.App.MediaPlayer.IsPlaying);
        }

        protected override void Save()
        {
            OnStopRingTone();
            var msg = "Bạn có chắc chắn lưu lại thay đổi hay không ?";
            if (GetService<IMessageBoxService>().AskYesNo(msg) == System.Windows.MessageBoxResult.No) return;
            this.Setting.Save();
            if(this.NeedReload)
            {
                this.App.Reload();
                this.NeedReload = false;
            }

            this.App.AddLog("Lưu khai báo bàn điều khiển thành công", true);            
        }

        protected override void Cancel()
        {
            OnStopRingTone();
            this.Setting.Reload();
            this.App.AddLog("Hủy thay đổi khai báo bàn điều khiển", true);
            Refesh();
        }

        void SelectRingTone()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Mp3|*.mp3|Wave|*.wav|All Files|*.*";

            if(dlg.ShowDialog() == true)
            {
                this.RingTone = dlg.FileName;
            }
        }

        void OnTestRingTone()
        {
            this.App.MediaPlayer.EnableLooping = false;
            this.App.MediaPlayer.FileName = this.RingTone;
            this.App.MediaPlayer.Volume = ClientSetting.Instance.RingtoneVolumn;
            this.App.MediaPlayer.PlayCommand.Execute(null);
        }

        void OnStopRingTone()
        {
            if (this.App.MediaPlayer.IsPlaying)
            {
                this.App.MediaPlayer.StopCommand.Execute(null);
            }
        }

        protected override void OnDispose()
        {
            OnStopRingTone();
            this.Setting.Reload();
            base.OnDispose();
        }
    }
}
