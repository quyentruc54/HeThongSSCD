using AutoMapper;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;

namespace NovaAlert.Dal
{
    public class DbLogger
    {
        #region Singleton
        private static volatile DbLogger instance;
        private static object syncRoot = new Object();

        private DbLogger() 
        {
            Mapper.CreateMap<DbLog, LogItem>().ReverseMap();
        }

        public static DbLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DbLogger();
                    }
                }

                return instance;
            }
        } 
        #endregion

        public void AddLog(byte panelId, int userId, string info)
        {
            using(var ctx = new NovaAlertContext())
            {
                var log = new DbLog()
                {
                    CreatedDate = DateTime.Now,
                    PanelId = panelId,
                    UserId = userId,
                    Info = info
                };
                ctx.DbLogs.Add(log);
                ctx.SaveChanges();
            }
        }

        public List<LogItem> SearchLog(DateTime start, DateTime end, byte? panelId = null, string searchText = null, int startIndex = 0, int numOfRecords = 0)
        {
            var list = new List<LogItem>();

            using (var ctx = new NovaAlertContext())
            {
                var query = BuildSearchQuery(start, end, panelId, searchText, ctx);

                List<DbLog> logList;
                if (numOfRecords == 0) logList = query.ToList();
                else logList = query.OrderByDescending(l => l.CreatedDate).Skip(startIndex).Take(numOfRecords).ToList();

                foreach(var item in logList)
                {
                    list.Add(Mapper.Map<DbLog, LogItem>(item));
                }
            }

            return list;
        }

        public uint CountLog(DateTime start, DateTime end, byte? panelId = null, string searchText = null)
        {
            using (var ctx = new NovaAlertContext())
            {
                var query = BuildSearchQuery(start, end, panelId, searchText, ctx);
                return (uint)query.Count();
            }
        }

        private static IQueryable<DbLog> BuildSearchQuery(DateTime start, DateTime end, byte? panelId, string searchText, NovaAlertContext ctx)
        {
            var query = ctx.DbLogs.Where(l => SqlFunctions.DateDiff("day", start, l.CreatedDate) >= 0 &&
                SqlFunctions.DateDiff("day", end, l.CreatedDate) <= 0);

            if (panelId.HasValue && panelId.Value > 0) query = query.Where(l => l.PanelId == panelId.Value);

            if (!string.IsNullOrEmpty(searchText)) query = query.Where(l => l.Info.Contains(searchText));
            return query;
        }
    }
}
