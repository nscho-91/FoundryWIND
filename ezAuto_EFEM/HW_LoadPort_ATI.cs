using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{

    class HW_LoadPort_ATI : HW_LoadPort_Mom , Control_Child
    {
        HW_LoadPortCover m_cover = new HW_LoadPortCover();
        HW_LoadPortCarrier m_carrier = new HW_LoadPortCarrier();
        HW_LoadPortLoader m_loader = new HW_LoadPortLoader();
        HW_LoadPortAlign m_align = new HW_LoadPortAlign();
        HW_LoadPortMap m_map = new HW_LoadPortMap();
        int m_diVacCheck = -1;
        int m_doAlram = -1; 

        public override void Init(int nID, Auto_Mom auto)
        {
            int nMode = 0, nType = 0;
            base.Init(nID, auto);
            m_control.Add(this);
            m_log.m_reg.Read("Mode", ref nMode);
            m_log.m_reg.Read("TypeSlot", ref nType); // ing 171106
            m_infoCarrier.m_eSlotType = (Info_Carrier.eSlotType)nType;  // ing 171106
            m_infoCarrier.Init(m_id + "InfoCarrier", m_log);
            m_cover.Init(m_id + "Cover", m_auto, m_log);
            m_carrier.Init(m_id + "Carrier", m_auto, m_infoCarrier, m_log);
            m_loader.Init(m_id + "Loader", m_auto, m_log);
            m_align.Init(m_id + "Align", m_auto, m_infoCarrier, m_log);
            m_map.Init(m_id + "Map", m_auto, m_infoCarrier, m_log);
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            m_map.ThreadStop();
        }

        void InitString()
        {
            m_map.InitString();
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            m_cover.ControlGrid(control, rGrid, eMode);
            m_carrier.ControlGrid(control, rGrid, eMode);
            m_loader.ControlGrid(control, rGrid, eMode);
            m_align.ControlGrid(control, rGrid, eMode);
            m_map.ControlGrid(control, rGrid, eMode);
            control.AddDI(rGrid, ref m_diVacCheck, m_id, "CheckVac", "DI Check Vacuum");
            control.AddDO(rGrid, ref m_doAlram, m_id, "Alram", "DO LED Alram"); 
        }

        protected override void RunTimer(bool bLED)
        {
            m_cover.RunLED(false, bLED && (m_eState == eState.Ready)); 
            m_cover.RunLED(true, bLED && (m_eState == eState.Ready) && !m_loader.IsLoad(true)); 
            m_carrier.RunLED(bLED);
            m_loader.RunLED(false, bLED && (m_eState == eState.LoadDone) && (m_infoCarrier.IsEnableUnload()) && (m_work.GetState() == eWorkRun.Ready));
            m_loader.RunLED(true, bLED && (m_eState == eState.Ready) && m_cover.IsOpen(false) && (m_infoCarrier.m_wafer.m_eSize < Wafer_Size.eSize.Empty) && (m_work.GetState() == eWorkRun.Run));
            if (m_cover.IsOpen(true)) m_infoCarrier.Clear(); 
        }

        public override void RunGrid(ezTools.eGrid eMode)
        {
            m_grid.Update(eMode);
            m_infoCarrier.RunGrid(m_grid, eMode); 
            m_cover.RunGrid(m_grid);
            m_carrier.RunGrid(m_grid);
            m_loader.RunGrid(m_grid);
            m_align.RunGrid(m_grid);
            m_map.RunGrid(m_grid, eMode);
            m_grid.Refresh();
        }

        protected override eHWResult IsCoverOpen(bool bOpen) 
        {
            if (!m_cover.IsLock(!bOpen)) return eHWResult.Error;
            if (!m_cover.IsOpen(bOpen)) return eHWResult.Error; 
            return eHWResult.OK; 
        }

        protected override eHWResult RunCover(bool bOpen) 
        {
            if (bOpen)
            {
                if (m_cover.CoverOpen()) return eHWResult.Error; 
            }
            else
            {
                if (m_cover.CoverClose()) return eHWResult.Error; 
            } 
            return eHWResult.OK; 
        }

        protected override eHWResult IsLoad(bool bLoad) 
        {
            if (m_loader.IsLoad(bLoad)) return eHWResult.OK;
            else return eHWResult.Error;
        }

        protected override eHWResult IsCarrierReady() 
        {
            if (m_carrier.IsReady()) return eHWResult.OK;
            else return eHWResult.Error; 
        }

        public override eHWResult RunLoad(bool bLoad) 
        {
            return m_loader.RunLoad(bLoad);
        }

        protected override eHWResult RunMapping() 
        {
            return m_map.DoMapping(); 
        }

        public override bool IsDoorOpenPos()
        {
            return m_map.IsDoorOpenPos();
        }

        public override Wafer_Size.eSize CheckCarrier()
        {
            return m_carrier.CheckCarrier();
        }

        protected override void ProcInit()
        {
            ProcCover(); 
        }

        protected override void ProcHome()
        {
            if (m_align.RunAlign(false) == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            if (m_loader.RunLoad(false) == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            if (m_map.RunHome() == eHWResult.Error)
            {
                SetState(eState.Error); 
            }
            else
            {
                SetState(eState.Ready); 
            }
        }

        protected override void ProcReady()
        {
            ProcCover(); 
            if (m_loader.CheckSW(true) == eHWResult.OK)
            {
                if (m_cover.IsOpen(true) || m_cover.IsLock(false))
                {
                    m_log.Popup("Please Cover Close !!");
                    return;
                }
                if (m_work.GetState() == eWorkRun.AutoUnload) return;
                if (m_work.GetState() == eWorkRun.Ready) return;
                if (m_work.GetState() == eWorkRun.Home) return;
                SetState(eState.RunLoad);
            }
        }

        void ProcCover()
        {
            if (m_cover.CheckSW(false) == eHWResult.OK)
            {
                m_cover.CoverClose();
            }
            if (m_cover.CheckSW(true) == eHWResult.OK)
            {
                if (m_loader.IsLoad(true))
                {
                    m_log.Popup("Carrier is Loaded !!");
                    return;
                }
                m_cover.CoverOpen();
            }
        }

        protected override void ProcLoad()
        {
            if (m_infoCarrier.m_wafer.m_eSize == Wafer_Size.eSize.Empty)
            {
                m_log.Popup("Carrier not Loaded !!");
                SetState(eState.Ready);
                return;
            }
            if (m_infoCarrier.m_wafer.m_eSize == Wafer_Size.eSize.Error)
            {
                m_log.Popup("Carrier Load Position Error !!");
                SetState(eState.Ready);
                return; 
            }
            if (m_loader.RunLoad(true) == eHWResult.Error)
            {
                SetState(eState.Error);
                m_align.RunAlign(false);
                m_loader.RunLoad(false);
                Thread.Sleep(1000);
                return; 
            }
            if (m_align.RunAlign(true) == eHWResult.Error)
            {
                SetState(eState.Error);
                m_align.RunAlign(false);
                Thread.Sleep(1000);
                return;
            }
            if (m_align.RunAlign(false) == eHWResult.Error)
            {
                SetState(eState.Error);
                m_loader.RunLoad(false);
                Thread.Sleep(2000);
                return;
            }
            if (m_map.DoMapping() == eHWResult.Error)
            {
                SetState(eState.Error);
                m_loader.RunLoad(false);
                return;
            }
            if (m_map.m_bMapOK)
            {
                if (m_map.DoDoorOpen() == eHWResult.Error)
                {
                    SetState(eState.Error);
                    m_loader.RunLoad(false);
                    return;
                }
                m_infoCarrier.m_eState = Info_Carrier.eState.MapDone; 
                SetState(eState.LoadDone);
            }
            else
            {
                m_log.Popup("Check Wafer state in Carrier !!"); 
                SetState(eState.RunUnload);
            }
        }

        protected override void ProcDone()
        {
         //   if (m_auto.m_strWork == EFEM.eWork.MSB.ToString()) m_infoCarrier.m_strLotID = m_xGem.m_XGem300Process[m_nLPNum].m_sJobID; // ing 170329
            if (m_loader.CheckSW(false) != eHWResult.OK) return;
            if ((m_infoCarrier.IsEnableUnload()) && (m_work.GetState() == eWorkRun.Ready))
            {
                SetState(eState.RunUnload);
            }
        }

        protected override void ProcUnload()
        {
            if (m_align.RunAlign(false) == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            if (m_map.DoDoorClose() == eHWResult.Error) 
            {
                SetState(eState.Error);
                return;
            }
            if (m_loader.RunLoad(false) == eHWResult.Error)
            {
                SetState(eState.Error);
                return; 
            }
            m_infoCarrier.m_eState = Info_Carrier.eState.Init;
            m_wtr.ClearIndexCarrier();
            if (m_infoCarrier.m_bRNR)
            {
                SetState(eState.RunLoad); 
            }
            else
            {
                SetState(eState.Ready); 
            }
        }

        public override bool IsDoorOpen()
        {
            return m_map.IsDoor(true);
        }

        public override bool IsDoorAxisMove()
        {
            return m_map.IsDoorAxisMove();
        }

        public override bool IsCoverClose()
        {
            return m_cover.IsOpen(false) || m_cover.IsLock(true);
        }
    }
}
