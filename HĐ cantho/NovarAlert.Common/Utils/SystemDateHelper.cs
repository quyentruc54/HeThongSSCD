using System;
using System.Runtime.InteropServices;

namespace NovaAlert.Common.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public class SystemDateHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME Time);

        public static void SetSystemTime(DateTime date)
        {
            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = (ushort)date.Year; // must be short
            st.wMonth = (ushort)date.Month;
            st.wDay = (ushort)date.Day;
            st.wHour = (ushort)(date.Hour%24);
            st.wMinute = (ushort)date.Minute;
            st.wSecond = (ushort)date.Second;
            st.wMilliseconds = (ushort)date.Millisecond;
            
            Privileges.EnablePrivilege(SecurityEntity.SE_SYSTEMTIME_NAME);
            SetLocalTime(ref st); // invoke this method.   
        }
    }
}
