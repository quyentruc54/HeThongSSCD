using EricOulashin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public class RecordChannel
    {
        WAVFile _waveFile;
        public int Index { get; private set; }
        public string RootDir { get; private set; }
        public bool IsRecording
        {
            get { return _waveFile != null; }
        }

        //public bool IsPaused { get; private set; }

        public DateTime? PausedTime { get; set; }
        public byte PausedType { get; set; }

        public byte CurrentPOId { get; private set; }

        public string RecordingFileName
        {
            get
            {
                if (_waveFile != null) return Path.GetFileName(_waveFile.Filename);
                else return null;
            }
        }

        Queue<byte> _buffer;

        public RecordChannel(int index, string rootDir)
        {
            this.Index = index;
            this.RootDir = Path.Combine(rootDir, string.Format("Kenh {0}", this.Index.ToString()));
            if (!Directory.Exists(this.RootDir)) Directory.CreateDirectory(this.RootDir);
            _waveFile = null;
            _buffer = new Queue<byte>();
        }

        string NewRecordFileName()
        {
            var ts = DateTime.Now.ToString("yyMMddHHmmss_") + this.Index.ToString() + ".wav";
            return Path.Combine(RootDir, ts);
        }

        public void StartRecord(byte poId)
        {
            if (!IsRecording)
            {
                this.CurrentPOId = poId;
                _waveFile = new WAVFile();
                _waveFile.Create(NewRecordFileName(), false, 8000, 8);
                //this.IsPaused = false;
                this.PausedTime = null;
            }            
        }

        public void StopRecord()
        {
            if(this.PausedTime.HasValue)
            {
                OnResume();
                this.PausedTime = null;
            }

            if(_waveFile != null)
            {
                if (_buffer.Count >= 0) _waveFile.AddSample_ByteArray(_buffer.ToArray());
                _waveFile.Close();
                _waveFile = null;
                _buffer.Clear();
            }
        }

        public void Pause(byte pauseType = 0)
        {
            if (!this.IsRecording) return;

            if (this.PausedTime == null)
            {
                this.PausedTime = DateTime.Now;
                this.PausedType = pauseType;
            }
            else if(this.PausedType != pauseType)
            {
                OnResume();
                this.PausedTime = DateTime.Now;
                this.PausedType = pauseType;
            }
        }

        public void Resume()
        {
            //if (this.IsRecording && this.IsPaused) this.IsPaused = false;
            if(this.IsRecording && this.PausedTime.HasValue)
            {
                OnResume();
                this.PausedTime = null;
            }
        }

        public void AddData(byte b)
        {
            if (this.PausedTime.HasValue) return;

            if (_waveFile != null)
            {
                lock(_buffer)
                {
                    _buffer.Enqueue(b);
                    if (_buffer.Count >= 8000)
                    {
                        _waveFile.AddSample_ByteArray(_buffer.ToArray());
                        _buffer.Clear();
                    }
                }
            }
        }        

        public string Description
        {
            get { return string.Format("Kênh {0}", this.Index); }
        }

        void OnResume()
        {
            if(_waveFile == null) return;
                        
            // Clear current buffer
            if(_buffer.Count > 0)
            {
                //_waveFile.AddSample_ByteArray(_buffer.ToArray());
                _buffer.Clear();
            }

            var data = BackgroundSound.Instance.GetSound(this.PausedType);
            if (data == null) return;            

            var duration = (int)(DateTime.Now - this.PausedTime.Value).TotalSeconds;
            var bgDuration = (int)(data.Length / 8000);

            while(duration > 0)
            {
                if (duration < bgDuration)
                {
                    var copyLength = duration * 8000;
                    var tmp = new byte[copyLength];
                    for (int i = 0; i < copyLength; i++)
                        tmp[i] = data[i];

                    _waveFile.AddSample_ByteArray(tmp);

                    duration = 0;
                }
                else
                {
                    _waveFile.AddSample_ByteArray(data);
                    duration -= bgDuration;
                }
            }            
            
        }
    }
}
