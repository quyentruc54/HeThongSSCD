using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace NovaAlert.Server
{
    public class GlobalSettingViewModel: DialogViewModelBase
    {
        public GlobalSetting Setting { get; private set; }
        public List<string> Comports { get; set; }
        public List<string> ModemPorts { get; set; }
        public List<Parity> ParityList { get; set; }
        public List<StopBits> StopBitList { get; set; }

        public GlobalSettingViewModel()
        {
            this.Setting = GlobalSetting.Instance;

            this.Comports = System.IO.Ports.SerialPort.GetPortNames().ToList();

            this.ParityList = new List<Parity>()
            {
                Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space
            };

            this.StopBitList = new List<StopBits>()
            {
                StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two
            };
        }

        protected override void OnOK()
        {
            if(GetService<IMessageBoxService>().AskYesNo("Hệ thống cần phải khởi động lại để các cập nhật có hiệu lực. Bạn có chắc chắn tiếp tục hay không ?") == System.Windows.MessageBoxResult.Yes)
            {
                this.Setting.Save();
                base.OnOK();
            }            
        }

        protected override void OnCancel()
        {
            this.Setting.Reload();
            base.OnCancel();
        }
    }
}
