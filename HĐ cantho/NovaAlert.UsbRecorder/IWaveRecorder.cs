using System;
namespace NovaAlert.UsbRecorder
{
    public interface IWaveRecorder: System.ComponentModel.INotifyPropertyChanged
    {
        RecordChannel[] Channels { get; }
        bool IsRecording();
        bool IsStarted { get; }
        int NumOfChannel { get; }
        void PauseRecord(byte id, byte type);
        void ResumeRecord(byte id);
        string RootDir { get; }
        void Start();
        string StartRecord(byte id, byte poId);
        void Stop(bool closeCurrentRecording);
        void StopRecord(byte id);
        string UsbDeviceSerialNumber { get; set; }
        IUsbReader UsbReader { get; }
        //DateTime LastReceived { get; }
        bool IsTimeOut { get; }
        void OnSystemDateTimeChanged(DateTime dt);
    }    
}
