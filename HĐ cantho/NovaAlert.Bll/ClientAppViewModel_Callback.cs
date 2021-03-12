using System;
using System.Linq;
using System.ServiceModel;
using NovaAlert.Common;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.Windows.Threading;
using System.Windows;

namespace NovaAlert.Bll
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class ClientAppViewModel : INovaAlertServiceCallback
    {
        DispatcherPriority _priority = DispatcherPriority.Normal;
        void INovaAlertServiceCallback.OnResourceChanged(ResourceChangedEventArgs e)
        {
            if (this.MainController != null)
            {
                Application.Current.Dispatcher.Invoke(_priority, new Action(() => this.MainController.OnResourceChanged(e)));
            }
        }

        void INovaAlertServiceCallback.OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (this.MainController != null)
            {
                Application.Current.Dispatcher.Invoke(_priority, new Action(() => this.MainController.OnPOStatusChanged(e)));
            }
        }

        void INovaAlertServiceCallback.OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(_priority, new Action<ChannelStatusChangedEventArgs>(OnChannelStatusChangedHandler), e);
        }

        void OnChannelStatusChangedHandler(ChannelStatusChangedEventArgs e)
        {
            if (this.MainController != null)
            {
                this.MainController.OnChannelStatusChanged(e);
            }

            if (this.Channels != null)
            {
                var ringChannels = this.Channels.Items.Where(c => c.LineStatus == eLineStatus.Ring && IsChannelUsable(c));
                this.IsRinging = ringChannels.Any();                
            }
        }

        void INovaAlertServiceCallback.OnDialCompleted(ChannelEventArgs e)
        {
            if (this.MainController != null)
            {
                Application.Current.Dispatcher.Invoke(_priority, new Action(() => this.MainController.OnDialCompleted(e)));
            }
        }

        void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {
            if (e.Item.SelectedPanelId == this.ClientId)
            {
                AddLog(string.Format("Bỏ chọn kênh {0}", e.Item.Id));
            }
            else
            {
                AddLog(string.Format("Chọn kênh {0}", e.Item.Id));
            }

            if (this.MainController != null)
            {
                this.MainController.ChannelClicked(sender, e);
            }
        }

        void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            if (e.Item.SelectedPanelId == this.ClientId)
            {
                AddLog(string.Format("Bỏ chọn đơn vị {0}", e.Item.Name));
            }
            else
            {
                AddLog(string.Format("Chọn đơn vị {0}", e.Item.Name));
            }

            if (this.MainController != null)
            {
                this.MainController.UnitClicked(sender, e);
            }
        }

        void INovaAlertServiceCallback.OnSystemDateTimeChanged(SystemDateTimeChangedEventArgs e)
        {
            Action<SystemDateTimeChangedEventArgs> st = new Action<SystemDateTimeChangedEventArgs>(SetSystemTime);
            st.BeginInvoke(e, null, null);
        }

        private void SetSystemTime(SystemDateTimeChangedEventArgs e)
        {
            //try
            //{

            //    //System.Threading.Thread.Sleep(100);
            //    //NovaAlert.Common.Utils.SystemDateHelper.SetSystemTime(e.DateTime);
            //    //this.CurDateTime = DateTime.Now;
            //    //this.CurDateTime = Service.GetDateTime();
            //}
            //catch (Exception ex)
            //{
            //    LogService.Logger.Error(ex);
            //}
        }

        void INovaAlertServiceCallback.OnServerMessage(eServerMessageType messageType, string msg)
        {
            switch (messageType)
            {
                case eServerMessageType.Notify:
                    ShowInfo(msg);
                    break;

                case eServerMessageType.Error:
                    ShowError(msg);
                    break;

                case eServerMessageType.RecorderError:
                    if(this.Control is MultiDestControlViewModel)
                    {
                        ((MultiDestControlViewModel)this.Control).CanRecord = false;
                    }
                    else
                    {
                        ShowError(msg);
                    }
                    break;

                case eServerMessageType.RecorderResume:
                    if (this.Control is MultiDestControlViewModel)
                    {
                        ((MultiDestControlViewModel)this.Control).CanRecord = true;
                    }
                    else
                    {
                        ShowInfo(msg);
                    }
                    break;
                default:
                    break;
            }
        }

        void INovaAlertServiceCallback.Reload()
        {
            if(!this.Menu.IsInConfigMode)
            {
                this.Menu.IsInConfigMode = false;
                OnWorkingModeChanged();
            }            
        }

        void INovaAlertServiceCallback.OnTaskChanged(TaskChangedEventArgs e)
        {
            if (this.MainController != null)
            {
                this.MainController.OnTaskChanged(e);
            }
        }

        void INovaAlertServiceCallback.OnRequestPrepare()
        {
            if (this.TSLController != null)
            {
                this.TSLController.OnRequestPrepare();
            }
        }

        void INovaAlertServiceCallback.OnTslStatusChanged(int phoneNumberId, eTslStatusType type, eTslStatus result, int? hostClientId)
        {
            if (this.TSLController != null)
            {
                this.TSLController.OnTslStatusChanged(phoneNumberId, type, result, hostClientId);
            }
        }
    }
}
