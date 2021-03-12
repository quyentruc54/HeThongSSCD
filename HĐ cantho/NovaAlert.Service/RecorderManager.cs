using NovaAlert.Common;
using NovaAlert.Common.Setting;
using NovaAlert.Service.Fake;
using NovaAlert.UsbRecorder;
using System;
using System.IO;
using System.Linq;

namespace NovaAlert.Service
{
    public static class RecorderManager
    {
        static IWaveRecorder _recorder;
        public static IWaveRecorder Recorder
        {
            get
            {                
                return _recorder;
            }            
        }

        public static void InitRecorder(string recordDir)
        {
            try
            {
                if (ClientSetting.Instance.IsFakeSystem)
                {
                    _recorder = new FakeRecorder();
                }
                else
                {
                    if (!Directory.Exists(recordDir))
                    {
                        Directory.CreateDirectory(recordDir);
                    }
                    _recorder = new WaveRecorder(recordDir, CommonResource.Instance.Channels.Count());
                    var devices = UsbReader.GetUsbDeviceList();
                    if (devices.Any())
                    {
                        _recorder.UsbDeviceSerialNumber = devices[0].Key;
                    }
                    _recorder.Start();
                }                
            }
            catch (Exception ex)
            {
                LogService.Logger.Error(ex);
            }
        }
    }
}
