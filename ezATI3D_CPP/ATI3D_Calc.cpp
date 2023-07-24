#include "stdafx.h"
#include <thread>
#include <Windows.h>
#include "ATI3D_Calc.h"

ATI3D_Calc::ATI3D_Calc()
{
	m_szRaw.x = m_szRaw.y = 0; 
	m_iszRaw.width = m_szRaw.x;
	m_iszRaw.height = m_szRaw.y;
	m_iszRawT.width = m_szRaw.y;
	m_iszRawT.height = m_szRaw.x;
	m_iszRawY.width = m_szRaw.y;
	m_iszRawY.height = 1;
}

ATI3D_Calc::~ATI3D_Calc()
{
}

void ATI3D_Calc::Init(long nID, ATI3D_Data *pDat)
{
	m_nID = nID; 
	m_pDat = pDat; 
	std::thread thread = std::thread(&ATI3D_Calc::RunThread, this, m_nID);
	thread.detach(); 
}

void ATI3D_Calc::Close()
{
	m_bThread = false; 
	while (!m_bFinish) Sleep(1); 
	ReAllocate(CPoint()); 
}

void ATI3D_Calc::RunThread(long nID)
{
	m_bThread = true; 
	Sleep(2000); 
	while (m_bThread)
	{
		Sleep(1); 
		if (m_bRun)
		{
			for (long y = m_nID; y < m_pDat->m_lGrab + m_pDat->m_nCalcOffset;)
			{
				if (m_pDat->IsCalcTimeout()) y = m_pDat->m_lGrab + m_pDat->m_nCalcOffset;
				else if (y <= m_pDat->m_nGrab - m_pDat->m_nCalcOffset)
				{
					switch (m_eCalc)
					{
					case e3D_Calc_Ave: Calc_Ave(y); break;
					case e3D_Calc_PeakDown: Calc_PeakDown(y); break; 
					}
					y += m_pDat->m_lThread; 
				}
				else Sleep(1); 
			}
			m_bRun = false;
		}
	}
	m_bFinish = true; 
}

void ATI3D_Calc::ReAllocate(CPoint szRaw)
{
	if (szRaw == m_szRaw) return; 
	if ((m_szRaw.x != 0) && (m_szRaw.y != 0))
	{
		delete[] m_aBufCalc;
		delete[] m_aBufCalcT;
		delete[] m_aBufSub1;
		delete[] m_aBufSub2;
		delete[] m_aBufH;
		delete[] m_aBufB;
		m_szRaw.x = m_szRaw.y = 0;
	}
	if (szRaw.x == 0) return; 
	m_szRaw = szRaw;
	m_iszRaw.width = m_szRaw.x;
	m_iszRaw.height = m_szRaw.y;
	m_iszRawT.width = m_szRaw.y;
	m_iszRawT.height = m_szRaw.x;
	m_iszRawY.width = m_szRaw.y;
	m_iszRawY.height = 1;
	m_nHScale = 65536 / m_szRaw.y; 
	long l = szRaw.x * szRaw.y;
	m_aBufCalc = new BYTE[l]; 
	m_aBufCalcT = new BYTE[l];
	m_aBufSub1 = new BYTE[l]; 
	m_aBufSub2 = new BYTE[l];
	m_aBufH = new WORD[szRaw.x];
	m_aBufB = new BYTE[szRaw.x];
}

bool ATI3D_Calc::Start_Ave(long nGVMin)
{
	m_nGVMin1 = nGVMin;
	ReAllocate(m_pDat->m_szRaw);
	m_eCalc = e3D_Calc_Ave;
	m_bRun = true;
	return false;
}

bool ATI3D_Calc::Calc_Ave(long yp)
{
	m_pDat->CalcRaw(yp, m_aBufCalc);
	if (ippiTranspose_8u_C1R(m_aBufCalc, m_szRaw.x, m_aBufCalcT, m_szRaw.y, m_iszRaw) != ippStsNoErr) return true;
	if (ippiSubC_8u_C1RSfs(m_aBufCalcT, m_szRaw.y, (BYTE)m_nGVMin1, m_aBufSub1, m_szRaw.y, m_iszRawT, 0) != ippStsNoErr) return true;
	memset(m_aBufH, 0, sizeof(WORD)* m_szRaw.x);
	memset(m_aBufB, 0, sizeof(BYTE)* m_szRaw.x);
	if (Calc_Ave(m_aBufSub1, m_aBufH, m_aBufB)) return true;
	m_pDat->MemCopy(yp, m_aBufH, m_aBufB);
	return false;
}

bool ATI3D_Calc::Calc_Ave(BYTE *pBufSub, WORD *pBufH, BYTE *pBufB)
{
	if (pBufSub == nullptr) return false;
	BYTE *pBufSubY = pBufSub;
	for (long x = 0; x < m_szRaw.x; x++, pBufSubY += m_szRaw.y)
	{
		long nSum = 0, nYSum = 0;
		for (long y = 0; y < m_szRaw.y; y++)
		{
			BYTE vBuf = pBufSubY[y];
			nSum += vBuf;
			nYSum += (y * vBuf);
		}
		if (nSum != 0)
		{
			pBufH[x] = (WORD)(m_nHScale * nYSum / nSum);
			pBufB[x] = pBufSubY[(long)(1.0 * nYSum / nSum + 0.5)];
		}
	}
	return false;
}

bool ATI3D_Calc::Start_PeakDown(long nGVMin1, long nGVMin2)
{
	m_nGVMin1 = nGVMin1;
	m_nGVMin2 = nGVMin2;
	ReAllocate(m_pDat->m_szRaw);
	m_eCalc = e3D_Calc_PeakDown;
	m_bRun = true; 
	return false; 
}

bool ATI3D_Calc::Calc_PeakDown(long yp)
{
	m_pDat->CalcRaw(yp, m_aBufCalc);
	if (ippiTranspose_8u_C1R(m_aBufCalc, m_szRaw.x, m_aBufCalcT, m_szRaw.y, m_iszRaw) != ippStsNoErr) return true;
	if (ippiSubC_8u_C1RSfs(m_aBufCalcT, m_szRaw.y, (BYTE)m_nGVMin1, m_aBufSub1, m_szRaw.y, m_iszRawT, 0) != ippStsNoErr) return true;
	if ((m_nGVMin2 >= 0) && ippiSubC_8u_C1RSfs(m_aBufCalcT, m_szRaw.y, (BYTE)m_nGVMin2, m_aBufSub2, m_szRaw.y, m_iszRawT, 0) != ippStsNoErr) return true;
	memset(m_aBufH, 0, sizeof(WORD) * m_szRaw.x); 
	memset(m_aBufB, 0, sizeof(BYTE) * m_szRaw.x);
	if (Calc_PeakDown(m_aBufSub1, m_aBufH, m_aBufB)) return true;
	if ((m_nGVMin2 >= 0) && Calc_PeakDown(m_aBufSub2, m_aBufH, m_aBufB)) return true;
	m_pDat->MemCopy(yp, m_aBufH, m_aBufB); 
	return false; 
}

bool ATI3D_Calc::Calc_PeakDown(BYTE *pBufSub, WORD *pBufH, BYTE *pBufB)
{
	if (pBufSub == nullptr) return false;
	BYTE *pBufSubY = pBufSub;
	for (long x = 0; x < m_szRaw.x; x++, pBufSubY += m_szRaw.y)
	{
		if (pBufH[x] == 0)
		{
			long nSum = 0, nYSum = 0;
			long y = m_szRaw.y - 1;
			while ((y > 0) && (pBufSubY[y] == 0)) y--;
			while ((y > 0) && (pBufSubY[y] > 0))
			{
				BYTE vBuf = pBufSubY[y];
				nSum += vBuf;
				nYSum += (y * vBuf);
				y--;
			}
			if (nSum != 0)
			{
				pBufH[x] = (WORD)(m_nHScale * nYSum / nSum);
				pBufB[x] = pBufSubY[(long)(1.0 * nYSum / nSum + 0.5)];
			}
		}
	}
	return false;
}

