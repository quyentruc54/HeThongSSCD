using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NovaAlert.UsbRecorder;
using NovaAlert.Common.Wpf;
using EricOulashin;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace UnitTestRecorder
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bg = BackgroundSound.Instance;
            var wave = new WAVFile(); 
            wave.Create("dv.wav", false, 8000, 8);
            
            wave.AddSample_ByteArray(bg.Sounds[2]);

            //var si = new byte[8000 * 5];
            //wave.AddSample_ByteArray(si);
            wave.Close();
        }

        [TestMethod]
        public void TestMessageBox()
        {
            var vm = new MsgBoxWithTimeoutViewModel("Test", timeout: 10);
            var view = new MsgBoxWithTimeoutView() { DataContext = vm };
            view.ShowDialog();
        }

        [TestMethod]
        public void TestNAudio()
        {
            using (var reader = new WaveFileReader("Sound\\CAU1.wav"))
            {                
                //WaveFormatConversionStream conv = new WaveFormatConversionStream(WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 8000, 1, 8), reader);               

                var sp = new VolumeSampleProvider(reader.ToSampleProvider());
                sp.Volume = 0.5f;
                using (var writer = new WaveFileWriter("dv.wav", WaveFormat.CreateALawFormat(8000, 1)))
                {
                    int sampleCount = (int)reader.SampleCount;
                    var buff = new float[sampleCount];
                    sp.Read(buff, 0, sampleCount);
                    writer.WriteSamples(buff, 0, sampleCount);
                    writer.Close();
                }
                reader.Close();
            }
        }
    }
}
