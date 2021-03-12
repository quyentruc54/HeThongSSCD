using System.Configuration;

namespace NovaAlert.Common.Setting
{
    [SettingsProvider(typeof(RegistrySettingsProvider))]
    public class ClientSetting : ApplicationSettingsBase
    {
        #region Singleton
        private ClientSetting()
            : base()
        {
        }

        private static ClientSetting _instance;
        

        public static ClientSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClientSetting();

                return _instance;
            }
        }
        #endregion

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public byte ClientId
        {
            get { return (byte)this["ClientId"]; }
            set { this["ClientId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public byte LocalSoundId
        {
            get { return (byte)this["LocalSoundId"]; }
            set { this["LocalSoundId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public byte RingtoneVolumn
        {
            get { return (byte)this["RingtoneVolumn"]; }
            set { this["RingtoneVolumn"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("2")]
        public byte CurrentMode
        {
            get { return (byte)this["CurrentMode"]; }
            set { this["CurrentMode"] = value; }
        }

        [UserScopedSetting]
        public string RingTone
        {
            get { return (string)this["RingTone"]; }
            set { this["RingTone"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool IsFakeSystem
        {
            get { return (bool)this["IsFakeSystem"]; }
            set { this["IsFakeSystem"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public byte POId
        {
            get { return (byte)this["POId"]; }
            set { this["POId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public int MultiDestGroupId
        {
            get { return (int)this["MultiDestGroupId"]; }
            set { this["MultiDestGroupId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public int AlertGroupId
        {
            get { return (int)this["AlertGroupId"]; }
            set { this["AlertGroupId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("100")]
        public int ResultViewer_Interval
        {
            get { return (int)this["ResultViewer_Interval"]; }
            set { this["ResultViewer_Interval"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("30")]
        public int ResultViewer_DataRefeshInterval
        {
            get { return (int)this["ResultViewer_DataRefeshInterval"]; }
            set { this["ResultViewer_DataRefeshInterval"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0.05")]
        public double ResultViewer_ScrollDistance
        {
            get { return (double)this["ResultViewer_ScrollDistance"]; }
            set { this["ResultViewer_ScrollDistance"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("SỞ CHỈ HUY QUÂN SỰ TỈNH LONG AN")]
        public string ResultViewer_Title
        {
            get { return (string)this["ResultViewer_Title"]; }
            set { this["ResultViewer_Title"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool ResultViewer_ShowCTT
        {
            get { return (bool)this["ResultViewer_ShowCTT"]; }
            set { this["ResultViewer_ShowCTT"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool ResultViewer_ShowPK
        {
            get { return (bool)this["ResultViewer_ShowPK"]; }
            set { this["ResultViewer_ShowPK"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool ResultViewer_ShowSubResult
        {
            get { return (bool)this["ResultViewer_ShowSubResult"]; }
            set { this["ResultViewer_ShowSubResult"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool ResultViewer_ShowVerticalLines
        {
            get { return (bool)this["ResultViewer_ShowVerticalLines"]; }
            set { this["ResultViewer_ShowVerticalLines"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool ResultViewer_AutoScroll
        {
            get { return (bool)this["ResultViewer_AutoScroll"]; }
            set { this["ResultViewer_AutoScroll"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool IsInDebugMode
        {
            get { return (bool)this["IsInDebugMode"]; }
            set { this["IsInDebugMode"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("100")]
        public int Recorder_ReadDelayInterval
        {
            get { return (int)this["Recorder_ReadDelayInterval"]; }
            set { this["Recorder_ReadDelayInterval"] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool Menu_CCPK_Visible
        {
            get { return (bool)this["Menu_CCPK_Visible"]; }
            set { this["Menu_CCPK_Visible"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool Menu_DisplayConfig_Visible
        {
            get { return (bool)this["Menu_DisplayConfig_Visible"]; }
            set { this["Menu_DisplayConfig_Visible"] = value; }
        }
    }
}
