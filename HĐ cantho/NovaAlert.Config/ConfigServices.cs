using NovaAlert.Common.Wpf;
using NovaAlert.Config.ViewModels;
using NovaAlert.Config.Views;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NovaAlert.Config
{
    public class ConfigServices
    {
        public static event EventHandler<SystemDateTimeChangedEventArgs> OnSystemDateTimeChangedHandler;

        //public static void RaiseDateTimeChangedEvent(DateTime dt)
        //{
        //    try
        //    {
        //        NovaAlert.Common.Utils.SystemDateHelper.SetSystemTime(dt);
        //        if (OnSystemDateTimeChangedHandler != null)
        //        {
        //            OnSystemDateTimeChangedHandler(null, new SystemDateTimeChangedEventArgs(dt));
        //        }
        //    }
        //    catch
        //    {                
        //    }
        //}
        public static ContactViewModel ShowContact(IClientApp app, IEnumerable<HostPhoneViewModel> channels = null, 
            bool allowMultiSelect = false, Action<HostPhoneViewModel, string> callFunc = null)
        {
            var vm = new ContactViewModel(app, true, channels);
            vm.AllowMultiSelect = allowMultiSelect;
            vm.CallFunc = callFunc;
            var v = new ContactListView();
            v.DataContext = vm;
            v.Height = 500;
            v.Width = 700;

            if (NovaAlert.Common.Wpf.ModalDialog.ShowControl(v, "Danh mục điện thoại") == true && vm.ItemSelected != null)
            {
                return vm;
            }
            return null;
        }

        public static void ShowResults(List<ResultData> results, eTaskType taskType, string hostName)
        {
            UserControl view;
            if (taskType == eTaskType.CTT)
            {
                view = new NovaAlert.Config.Views.ResultDataListView() { DataContext = results };
                ModalDialog.ShowControl(view, string.Format("{0} - Kết quả CTT", hostName), false, true);
            }
            else
            {
                view = new NovaAlert.Config.Views.ResultDataListView_CCPK() { DataContext = results };
                ModalDialog.ShowControl(view, string.Format("{0} - Kết quả chuyển cấp", hostName), false, true);
            }
        }
    }
}
