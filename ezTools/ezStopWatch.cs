using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics; 

namespace ezTools
{
    public class ezStopWatch
    {
        Stopwatch m_sw = new Stopwatch(); 

        public ezStopWatch()
        {
            m_sw.Start(); 
        }

        public void Start()
        {
            m_sw.Restart();
        }

        public void Stop()
        {
            m_sw.Stop();
        }

        public long Getms(bool bStop)
        {
            if (bStop) Stop();
            return m_sw.ElapsedMilliseconds;
        }

        public long Check()
        {
            return Getms(false); 
        }

        public string Get(bool bStop)
        {
            if (bStop) Stop();
            return m_sw.Elapsed.ToString(); 
        }

        public void Reset()
        {
            m_sw.Reset();
        }
    }
}
