using NovaAlert.Common.Mvvm;
using NovaAlert.UsbRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
namespace TestRecorder
{
    class RecorderViewModel: ViewModelBase
    {
        public RelayCommand RecordCommand { get; private set; }
        public RelayCommand StopRecordCommand { get; private set; }
        public RelayCommand OpenRecordFolderCommand { get; private set; }

        public RelayCommand OpenDeviceCommand { get; private set; }
        public RelayCommand CloseDeviceCommand { get; private set; }

        string _rootDir;
        public string RootDir
        {
            get { return _rootDir; }
            set { _rootDir = value; OnPropertyChanged("RootDir"); }
        }

        RecordChannel _selectedChannel;
        public RecordChannel SelectedChannel
        {
          get { return _selectedChannel; }
          set { _selectedChannel = value; OnPropertyChanged("SelectedChannel");}
        }

        public List<KeyValuePair<string, string>> Devices { get; private set; }

        string _selectedSerialNumber;
        public string SelectedSerialNumber
        {
            get { return _selectedSerialNumber; }
            set 
            { 
                if(_selectedSerialNumber != value)
                {
                    _selectedSerialNumber = value;
                    OnPropertyChanged("SelectedSerialNumber");

                    //if (this.Recorder.IsStarted) this.Recorder.Stop();
                    //this.Recorder.UsbDeviceSerialNumber = _selectedSerialNumber;
                    //this.Recorder.Start();
                }                
            }
        }

        public WaveRecorder Recorder { get; private set; }

        public bool IsRecording
        {
            get { return this.Recorder.IsRecording(); }
        }

        public bool IsIdle
        {
            get { return !IsRecording; }
        }

        public RecorderViewModel()
        {            
            _rootDir = Path.Combine(System.Environment.CurrentDirectory, "TestRecords");
            if (!Directory.Exists(_rootDir)) Directory.CreateDirectory(_rootDir);

            this.Recorder = new WaveRecorder(_rootDir);
            _selectedChannel = this.Recorder.Channels.First();
            
            this.Devices = UsbReader.GetUsbDeviceList();
            if (this.Devices.Count > 0) this.SelectedSerialNumber = this.Devices[0].Key;

            this.RecordCommand = new RelayCommand(p => OnRecord(), p => CanRecord());
            this.StopRecordCommand = new RelayCommand(p => OnStopRecord(), p => this.Recorder.IsStarted && !CanRecord());
            this.OpenRecordFolderCommand = new RelayCommand(p => OpenRecordFolder());

            this.OpenDeviceCommand = new RelayCommand(p => OnOpenDevice(), p => this.Recorder.IsStarted == false && !string.IsNullOrEmpty(this.SelectedSerialNumber));
            this.CloseDeviceCommand = new RelayCommand(p => OnCloseDevice(), p => this.Recorder.IsStarted);
        }

        public new void Refesh()
        {
            OnPropertyChanged("IsRecording");
            OnPropertyChanged("IsIdle");
        }

        bool CanRecord()
        {
            return this.Recorder.IsStarted && this.SelectedChannel != null && !this.SelectedChannel.IsRecording;
        }
        void OnRecord()
        {            
            //this.SelectedChannel.StartRecord();
            //RecordAll(true);
        }

        void OnStopRecord()
        {            
            this.SelectedChannel.StopRecord();
            //RecordAll(false);
        }

        void OpenRecordFolder()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(this.RootDir);
            p.Start();
        }

        //void RecordAll(bool isRecord)
        //{
        //    foreach(var channel in this.Recorder.Channels)
        //    {
        //        if (isRecord) channel.StartRecord();
        //        else channel.StopRecord();
        //    }
        //}

        void OnOpenDevice()
        {
            if (this.Recorder.IsStarted) this.Recorder.Stop(true);
            this.Recorder.UsbDeviceSerialNumber = this.SelectedSerialNumber;
            this.Recorder.Start();
        }

        void OnCloseDevice()
        {
            this.Recorder.Stop(true);            
        }

        // test
    }
}
