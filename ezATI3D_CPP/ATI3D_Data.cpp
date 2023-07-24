#include "stdafx.h"
#include "ATI3D_Data.h"
#include <memory.h>

ATI3D_Data::ATI3D_Data()
{
	m_lGrab = 0;
}

ATI3D_Data::~ATI3D_Data()
{
}

void ATI3D_Data::Init(long lThread)
{
	m_eStat = e3D_Stat_Ready;
	m_lThread = lThread; 
}

void ATI3D_Data::Close()
{
	if (m_bAllocBuffer)
	{
		m_bAllocBuffer = false;
		delete[] m_pBufH;
		delete[] m_pBufB;
	}
	if (m_pRaw == nullptr) return;
	delete[] m_pRaw; 
}

void ATI3D_Data::ReAllocate(long lGrab)
{
	if (lGrab == m_lGrab) return; 
	if (m_lGrab != 0) delete[] m_pRaw;
	m_lGrab = lGrab; 
	m_pRaw = new BYTE*[lGrab + 1];
}

bool ATI3D_Data::IsBusy()
{
	if (IsCalcTimeout())
	{
		m_eStat = e3D_Stat_Ready; 
		return false; 
	}
	return (m_eStat != e3D_Stat_Ready) && (m_eStat != e3D_Stat_Done);
}

void ATI3D_Data::SetMode(e3D_Raw eRaw)
{
	m_eRaw = eRaw; 
	switch (m_eRaw)
	{
	case e3D_Raw_Line: m_nCalcOffset = m_szRaw.y; break;
	case e3D_Raw_Copy: m_nCalcOffset = 0; break;
	default: m_nCalcOffset = 0; break;
	}
}

void ATI3D_Data::SetBuffer(e3D_Raw eRaw, CPoint szRaw, CPoint szBuf, WORD **pBufH, BYTE **pBufB)
{
	m_szRaw = szRaw;
	m_szBuf = szBuf;
	if (m_bAllocBuffer)
	{
		m_bAllocBuffer = false;
		delete[] m_pBufH;
		delete[] m_pBufB;
	}
	m_pBufH = pBufH;
	m_pBufB = pBufB;
	SetMode(eRaw); 
}

void ATI3D_Data::SetBuffer(e3D_Raw eRaw, CPoint szRaw, CPoint szBuf)
{
	m_szRaw = szRaw;
	m_szBuf = szBuf;
	if (m_bAllocBuffer)
	{
		m_bAllocBuffer = false;
		delete[] m_pBufH;
		delete[] m_pBufB;
	}
	m_bAllocBuffer = true;
	m_pBufH = new WORD*[szBuf.y];
	m_pBufB = new BYTE*[szBuf.y];
	SetMode(eRaw);
}

void ATI3D_Data::SetBuffer(long yp, WORD *pBufH, BYTE *pBufB)
{
	m_pBufH[yp] = pBufH; 
	m_pBufB[yp] = pBufB; 
}

bool ATI3D_Data::Start(bool bInvDir, long lGrab, long msCalc, CPoint cpStart)
{
	if (IsBusy()) return true; 
	m_nGrab = -1;
	m_bInvDir = bInvDir; 
	ReAllocate(lGrab);
	m_msCalc = msCalc;
	m_cpStart = cpStart; 
	if (cpStart.x < 0) return true;
	if (cpStart.x + m_szRaw.x > m_szBuf.x) return true; 
	if (cpStart.y < 0) return true; 
	if (cpStart.y + lGrab - m_szRaw.y > m_szBuf.y) return true; 
	for (long y = 0; y < m_szBuf.y; y++)
	{
		memset(m_pBufB[y], 0, m_szBuf.x); 
		memset(m_pBufH[y], 0, 2 * m_szBuf.x);
	}
	m_swCalc.Start(); 
	m_eStat = e3D_Stat_Grab;
	return false;
}

void ATI3D_Data::GrabDone(long nGrab, BYTE *pBufRaw)
{
	m_pRaw[nGrab] = pBufRaw; 
	m_nGrab = nGrab; 
	if (m_nGrab == m_lGrab) m_eStat = e3D_Stat_Calc;
	else m_eStat = e3D_Stat_Grab;
}

long ATI3D_Data::GetCalcIndex()
{
	return m_lGrab + m_nCalcOffset; 
}

void ATI3D_Data::CalcRaw(int yp, BYTE *pBufCalc)
{
	switch (m_eRaw)
	{
	case e3D_Raw_Line: CalcRawLine(yp, pBufCalc); break;
	case e3D_Raw_Copy: CalcRawCopy(yp, pBufCalc); break;
	default: CalcRawLine(yp, pBufCalc); break;
	}
}

void ATI3D_Data::CalcRawCopy(int yp, BYTE *pBufCalc)
{
	if (m_bInvDir == false)
	{
		memcpy(pBufCalc, m_pRaw[yp], m_szRaw.x * m_szRaw.y);
		return; 
	}
	BYTE *pDst = pBufCalc;
	BYTE *pSrc = &m_pRaw[yp][m_szRaw.x * (m_szRaw.y - 1)];
	for (long y = 0; y < m_szRaw.y; y++, pSrc -= m_szRaw.x, pDst += m_szRaw.x) memcpy(pDst, pSrc, m_szRaw.x);
}

void ATI3D_Data::CalcRawLine(int yp, BYTE *pBufCalc)
{
	BYTE *pDst = pBufCalc;
	BYTE *pSrc;
	long y0 = 0;
	long dy = 0;
	if (m_bInvDir)
	{
		y0 = m_szRaw.y - 1;
		dy = -1;
	}
	else
	{
		y0 = 0;
		dy = 1;
	}
	for (long y = 0, dSrc = 0; y < m_szRaw.y; y++, y0 += dy, dSrc += m_szRaw.x)
	{
		pSrc = &m_pRaw[yp + y0][dSrc];
		memcpy(pDst, pSrc, m_szRaw.x);
		pDst += m_szRaw.x;
	}
}

bool ATI3D_Data::IsCalcTimeout()
{
	return m_swCalc.Check() >= m_msCalc; 
}

void ATI3D_Data::MemCopy(long yp, WORD * pSrcH, BYTE * pSrcB)
{
	yp += m_cpStart.y; 
	memcpy(&m_pBufH[yp][m_cpStart.x], pSrcH, 2 * m_szRaw.x);
	memcpy(&m_pBufB[yp][m_cpStart.x], pSrcB, m_szRaw.x);
}
