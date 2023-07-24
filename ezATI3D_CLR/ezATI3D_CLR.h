#pragma once

#include "..\ezATI3D_CPP\ATI3D.h"

using namespace System;

namespace ezATI3D_CLR 
{
	public ref class ATI_3D
	{
	protected:

		ATI3D *m_pATI3D = nullptr; 

	public:

		ATI_3D()
		{
			m_pATI3D = new ATI3D(); 
		}

		virtual ~ATI_3D()
		{
			if (m_pATI3D == nullptr) return; 
			delete m_pATI3D; 
			m_pATI3D = nullptr; 
		}

		long Init(long nThread)
		{
			return m_pATI3D->Init(nThread); 
		}

		void ThreadStop()
		{
			m_pATI3D->Close(); 
		}

		void GrabDone(long nGrab, byte *pRaw)
		{
			m_pATI3D->m_dat.GrabDone(nGrab, pRaw); 
		}

		bool IsDone()
		{
			return m_pATI3D->IsDone(); 
		}

		void SetBuffer(long eRaw, long xRaw, long yRaw, long xBuf, long yBuf)
		{
			m_pATI3D->m_dat.SetBuffer((e3D_Raw)eRaw, CPoint(xRaw, yRaw), CPoint(xBuf, yBuf));
		}

		void SetBuffer(long yp, WORD *pBufH, BYTE *pBufB)
		{
			m_pATI3D->m_dat.SetBuffer(yp, pBufH, pBufB); 
		}

		byte* GetBufCalc(long nIndex)
		{
			return m_pATI3D->m_aCalc[nIndex].m_aBufCalc; 
		}

		bool StartCalc_Ave(bool bInvDir, long lGrab, long msCalc, long xStart, long yStart, long nGVMin)
		{
			return m_pATI3D->StartCalc_Ave(bInvDir, lGrab, msCalc, CPoint(xStart, yStart), nGVMin);
		}

		bool StartCalc_PeakDown(bool bInvDir, long lGrab, long msCalc, long xStart, long yStart, long nGVMin1, long nGVMin2)
		{
			return m_pATI3D->StartCalc_PeakDown(bInvDir, lGrab, msCalc, CPoint(xStart, yStart), nGVMin1, nGVMin2); 
		}
	};
}
