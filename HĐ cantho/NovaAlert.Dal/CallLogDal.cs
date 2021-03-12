using AutoMapper;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;

namespace NovaAlert.Dal
{
    public class CallLogDal
    {
        #region Singleton
        private static volatile CallLogDal instance;
        private static object syncRoot = new Object();

        private CallLogDal() 
        {
            //Mapper.CreateMap<eCallType, byte>().ReverseMap();
            Mapper.CreateMap<byte, eCallType>().ConvertUsing(x => System.Enum.GetValues(typeof(eCallType)).Cast<eCallType>().First(y => (byte)y == x));
            Mapper.CreateMap<eCallType, byte>().ConvertUsing(x => (byte)x);

            Mapper.CreateMap<CallLogDb, CallLog>().ReverseMap();
            Mapper.CreateMap<CallLogDb, SearchCallLogResult>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            Mapper.CreateMap<CallLogDetailDb, CallLogDetail>().ReverseMap();
        }

        public static CallLogDal Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CallLogDal();
                    }
                }

                return instance;
            }
        } 
        #endregion

        public void SaveCallLog(CallLog en)
        {
            lock(syncRoot)
            {
                using (var ctx = new NovaAlertContext())
                {
                    var cl = ctx.CallLogDbs.Where(c => c.CallLogId == en.CallLogId).FirstOrDefault();
                    if (cl == null)
                    {
                        cl = new CallLogDb();
                        ctx.CallLogDbs.Add(cl);
                    }

                    Mapper.Map<CallLog, CallLogDb>(en, cl);
                    ctx.SaveChanges();
                }
            }            
        }

        public void SaveCallLogDetail(CallLogDetail en)
        {
            lock(syncRoot)
            {
                using (var ctx = new NovaAlertContext())
                {
                    var cl = ctx.CallLogDetailDbs.Where(c => c.CallLogDetailId == en.CallLogDetailId).FirstOrDefault();
                    if (cl == null)
                    {
                        cl = new CallLogDetailDb();
                        ctx.CallLogDetailDbs.Add(cl);
                    }

                    Mapper.Map<CallLogDetail, CallLogDetailDb>(en, cl);
                    ctx.SaveChanges();
                }
            }            
        }

        public List<SearchCallLogResult> Search(SearchCallLogCriteria cr)
        {
            if (!cr.CanSearch()) return null;

            using (var ctx = new NovaAlertContext())
            {
                var query = BuildCallLogSearchQuery(cr, ctx);                                
                var list = new List<SearchCallLogResult>();

                List<CallLogDb> dbList;
                if (cr.NumOfRecords > 0) dbList = query.OrderByDescending(det => det.StartTime).Skip(cr.StartIndex).Take(cr.NumOfRecords).ToList();
                else dbList = query.ToList();

                foreach(var call in dbList)
                {
                    var result = new SearchCallLogResult();
                    Mapper.Map<CallLogDb, SearchCallLogResult>(call, result);                    
                    list.Add(result);
                }

                return list;
            }
        }

        public uint Count(SearchCallLogCriteria cr)
        {
            if (!cr.CanSearch()) return 0;

            using (var ctx = new NovaAlertContext())
            {
                var query = BuildCallLogSearchQuery(cr, ctx);

                return (uint)query.Count();
            }
        }

        private static IQueryable<CallLogDb> BuildCallLogSearchQuery(SearchCallLogCriteria cr, NovaAlertContext ctx)
        {
            var query = ctx.CallLogDbs.Include("Details").Where(c => c.DeletedDate == null);

            if (cr.CallType > 0)
                query = query.Where(c => c.CallType == cr.CallType);

            if (cr.ChannelId.HasValue)
                query = query.Where(c => c.Details.Any(det => det.ChannelId == cr.ChannelId.Value));

            if (!string.IsNullOrEmpty(cr.UnitName))
                query = query.Where(c => c.Details.Any(det => det.UnitName != null && det.UnitName.Contains(cr.UnitName)));

            if (cr.StartDate.HasValue)
                query = query.Where(c => SqlFunctions.DateDiff("day", c.StartTime, cr.StartDate.Value) <= 0);

            if (cr.EndDate.HasValue)
                query = query.Where(c => SqlFunctions.DateDiff("day", c.StartTime, cr.EndDate.Value) >= 0);
            return query;
        }

        public void DeleteCallLog(Guid id)
        {
            using (var ctx = new NovaAlertContext())
            {
                var cl = new CallLogDb() { CallLogId = id };
                ctx.CallLogDbs.Attach(cl);
                cl.DeletedDate = DateTime.Now;
                ctx.SaveChanges();
            }
        }
    }
}
