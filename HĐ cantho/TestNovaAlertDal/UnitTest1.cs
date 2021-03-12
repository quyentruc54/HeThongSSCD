using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NovaAlert.Dal;

namespace TestNovaAlertDal
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using(var ctx = new NovaAlertContext())
            {
                
                var list = ctx.ViewResults.Where(r => r.PhoneNumberId != null).OrderBy(r => r.Id).ToList();
            }            
        }

        [TestMethod]
        public void TestGetSubResult()
        {
            var list = NovaAlertCommon.GetSubResults(1, NovaAlert.Entities.eTaskType.CTT);
        }
    }
}
