using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using NovaAlert.Sound;
using System;
using System.Collections.Generic;

namespace NovaAlert.Service
{
    public class SoundCardConfigViewModel: ViewModelBase
    {
        public List<KeyValuePair<byte, string>> SoundCardList { get { return SoundChannel.GetSoundCards(); } }

        byte _soundCard1;
        public byte SoundCard1
        {
            get { return _soundCard1; }
            set 
            {
                if (_soundCard1 != value)
                {
                    _soundCard1 = value;
                    OnPropertyChanged("SoundCard1");
                    IsModified = true;
                }
            }
        }

        byte _soundCard2;
        public byte SoundCard2
        {
            get { return _soundCard2; }
            set 
            { 
                if(_soundCard2 != value)
                {
                    _soundCard2 = value;
                    OnPropertyChanged("SoundCard2");
                    IsModified = true;
                }                
            }
        }

        bool _isModified;
        public bool IsModified
        {
            get { return _isModified; }
            set { _isModified = value; OnPropertyChanged("IsModified"); }
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand RefeshCommand { get; private set; }

        public Action CreateSoundChannels { get; private set; }
        public SoundCardConfigViewModel(Action createSoundChannels)
        {
            LoadSettings();
            this.SaveCommand = new RelayCommand(p => OnSave(), p => this.IsModified && this.SoundCard1 != this.SoundCard2);
            this.CancelCommand = new RelayCommand(p => LoadSettings(), p => IsModified);
            this.RefeshCommand = new RelayCommand(p => OnPropertyChanged("SoundCardList"));
            this.CreateSoundChannels = createSoundChannels;
        }

        void LoadSettings()
        {
            var settings = GlobalSetting.Instance;
            this.SoundCard1 = settings.SoundCard1;
            this.SoundCard2 = settings.SoundCard2;
            IsModified = false;
        }

        void OnSave()
        {
            var settings = GlobalSetting.Instance;
            settings.SoundCard1 = this.SoundCard1;
            settings.SoundCard2 = this.SoundCard2;
            settings.Save();
            IsModified = false;

            if (this.CreateSoundChannels != null)
            {
                CreateSoundChannels();
            }
        }        
    }
}
