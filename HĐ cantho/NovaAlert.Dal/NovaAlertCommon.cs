using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovaAlert.Entities;
using AutoMapper;

namespace NovaAlert.Dal
{
    public class NovaAlertCommon
    {
        #region Singleton
        private static NovaAlertCommon _instance;
        public static NovaAlertCommon Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NovaAlertCommon();
                }
                return _instance;
            }
        }
        #endregion

        #region Event
        public event EventHandler<EntityChangedEventArgs> OnGroupChanged;
        public event EventHandler<EntityChangedEventArgs> OnContactChanged;
        #endregion

        #region Ctor
        NovaAlertContext _ctx;
        private NovaAlertCommon()
        {
            _ctx = new NovaAlertContext();

            Mapper.CreateMap<DayTypeConfigDb, DayTypeConfig>().ReverseMap();
            Mapper.CreateMap<PODb, PO>().ReverseMap();
            Mapper.CreateMap<AlarmDb, Alarm>().ReverseMap();
            //Mapper.CreateMap<CallLogDb, CallLog>().ReverseMap();
            Mapper.CreateMap<EnumDb, NovaAlert.Entities.Enum>().ReverseMap();
            Mapper.CreateMap<PanelDb, Panel>().ReverseMap();

            Mapper.CreateMap<PhoneNumber, Contact>().ReverseMap();//.ForMember(c => c.PhoneNumberId, m => m.MapFrom(pn => pn.PhoneNumberId));
            Mapper.CreateMap<DisplayData, DisplayDataDb>().ReverseMap();
            Mapper.CreateMap<RadioTimeDb, RadioTime>().ReverseMap();
        } 
        #endregion

        #region Enums

        public List<NovaAlert.Entities.Enum> GetEnums(string type)
        {
            var list = new List<NovaAlert.Entities.Enum>();
            foreach(var item in _ctx.EnumDbs.Where(e => e.Type == type).ToList())
            {
                list.Add(Mapper.Map<EnumDb, NovaAlert.Entities.Enum>(item));
            }

            return list;
        }

        public NovaAlert.Entities.Enum GetEnum(string type, byte value)
        {
            var en = _ctx.EnumDbs.Where(e => e.Type == type && e.Value == value).FirstOrDefault();
            if (en != null) return Mapper.Map<EnumDb, NovaAlert.Entities.Enum>(en);
            else return null;
        }

        #endregion

        public Panel GetPanelInfo(int panelId)
        {
            var pd = _ctx.PanelDbs.Where(p => p.PanelId == panelId).FirstOrDefault();
            return Mapper.Map<PanelDb, Panel>(pd);
        }

        public List<DayTypeConfig> GetDayTypes()
        {
            var list = new List<DayTypeConfig>();
            foreach(var item in _ctx.DayTypeConfigDbs.ToList())
                list.Add(Mapper.Map<DayTypeConfigDb, DayTypeConfig>(item));
            return list;
        }

        public DayTypeConfig GetDayType(byte dayOfWeek)
        {
            return Mapper.Map<DayTypeConfigDb, DayTypeConfig>(_ctx.DayTypeConfigDbs.Where(dt => dt.DayOfWeek == dayOfWeek).FirstOrDefault());
        }

        public void SaveDayTypes(List<DayTypeConfig> list)
        {
            foreach (var item in list)
            {
                var obj = _ctx.DayTypeConfigDbs.Where(a => a.DayOfWeek == item.DayOfWeek).FirstOrDefault();
                //Mapper.Map<DayTypeConfig, DayTypeConfigDb>(item, obj);
                obj.DayType = item.DayType;
            }
            _ctx.SaveChanges();
        }

        #region Alarm
        List<Alarm> _alarmList;
        public List<Alarm> AlarmList
        {
            get
            {
                if (_alarmList == null)
                {
                    LoadAlarm();
                }
                return _alarmList;
            }
        }

        private void LoadAlarm()
        {
            _alarmList = new List<Alarm>();
            var alarmTypes = _ctx.EnumDbs.Where(e => e.Type == "AlarmType").ToList();
            foreach (var item in _ctx.AlarmDbs.ToList())
            {
                var alarm = Mapper.Map<AlarmDb, Alarm>(item);
                var t = alarmTypes.Where(tt => tt.Value == alarm.AlarmType).FirstOrDefault();
                if (t != null) alarm.Name = t.Desc_VN;
                _alarmList.Add(alarm);
            }
        }

        public void SaveAlarms(List<Alarm> list)
        {
            foreach (var item in list)
            {
                AlarmDb obj = _ctx.AlarmDbs.Where(a => a.AlarmId == item.AlarmId).FirstOrDefault();
                Mapper.Map<Alarm, AlarmDb>(item, obj);
            }
            _ctx.SaveChanges();
            _alarmList = null;
        } 
        #endregion

        #region RadioTime
        List<RadioTime> _radioTimeList;
        public List<RadioTime> RadioTimeList
        {
            get 
            {
                if (_radioTimeList == null) _radioTimeList = LoadRadioTimes();
                return _radioTimeList; 
            }
        }
        private List<RadioTime> LoadRadioTimes()
        {
            var list = new List<RadioTime>();
            foreach (var item in _ctx.RadioTimeDbs.ToList())
            {
                var rt = Mapper.Map<RadioTimeDb, RadioTime>(item);
                list.Add(rt);
            }

            return list;
        } 
        public void SaveRadioTimes(List<RadioTime> list)
        {
            foreach (var item in list)
            {
                RadioTimeDb obj = _ctx.RadioTimeDbs.Where(a => a.DayType == item.DayType && a.ListOrder == item.ListOrder).FirstOrDefault();
                Mapper.Map<RadioTime, RadioTimeDb>(item, obj);
            }
            _ctx.SaveChanges();
            _radioTimeList = null;
        }
        #endregion

        public List<string> GetUnitNames()
        {
            return _ctx.PhoneNumbers.OfType<PhoneNumber>().Where(p=>p.UnitName != null && p.UnitName != "").Select(p => p.UnitName).ToList();
        }

        public List<string> GetUnitNameAbbrs()
        {
            return _ctx.PhoneNumbers.OfType<PhoneNumber>()
                .Where(p => p.NameAbbr != null && p.NameAbbr != "")
                .Select(p => p.NameAbbr).ToList();
        }

        //List<PhoneNumber> _phoneNumbers;
        //public List<PhoneNumber> GetPhoneNumbers()
        //{
        //    if (_phoneNumbers == null) _phoneNumbers = _ctx.PhoneNumbers.AsNoTracking().OfType<PhoneNumber>().ToList();
        //    return _phoneNumbers;
        //}
        //public void SaveChanges()
        //{
        //    _ctx.SaveChanges();
        //}

        public static List<HostPhone> LoadPhones()
        {
            var list = new List<HostPhone>();
            
            using (var ctx = new NovaAlertContext())
            {
                foreach (var c in ctx.ChannelDbs.Include("HostPhoneNumber").Where(c => c.HostPhoneNumber != null && c.HostPhoneNumber.Address != null).ToList())
                {
                    var phone = new HostPhone(c.PhoneNumberId, string.Format("{0}", c.PhoneNumberId))
                    {
                        AreaCode = c.HostPhoneNumber.AreaCode,
                        Number = c.HostPhoneNumber.Number,
                        Address = c.HostPhoneNumber.Address,
                        IsRestricted = c.HostPhoneNumber.IsRestricted,
                        AutoRecording = c.AutoRecording,
                        MultiDestEnabled = c.MultiDestEnabled,
                        CCPKEnabled = c.CCPKEnabled,
                        AlertEnabled = c.AlertEnabled,
                        //TSLEnabled = c.TSLEnabled,
                        HotUnitId = c.HotUnitId
                    };
                    list.Add(phone);
                }
            }
            return list;
        }

        //public static List<UnitPhone> LoadUnitPhones(int? groupId = null)
        //{
        //    var list = new List<UnitPhone>();
        //    using (var ctx = new NovaAlertContext())
        //    {
        //        var query = ctx.GroupUnit.AsQueryable();
        //        if (groupId.HasValue) query = query.Where(g => g.GroupId == groupId.Value);
        //        //int key = 1;

        //        foreach (var item in query.ToList())
        //        {
        //            var up = new UnitPhone(item.PhoneNumberId, item.PhoneNumber.NameAbbr)
        //            {
        //                AreaCode = item.PhoneNumber.AreaCode,
        //                Number = item.PhoneNumber.Number,
        //                ListOrder = item.ListOrder,
        //                GroupId = item.GroupId,
        //                PhoneNumberId = item.PhoneNumberId,
        //                Password = item.PhoneNumber.Password
        //            };

        //            GetLastestTask(up);
        //            list.Add(up);
        //        }
        //    }
        //    return list;
        //}

        //public static List<UnitPhone> LoadUnitPhonesByGroup(int groupId)
        //{
        //    var list = new List<UnitPhone>();
        //    using (var ctx = new NovaAlertContext())
        //    {
        //        var query = ctx.GroupUnit.Where(g => g.GroupId == groupId);
                
        //        foreach (var item in query.ToList())
        //        {
        //            var up = new UnitPhone(item.PhoneNumberId, item.PhoneNumber.NameAbbr)
        //            {
        //                AreaCode = item.PhoneNumber.AreaCode,
        //                Number = item.PhoneNumber.Number,
        //                ListOrder = item.ListOrder,
        //                GroupId = item.GroupId,
        //                PhoneNumberId = item.PhoneNumberId,
        //                Password = item.PhoneNumber.Password
        //            };

        //            GetLastestTask(up);
        //            list.Add(up);
        //        }
        //    }
        //    return list;
        //}

        public static List<GroupUnit> GetUnitsInGroup(int id)
        {
            var list = new List<GroupUnit>();
            using (var ctx = new NovaAlertContext())
            {
                foreach(var item in ctx.GroupUnitDbs.Where(g => g.GroupId == id && g.IsDeleted == false).ToList())
                    list.Add(new GroupUnit()
                    {
                        GroupId = id, UnitId = item.PhoneNumberId, ListOrder = item.ListOrder
                    });
            }

            return list;
        }

        public static List<UnitPhone> LoadUnitPhones(int? id = null)
        {
            var list = new List<UnitPhone>();
            using (var ctx = new NovaAlertContext())
            {
                var query = ctx.PhoneNumbers.OfType<PhoneNumber>().Where(u => u.DeletedDate == null);
                if (id.HasValue) query = query.Where(u => u.PhoneNumberId == id.Value);

                foreach(var item in query.ToList())
                {
                    var up = new UnitPhone(item.PhoneNumberId, item.NameAbbr)
                    {
                        AreaCode = item.AreaCode,
                        Number = item.Number,
                        ListOrder = -1,                        
                        PhoneNumberId = item.PhoneNumberId,
                        Password = item.Password,
                        TSLAreaCode = item.TSLAreaCode,
                        TSLNumber = item.TSLNumber
                    };

                    list.Add(up);
                }
            }

            return list;
        }

        public static List<GroupDb> GetAllGroups()
        {
            using (var ctx = new NovaAlertContext())
            {
                return ctx.GroupDbs.Where(g => g.DeletedDate == null).ToList();
            }
        }

        public static List<PO> GetPOes()
        {
            using (var ctx = new NovaAlertContext())
            {
                var list = new List<PO>();
                foreach (var item in ctx.PODbs.ToList())
                    list.Add(new PO() { Id = item.Id, Address = item.Address });
                return list;
            }
        }

        #region GroupUnitTask
        //public static void GetLastestTask(UnitPhone unit, eTaskType taskType = eTaskType.CTT)
        //{
        //    using (var ctx = new NovaAlertContext())
        //    {
        //        var dt = ctx.GroupUnitTasks.Where(t => t.PhoneNumberId == unit.PhoneNumberId && t.TaskType == (byte)taskType)
        //            .OrderByDescending(t => t.Id).FirstOrDefault();
        //        if (dt != null)
        //        {
        //            unit.Task = new Task()
        //            {
        //                CurrentTask = (eTask)dt.Task,
        //                Level = (eTaskLevel)dt.Level,
        //                Result = (eTaskResult)dt.Result,
        //                CreatedDate = dt.CreatedDate
        //            };
        //        }
        //    }
        //}

        public static Task GetLastestTask(int phoneNumberId, eTaskType taskType)
        {
            using (var ctx = new NovaAlertContext())
            {
                var task = new Task();

                var dt = ctx.GroupUnitTasks.Where(t => t.PhoneNumberId == phoneNumberId && t.TaskType == (byte)taskType)
                    .OrderByDescending(t => t.Id).FirstOrDefault();
                if (dt != null)
                {
                    task.CurrentTask = (eTask)dt.Task;
                    task.Level = (eTaskLevel)dt.Level;
                    task.Result = (eTaskResult)dt.Result;
                    task.CreatedDate = dt.CreatedDate;
                }

                return task;
            }
        }

        public static void UpdateTask(UnitPhone unit, long? duration, eTaskType taskType)
        {
            Entities.Task task = NovaAlert.Dal.NovaAlertCommon.GetLastestTask(unit.Id, Entities.eTaskType.CTT);
            using (var ctx = new NovaAlertContext())
            {
                var dt = new GroupUnitTask()
                {
                    GroupId = 0, //unit.GroupId,
                    TaskType = (byte)taskType,
                    PhoneNumberId = unit.PhoneNumberId,
                    Task = (byte)unit.Task.CurrentTask,
                    Level = (byte)unit.Task.Level,
                    Result = (byte)unit.Task.Result,
                    CreatedDate = DateTime.Now,
                    Duration = duration
                };

                ctx.GroupUnitTasks.Add(dt);
                ctx.SaveChanges();
            }
        }        
        #endregion

        public List<Contact> GetAllContacts()
        {
            using (var ctx = new NovaAlertContext())
            {
                var list = new List<Contact>();
                foreach(var item in ctx.PhoneNumbers.OfType<PhoneNumber>().Where(c => c.DeletedDate == null).ToList())
                {
                    list.Add(Mapper.Map<PhoneNumber, Contact>(item));
                }
                return list;
            }
        }
        public void SaveContact(Contact ct)
        {
            using (var ctx = new NovaAlertContext())
            {
                PhoneNumber ph = null;
                if(ct.PhoneNumberId == 0)
                {
                    ph = new PhoneNumber();
                    ctx.PhoneNumbers.Add(ph);
                }
                else
                {                    
                    ph = ctx.PhoneNumbers.OfType<PhoneNumber>().Where(p => p.PhoneNumberId == ct.PhoneNumberId).FirstOrDefault();
                    if (ph == null)
                    {
                        throw new InvalidOperationException("Invalid phone number id.");
                    }
                }
                
                Mapper.Map<Contact,PhoneNumber>(ct,ph);
                ctx.SaveChanges();

                if (OnContactChanged != null)
                {
                    OnContactChanged(this, new EntityChangedEventArgs(ph.PhoneNumberId));
                }
            }
        }

        public void DeleteContact(int id)
        {
            using (var ctx = new NovaAlertContext())
            {
                var ct = ctx.PhoneNumbers.OfType<PhoneNumber>().Where(c => c.PhoneNumberId == id).FirstOrDefault();
                if (ct == null)
                {
                    return;
                }

                ct.DeletedDate = DateTime.Now;
                var groupUnits = ctx.GroupUnitDbs.Where(g => g.PhoneNumberId == id);
                foreach(var item in groupUnits)
                {
                    item.IsDeleted = true;
                }

                ctx.SaveChanges();
                if (OnContactChanged != null)
                {
                    OnContactChanged(this, new EntityChangedEventArgs(ct.PhoneNumberId));
                }
            }
        }

        public List<ContactGroup> GetAllContactGroups()
        {
            using (var ctx = new NovaAlertContext())
            {
                var list = new List<ContactGroup>();
                var groups = ctx.GroupDbs.Where(g => g.DeletedDate == null).ToList();
                foreach(var g in groups)
                {
                    var cg = new ContactGroup() { Id = g.GroupId, Name = g.Name, Contacts = new List<ContactInGroup>() };
                    foreach(var item in g.GroupUnits.Where(u=>u.IsDeleted == false).ToList())
                    {
                        cg.Contacts.Add(new ContactInGroup()
                        {
                            ListOrder = item.ListOrder,
                            Contact = Mapper.Map<PhoneNumber, Contact>(item.PhoneNumber)
                        });
                    }

                    list.Add(cg);
                }

                return list;
            }
        }
        public void SaveGroup(ContactGroup cg)
        {
            using (var ctx = new NovaAlertContext())
            {
                var g = ctx.GroupDbs.Where(gr =>gr.GroupId==cg.Id).FirstOrDefault();
                if (g == null)
                {
                    g = new GroupDb();
                    ctx.GroupDbs.Add(g);
                }
                g.Name = cg.Name;
                if(cg.IsDeleted)
                {
                    g.DeletedDate = DateTime.Now;
                }
                else
                {
                    foreach (var item in cg.Contacts)
                    {
                        var gu = g.GroupUnits.Where(u => u.PhoneNumberId == item.Contact.PhoneNumberId).FirstOrDefault();
                        if (gu == null)
                        {
                            gu = new GroupUnitDb();
                            g.GroupUnits.Add(gu);
                        }
                        gu.PhoneNumberId = item.Contact.PhoneNumberId;
                        gu.ListOrder = item.ListOrder;
                        gu.IsDeleted = item.IsDeleted;
                    }
                }
                
                ctx.SaveChanges();
                //if(cg.Id == 0) cg.Id = g.GroupId;
                if (OnGroupChanged != null) OnGroupChanged(this, new EntityChangedEventArgs(cg.Id));
            }
        }
        public List<Channel> GetAllChannels()
        {
            using (var ctx = new NovaAlertContext())
            {
                var list = new List<Channel>();
                var channels = ctx.ChannelDbs.ToList();
                foreach (var c in channels)
                {
                    var cc = new Channel()
                    {
                        ChannelId = c.ChannelId,
                        Number = c.HostPhoneNumber.Number,
                        AreaCode = c.HostPhoneNumber.AreaCode,
                        IsRestricted = c.HostPhoneNumber.IsRestricted,
                        AlertEnabled = c.AlertEnabled,
                        MultiDestEnabled = c.MultiDestEnabled,
                        CCPKEnabled = c.CCPKEnabled,
                        AutoRecording = c.AutoRecording,
                        //TypeCde = c.HostPhoneNumber.TypeCde     
                        //TSLEnabled = c.TSLEnabled,
                        HotUnitId = c.HotUnitId
                    };

                    list.Add(cc);
                }

                return list;
            }
        }

        public void SaveChannels(List<Channel> list)
        {
            using (var ctx = new NovaAlertContext())
            {
                foreach (var c in list)
                {
                    //ctx.ChannelDbs.ToList();
                    var obj = ctx.ChannelDbs.Where(ch => ch.ChannelId == c.ChannelId).FirstOrDefault();

                    obj.HostPhoneNumber.Number = c.Number;
                    obj.HostPhoneNumber.AreaCode = c.AreaCode;
                    obj.HostPhoneNumber.IsRestricted = c.IsRestricted;
                    obj.AlertEnabled = c.AlertEnabled;
                    obj.MultiDestEnabled = c.MultiDestEnabled;
                    obj.CCPKEnabled = c.CCPKEnabled;
                    obj.AutoRecording = c.AutoRecording;
                    obj.HotUnitId = c.HotUnitId;
                    //obj.TSLEnabled = c.TSLEnabled;
                    //obj.HostPhoneNumber.TypeCde = c.TypeCde;
                }
                ctx.SaveChanges();
            }
        }

        public static List<ResultData> GetResults(eTaskType taskType, int phoneNumberId = 0)
        {
            var results = new List<ResultData>();
            using(var ctx = new NovaAlertContext())
            {
                //var list = ctx.ViewResults.Where(r => r.TaskType == (byte)taskType && r.PhoneNumberId != null).OrderBy(r => r.Id).ToList();

                var list = new List<ViewResult>();
                if(phoneNumberId == 0)
                {
                    list = ctx.ViewResults.Where(r => r.TaskType == (byte)taskType && r.PhoneNumberId != null).OrderBy(r => r.Id).ToList();
                }
                else
                {
                    list = ctx.ViewResults.Where(r => r.TaskType == (byte)taskType && r.PhoneNumberId == phoneNumberId).OrderBy(r => r.Id).ToList();
                }
                
                foreach(var item in list)
                {
                    var re = new ResultData()
                    {
                        DisplayId = item.Id,
                        PhoneNumberId = item.PhoneNumberId.Value,
                        UnitName = item.UnitName,
                        Task = (eTask)(item.Task ?? 0),
                        Level = (eTaskLevel)(item.Level ?? 0),
                        Result = (eTaskResult)(item.Result ?? 0),
                        TaskType = (eTaskType)item.TaskType
                    };

                    if (re.Result == eTaskResult.NL)
                    {
                        if (item.Duration.HasValue)
                        {
                            re.TimeReceive = item.CreatedDate;
                            re.IntervalReceive = TimeSpan.FromSeconds(item.Duration.Value);
                        }
                    }
                    else if (re.Result == eTaskResult.CTT)
                    {
                        if (item.Duration.HasValue)
                        {
                            re.TimeChange = item.CreatedDate;
                            re.IntervalChange = TimeSpan.FromSeconds(item.Duration.Value);
                        }

                        var temp = ctx.GroupUnitTasks.Where(r => r.Result == (byte)eTaskResult.NL && r.PhoneNumberId == item.PhoneNumberId
                            && r.TaskType == (byte)eTaskType.CTT).OrderByDescending(r => r.Id).FirstOrDefault();
                        if (temp != null)
                        {
                            if (temp.Duration.HasValue)
                            {
                                re.TimeReceive = temp.CreatedDate;
                                re.IntervalReceive = TimeSpan.FromSeconds(temp.Duration.Value);
                            }
                        }
                    } 

                    results.Add(re);
                }
            }

            return results;
        }

        public static List<DisplayData> GetDisplayData()
        {
            var list = new List<DisplayData>();
            using(var ctx = new NovaAlertContext())
            {
                foreach(var item in ctx.DisplayDataDbs.ToList())
                {
                    list.Add(Mapper.Map<DisplayDataDb, DisplayData>(item));
                }
            }

            return list;
        }

        public static void SaveDisplayData(List<DisplayData> list)
        {
            using (var ctx = new NovaAlertContext())
            {
                foreach(var item in list)
                {
                    var dbItem = ctx.DisplayDataDbs.Where(d => d.DisplayId == item.DisplayId).FirstOrDefault();
                    if (dbItem != null)
                    {
                        if(dbItem.PhoneNumber_1 != item.PhoneNumber_1) dbItem.PhoneNumber_1 = item.PhoneNumber_1;

                        if (dbItem.PhoneNumber_2 != item.PhoneNumber_2) dbItem.PhoneNumber_2 = item.PhoneNumber_2;
                    }                        
                }
                ctx.SaveChanges();
            }
        }


        public static void SaveSubResult(int phoneNumberId, ResultData re)
        {
            using (var ctx = new NovaAlertContext())
            {
                var sr = new SubResult()
                {
                    DisplayId = re.DisplayId,
                    CreatedDate = DateTime.Now,
                    ParentId = phoneNumberId,
                    PhoneNumberId = re.PhoneNumberId,
                    UnitName = re.UnitName,
                    TaskType = (byte)re.TaskType,
                    Task = (byte)re.Task,
                    Level = (byte)re.Level,
                    Result = (byte)re.Result,
                    TimeReceive = re.TimeReceive,
                    TimeChange = re.TimeChange
                };

                if (re.IntervalReceive.HasValue)
                {
                    sr.IntervalReceive = (long)re.IntervalReceive.Value.TotalSeconds;
                }

                if (re.IntervalChange.HasValue)
                {
                    sr.IntervalChange = (long)re.IntervalChange.Value.TotalSeconds;
                }

                ctx.SubResults.Add(sr);
                ctx.SaveChanges();
            }
        }

        public static List<ResultData> GetSubResults(int phoneNumberId, eTaskType type)
        {
            using (var ctx = new NovaAlertContext())
            {
                var sql = string.Format("EXEC GetSubResult {0}, {1}", phoneNumberId, (byte)type);
                var subResults = ctx.Database.SqlQuery<SubResult>(sql).ToList();

                var list = new List<ResultData>();
                foreach(var item in subResults)
                {
                    var r = new ResultData()
                    {
                        DisplayId = item.DisplayId,
                        PhoneNumberId = item.PhoneNumberId,
                        UnitName = item.UnitName,
                        TaskType = (eTaskType)item.TaskType,
                        Task = (eTask)item.Task,
                        Level = (eTaskLevel)item.Level,
                        Result = (eTaskResult)item.Result,
                        TimeReceive = item.TimeReceive,
                        TimeChange = item.TimeChange
                    };

                    if (item.IntervalReceive.HasValue) r.IntervalReceive = TimeSpan.FromSeconds(item.IntervalReceive.Value);
                    if (item.IntervalChange.HasValue) r.IntervalChange = TimeSpan.FromSeconds(item.IntervalChange.Value);

                    list.Add(r);
                }

                return list;
            }
        }
    }
}

