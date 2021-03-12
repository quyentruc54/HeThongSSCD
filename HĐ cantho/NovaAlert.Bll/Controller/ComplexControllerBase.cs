using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.ComponentModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Abstract class for complex controller. A complex controller is a controller which can contain other controllers.
    /// </summary>
    public abstract class ComplexControllerBase : ControllerBase
    {
        const string NotEnoughChannels = "Không đủ kênh hoặc không có kênh điện thoại phù hợp";

        #region Members & Properties
        IController _activeController;
        public IController ActiveController
        {
            get
            {
                return _activeController;
            }
            protected set
            {
                var newController = value;
                if (newController != _activeController)
                {
                    var oldController = _activeController;
                    _activeController = newController;
                    OnPropertyChanged("ActiveController");
                    OnPropertyChanged("IsRecording");

                    if (_activeController != null)
                    {
                        OnControllerDeactivate(oldController);
                    }
                }
                this.ClientApp.VolumnControl.Obj = GetLastActiveController() as IAdjustVolumn;
            }
        }
        public ObservableCollection<IController> Controllers { get; private set; }

        #endregion

        #region Ctor
        public ComplexControllerBase(ClientAppViewModel app)
            : base(app)
        {
            this.Controllers = new ObservableCollection<IController>();
            this.Controllers.CollectionChanged += Controllers_CollectionChanged;
        }
        #endregion

        #region Override
        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
        }

        public override void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
        }

        public override void OnDialCompleted(ChannelEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
        }

        public override bool CanFinalise()
        {
            return this.Controllers.Count == 0 || this.Controllers.All(c => c.CanFinalise());
        }

        bool _isFinalising = false;
        public override void Finalise()
        {
            lock (Controllers)
            {
                _isFinalising = true;
                foreach (var ctrl in this.Controllers.Where(c => c.FinalisedDate == null))
                {
                    ctrl.Finalise();
                }
            }
            base.Finalise();
        }
        #endregion

        #region Helpers
        public IController GetLastActiveController()
        {
            if (_activeController is ComplexControllerBase)
            {
                return ((ComplexControllerBase)_activeController).ActiveController;
            }
            else
            {
                return _activeController;
            }
        }

        protected void ProcessMessageInController<T>(T e) where T : BaseEventArgs
        {
            bool isUserAction = false;
            lock (Controllers)
            {
                foreach (var ctrl in Controllers.Where(c => c.FinalisedDate == null))
                {
                    if (e is ResourceChangedEventArgs)
                        ctrl.OnResourceChanged(e as ResourceChangedEventArgs);
                    else if (e is POStatusChangedEventArgs)
                        ctrl.OnPOStatusChanged(e as POStatusChangedEventArgs);
                    else if (e is ChannelStatusChangedEventArgs)
                        ctrl.OnChannelStatusChanged(e as ChannelStatusChangedEventArgs);
                    else if (e is PhoneEventArgs<HostPhoneViewModel>)
                    {
                        ctrl.ChannelClicked(this, e as PhoneEventArgs<HostPhoneViewModel>);
                        isUserAction = true;
                    }
                    else if (e is PhoneEventArgs<UnitPhoneViewModel>)
                    {
                        ctrl.UnitClicked(this, e as PhoneEventArgs<UnitPhoneViewModel>);
                        isUserAction = true;
                    }
                    else if (e is ChannelEventArgs)
                        ctrl.OnDialCompleted(e as ChannelEventArgs);
                    else if (e is TaskChangedEventArgs)
                        ctrl.OnTaskChanged(e as TaskChangedEventArgs);

                    if (e.Handled)
                    {
                        if (ctrl.FinalisedDate == null && isUserAction)
                            this.ActiveController = ctrl;
                        break;
                    }
                }
            }
        }

        public override IEnumerable<ISwitchConnection> GetConnections()
        {
            return this.Controllers.SelectMany(m => m.GetConnections()).OfType<ISwitchConnection>();
        }

        public override IEnumerable<UnitPhoneViewModel> GetUnits()
        {
            return this.Controllers.SelectMany(m => m.GetUnits());
        }

        public bool IsFreeChannel(HostPhoneViewModel channel)
        {
            lock (this.Controllers)
            {
                if (channel.Status == ePhoneStatus.Free || channel.Status == ePhoneStatus.Selected ||
                    channel.LineStatus == eLineStatus.Ring)
                {
                    return this.Controllers.SelectMany(c => c.GetConnections()).SelectMany(c => c.Devices)
                    .All(d => d.Address != channel.HostPhone.Address);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsFreeUnit(UnitPhoneViewModel unit)
        {
            lock (this.Controllers)
            {
                return this.Controllers.SelectMany(c => c.GetUnits()).All(u => u.Id != unit.Id);
            }
        }

        public IController GetController(HostPhoneViewModel channel)
        {
            return Controllers.FirstOrDefault(c => 
                c.GetConnections().SelectMany(cc => cc.Devices).Any(d => d.Address == channel.HostPhone.Address));
        }

        public virtual void ReleaseAllFreeChannels()
        {
            if (this.ClientApp.Channels == null)
            {
                return;
            }

            lock (Controllers)
            {
                var selectedChannels = this.ClientApp.GetSelectedChannels();
                foreach (var item in selectedChannels)
                {
                    var controller = GetController(item);
                    if (controller == null)
                    {
                        this.ClientApp.Service.Release(item.ResourceType, item.Id);
                    }
                    else if (controller.FinalisedDate == null && controller.CanFinalise())
                    {
                        controller.Finalise();
                        this.ClientApp.Service.Release(item.ResourceType, item.Id);
                    }
                }
            }
        }

        public void ReleaseAllFreeUnits()
        {
            if (this.ClientApp.Units == null)
            {
                return;
            }

            lock (Controllers)
            {
                foreach (var item in ClientApp.Units.Items.Where(unit => unit.SelectedPanelId == ClientApp.ClientId && IsFreeUnit(unit)).ToList())
                {
                    this.ClientApp.Service.Release(item.ResourceType, item.Id);
                }
            }
        }

        public List<KeyValuePair<UnitPhoneViewModel, HostPhoneViewModel>> GetAvailableChannels(IEnumerable<UnitPhoneViewModel> units)
        {
            int unitsCount = units.Count();
            var list = new List<KeyValuePair<UnitPhoneViewModel, HostPhoneViewModel>>();

            var channels = this.ClientApp.GetChannels(c => this.ClientApp.IsChannelUsable(c) && IsFreeChannel(c)).ToList();
            if (channels.Count < unitsCount)
            {
                throw new InvalidOperationException(NotEnoughChannels);
            }

            var hotChannels = channels.Where(c => c.HostPhone.HotUnitId.HasValue);
            List<int> hotUnitIds = new List<int>();

            // pairing hot channel && unit first
            foreach (var c in hotChannels)
            {
                hotUnitIds.Add(c.HostPhone.HotUnitId.Value);

                var unit = units.FirstOrDefault(u => u.Id == c.HostPhone.HotUnitId);
                if (unit != null)
                {
                    list.Add(new KeyValuePair<UnitPhoneViewModel, HostPhoneViewModel>(unit, c));
                }
                channels.Remove(c);
            }

            if (channels.Count < units.Where(u => !hotUnitIds.Contains(u.Id)).Count())
            {
                throw new InvalidOperationException(NotEnoughChannels);
            }

            // pairing remain channel && unit
            foreach (var unit in units.Where(u => !hotUnitIds.Contains(u.Id)))
            {
                //var excludeAreaCodes = ClientApp.Service.GetParameterValue(eGlobalParameter.RestrictedAreaCode);

                var ch = channels.FirstOrDefault(c => c.CanMakeCall(unit.AreaCode, this.ClientApp.RestrictedAreaCodes));

                if (ch != null)
                {
                    list.Add(new KeyValuePair<UnitPhoneViewModel, HostPhoneViewModel>(unit, ch));
                    channels.Remove(ch);
                }
            }

            if (list.Count < unitsCount)
            {
                throw new InvalidOperationException(NotEnoughChannels);
            }

            return list;
        }

        protected virtual void Controllers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var newItems = e.NewItems.OfType<IController>();
                if (!newItems.Any())
                {
                    return;
                }

                foreach (var item in newItems)
                {
                    item.OnFinalised += Controller_OnFinalised;
                    item.PropertyChanged += Controller_PropertyChanged;
                }

                this.ActiveController = newItems.LastOrDefault();
            }
            else
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && this.Controllers.Count == 0)
                {
                    this.ActiveController = null;
                }
            }
        }

        protected virtual void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRecording")
                OnPropertyChanged("IsRecording");
        }

        protected virtual void Controller_OnFinalised(object sender, EventArgs e)
        {
            var ctrl = sender as IController;
            if (ctrl != null)
            {
                ctrl.OnFinalised -= Controller_OnFinalised;
                ctrl.PropertyChanged -= Controller_PropertyChanged;

                lock (Controllers)
                {
                    if (!_isFinalising && this.Controllers.Contains(ctrl)) this.Controllers.Remove(ctrl);
                }
            }
        }

        protected virtual void AddController(IController ctrl)
        {
            lock (Controllers)
            {
                Controllers.Add(ctrl);
            }
        }

        protected virtual void OnControllerDeactivate(IController oldController)
        {
        }

        public override void Dispose()
        {
            if (this._isFinalising == false)
            {
                this.Finalise();
            }
        }
        #endregion
    }
}
