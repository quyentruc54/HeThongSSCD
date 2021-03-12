using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NovaAlert.SwitchIC;
using NovaAlert.SwitchIC.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Test.NovaAlert.SwitchIC
{
    [TestClass]
    public class TestTSL
    {
        //[TestMethod]
        //public void TestParseTSLResult()
        //{
        //    var msg = new TSL_ResultMessage();
        //    msg.Task = 1;
        //    msg.Level = 2;
        //    msg.Result = 1;
        //    msg.Name = "Đơn vị";

        //    var s = JsonConvert.SerializeObject(msg);

        //    //var pattern = @"({.*?})";
        //    //var match = Regex.Match(s, pattern);
        //    //if(match != null)
        //    //{
        //    //    var obj = JsonConvert.DeserializeObject(match.Value);
        //    //}
        //}
    }
}
