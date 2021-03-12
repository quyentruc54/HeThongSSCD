using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NovaAlert.Common.Setting
{
    public abstract class SqlSettingBase : SettingsProvider
    {
        #region Members
        protected SqlConnection _sqlconn;
        protected DataTable _dtConfigs;
        protected SqlDataAdapter _sqlAdapter;

        protected string _tableName = "TConfigs";
        public string TableName
        {
            get { return _tableName; }
            set
            {
                ThrowExceptionIfSettingLoaded();
                _tableName = value;
            }
        }

        protected string _keyNameColumn = "KeyName";
        public string KeyNameColumn
        {
            get { return _keyNameColumn; }
            set
            {
                ThrowExceptionIfSettingLoaded();
                _keyNameColumn = value;
            }
        }

        protected string _keyValueColumn = "KeyValue";
        public string KeyValueColumn
        {
            get { return _keyValueColumn; }
            set
            {
                ThrowExceptionIfSettingLoaded();
                _keyValueColumn = value;
            }
        }

        protected bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
        }
        #endregion

        #region ctor()
        public SqlSettingBase()
            : base()
        {
            _isLoaded = false;
        }
        #endregion

        #region Methods
        private void ThrowExceptionIfSettingLoaded()
        {
            if (_isLoaded)
            {
                throw new InvalidOperationException("This value can not be changed when setting is loaded.");
            }
        }

        protected virtual void LoadSettingFromDatabase(string constr)
        {
            try
            {
                _sqlconn = new SqlConnection(constr);
                _sqlAdapter = new SqlDataAdapter("SELECT * FROM " + _tableName, _sqlconn);
                _dtConfigs = new DataTable(_tableName);
                _sqlAdapter.Fill(_dtConfigs);

                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(_sqlAdapter);
                DataColumn col = _dtConfigs.Columns[_keyNameColumn];
                _dtConfigs.PrimaryKey = new DataColumn[] { col };
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                throw new Exception("SqlSettingProvider load setting from database error", ex);
            }
        }

        public override string ApplicationName
        {
            get { return "KK_Application"; }
            set { }
        }

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(this.ApplicationName, col);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            // Iterate through the settings to be stored
            // Only IsDirty=true properties should be included in propvals
            object value;
            foreach (SettingsPropertyValue propval in propvals)
            {
                // NOTE: this provider allows setting to both user- and application-scoped
                // settings. The default provider for ApplicationSettingsBase - 
                // LocalFileSettingsProvider - is read-only for application-scoped setting. This 
                // is an example of a policy that a provider may need to enforce for implementation,
                // security or other reasons.
                value = propval.SerializedValue == null ? propval.PropertyValue : propval.SerializedValue;
                SetValue(propval.Name, value);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {

            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting.Name);
                values.Add(value);
            }
            return values;
        }

        protected object GetValue(string settingName)
        {
            if (!_isLoaded)
            {
                throw new InvalidOperationException("The setting is not loaded yet.");
            }

            DataRow dr = _dtConfigs.Rows.Find(settingName);
            if (dr != null) return dr[_keyValueColumn];
            else return null;
        }

        protected void SetValue(string settingName, object value)
        {
            if (!_isLoaded)
            {
                throw new InvalidOperationException("The setting is not loaded yet.");
            }

            DataRow dr;
            dr = _dtConfigs.Rows.Find(settingName);
            if (dr != null)
                dr[_keyValueColumn] = value;
            else
            {
                dr = _dtConfigs.NewRow();
                dr[_keyNameColumn] = settingName;
                dr[_keyValueColumn] = value;
                _dtConfigs.Rows.Add(dr);
            }

            try
            {
                _sqlAdapter.Update(_dtConfigs);
            }
            catch (Exception ex)
            {
                throw new Exception("SqlSettingProvider.SetValue error", ex);
            }
        }
        #endregion
    }

    public class SqlSettingProvider : SqlSettingBase
    {
        public SqlSettingProvider()
            : base()
        {
            _tableName = "GlobalSetting";
            _keyNameColumn = "Parameter";
            _keyValueColumn = "Value";

            var constr = RegistryDatabaseSetting.Instance.GetConstr();
            LoadSettingFromDatabase(constr);
        }
    }
}
