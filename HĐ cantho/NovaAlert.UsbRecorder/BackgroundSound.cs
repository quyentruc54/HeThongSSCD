using EricOulashin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public class BackgroundSound
    {
        static BackgroundSound _instance;
        public static BackgroundSound Instance
        {
            get
            { 
                if (_instance == null) _instance = new BackgroundSound();
                return _instance;
            }
        }

        public byte[][] Sounds { get; private set; }
        private BackgroundSound()
        {
            this.Sounds = new byte[4][];

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sound");
            var fileName = Path.Combine(dir, "HOLD_ALAW.WAV");
            this.Sounds[0] = GetWavData(fileName);

            fileName = Path.Combine(dir, "CAU1_ALAW.WAV");
            this.Sounds[1] = GetWavData(fileName);

            fileName = Path.Combine(dir, "CAU2_ALAW.WAV");
            this.Sounds[2] = GetWavData(fileName);

            fileName = Path.Combine(dir, "CAU3_ALAW.WAV");
            this.Sounds[3] = GetWavData(fileName);

        }

        byte[] GetWavData(string fileName)
        {
            var wav = new WAVFile();
            wav.Open(fileName, WAVFile.WAVFileMode.READ);
            wav.SeekToAudioDataStart();
            byte[] data = new byte[wav.DataSizeBytes];
            byte[] arr;
            int start = 0;

            do
            {
                arr = wav.GetNextSample_ByteArray();
                arr.CopyTo(data, start);
                start += arr.Length;                
            } 
            while (wav.NumSamplesRemaining > 1);

            wav.Close();

            return data;
        }

        public byte[] GetSound(byte id)
        {
            return this.Sounds[id];
        }
    }
}
