using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public enum eLogic
    {
        Set,
        AND,
        OR,
        Match,
        Match0,
        Error
    }; 

    public class Info_Strip
    {
        const double c_fPi = 3.1415926535897932384626433832795;

        public int m_nID; 
        public int m_nStrip = -1;
        public int m_nStripResult = -1;
        int m_nUnit, m_lUnit;
        public eResult[] m_aUnitResult;
        public string m_sError;
        public int m_index2DID = -1;
        public int m_nBad = -1;
        public bool[] m_bInspect = new bool[2] { false, false };
        public bool m_bImageVS = false;
        CPoint m_cpOffset, m_cpDelta;
        double m_fAngle, m_fSin, m_fCos;
        public int m_nMarkAngle = 0;
        public string m_sXMap;
        public string m_sOCR;
        public string m_sEMap;
        public string m_s2DMatch;
        public string m_sPos = "Init";
        public string m_sState = "Ready";
        public CPoint m_cpTray = new CPoint(0, 0);
        public string[] m_sResult = new string[2];
        public string[] m_sResultUnit = new string[2]; 
        public int m_nBundle = 0;
        public int m_nDefect = 0;   //박상영20180103
/*
        CPoint m_cpOffset, m_cpDelta;
        public string m_sError, m_str2DID, m_sXMap, m_sOCR;
        public string[] m_strCode = new string[c_nResultCode];
        public bool m_bRun;
        public bool[] m_bInspect = new bool[2];
        public int m_nID, m_index2DID, m_nStrip, m_nStripResult, ;

        public CPoint[] m_cpEdgy = new CPoint[4]; 
*/
        public Info_Strip()
        {

        }

        public void ReAllocate(int nUnit)
        {
            m_nUnit = nUnit;
            if (nUnit > m_lUnit) 
            {
                m_aUnitResult = new eResult[nUnit]; 
                m_lUnit = nUnit; 
            }
        }

        public int CalcResult()
        {
            int n;
            for (n = 0; n < m_nUnit; n++) if (m_aUnitResult[n] >= eResult.eError) 
            { 
                m_nStripResult = (int)m_aUnitResult[n]; 
                return m_nStripResult; 
            }
            m_nStripResult = 0; 
            for (n = 0; n < m_nUnit; n++) if (m_aUnitResult[n] > 0) m_nStripResult++;
            return m_nStripResult;
        }

        public eResult CalcLogic(int nUnit, eLogic eLogic, eResult eUnitResult) 
        {
            if ((eLogic == eLogic.Set) || (eUnitResult >= eResult.eError)) 
            { 
                m_aUnitResult[nUnit] = eUnitResult; 
                return m_aUnitResult[nUnit]; 
            }
            if (m_aUnitResult[nUnit] >= eResult.eError) return m_aUnitResult[nUnit];
            switch (eLogic)
            {
                case eLogic.AND:
                    if (eUnitResult == eResult.eInit) m_aUnitResult[nUnit] = eResult.eInit; 
                    break;
                case eLogic.OR: 
                    if (eUnitResult == eResult.eInspect) m_aUnitResult[nUnit] = eUnitResult; 
                    break;
                case eLogic.Match: 
                    if (m_aUnitResult[nUnit] != eUnitResult) eUnitResult = m_aUnitResult[nUnit] = eResult.eVerify; 
                    break;
                case eLogic.Error:
                    if (eUnitResult > eResult.eInit) eUnitResult = m_aUnitResult[nUnit] = eResult.eError; 
                    break;
                case eLogic.Match0:
                    if ((m_aUnitResult[nUnit] == eResult.eInit) && (m_aUnitResult[nUnit] != eUnitResult)) 
                        eUnitResult = m_aUnitResult[nUnit] = eResult.eVerify; 
                    else 
                        return eUnitResult;
                break;
            }
            return m_aUnitResult[nUnit];
        }

        public eResult GetResult(int nUnit)
        {
            if (nUnit >= m_nUnit) return 0;
            return m_aUnitResult[nUnit];
        }

        public void SetResult(int nUnit, eResult eUnitResult)
        {
            m_aUnitResult[nUnit] = eUnitResult;
        }

        public void SetResult(int nStrip, string sError)
        {
            m_nStripResult = nStrip; m_sError = sError;
        }

        public void ClearResult(int lUnit)
        {
            int n;
            m_nStripResult = m_index2DID = m_nBad = -1; 
            m_bInspect[0] = m_bInspect[1] = false;
            ReAllocate(lUnit); SetStripPos(new CPoint(0, 0), new CPoint(0, 0), 0);
            for (n = 0; n < m_nUnit; n++) m_aUnitResult[n] = 0;
        }

        public void SetStripPos(CPoint cpOffset, CPoint cpDelta, double fAngle)
        {
            m_cpOffset = cpOffset; 
            m_cpDelta = cpDelta; 
            m_fAngle = fAngle; 
            m_fSin = Math.Sin(fAngle); 
            m_fCos = Math.Cos(fAngle);
        }

        public CPoint GetOffset(CPoint cp)
        {
            int x, y; CPoint pos;
            pos = cp; 
            pos -= m_cpOffset; 
            x = pos.x; y = pos.y;
            pos = new CPoint((int)(x * m_fCos - y * m_fSin), (int)(x * m_fSin + y * m_fCos)) + m_cpOffset + m_cpDelta;
            return pos - cp;
        }

        public double GetStripAngle()
        {
            return 180 * m_nMarkAngle * m_fAngle / c_fPi;
        }


    }
}
