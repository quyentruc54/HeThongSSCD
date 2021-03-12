using System.Configuration;

namespace NovaAlert.Common.Setting
{
    [SettingsProvider(typeof(SqlSettingProvider))]
    public class GlobalSetting : ApplicationSettingsBase
    {
        #region singleton
        static GlobalSetting _instance;
        public static GlobalSetting Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GlobalSetting();
                }

                return _instance;
            }
        }

        private GlobalSetting():base()
        {
        }
        #endregion

        public string GetParamByName(string name)
        {
            var r = string.Empty;

            try
            {
                r = this[name].ToString();
            }
            catch { }            

            return r;
        }

        [UserScopedSetting]
        [DefaultSettingValue("069")]        
        public string RestrictedAreaCode
        {
            get { return (string)this["RestrictedAreaCode"]; }
            set
            {
                this["RestrictedAreaCode"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("SỞ CHỈ HUY - QUÂN KHU 7")]
        public string OfficeName
        {
            get { return (string)this["OfficeName"]; }
            set
            {
                this["OfficeName"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool Menu_DisplayConfig_Visible
        {
            get { return (bool)this["Menu_DisplayConfig_Visible"]; }
            set
            {
                this["Menu_DisplayConfig_Visible"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool Menu_CCPK_Visible
        {
            get { return (bool)this["Menu_CCPK_Visible"]; }
            set
            {
                this["Menu_CCPK_Visible"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool ShowTSL
        {
            get { return (bool)this["ShowTSL"]; }
            set
            {
                this["ShowTSL"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("9600")]
        public int BaudRate
        {
            get
            {
                return ((int)(this["BaudRate"]));
            }
            set
            {
                this["BaudRate"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("None")]
        public System.IO.Ports.Parity Parity
        {
            get
            {
                return ((System.IO.Ports.Parity)(this["Parity"]));
            }
            set
            {
                this["Parity"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("8")]
        public byte DataBits
        {
            get
            {
                return ((byte)(this["DataBits"]));
            }
            set
            {
                this["DataBits"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("One")]
        public global::System.IO.Ports.StopBits StopBits
        {
            get
            {
                return ((global::System.IO.Ports.StopBits)(this["StopBits"]));
            }
            set
            {
                this["StopBits"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("0")]
        public byte SoundCard1
        {
            get
            {
                return ((byte)(this["SoundCard1"]));
            }
            set
            {
                this["SoundCard1"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("1")]
        public byte SoundCard2
        {
            get
            {
                return ((byte)(this["SoundCard2"]));
            }
            set
            {
                this["SoundCard2"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("COM1")]
        public string PortName
        {
            get
            {
                return ((string)(this["PortName"]));
            }
            set
            {
                this["PortName"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("False")]
        public bool UseSwitchPortForLP
        {
            get { return ((bool)(this["UseSwitchPortForLP"])); }
            set { this["UseSwitchPortForLP"] = value; }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("D:\\Working\\NovaAlert\\NovaAlert.Server\\bin\\x86\\Debug\\Record")]
        public string RecordFolder
        {
            get
            {
                return ((string)(this["RecordFolder"]));
            }
            set
            {
                this["RecordFolder"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("False")]
        public bool AlarmOnSwitch
        {
            get
            {
                return ((bool)(this["AlarmOnSwitch"]));
            }
            set
            {
                this["AlarmOnSwitch"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("COM2")]
        public string LPPortName
        {
            get
            {
                return ((string)(this["LPPortName"]));
            }
            set
            {
                this["LPPortName"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("")]
        public string ModemPortName
        {
            get
            {
                return ((string)(this["ModemPortName"]));
            }
            set
            {
                this["ModemPortName"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string ModemAreaCode
        {
            get
            {
                return ((string)(this["ModemAreaCode"]));
            }
            set
            {
                this["ModemAreaCode"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string ModemNumber
        {
            get
            {
                return ((string)(this["ModemNumber"]));
            }
            set
            {
                this["ModemNumber"] = value;
            }
        }

        [UserScopedSetting()]        
        [DefaultSettingValue("4")]
        public int MinRecordSize
        {
            get
            {
                return ((int)(this["MinRecordSize"]));
            }
            set
            {
                this["MinRecordSize"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DefaultSettingValue("1")]
        public int TSL_PrepareTries
        {
            get
            {
                return ((int)(this["TSL_PrepareTries"]));
            }
            set
            {
                this["TSL_PrepareTries"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("5")]
        public int TSL_ReceiveTries
        {
            get
            {
                return ((int)(this["TSL_ReceiveTries"]));
            }
            set
            {
                this["TSL_ReceiveTries"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0.5")]
        public float Recorder_BackgroundSoundVolume
        {
            get
            {
                return ((float)(this["Recorder_BackgroundSoundVolume"]));
            }
            set
            {
                this["Recorder_BackgroundSoundVolume"] = value;
            }
        }
    }
}
