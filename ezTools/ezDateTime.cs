using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ezTools
{

    public class ezDateTime 
    {
        [DllImport("kernel32")]
        public static extern int SetSystemTime(ref SYSTEMTIME lpSystemTime);

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
        public SYSTEMTIME m_sTime = new SYSTEMTIME(); 

        public DateTime m_dt;

        public ezDateTime()
        {
            Check();
        }

        public void Check()
        {
            m_dt = DateTime.Now;
            DateTimeToSystemTime(); 
        }

        void DateTimeToSystemTime()
        {
            m_sTime.wYear = (ushort)m_dt.Year;
            m_sTime.wMonth = (ushort)m_dt.Month;
            m_sTime.wDayOfWeek = (ushort)m_dt.DayOfWeek;
            m_sTime.wDay = (ushort)m_dt.Day;
            m_sTime.wHour = (ushort)m_dt.Hour;
            m_sTime.wMinute = (ushort)m_dt.Minute;
            m_sTime.wSecond = (ushort)m_dt.Second;
            m_sTime.wMilliseconds = (ushort)m_dt.Millisecond; 
        }

        public int SetSystemTime()
        {
            return SetSystemTime(ref m_sTime);
        }

        public string GetDate()
        {
            return m_dt.Year.ToString("00.") + m_dt.Month.ToString("00.") + m_dt.Day.ToString("00.");
        }

        public string GetTime(bool bms = false)
        {
            if (bms) return m_dt.Hour.ToString("00-") + m_dt.Minute.ToString("00-") + m_dt.Second.ToString("00") + "." + m_dt.Millisecond.ToString("000");
            return m_dt.Hour.ToString("00-") + m_dt.Minute.ToString("00-") + m_dt.Second.ToString("00");
        }

        public string GetDateTime()
        {
            return GetDate() + ". " + GetTime(); 
        }

        public string GetPeriod(ezDateTime dt)
        {
            long nElapse = Math.Abs(m_dt.Ticks - dt.m_dt.Ticks); 
            TimeSpan ts = new TimeSpan(nElapse);
            return ts.Hours.ToString("00-") + ts.Minutes.ToString("00-") + ts.Seconds.ToString("00"); 
        }

        public int GetPeriodSec(ezDateTime dt)
        {
            long nElapse = Math.Abs(m_dt.Ticks - dt.m_dt.Ticks);
            TimeSpan ts = new TimeSpan(nElapse);
            return (int)ts.TotalSeconds; 
        }

        public int GetPeriodms(ezDateTime dt)
        {
            long nElapse = Math.Abs(m_dt.Ticks - dt.m_dt.Ticks);
            TimeSpan ts = new TimeSpan(nElapse);
            return (int)(1000 * ts.TotalSeconds + ts.Milliseconds);
        }

    }
}
