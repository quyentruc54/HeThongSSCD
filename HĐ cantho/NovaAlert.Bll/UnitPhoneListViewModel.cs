using System;
using System.Collections.Generic;
using System.Linq;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.Windows.Data;
using System.ComponentModel;

namespace NovaAlert.Bll
{
    public class UnitPhoneListViewModel: PhoneListViewModel<UnitPhoneViewModel>
    {
        public bool ShowTask { get; protected set; }
        public bool ShowResult { get; protected set; }        

        public ICollectionView UnitCollectionView { get; private set; }
        public ICollectionView TempUnitCollectionView { get; private set; }
        
        public UnitPhoneListViewModel(ClientAppViewModel app):base(app)
        {
            GenerateViews();
            this.ShowResult = true;

            switch (this.App.WorkingMode)
            {
                case eWorkingMode.Alert:
                case eWorkingMode.CCPK_Alert:
                case eWorkingMode.TSL_Alert:
                    this.ShowTask = true;
                    break;
                
                default:
                    this.ShowTask = false;                    
                    break;
            }
        }

        private void GenerateViews()
        {
            var cs = new CollectionViewSource();
            cs.Source = this.Items;            
            cs.SortDescriptions.Clear();
            cs.SortDescriptions.Add(new SortDescription("ListOrder", ListSortDirection.Ascending));
            cs.View.Filter = new Predicate<object>(AllItemFilter);
            this.UnitCollectionView = cs.View;

            cs = new CollectionViewSource();
            cs.Source = this.Items;
            cs.View.Filter = new Predicate<object>(TempItemFilter);
            this.TempUnitCollectionView = cs.View;
        }

        bool AllItemFilter(object obj)
        {
            var unit = obj as UnitPhoneViewModel;
            return unit != null && unit.ListOrder >= 0;
        }

        bool TempItemFilter(object obj)
        {
            var unit = obj as UnitPhoneViewModel;
            return unit != null && unit.ListOrder < 0;
        }

        protected override void GetItemsFromServer()
        {            
            List<UnitPhone> list = null;
            if (this.App.IsCCPKAlertMode)
            {
                list = this.App.Service.GetUnitPhones(this.App.GroupId, eTaskType.CCPK, true);
            }
            else if (this.App.IsAlertMode)
            {
                list = this.App.Service.GetUnitPhones(this.App.GroupId, eTaskType.CTT, true);
            }
            else
            {
                list = this.App.Service.GetUnitPhones(this.App.GroupId, eTaskType.CTT, false);
            }
            
            foreach(var item in list)
            {
                var up = UnitPhoneFactory.Create(this.App.WorkingMode, item);
                if(up != null) this.Items.Add(up);
            }
        }
        
        public UnitPhoneViewModel Search(string number, string areaCode)
        {
            UnitPhoneViewModel unit = null;
            if (!string.IsNullOrEmpty(number))
            {
                unit = Items.Where(u => u.Number == number && u.AreaCode == areaCode).FirstOrDefault();
                if (unit == null)
                {
                    unit = Items.Where(u => u.Number == number).FirstOrDefault();
                }
            }

            return unit;
        }

        public void SetSelectContact(int id)
        {
            ClearTempUnits();
            UnitPhone up = null;
            if (this.App.IsCCPKAlertMode) up = this.App.Service.GetUnitPhone(id, eTaskType.CCPK);
            else up = this.App.Service.GetUnitPhone(id, eTaskType.CTT);
            if (up != null)
            {
                var phone = new UnitPhone(up.PhoneNumberId, up.Name);
                var vm = new UnitPhoneViewModel(phone)
                {
                    AreaCode = up.AreaCode,
                    Number = up.Number,
                    ListOrder = -1,
                    Password = up.Password
                };
                
                vm.ListOrder = -1;
                this.App.Units.Items.Add(vm);
                vm.OnClickCommand.Execute(null);
            }
        }

        public void SetSelectedContact(Contact ct)
        {
            ClearTempUnits();

            if (ct == null) return;

            var vm = this.Items.Where(it => it.PhoneNumberId == ct.PhoneNumberId).FirstOrDefault();
            if (vm != null)
            {
                if (vm.SelectedPanelId == null)
                {
                    vm.OnClickCommand.Execute(null);
                }
            }
            else
            {
                var phone = new UnitPhone(ct.PhoneNumberId, ct.UnitName);
                vm = new UnitPhoneViewModel(phone)
                {
                    AreaCode = ct.AreaCode,
                    Number = ct.Number,
                    ListOrder = -1,
                    //GroupId = -1,                    
                    Password = ct.Password
                };
                //vm.SelectedPanelId = this.App.ClientId;
                vm.ListOrder = -1;
                this.App.Units.Items.Add(vm);
                vm.OnClickCommand.Execute(null);
            }
        }

        public void ClearTempUnits()
        {
            var list = GetTempUnits().Where(u => this.App.MainController.IsFreeUnit(u)).ToList();
            if(list.Count > 0)
            {
                foreach (var item in list)
                    this.Items.Remove(item);
            }            
        }
        
        public bool IsTempUnitsConnected()
        {
            var list = GetTempUnits();
            return list.Count > 0 && list.All(u => !this.App.MainController.IsFreeUnit(u));
        }

        List<UnitPhoneViewModel> GetTempUnits()
        {
            return this.Items.Where(u => u.ListOrder < 0).ToList();
        }
    }       
}
