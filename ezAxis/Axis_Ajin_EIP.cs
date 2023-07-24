using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools;

namespace ezAxis
{
    public class Axis_Ajin_EIP : Axis_Mom
    {
        const uint c_nAlarmReset = 0x02;
        public int[] m_nPos = new int[3];
        public uint[] m_mHome = new uint[2];
        public uint[] m_mLimitM = new uint[2];
        public uint[] m_mLimitP = new uint[2];
        public uint[] m_mInPos = new uint[2];
        public uint[] m_mAlarm = new uint[2];
        public uint[] m_mEmergency = new uint[2];

        int m_msMove = 30000;
        int m_msHome = 60000;
        string m_id = "";
        public string m_strID = ""; 
        bool m_bEnable = false;
        bool m_bHome = false;
        bool m_bPlusLimit = false;
        bool m_bMinusLimit = false; 
        int m_nID = 0;
        int[] m_nV = new int[2]; 
        double m_fPosDst = 0;
        double m_fInitV = 1;
        double[] m_fA = new double[2];
        uint m_mPulse = 0;
        uint m_mEncoder = 0;
        int m_mHomeDir = 0;
        uint m_mHomeSensor = 4;
        uint m_mHomeZ = 0;
        double[] m_fHomeV = new double[2];
        double m_fHomeA = 1; 
        eAxis m_eStat = eAxis.eInit;
        ezStopWatch m_swHome = new ezStopWatch(); 
        ezStopWatch m_swWait = new ezStopWatch(); 
        Log m_log;
        bool m_bThreadCheck = false;
        Thread m_threadCheck;
        bool m_bThreadRepeat = false;
        bool m_bRepeat = false; 
        Thread m_threadRepeat;

        public Axis_Ajin_EIP(int nID, Log log)
        {
            m_nID = (int)nID; m_log = log; m_id = m_strID = m_nID.ToString() + ".Axis";
            m_nPos[0] = m_nPos[1] = m_nPos[2] = 0; m_nV[0] = 10000; m_nV[1] = 1000;
            m_fHomeV[0] = 100; m_fHomeV[1] = 1000;
            m_threadCheck = new Thread(new ThreadStart(ThreadCheck)); m_threadCheck.Start();
            m_threadRepeat = new Thread(new ThreadStart(ThreadRepeat)); m_threadRepeat.Start();
        }

        public void ThreadStop()
        {
            if (m_bThreadCheck) { m_bThreadCheck = false; m_threadCheck.Join(); }
            if (m_bThreadRepeat) { m_bThreadRepeat = false; m_threadRepeat.Join(); }
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode, ezJob job = null)
        {
            string strID = m_nID.ToString() + ".";
            rGrid.Set(ref m_strID, m_id, "ID", "Axis ID");
            rGrid.Set(ref m_bEnable, m_id, "Enable", "Axis Ready to Work");
            rGrid.Set(ref m_nPos[0], m_id + "_Move", "Pos0", "Position (Pulse)");
            rGrid.Set(ref m_nPos[1], m_id + "_Move", "Pos1", "Position (Pulse)");
            rGrid.Set(ref m_nPos[2], m_id + "_Move", "Pos2", "Position (Pulse)");
            rGrid.Set(ref m_nV[0], m_id + "_Move", "Speed", "Speed (Pulse/sec)");
            rGrid.Set(ref m_fA[0], m_id + "_Move", "Acc", "Acc (sec)");
            rGrid.Set(ref m_nV[1], m_id + "_Jog", "Speed", "Speed (Pulse/sec)");
            rGrid.Set(ref m_fA[1], m_id + "_Jog", "Acc", "Acc (sec)");
            rGrid.Set(ref m_fInitV, m_id + "_Mode", "InitV", "Init V (pulse/sec)");
            rGrid.Set(ref m_mPulse, m_id + "_Mode", "Pulse", "1HLH(0), 1HHL(1), 1LLH(2), 1LHL(3), 2CcwH(4), 2CcwL(5), 2CwH(6), 2CwL(7), 2P(8), 2PR(9)");
            rGrid.Set(ref m_mEncoder, m_id + "_Mode", "Encoder", "Nor.U/D(0), Nor.1X(1), Nor.2X(2), Nor.4X(3), Inv.U/D(4), Inv.1X(5), Inv.2X(6), Inv.4X(7)");
            rGrid.Set(ref m_mHome[0], m_id + "_Sensor", "Home", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mLimitM[0], m_id + "_Sensor", "Limit-", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mLimitP[0], m_id + "_Sensor", "Limit+", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mInPos[0], m_id + "_Sensor", "InPos", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mAlarm[0], m_id + "_Sensor", "Alarm", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mEmergency[0], m_id + "_Sensor", "EMG", "LOW(0), HIGH(1), UNUSED(2), USED(3)");
            rGrid.Set(ref m_mHomeDir, m_id + "_Home", "Dir", "CCW(0), CW(1)");
            rGrid.Set(ref m_mHomeSensor, m_id + "_Home", "Sensor", "Limit+(0), Limit-(1), No(2), No(3), Home(4)");
            rGrid.Set(ref m_mHomeZ, m_id + "_Home", "Z_Phase", "Disable(0), +(1), -(2)");
            rGrid.Set(ref m_fHomeV[1], m_id + "_Home", "HighV", "Home V (pulse/sec)");
            rGrid.Set(ref m_fHomeV[0], m_id + "_Home", "LowV", "Home V (pulse/sec)");
            rGrid.Set(ref m_fHomeA, m_id + "_Home", "Acc", "Home Acc (sec)");
            rGrid.Set(ref m_msMove, m_id + "_Timeout", "Move", "Axis Move Timeout (ms)");
            rGrid.Set(ref m_msHome, m_id + "_Timeout", "Home", "Home Search Timeout (ms)"); 
            if (eMode == eGrid.eUpdate)
            {
                SaveAxis(); 
            }
        }

        public bool IsAxisMove()
        {
            uint InMotion = 0; ;
            CANM.AnmStatusReadInMotion(m_nID, ref InMotion);

            if (InMotion == 1)
                return true;
            else
                return false;
        }

        public void ServoOn(bool bOn)
        {
            if (bOn) CANM.AnmSignalServoOn(m_nID, 1);
            else { CANM.AnmSignalServoOn(m_nID, 0); m_eStat = eAxis.eInit; }
        }

        public bool IsServoOn()
        {
            uint nOn = 0; 
            CANM.AnmSignalIsServoOn(m_nID, ref nOn); 
            return (nOn != 0); 
        }

        public void ResetAlarm()
        {
            uint nOutput = 0;
            if (m_mAlarm[1] == 0) return;
            CANM.AnmSignalReadOutput(m_nID, ref nOutput);
            nOutput |= c_nAlarmReset; CANM.AnmSignalWriteOutput(m_nID, nOutput); Thread.Sleep(50);
            nOutput -= c_nAlarmReset; CANM.AnmSignalWriteOutput(m_nID, nOutput); 
        }

        public bool Move(double fPos, double fSlow = 1.0)
        {
            double v, a;
            if (!m_bEnable) return true;
            if (WaitReady()) { m_log.Add(m_strID + " : Axis is Not Ready."); return true; }
            if (fSlow == 0) fSlow = 1; v = m_nV[0] * fSlow; a = m_fA[0] / fSlow;
            m_fPosDst = fPos; CANM.AnmMoveStartPos(m_nID, fPos, v, a, a); Thread.Sleep(5);
            m_eStat = eAxis.eMove; return false;
        }

        public bool Move(double fPos, double fV, double fAcc)
        {
            double v, a;
            if (!m_bEnable) return true;
            if (WaitReady()) { m_log.Add(m_strID + " : Axis is Not Ready."); return true; }
            a = fAcc; 
            if (fV > 0) v = fV; else v = m_nV[0];
            m_fPosDst = fPos; CANM.AnmMoveStartPos(m_nID, fPos, v, a, a); Thread.Sleep(5);
            m_eStat = eAxis.eMove; return false;
        }

        public bool MoveV(double fPos, double fV)
        {
            double v, a;
            if (!m_bEnable) return true;
            if (WaitReady()) { m_log.Add(m_strID + " : Axis is Not Ready."); return true; }
            a = m_fA[0]; if (fV > 0) v = fV; else v = m_nV[0];
            m_fPosDst = fPos; CANM.AnmMoveStartPos(m_nID, fPos, v, a, a); Thread.Sleep(5);
            m_eStat = eAxis.eMove; return false;
        }

        public bool MoveV(double fPos, double fPosV, double fV)
        {
            if (!m_bEnable) return true;
            if (WaitReady()) { m_log.Add(m_strID + " : Axis is Not Ready."); return true; }
            CANM.AnmMotSetProfileMode(m_nID, 3); CANM.AnmOverrideSetMaxVel(m_nID, m_nV[0]);
            m_fPosDst = fPos; CANM.AnmOverrideVelAtPos(m_nID, fPos, m_nV[0], m_fA[0], m_fA[0], fPosV, fV, 0);
            m_eStat = eAxis.eMove; return false;
        }

        public void Jog(double fScale)
        {
            CANM.AnmMoveVel(m_nID, m_nV[1] * fScale, m_fA[1], m_fA[1]);
        }

        public void StopAxis(bool bEmg = false)
        {
            if (bEmg) CANM.AnmMoveEStop(m_nID); 
            else CANM.AnmMoveSStop(m_nID); 
        }

        public bool HomeSearch()
        {
            if (!m_bEnable) { m_eStat = eAxis.eReady; return true; }
            if (m_eStat > eAxis.eReady) { m_log.Add(m_strID + " : Axis is Not Ready"); return true; }
            ResetAlarm(); ServoOn(true); CANM.AnmHomeSetStart(m_nID);
            m_eStat = eAxis.eHome; m_swHome.Start(); return false;
        }

        public bool IsReady()
        {
            return m_eStat == eAxis.eReady; 
        }

        public void SetTrigger(double fPos0, double fPos1, double dPos, bool bCmd, double dTrigTime = 2)
        {
            uint nEncoder;
            if (bCmd) nEncoder = 0; else nEncoder = 1;
            CANM.AnmTriggerSetReset(m_nID);
            CANM.AnmTriggerSetTimeLevel(m_nID, dTrigTime, 1, nEncoder, 0);
            CANM.AnmTriggerSetBlock(m_nID, fPos0, fPos1, dPos);
        }

        public void StopTrigger()
        {
            CANM.AnmTriggerSetReset(m_nID);
        }

        public double GetPos(bool bCmd)
        {
            double posNow = 0;
            if (bCmd) CANM.AnmStatusGetCmdPos(m_nID, ref posNow);
            else CANM.AnmStatusGetActPos(m_nID, ref posNow);
            return posNow;
        }

        public void SetPos(bool bCmd, double fPos)
        {
            if (bCmd) CANM.AnmStatusSetCmdPos(m_nID, fPos);
            else CANM.AnmStatusSetActPos(m_nID, fPos);
        }

        public void SetCaption(string strID)
        {
            m_strID = m_nID.ToString() + "." +strID; 
        }

        public void OverrideVel(double fV)
        {
            if (fV < 0) fV = m_nV[0];
            CANM.AnmOverrideVel(m_nID, fV);
        }

        public bool WaitReady(double dp = -1)
        {
            if (m_eStat == eAxis.eInit) return true;
            m_swWait.Start();
            while ((m_eStat != eAxis.eReady) && (m_swWait.Check() <= m_msMove)) 
                Thread.Sleep(1);
            if (m_swWait.Check() > m_msMove)
            { 
                m_log.Add(m_strID + " : Axis move TimeOut !!"); 
                return true; 
            }
            if ((dp > 0) && (Math.Abs(GetPos(false) - m_fPosDst) > dp))
            { m_log.Add(m_strID + " : Axis move Pos Error !!"); return true; }
            return false;
        }

        public bool IsHome()
        {
            return m_bHome; 
        }

        public bool IsPlusLimit() 
        {
            return m_bPlusLimit;
        }

        public bool IsMinusLimit() 
        {
            return m_bMinusLimit;
        }

        void ThreadCheck()
        {
	        uint nStat = 0, nLimitP, nLimitM; 
	        m_bThreadCheck = true; Thread.Sleep(500);
            CANM.AnmSignalWriteOutput(m_nID, 0); 
            CANM.AnmMotSetAccelUnit(m_nID, 1);
            while (m_bThreadCheck)
	        {
		        Thread.Sleep(1);
		        CANM.AnmHomeReadSignal(m_nID, ref m_mHome[1]); m_bHome = (m_mHome[1] != 0);
		        nLimitP = m_mLimitP[1]; nLimitM = m_mLimitM[1];
                CANM.AnmSignalReadLimit(m_nID, ref m_mLimitP[1], ref m_mLimitM[1]);
                m_bPlusLimit = (m_mLimitP[1] != 0); m_bMinusLimit = (m_mLimitM[1] != 0); 
                m_log.Notify(m_strID + ": Servo limit(+) !!", (m_mLimitP[0] < 2) && (nLimitP == 0) && (m_mLimitP[1] != 0), false);
                m_log.Notify(m_strID + ": Servo limit(-) !!", (m_mLimitM[0] < 2) && (nLimitM == 0) && (m_mLimitM[1] != 0), false);
                CANM.AnmSignalReadInpos(m_nID, ref m_mInPos[1]);
                CANM.AnmSignalReadServoAlarm(m_nID, ref m_mAlarm[1]);
                CANM.AnmSignalReadStop(m_nID, ref m_mEmergency[1]); if (m_mEmergency[0] == 2) m_mEmergency[1] = 0; 
		        if (m_eStat == eAxis.eHome)
		        {
			        if (m_swHome.Check() > m_msHome) { m_eStat = eAxis.eInit; m_log.Add(m_strID + " : Find Home TimeOver"); }
			        else CANM.AnmHomeGetResult(m_nID, ref nStat);
			        if (nStat == 1)
			        {
				        m_eStat = eAxis.eReady; 
				        m_log.Add(m_strID + " : Find Home Finished" + m_swHome.Check().ToString() + " ms");
			        }
		        }
		        else if (m_eStat == eAxis.eMove)
		        {
                    CANM.AnmStatusReadInMotion(m_nID, ref nStat); if (nStat == 0) m_eStat = eAxis.eReady;
                    m_log.Notify(m_strID + " : Servo alarm !!", (m_eStat != eAxis.eInit) && ((m_mAlarm[1] != 0) || (m_mEmergency[1] != 0)), true);
                    if ((m_eStat != eAxis.eInit) && ((m_mAlarm[1] != 0) || (m_mEmergency[1] != 0))) 
                    { 
                        m_eStat = eAxis.eInit;  
                        Thread.Sleep(100); 
                    }
		        }
	        }
        } 

        public void StartRepeat()
        {
            m_bRepeat = !m_bRepeat; 
        }

        void ThreadRepeat()
        {
            int n = 1;
            m_bThreadRepeat = true; Thread.Sleep(500); 
            while (m_bThreadRepeat)
            {
                Thread.Sleep(1); 
                if (m_bRepeat)
                {
                    Thread.Sleep(100); Move(m_nPos[n]);
                    n = 3 - n; while (!IsReady()) Thread.Sleep(1);
                }
            }
        }

        public void SaveAxis()
        {
            CANM.AnmMotSetMoveUnitPerPulse(m_nID, 1, 1);
            CANM.AnmMotSetMinVel(m_nID, m_fInitV);
            CANM.AnmMotSetPulseOutMethod(m_nID, m_mPulse);
            CANM.AnmMotSetEncInputMethod(m_nID, m_mEncoder);
            CANM.AnmSignalSetLimit(m_nID, 0, m_mLimitP[0], m_mLimitM[0]);
            CANM.AnmSignalSetInpos(m_nID, m_mInPos[0]);
            CANM.AnmSignalSetServoAlarm(m_nID, m_mAlarm[0]);
            CANM.AnmSignalSetStop(m_nID, 0, m_mEmergency[0]);
            CANM.AnmMotSetAbsRelMode(m_nID, 0);
            CANM.AnmMotSetProfileMode(m_nID, 3);
            CANM.AnmMotSetAccelUnit(m_nID, 1);
            CANM.AnmHomeSetSignalLevel(m_nID, m_mHome[0]);
            CANM.AnmHomeSetMethod(m_nID, m_mHomeDir, m_mHomeSensor, m_mHomeZ, 1000, 0);
            CANM.AnmHomeSetVel(m_nID, m_fHomeV[1], m_fHomeV[1], m_fHomeV[0], m_fHomeV[0], m_fHomeA, m_fHomeA);
            CANM.AnmMotSetMaxVel(m_nID, 4000000.0);
        }

        public void LoadAxis()
        {
            uint dw = 0; double f0 = 0, f1 = 0;
            CANM.AnmMotGetMinVel(m_nID, ref m_fInitV);
            CANM.AnmMotGetPulseOutMethod(m_nID, ref m_mPulse);
            CANM.AnmMotGetEncInputMethod(m_nID, ref m_mEncoder);
            CANM.AnmSignalGetLimit(m_nID, ref dw, ref m_mLimitP[0], ref m_mLimitM[0]);
            CANM.AnmSignalGetInpos(m_nID, ref m_mInPos[0]);
            CANM.AnmSignalGetServoAlarm(m_nID, ref m_mAlarm[0]);
            CANM.AnmSignalGetStop(m_nID, ref dw, ref m_mEmergency[0]);
            CANM.AnmHomeGetSignalLevel(m_nID, ref m_mHome[0]);
            CANM.AnmHomeGetMethod(m_nID, ref m_mHomeDir, ref m_mHomeSensor, ref m_mHomeZ, ref f0, ref f1);
        }

        public string GetID()
        {
            return m_strID; 
        }

        public double GetPosDst()
        {
            return m_fPosDst; 
        }
    }
}
