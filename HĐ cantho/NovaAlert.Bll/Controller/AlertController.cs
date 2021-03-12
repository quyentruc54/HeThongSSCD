using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho CTT
    /// </summary>
    public class AlertController: MainController
    {
        public eTaskType TaskType { get; protected set; }

        #region Ctor
        public AlertController(ClientAppViewModel app)
            : base(app, true)
        {
            this.TaskType = eTaskType.CTT;
        } 
        #endregion

        #region Helpers        
        public void ChangeTask(eTask task)
        {
            var selectedUnits = this.ClientApp.GetSelectedUnits();

            if(!selectedUnits.Any())
            {
                this.ClientApp.ShowError("Vui lòng chọn các đơn vị cần khai báo.");
                return;
            }

            this.ClientApp.AddLog("Khai báo nhiệm vụ");
            
            if(selectedUnits.Any(u => u.Task.Result == eTaskResult.CTT || u.Task.Result == eTaskResult.NL))
            {
                if (this.TaskType == eTaskType.CTT)
                {
                    this.ClientApp.ShowError("Không thể thay đổi nhiệm vụ với các đơn vị đã nhận lệnh hoặc đã chuyển trạng thái.");
                }
                else
                {
                    this.ClientApp.ShowError("Không thể thay đổi nhiệm vụ với các đơn vị đã nhận lệnh hoặc đã chuyển cấp.");
                }
                return;
            }

            if(this.ClientApp.GetUnits(u => u.SelectedPanelId != this.ClientApp.ClientId &&
                u.Task.CurrentTask != eTask.None && u.Task.CurrentTask != task).Any())
            {
                this.ClientApp.ShowError("Nhiệm vụ được chọn không giống với nhiệm vụ đang khai báo cho các đơn vị còn lại.");
                return;
            }

            foreach (var u in selectedUnits)
            {
                u.Task.CurrentTask = task;
            }
        }

        public void ChangeTaskLevel(eTaskLevel level)
        {
            this.ClientApp.AddLog("Khai báo cấp báo động");
            var selectedUnits = this.ClientApp.GetSelectedUnits();

            if (selectedUnits.Any(u => u.Task.Result == eTaskResult.CTT || u.Task.Result == eTaskResult.NL))
            {
                this.ClientApp.ShowError("Không thể thay đổi cấp báo động với các đơn vị đã nhận lệnh hoặc đã chuyển trạng thái.");
                return;
            }

            if (!selectedUnits.Any())
            {
                this.ClientApp.ShowError("Vui lòng chọn các đơn vị cần khai báo.");                
            }
            else
            {
                foreach (var u in selectedUnits)
                {
                    u.Task.Level = level;
                }
            }            
        }

        public void ClearTask()
        {
            this.ClientApp.AddLog("Xóa nhiệm vụ");
            var selectedUnits = this.ClientApp.GetSelectedUnits();
            if (!selectedUnits.Any())
            {
                this.ClientApp.ShowError("Vui lòng chọn các đơn vị cần khai báo.");                
            }
            else
            {
                foreach (var u in selectedUnits)
                {
                    u.Task.TaskObj = new Task();
                    this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, null, this.TaskType);
                }
                this.ClientApp.ShowInfo("Xóa nhiệm vụ thành công.");
            }            
        }

        public virtual void CommitTaskChanges()
        {
            this.ClientApp.AddLog("Lưu nhiệm vụ");
            var taskChangedUnits = this.ClientApp.GetSelectedUnits(u => u.IsTaskChanged);

            if(!taskChangedUnits.Any())
            {
                if (this.ClientApp.GetSelectedUnits().Any())
                {
                    this.ClientApp.ShowError("Vui lòng khai báo nhiệm vụ và cấp báo động.");
                }
                else
                {
                    this.ClientApp.ShowError("Vui lòng chọn các đơn vị cần khai báo.");
                }
            }
            else
            {
                if (taskChangedUnits.Any(u => u.Task.TaskObj.CurrentTask == eTask.None))
                {
                    this.ClientApp.ShowError("Vui lòng khai báo nhiệm vụ.");
                    return;
                }

                if (taskChangedUnits.Any(u => u.Task.TaskObj.Level == eTaskLevel.None))
                {
                    this.ClientApp.ShowError("Vui lòng khai báo cấp báo động.");
                    return;
                }

                foreach (var u in taskChangedUnits)
                {
                    this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, null, this.TaskType);
                }
                this.ClientApp.ShowInfo("Lưu nhiệm vụ thành công.");
            }            
        }

        public void OnConnect()
        {
            this.ClientApp.AddLog("Kết nối");
            lock (this.Controllers)
            {
                var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
                if (ctrl != null)
                {
                    ctrl.Finalise();                    
                }
                else
                {
                    var freeSelectedUnits = this.ClientApp.GetSelectedUnits(u =>IsFreeUnit(u));

                    if (freeSelectedUnits.Any())
                    {
                        if (ValidateUnitsBeforeConnecting(freeSelectedUnits) == false)
                        {
                            return;
                        }

                        ctrl = AlertConferenceFactory.CreateAlertConferenceController(this.ClientApp, freeSelectedUnits, this.TaskType);
                        this.Controllers.Add(ctrl);
                        this.ActiveController = ctrl;
                    }                    
                }

                OnPropertyChanged("HasConference");
                OnPropertyChanged("IsRecording");
            }
        }

        protected virtual bool ValidateUnitsBeforeConnecting(IEnumerable<UnitPhoneViewModel> units)
        {
            if (units.Any(u => u.IsTaskChanged))
            {
                this.ClientApp.ShowError("Vui lòng cập nhật nhiệm vụ / cấp báo động trước.");
                return false;
            }

            // ktra cac don vi chua khai bao nv, cap bd
            var invalidUnits = units.Where(u => !u.Task.IsValid || u.Task.Result == eTaskResult.NL || u.Task.Result == eTaskResult.CTT);
            if (invalidUnits.Any())
            {
                var sb = new StringBuilder();
                sb.Append("Vui lòng khai báo nhiệm vụ, cấp báo động cho đơn vị: ");
                sb.Append(string.Join(", ", invalidUnits.Select(u => u.Name).ToArray()));
                this.ClientApp.ShowError(sb.ToString());
                return false;
            }

            return true;
        }

        public void OnIssue()
        {
            this.ClientApp.AddLog("Phát lệnh");
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl != null && ctrl.CanIssue)
            {
                ctrl.OnIssue();
                OnPropertyChanged("Issued");
            }
        }

        public bool CanIssue()
        {
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl == null)
            {
                return false;
            }
            else
            {
                return ctrl.Issued || ctrl.CanIssue;
            }
        }

        public virtual void OnReceive()
        {
            this.ClientApp.AddLog("Đã nhận lệnh");
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl != null)
            {
                ctrl.OnReceive();
            }
            else
            {
                var canReceiveUnits = this.ClientApp.GetSelectedUnits(u => u.CanReceive());
                foreach (var unit in canReceiveUnits)
                {
                    var old =   (DateTime.Now - unit.Task.TaskObj.CreatedDate).TotalSeconds;
                    unit.Task.Result = eTaskResult.NL;
                    this.ClientApp.UpdateTaskAsync(unit.Id, unit.Task.TaskObj,(long) old, this.TaskType);
                    //    this.ClientApp.UpdateTaskAsync(unit.Id, unit.Task.TaskObj, unit.Task.LastDuration, this.TaskType);
                    ReleaseResource(unit);
                }
            }
        }

        public virtual bool CanReceive()
        {
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl != null)
            {
                return ctrl.Issued && ctrl.SelectedMember != null && ctrl.SelectedMember.CanReceiveCommand();
            }

            var selectedUnits = this.ClientApp.GetSelectedUnits();
            return selectedUnits.Any() && selectedUnits.All(u => u.CanReceive());
        }

        public bool CanChangeStatus()
        {
            if (this.HasConference == true)
            {
                return false;
            }

            var selectedUnits = this.ClientApp.GetSelectedUnits();
            return selectedUnits.Any() && selectedUnits.All(u => u.Task.Result == eTaskResult.NL);
        }

        public void OnChangeStatus()
        {
            var canChangeStatusUnits = this.ClientApp.GetSelectedUnits(u => u.Task.Result == eTaskResult.NL);
            foreach (var u in canChangeStatusUnits)
            {
                u.Task.Result = eTaskResult.CTT;
                this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, u.Task.LastDuration, this.TaskType);
                this.ClientApp.Service.Release(eResourceType.UnitPhone, u.Id);
            }
        }

        public override bool? HasConference
        {
            get
            {
                var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
                return ctrl != null;
            }
        }

        public override bool CanConnect()
        {
            return this.HasConference == true || this.ClientApp.GetSelectedUnits(u => IsFreeUnit(u)).Any();            
        }

        public bool Issued
        {
            get
            {
                var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
                return ctrl != null && ctrl.Issued;
            }
        }
        #endregion

        #region Override        
        protected override void ReleaseResource(PhoneViewModel item)
        {
            if (item.ResourceType == eResourceType.UnitPhone)
            {
                var unit = item as UnitPhoneViewModel;
                if (unit.IsTaskChanged)
                {
                    ClientApp.Service.RefeshTask(unit.Id);
                }
            }
            base.ReleaseResource(item);
        }

        public override void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
            var item = e.Item;

            if (this.HasConference == true)
            {
                e.Handled = true;
                return;
            }


            if (e.Item.Status == ePhoneStatus.Ring && this.ClientApp.Channels.PO.Status == ePOStatus.OffHook)
            {
                var list = this.Controllers.OfType<ICallController>().Where(c => c.IsHolding == false).ToList();
                foreach (var c in list) c.Finalise();
                AddController(new InCallController(this.ClientApp, e.Item));
            }
            else
            {
                base.ChannelClicked(sender, e);
            }
        }

        public override void OnTaskChanged(TaskChangedEventArgs e)
        {
            var up = this.ClientApp.Units.Items.Where(u => u.Id == e.Id).FirstOrDefault();
            if (up != null)
            {                
                if (e.Task != null) up.Task.TaskObj = e.Task;
                e.Handled = true;
            }
        }
        #endregion

        protected override void Controller_OnFinalised(object sender, EventArgs e)
        {
            base.Controller_OnFinalised(sender, e);
            OnPropertyChanged("Issued");
        }
    }

    public class CCPK_AlertController: AlertController
    {
        #region Ctor
        public CCPK_AlertController(ClientAppViewModel app)
            : base(app)
        {
            this.TaskType = eTaskType.CCPK;
        } 
        #endregion

        public override void CommitTaskChanges()
        {
            this.ClientApp.AddLog("Lưu cấp báo động");
            var hasTaskChangedUnits = this.ClientApp.GetSelectedUnits(u => u.IsTaskChanged);

            if (hasTaskChangedUnits.Any())
            {
                if (hasTaskChangedUnits.Any(u => u.Task.TaskObj.Level == eTaskLevel.None))
                {
                    this.ClientApp.ShowError("Vui lòng khai báo cấp báo động.");
                    return;
                }

                foreach (var u in hasTaskChangedUnits)
                {
                    this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, null, this.TaskType);
                }
                this.ClientApp.ShowInfo("Lưu cấp báo động thành công.");
            }
            else
            {
                if (this.ClientApp.GetSelectedUnits().Any())
                {
                    this.ClientApp.ShowError("Vui lòng khai báo cấp báo động.");
                }
                else
                {
                    this.ClientApp.ShowError("Vui lòng chọn các đơn vị cần khai báo.");
                }
            }
        }

        public override bool CanReceive()
        {
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl != null)
            {
                return ctrl.Issued && ctrl.SelectedMember != null && ctrl.SelectedMember.CanReceiveCommand();
            }

            var selectedUnits = this.ClientApp.GetSelectedUnits();
            return selectedUnits.Any() && selectedUnits.All(u => u.CanReceiveCCPK());
        }

        public override void OnReceive()
        {
            this.ClientApp.AddLog("Đã nhận lệnh");
            var ctrl = this.Controllers.OfType<AlertConferenceController>().FirstOrDefault();
            if (ctrl != null)
            {
                ctrl.OnReceive();
            }
            else
            {
                var canReceiveUnits = this.ClientApp.GetSelectedUnits(u => u.CanReceiveCCPK());
                foreach (var u in canReceiveUnits)
                {
                    u.Task.Result = eTaskResult.NL;
                    this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, u.Task.LastDuration, this.TaskType);
                    ReleaseResource(u);
                }
            }
        }

        protected override bool ValidateUnitsBeforeConnecting(IEnumerable<UnitPhoneViewModel> units)
        {
            if (units.Any(u => u.IsTaskChanged))
            {
                this.ClientApp.ShowError("Vui lòng cập nhật cấp báo động trước.");
                return false;
            }

            // ktra cac don vi chua khai bao nv, cap bd
            var invalidUnits = units.Where(u => u.Task.Level == eTaskLevel.None || u.Task.Level == eTaskLevel.TB || 
                u.Task.Result == eTaskResult.NL || u.Task.Result == eTaskResult.CTT);

            if (invalidUnits.Any())
            {
                var sb = new StringBuilder();
                sb.Append("Vui lòng khai báo cấp báo động cho đơn vị: ");
                sb.Append(string.Join(", ", invalidUnits.Select(u => u.Name).ToArray()));
                this.ClientApp.ShowError(sb.ToString());
                return false;
            }

            return true;
        }
    }
}
