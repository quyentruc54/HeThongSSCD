using com.rusanu.DBUtil;
using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Setting;
using System;
using System.Data.SqlClient;
using System.IO;

namespace NovaAlert.Server
{
    public class ConfigDatabaseViewModel : DialogViewModelBase
    {
        #region Members & Properties
        RegistryDatabaseSetting _setting = RegistryDatabaseSetting.Instance;

        public string DataSource
        {
            get { return _setting.DataSource; }
            set
            {
                _setting.DataSource = value;
                OnPropertyChanged("DataSource");
            }
        }

        public string InitialCatalog
        {
            get { return _setting.InitialCatalog; }
            set
            {
                _setting.InitialCatalog = value;
                OnPropertyChanged("InitialCatalog");
            }
        }

        public string UserId
        {
            get { return _setting.UserId; }
            set
            {
                _setting.UserId = value;
                OnPropertyChanged("UserId");
            }
        }

        public string Password
        {
            get { return _setting.Password; }
            set
            {
                _setting.Password = value;
                OnPropertyChanged("Password");
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                OnPropertyChanged("IsDirty");
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                OnPropertyChanged("IsValid");
            }
        }

        RelayCommand _testCommand;
        public RelayCommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand(p => DoTest(), p => IsDirty);
                }

                return _testCommand;
            }
        }

        RelayCommand _createDatabaseCommand;
        public RelayCommand CreateDatabaseCommand
        {
            get
            {
                if (_createDatabaseCommand == null)
                {
                    _createDatabaseCommand = new RelayCommand(p => OnCreateDatabase());
                }
                return _createDatabaseCommand;
            }
        }

        #endregion

        #region Ctor
        public ConfigDatabaseViewModel()
        {
            IsDirty = false;
            DoTest();
            this.PropertyChanged += ConfigDatabaseViewModel_PropertyChanged;
        }

        void ConfigDatabaseViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty" && e.PropertyName != "IsValid")
            {
                IsDirty = true;
                IsValid = false;
            }
        }
        #endregion

        #region Helpers
        private void DoTest()
        {
            IsValid = _setting.IsValid();
        }

        protected override void OnCancel()
        {
            _setting.Reload();
            base.OnCancel();
        }

        protected override void OnOK()
        {
            _setting.Save();
            base.OnOK();
        }

        protected override bool CanOK()
        {
            return IsDirty && IsValid;
        }

        void OnCreateDatabase()
        {            
            using(var cn = new SqlConnection(_setting.GetMasterConstr()))
            {
                try
                {
                    cn.Open();
                    string dbName = _setting.InitialCatalog;

                    var sql = string.Format("Select db_id('{0}')", dbName);
                    var cmd = cn.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        if(GetService<IMessageBoxService>().AskYesNo(
                            string.Format("Kho dữ liệu tên '{0}' đã tồn tại. Bạn có muốn xóa dữ liệu hiện tại và tiếp tục không ?", dbName)) == System.Windows.MessageBoxResult.No)
                        {
                            return;
                        }

                        cmd.CommandText = string.Format("Drop Database [{0}]", dbName);
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = string.Format("Create Database [{0}]", dbName);
                    cmd.ExecuteNonQuery();

                    var nl = System.Environment.NewLine;
                    cmd.CommandText = string.Format("Use {0}", dbName);
                    cmd.ExecuteNonQuery();

                    SqlCmd sqlCmd = new SqlCmd(cn);
                    sqlCmd.ExecuteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "create.sql"));

                    cn.Close();
                    GetService<IMessageBoxService>().ShowInfo("Tạo mới dữ liệu thành công.");
                }
                catch(Exception ex)
                {
                    GetService<IMessageBoxService>().ShowError("Lỗi, tạo dữ liệu không thành công.");
                    LogService.Logger.Error(ex);
                }
            }
        }
        #endregion
    }
}
