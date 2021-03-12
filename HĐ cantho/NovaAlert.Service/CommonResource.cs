using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using NovaAlert.Common;
using NovaAlert.Dal;
using NovaAlert.Entities;
using NovaAlert.Sound;
using NovaAlert.Entities.ViewModel;
using System.Threading;
using NovaAlert.Common.Setting;
using NovaAlert.Service.TSL;

namespace NovaAlert.Service
{
	public class CommonResource
	{
		#region Singleton
		static CommonResource _instance;
		public static CommonResource Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CommonResource();
				}
				return _instance;
			}
		}
		#endregion

		#region Members & Properties        
		public List<POViewModel> POes { get; private set; }
		public ObservableCollection<Client> Clients { get; private set; }
		public ObservableCollection<HostPhoneViewModel> Channels { get; private set; }
		public ObservableCollection<UnitPhoneViewModel> UnitPhones { get; private set; }
		public ObservableCollection<SwitchConnection> Connetions { get; private set; }
		public List<SoundChannel> SoundChannels { get; private set; }
		public ClientActionQueue Actions { get; private set; }
		public event EventHandler<ResourceChangedEventArgs> OnResourceChanged;
		public TSL_Modem Modem { get; private set; }
		#endregion

		#region Ctor
		private CommonResource()
		{            
			this.Clients = new ObservableCollection<Client>();
			this.Actions = new ClientActionQueue();

			LoadPhones();

			// Load UnitPhone from database
			this.UnitPhones = new ObservableCollection<UnitPhoneViewModel>();
			this.UnitPhones.CollectionChanged += Resource_CollectionChanged;
			foreach (var item in NovaAlertCommon.LoadUnitPhones())
				this.UnitPhones.Add(new UnitPhoneViewModel(item));

			// Load PO list
			this.POes = new List<POViewModel>();
			foreach (var po in NovaAlertCommon.GetPOes())
				this.POes.Add(new POViewModel(po));

			// Connection
			this.Connetions = new ObservableCollection<SwitchConnection>();
			this.SoundChannels = new List<SoundChannel>();
			this.SoundConfigVM = new SoundCardConfigViewModel(this.CreateSoundChannels);

			NovaAlertCommon.Instance.OnContactChanged += OnContactChanged; 
		}

			   
		#endregion        

		#region Event Handlers
		void res_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var res = sender as IResource;
			var ev = new ResourceChangedEventArgs(res.ResourceType, res.Id, res.SelectedPanelId, null);
			if (e.PropertyName == "SelectedPanelId")
			{
				NotifyClient(ev);
			}
		}

		public void NotifyClient(ResourceChangedEventArgs ev)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
			{
				if (this.OnResourceChanged != null) OnResourceChanged(this, ev);
			}));
		}

		void Resource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var res in e.NewItems.OfType<INotifyPropertyChanged>())
					{
						res.PropertyChanged += res_PropertyChanged;
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var res in e.OldItems.OfType<INotifyPropertyChanged>())
					{
						res.PropertyChanged -= res_PropertyChanged;
					}
					break;
			}
		}

		void OnContactChanged(object sender, EntityChangedEventArgs e)
		{
			var list = this.UnitPhones.Where(u => u.Id == e.Id).ToList();
			foreach (var up in list)
			{
				this.UnitPhones.Remove(up);
			}

			foreach(var item in NovaAlertCommon.LoadUnitPhones(e.Id))
			{
				this.UnitPhones.Add(new UnitPhoneViewModel(item));
			}
		}
		#endregion

		#region Helpers
		public List<IResource> GetResource(eResourceType type, int id)
		{
			var list = new List<IResource>();
			if (type == eResourceType.Channel)
			{
				list.AddRange(this.Channels.Where(c => c.Id == id));
			}
			else if (type == eResourceType.UnitPhone)
			{
				list.AddRange(this.UnitPhones.Where(p => p.Id == id));
				if(list.Count == 0)
				{
					var units = NovaAlertCommon.LoadUnitPhones(id);
					if (units.Count > 0)
					{
						foreach (var p in units)
						{
							var vm = new UnitPhoneViewModel(p);                            
							this.UnitPhones.Add(vm);
							list.Add(vm);
						}
					}
				}
			}
			else if(type == eResourceType.Modem)
			{
				list.Add(this.Modem);
			}

			return list;            
		}

		public void InitSoundChannels(IntPtr owner)
		{
			try
			{
				this.OwnerHandle = owner;
				CreateSoundChannels();
			}
			catch(Exception ex)
			{
				LogService.Logger.Error(ex);
			}
		}

		private void CreateSoundChannels()
		{
			foreach (var ch in this.SoundChannels)
			{
				ch.Stop();
				ch.Dispose();
			}
			this.SoundChannels.Clear();

			var count = SoundChannel.GetDeviceCount();

			LogService.Logger.Error(string.Format("Detect sound card: {0}", count));

			byte address = 0x19;
			var settings = GlobalSetting.Instance;

			// SoundCard 1
			if (settings.SoundCard1 >= 0)
			{
				this.SoundChannels.Add(new SoundChannel(this.OwnerHandle, settings.SoundCard1, eSpeakerPan.Left, address++));
				this.SoundChannels.Add(new SoundChannel(this.OwnerHandle, settings.SoundCard1, eSpeakerPan.Right, address++));
			}

			// SoundCard 2
			if (settings.SoundCard2 >= 0)
			{
				this.SoundChannels.Add(new SoundChannel(this.OwnerHandle, settings.SoundCard2, eSpeakerPan.Left, address++));
				this.SoundChannels.Add(new SoundChannel(this.OwnerHandle, settings.SoundCard2, eSpeakerPan.Right, address));
			}
		}

		public SoundCardConfigViewModel SoundConfigVM { get; private set; }
		public IntPtr OwnerHandle { get; private set; }

		public IEnumerable<Client> GetActiveClients()
		{
			return this.Clients.Where(c => c.IsDisconnected == false);
		}

		// Load Channels from database
		public void LoadPhones()
		{
			bool isReload = false;
			if (this.Channels != null)
			{
				this.Channels.Clear();
				isReload = true;
			}
			else
			{
				this.Channels = new ObservableCollection<HostPhoneViewModel>();
				this.Channels.CollectionChanged += Resource_CollectionChanged;
			}

			foreach (var item in NovaAlertCommon.LoadPhones())
			{
				this.Channels.Add(new HostPhoneViewModel(item));
			}

			if(isReload)
			{
				SendReloadEvent();
			}
		}  

		void SendReloadEvent()
		{
			foreach (var c in CommonResource.Instance.Clients)
			{
				try
				{
					c.Callback.Reload();
				}
				catch
				{
                    // No need to process here
				}
			}
		}

		public void InitDialupModem()
		{
			var portName = GlobalSetting.Instance.ModemPortName;

			if (!string.IsNullOrEmpty(portName))
			{
				this.Modem = TSL_Modem.CreateModem(portName);
				if (this.Modem != null)
				{
					this.Modem.AreaCode = GlobalSetting.Instance.ModemAreaCode;
					this.Modem.Number = GlobalSetting.Instance.ModemNumber;
					this.Modem.OnAcceptHandler += Modem_OnAcceptHandler;
					this.Modem.PropertyChanged += res_PropertyChanged;
					LogService.Logger.Debug(string.Format("Init modem on {0} success", GlobalSetting.Instance.ModemPortName));
				}
				else
				{
					LogService.Logger.Debug(string.Format("Fail to init modem on {0}", GlobalSetting.Instance.ModemPortName));
				}
			}
		} 
		#endregion    

		#region TSL
		void Modem_OnAcceptHandler(object sender, EventArgs e)
		{
			ThreadStart ts = new ThreadStart(
				() =>
				{
					System.Threading.Thread.Sleep(10000);
					var client = new ClientModem(this.Modem);
					client.WaitAndReplyCommandFromServer(SendAndGetPrepareResult);
				});

			var thread = new Thread(ts);
			thread.SetApartmentState(ApartmentState.MTA);
			thread.IsBackground = true;
			thread.Start();
		}
		
		public bool SendAndGetPrepareResult()
		{
			lock (this)
			{
				foreach (var c in this.Clients)
				{
					try
					{
						c.Callback.OnRequestPrepare();
					}
					catch
					{
					}
				}

				return Monitor.Wait(this, 15000);
			}
		}
		#endregion
	}
}
