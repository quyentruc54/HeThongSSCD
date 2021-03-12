using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.DirectX.DirectSound;

namespace NovaAlert.Sound
{
    public class DSndManager : IDisposable
    {
        private DevicesCollection _devList;
        private Device[] _devices;
        private BufferDescription _bufferDescription;
        private SecondaryBuffer _sBuffer;
        private Queue<string> m_Queue;
        private bool m_bWorking;
        private Thread m_Thread;
        private eSpeakerPan _speakerPan;        

        public DSndManager(IntPtr owner, eSpeakerPan speakerPan)
        {            
            m_Queue = new Queue<string>();
            m_Thread = null;
            m_bWorking = false;
            _speakerPan = speakerPan;

            // Create Devices
            _devList = new DevicesCollection();
            _devices = new Device[_devList.Count];
            for (int i = 0; i < _devices.Length; i++)
            {
                _devices[i] = new Device(_devList[i].DriverGuid);
                _devices[i].SetCooperativeLevel(owner, CooperativeLevel.Priority);
            }

            _bufferDescription = new BufferDescription() { Flags = BufferDescriptionFlags.ControlVolume | 
                BufferDescriptionFlags.ControlFrequency | BufferDescriptionFlags.ControlPan | BufferDescriptionFlags.GlobalFocus };

            StartThread();
        }

        public eSpeakerPan SpeakerPan
        {
            get
            {
                return _speakerPan;
            }
        }

        public void PlayFile(string file, byte devId)
        {
            if (_sBuffer != null) _sBuffer.Dispose();

            Device device;
            if (devId < _devices.Length)
                device = _devices[devId];
            else
                device = _devices[0];

            _sBuffer = new SecondaryBuffer(file, _bufferDescription, device) { Volume = 0 };

            switch (_speakerPan)
            {
                case eSpeakerPan.Left:
                    _sBuffer.Pan = -10000;
                    break;
                case eSpeakerPan.Right:
                    _sBuffer.Pan = 10000;
                    break;
                case eSpeakerPan.Stereo:
                    _sBuffer.Pan = 0;
                    break;
            }

            _sBuffer.Play(0, BufferPlayFlags.Default);
        }

        public void StartThread()
        {
            m_bWorking = true;
            m_Thread = new Thread(new ThreadStart(ProcessingThread));
            m_Thread.Start();
        }

        public void StopThread()
        {
            if (m_Thread != null)
            {
                m_bWorking = false;                
                if (m_Thread.ThreadState == ThreadState.Suspended) m_Thread.Resume();
            }
        }

        private void ProcessingThread()
        {
            string strFile;

            while (m_bWorking)
            {
                strFile = "";
                // Check to see if _sBuffer is playing
                if (_sBuffer != null)
                {
                    if (_sBuffer.Status.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                }

                Monitor.Enter(m_Queue);
                if (m_Queue.Count > 0) strFile = m_Queue.Dequeue();
                Monitor.Exit(m_Queue);

                if (strFile != "")
                {
                    byte devId = Convert.ToByte(strFile.Substring(0, 2));
                    strFile = strFile.Substring(2);
                    PlayFile(strFile, devId);
                }
                else
                {
                    Thread.CurrentThread.Suspend();                    
                }
            }
        }

        public void ClearQueue()
        {
            Monitor.Enter(m_Queue);
            m_Queue.Clear();
            Monitor.Exit(m_Queue);
        }

        public void Play(string soundFile, byte devId)
        {
            Monitor.Enter(m_Queue);
            m_Queue.Enqueue(devId.ToString("00") + soundFile);
            Monitor.Exit(m_Queue);
            //_waitForNewTask.Set();
            if (m_Thread.ThreadState == ThreadState.Suspended) m_Thread.Resume();
        }

        #region IDisposable Members

        public void Dispose()
        {
            StopThread();
            if (_sBuffer != null) _sBuffer.Dispose();
            for (int i = 0; i < _devices.Length; i++)
                _devices[i].Dispose();
            _bufferDescription.Dispose();
        }

        #endregion
    }    
}
