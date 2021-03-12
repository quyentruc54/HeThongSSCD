using System.Configuration;
using System.ComponentModel;
using System.Data.SqlClient;

namespace NovaAlert.Common.Setting
{
    [SettingsProvider(typeof(RegistrySettingsProvider))]
    public class RegistryDatabaseSetting : ApplicationSettingsBase
    {
        #region Singleton
        private RegistryDatabaseSetting()
            : base()
        {
        }

        private static RegistryDatabaseSetting _instance;
        public static RegistryDatabaseSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RegistryDatabaseSetting();

                return _instance;
            }
        }
        #endregion

        #region Properties

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string DataSource
        {
            get { return (string)this["DataSource"]; }
            set { this["DataSource"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string InitialCatalog
        {
            get { return (string)this["InitialCatalog"]; }
            set { this["InitialCatalog"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("sa")]
        public string UserId
        {
            get { return (string)this["UserId"]; }
            set { this["UserId"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("Fixed")]
        public eAuthenticateMode AuthenticateMode
        {
            get { return (eAuthenticateMode)this["AuthenticateMode"]; }
            set { this["AuthenticateMode"] = value; }
        }

        [Browsable(false)]
        [UserScopedSetting]
        [DefaultSettingValue("30")]
        public int ConnectionTimeout
        {
            get { return (int)this["ConnectionTimeout"]; }
            set { this["ConnectionTimeout"] = value; }
        }
        #endregion

        #region Helpers
        public string GetConstr()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = this.DataSource;
            builder.InitialCatalog = this.InitialCatalog;

            builder.UserID = this.UserId;
            builder.Password = this.Password;

            builder.MultipleActiveResultSets = true;
            builder.ConnectTimeout = this.ConnectionTimeout;

            return builder.ConnectionString;
        }

        public string GetMasterConstr()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = this.DataSource;
            builder.InitialCatalog = "master";

            builder.UserID = this.UserId;
            builder.Password = this.Password;

            builder.MultipleActiveResultSets = true;
            builder.ConnectTimeout = this.ConnectionTimeout;

            return builder.ConnectionString;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.DataSource) ||
                string.IsNullOrEmpty(this.InitialCatalog) ||
                string.IsNullOrEmpty(this.UserId))
            {
                return false;
            }

            SqlConnection cn = new SqlConnection(GetConstr());
            bool valid = false;
            try
            {
                cn.Open();
                cn.Close();
                valid = true;
            }
            catch { }

            return valid;
        }
        #endregion
    }

    public enum eAuthenticateMode
    {
        Manual = 0, Trusted = 1, Fixed = 2
    }
}
