using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public partial class HW_ImageVS : HW_ImageVS_Mom
    {
        #region SocketMsg

        enum eData
        {
            WAFERID,
            DCNT,
            TOTALDIE,
            BEFDIE,
            BEFDCNT,
            AFTDIE,
            AFTDCNT,
            BADCHIPCLSFY,
            BADDCNTCLSFY,
            SLOTNO,
            None
        }

        #endregion

        string m_strWaferID;
        int m_nDefectCount;
        int m_nTotalDie;
        int m_nBeforDie;
        int m_nBeforDefectCount;
        int m_nAfterDie;
        int m_nAfterDefectCount;
        string m_strSlotNum;
        int[] m_aBadChipClassfy = new int[100];
        int[] m_aBadDefectCountClassFy = new int[100];

        enum eError
        {
            Connect,
            Timeout,
            Protocol
        }

        public HW_ImageVS()
        {
            InitializeComponent();
        }

        public override void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto);
            InitString();
        }

        public void InitString()
        {
            InitString(eError.Connect, "Not Connect !!");
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public override void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bUse, "ImageVS", "ImageVS Use", "ImageVS Use Check");
        }
        protected override void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode);
            m_socket.RunGrid(m_grid);
            m_grid.Refresh();
        }

        protected override void RunThread()
        {
            // ing
        }

        public override bool IsConnected()
        {
            if (IsImageVSEnable())
                if (m_socket.IsConnect()) return true;
                else
                {
                    SetAlarm(eAlarm.Warning, eError.Connect); // ing 161208
                    return false;
                }
            else
                return false;
        }

        eData GetData(string sData)
        {
            for (int n = 0; n < (int)eData.None; n++)
            {
                if (sData == ((eData)n).ToString())   //KJW 160718 eCMD-> eData
                    return (eData)n;
            }
            return eData.None;
        }

        bool GetData(string[] sDatas)
        {
            string[] s;
            eData data = GetData(sDatas[0]);
            switch (data)
            {
                case eData.WAFERID:
                    m_strWaferID = sDatas[1];
                    break;
                case eData.DCNT:
                    m_nDefectCount = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.TOTALDIE:
                    m_nTotalDie = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.BEFDIE:
                    m_nBeforDie = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.BEFDCNT:
                    m_nBeforDefectCount = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.AFTDIE:
                    m_nAfterDie = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.AFTDCNT:
                    m_nAfterDefectCount = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.BADCHIPCLSFY:
                    s = sDatas[1].Split(';');
                    for (int n = 0; n < s.Length; n ++)
                    {
                        m_aBadChipClassfy[n] = Convert.ToInt32(s[n]);
                    }
                    break;
                case eData.BADDCNTCLSFY:
                    s = sDatas[1].Split(';');
                    for (int n = 0; n < s.Length; n ++)
                    {
                        m_aBadDefectCountClassFy[n] = Convert.ToInt32(s[n]);
                    }
                    break;
                case eData.SLOTNO:
                    m_strSlotNum = sDatas[1];
                    break;
            }
            return false;
        }

        bool ReadMsg(string[] sMsgs, ref int nIndex)
        {
            while (nIndex < sMsgs.Length - 1)
            {
                GetData(sMsgs[nIndex].Split(':'));
                nIndex++;
            }
            return true;
        }

        protected override void CallMsgRcv(byte[] byteMsg, int nSize)
        {
            base.CallMsgRcv(byteMsg, nSize);
            string sMsg = Encoding.Default.GetString(byteMsg, 0, nSize);
            m_log.Add("<< " + sMsg);
            sMsg = sMsg.TrimStart('\0');
            string[] sMsgs = sMsg.Split(',');
            /*if (sMsgs.Length < 3)
            {
                m_log.Popup("Too Short Massage : " + sMsg);
                return;
            }*/

            int nIndex = 0;
            ReadMsg(sMsgs, ref nIndex);

            m_log.Add("WaferID : " + m_strWaferID + ", DCNT : " + m_nDefectCount.ToString() + ", TotalDefectCount : " + m_nDefectCount.ToString() + 
            ", TotalDie : " + m_nTotalDie + ", BeforDie : " + m_nBeforDie.ToString() + ", BeforDefectCount : " + m_nBeforDefectCount.ToString() + 
            ", AfterDie : " + m_nAfterDie.ToString() + ", AfterDefectCount : " + m_nAfterDefectCount.ToString() + 
            ", BadChipClassfy : " + m_aBadChipClassfy.ToString() + ", BadDefectCount : " + m_aBadDefectCountClassFy.ToString() +
            ", SlotNumber : " + m_strSlotNum);
            if (m_xGem != null && (m_xGem.IsOnlineRemote() || m_xGem.IsOnlineLocal()))
            {
                m_infoWafer = new Info_Wafer(-1, -1, -1, m_log);
                try
                {
                    m_infoWafer.m_strWaferID = m_strWaferID;
                    m_infoWafer.m_nSlot = Convert.ToInt32(m_strSlotNum);
                }
                catch
                {
                    m_log.Popup("Slot Number Is Wrong !!");
                }
                m_xGem.ReviewDone(m_infoWafer, m_nDefectCount);
            }
        }

        public override bool IsImageVSEnable()
        {
            return m_bUse;
        }
    }
}
