using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Test.NovaAlert.SwitchIC
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var arr = new byte[] { 30, 31, 32 };
            var sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
                sb.AppendFormat("{0}", (char)arr[i]);

            string s = "";
            Assert.AreEqual(sb.ToString(), s);

        }
    }
}
