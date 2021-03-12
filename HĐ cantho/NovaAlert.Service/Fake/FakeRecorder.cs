using NovaAlert.UsbRecorder;
using System;
using System.ComponentModel;

namespace NovaAlert.Service.Fake
{
    public class FakeRecorder: IWaveRecorder
    {
        public event PropertyChangedEventHandler PropertyChanged;

        RecordChannel[] IWaveRecorder.Channels
        {
            get { return null; }
        }

        bool IWaveRecorder.IsRecording()
        {
            return false;
        }

        bool IWaveRecorder.IsStarted
        {
            get { return true; }
        }

        int IWaveRecorder.NumOfChannel
        {
            get { return 0; }
        }

        void IWaveRecorder.PauseRecord(byte id, byte type)
        {
            
        }

        void IWaveRecorder.ResumeRecord(byte id)
        {
            
        }

        string IWaveRecorder.RootDir
        {
            get { return null; }
        }

        void IWaveRecorder.Start()
        {
            
        }

        string IWaveRecorder.StartRecord(byte id, byte poId)
        {
            return null;
        }

        void IWaveRecorder.Stop(bool closeCurrentRecording)
        {
            
        }

        void IWaveRecorder.StopRecord(byte id)
        {
            
        }

        string IWaveRecorder.UsbDeviceSerialNumber
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        IUsbReader IWaveRecorder.UsbReader
        {
            get { return null; }
        }


        //DateTime IWaveRecorder.LastReceived
        //{
        //    get { throw new NotImplementedException(); }
        //}

        bool IWaveRecorder.IsTimeOut
        {
            get { return false; }
        }

        void IWaveRecorder.OnSystemDateTimeChanged(DateTime dt)
        {

        }
    }    
}
