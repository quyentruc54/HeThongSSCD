using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.Dal
{
    public class TslRepository
    {
        #region Singleton
        private static TslRepository _instance;
        public static TslRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TslRepository();
                }
                return _instance;
            }
        }
        #endregion

        public TslStatus GetLatestStatus(int phoneNumberId, eTslStatusType type)
        {
            using(var ctx = new NovaAlertContext())
            {
                var st = ctx.TslStatusDbs.Where(t => t.PhoneNumberId == phoneNumberId && t.Type == (byte)type)
                    .OrderByDescending(t => t.Id).FirstOrDefault();

                if(st != null)
                {
                    return new TslStatus()
                    {
                        Id = st.Id,
                        Type = (eTslStatusType)st.Type,
                        Status = (eTslStatus)st.Status,
                        CreatedDate = st.CreatedDate,
                        DeletedDate = st.DeletedDate
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public void UpdateStatus(int phoneNumberId, eTslStatusType type, eTslStatus status)
        {
            using(var ctx = new NovaAlertContext())
            {
                var st = new TslStatusDb()
                {
                    CreatedDate = DateTime.Now,
                    PhoneNumberId = phoneNumberId,
                    Type = (byte)type,
                    Status = (byte)status
                };
                ctx.TslStatusDbs.Add(st);
                ctx.SaveChanges();
            }
        }

        //public void DeleteStatus(int phoneNumberId, eTslStatusType type)
        //{
        //    using(var ctx = new NovaAlertContext())
        //    {
        //        var st = ctx.TslStatusDbs.Where(t => t.PhoneNumberId == phoneNumberId && t.Type == (byte)type)
        //            .OrderByDescending(t => t.Id).FirstOrDefault();

        //        if(st != null)
        //        {
        //            st.DeletedDate = DateTime.Now;
        //            ctx.SaveChanges();
        //        }                
        //    }
        //}
    }
}
