using Microsoft.Win32;
using NovaAlert.Common.Mvvm;
using NovaAlert.Config.ViewModels;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovaAlert.Bll
{
    public enum SoundSelection
    {
        None = 0,
        Main = 1,
        Second = 2
    }
    public enum FunctionSelection
    {
        None = 0,
        BDPK = 1,
        DBCD = 2,
        Mic = 3,
        File = 4
    }
    public class PrivateAlarmViewModel : ViewModelBase
    {
        public RelayCommand PrevDayCommand { get; set; }
        public RelayCommand NextDayCommand { get; set; }
        public RelayCommand BDPKCommand { get; set; }
        public RelayCommand BDCDCommand { get; set; }
        public RelayCommand MainSoundCommand { get; set; }
        public RelayCommand SecondSoundCommand { get; set; }
        public RelayCommand MicCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        DayTypeViewModel _dtView;

        public DayTypeViewModel DTView
        {
            get { return _dtView; }
            set { _dtView = value; OnPropertyChanged("DTView"); }
        }

        DayOfWeek _daySelected;
        DateTime _dateSelected;

        List<DayTypeConfig> DTCList;
        SoundSelection _soundType = SoundSelection.None;
        public SoundSelection SoundType
        {
            get { return _soundType; }
            set
            {
                _soundType = value;
                OnPropertyChanged("MSSelected");
                OnPropertyChanged("SSSelected");
            }
        }

        FunctionSelection _funcType = FunctionSelection.None;
        public FunctionSelection FuncType
        {
            get { return _funcType; }
            set
            {
                _funcType = value;
                OnPropertyChanged("FuncType");
                OnPropertyChanged("PKSelected");
                OnPropertyChanged("CDSelected");
                OnPropertyChanged("MicSelected");
                OnPropertyChanged("FileSelected");
            }
        }
        public bool PKSelected { get { return FuncType == FunctionSelection.BDPK; } }
        public bool CDSelected { get { return FuncType == FunctionSelection.DBCD; } }
        public bool MicSelected { get { return FuncType == FunctionSelection.Mic; } }
        public bool FileSelected { get { return FuncType == FunctionSelection.File; } }
        public bool MSSelected { get { return SoundType == SoundSelection.Main; } }
        public bool SSSelected { get { return SoundType == SoundSelection.Main || SoundType == SoundSelection.Second; } }
        public ClientAppViewModel App { get; private set; }

        public SwitchConnection MainConnection { get; private set; }
        public SwitchConnection SecondConnection { get; private set; }


        const byte SoundChannel = 0;
        public PrivateAlarmViewModel(ClientAppViewModel app)
        {
            this.App = app;
            var service = NovaAlert.Config.Proxy.CreateProxy();
            DTCList = service.GetDayTypes();
            _dateSelected = App.Service.GetDateTime();
            _daySelected = _dateSelected.DayOfWeek;
            LoadDay();
            PrevDayCommand = new RelayCommand(p => PrevDay());
            NextDayCommand = new RelayCommand(p => NextDay());
            BDPKCommand = new RelayCommand(p => BDPK(), p => FunctionEnable(FunctionSelection.BDPK));
            BDCDCommand = new RelayCommand(p => BDCD(), p => FunctionEnable(FunctionSelection.DBCD));
            MainSoundCommand = new RelayCommand(p => MainSound(), p => SoundEnable(SoundSelection.Main));
            SecondSoundCommand = new RelayCommand(p => SecondSound(), p => SoundEnable(SoundSelection.Second));
            MicCommand = new RelayCommand(p => Mic(), p => FunctionEnable(FunctionSelection.Mic));
            FileOpenCommand = new RelayCommand(p => FileOpen(), p => FunctionEnable(FunctionSelection.File));

            this.MainConnection = new SwitchConnection(this.App.ClientId, new SwitchConnectionEnd(OtherDevice.SoundChannels[0]), new SwitchConnectionEnd(OtherDevice.Amply));
            this.SecondConnection = new SwitchConnection(this.App.ClientId, new SwitchConnectionEnd(OtherDevice.SoundChannels[0]), new SwitchConnectionEnd(OtherDevice.Speakers[this.App.ClientId - 1]));

        }

        private void SecondSound()
        {
            if (_soundType == SoundSelection.None)
            {
                SoundType = SoundSelection.Second;
            }
            else
            {
                SoundType = SoundSelection.None;
                //Goi Ket thúc phát
                EndPlay();
            }

        }

        private void MainSound()
        {
            if (_soundType == SoundSelection.None)
            {
                SoundType = SoundSelection.Main;
            }
            else
            {
                EndPlay();
            }
        }



        private void FileOpen()
        {
            if (_funcType == FunctionSelection.None)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = this.App.Service.GetSoundPath();
                openFile.Filter = "(.wav)|*.wav|(.mp3)|*.mp3";
                openFile.Multiselect = false;
                if (openFile.ShowDialog() == true)
                {
                    FuncType = FunctionSelection.File;
                    StartPlay(openFile.FileName);
                }
            }
            else
            {
                EndPlay();
            }
        }

        private void Mic()
        {
            if (_funcType == FunctionSelection.None)
            {
                SetMicConnections(true);
                FuncType = FunctionSelection.Mic;
            }
            else
            {
                SetMicConnections(false);
                FuncType = FunctionSelection.None;
                SoundType = SoundSelection.None;
            }
        }

        private void SetMicConnections(bool isconnected)
        {
            var conn = new SwitchConnection(this.App.ClientId, new SwitchConnectionEnd(OtherDevice.Micro), new SwitchConnectionEnd(OtherDevice.Speakers[0]));
            conn.IsConnected = isconnected;
            this.App.Service.SendConnection(conn);
            if (_soundType == SoundSelection.Main)
            {
                conn = new SwitchConnection(this.App.ClientId, new SwitchConnectionEnd(OtherDevice.Micro), new SwitchConnectionEnd(OtherDevice.Amply));
                conn.IsConnected = isconnected;
                this.App.Service.SendConnection(conn);
            }
        }

        private void BDCD()
        {
            if (_funcType == FunctionSelection.None)
            {
                FuncType = FunctionSelection.DBCD;
                StartPlay(SoundFile.ChienDau);
            }
            else
            {
                EndPlay();
            }
        }

        private void BDPK()
        {
            if (_funcType == FunctionSelection.None)
            {
                FuncType = FunctionSelection.BDPK;
                StartPlay(SoundFile.PhongKhong);
            }
            else
            {
                EndPlay();
            }
        }

        private void SetConnection(bool isconnected)
        {
            if (isconnected)
            {
                this.SecondConnection.Source.IsConnected = true;
                this.SecondConnection.Dest.IsConnected = false;
            }
            else
            {
                this.SecondConnection.IsConnected = false;
            }

            this.App.Service.SendConnection(this.SecondConnection);

            if (_soundType == SoundSelection.Main)
            {
                if (isconnected)
                {
                    this.MainConnection.Source.IsConnected = true;
                    this.MainConnection.Dest.IsConnected = false;
                }
                else
                {
                    this.MainConnection.IsConnected = false;
                }

                this.App.Service.SendConnection(this.MainConnection);
                this.App.Service.SendAmplyPower(isconnected);
            }
        }
        private void StartPlay(string fileNm)
        {
            SetConnection(true);
            this.App.Service.PlaySound(SoundChannel, 0, fileNm);
        }

        private void EndPlay()
        {
            if (_funcType != FunctionSelection.None)
            {
                //Goi Ket thúc phát
                this.App.Service.StopSound(SoundChannel);
                SetConnection(false);
            }

            SoundType = SoundSelection.None;
            FuncType = FunctionSelection.None;

        }
        bool SoundEnable(SoundSelection type)
        {
            return _soundType == SoundSelection.None || _soundType == SoundSelection.Main || _soundType == type;
        }
        bool FunctionEnable(FunctionSelection type)
        {
            return _soundType != SoundSelection.None && (_funcType == FunctionSelection.None || _funcType == type);
        }
        private void NextDay()
        {
            _dateSelected = _dateSelected.AddDays(1);
            _daySelected = _dateSelected.DayOfWeek;
            LoadDay();
        }

        private void PrevDay()
        {
            _dateSelected = _dateSelected.AddDays(-1);
            _daySelected = _dateSelected.DayOfWeek;
            LoadDay();
        }

        private void LoadDay()
        {
            var obj = DTCList.Where(d => d.DayOfWeek == (byte)_daySelected).FirstOrDefault();
            DTView = new DayTypeViewModel(this.App, obj);
        }
    }
}
