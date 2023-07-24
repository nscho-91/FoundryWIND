#include "stdafx.h"
#include "ATI3D_Height.h"


ATI3D_Height::ATI3D_Height()
{
}

ATI3D_Height::~ATI3D_Height()
{
}

void ATI3D_Height::Init(ATI3D_Data * pDat)
{
	m_pDat = pDat; 
}

void ATI3D_Height::Close()
{
	if (m_aCountX != nullptr)
	{
		delete[]m_aCountX;
		m_aCountX = nullptr; 
		m_lszROI.cx = 0; 
	}
	if (m_aCountY != nullptr)
	{
		delete[]m_aCountY;
		m_aCountY = nullptr;
		m_lszROI.cy = 0;
	}
}

void ATI3D_Height::Set(long nOverlap, long nStartOverlap)
{
	m_nOverlap = nOverlap;
	m_nStartOverlap = nStartOverlap; 
}

void ATI3D_Height::CalcOverlap(CPoint cpROI, CSize szROI)
{
	long n = 0;
	if (cpROI.x < m_nStartOverlap) 
	{
		if (cpROI.x > m_nStartOverlap - m_nOverlap / 2) n = 1;
	}
	else 
	{
		long cx = cpROI.x - m_nStartOverlap;
		long sx = m_pDat->m_szRaw.x - m_nOverlap;
		n = cx / sx;
		if (m_nStartOverlap > 0) n++;
		if ((n > 0) && (cx % sx < m_nOverlap / 2)) n--;
	}
	cpROI.x += n * m_nOverlap; 
	m_cpROI = cpROI; 
	m_szROI = szROI; 
	ReAllocate(szROI);
}

void ATI3D_Height::ReAllocate(CSize szROI)
{
	if ((szROI.cx > 0) && (szROI.cx > m_lszROI.cx))
	{
		if (m_lszROI.cx > 0) delete[]m_aCountX;
		m_aCountX = new long[szROI.cx * 2 + 1];
		m_lszROI.cx = szROI.cx; 
	}
	if ((szROI.cy > 0) && (szROI.cy > m_lszROI.cy))
	{
		if (m_lszROI.cy > 0) delete[]m_aCountY;
		m_aCountY = new long[szROI.cy * 2 + 1];
		m_lszROI.cy = szROI.cy;
	}
}

bool ATI3D_Height::CalcGV()
{
	long x, y, nAve = 0, nMax = 0, nCount = 0;
	for (y = m_cpROI.y - m_szROI.cy; y < m_cpROI.y + m_szROI.cy; y++) for (x = m_cpROI.x - m_szROI.cx; x < m_cpROI.x + m_szROI.cx; x++)
	{
		long nVal = m_pDat->m_pBufH[y][x];
		if (nVal) 
		{ 
			if (nMax < nVal) nMax = nVal; 
			nAve += nVal; 
			nCount++; 
		}
	}
	if (nCount == 0) return true;
	m_midGV = (nAve / nCount + nMax) / 2; 
	return false;
}

double ATI3D_Height::CalcHeight_Bump(CSize szBump, double ratioBump)
{
	double fSum = 0, fCount = 0;
	long nGV = m_midGV;
	if (ratioBump < 1.0) 
	{
		for (long y = m_cpROI.y - szBump.cy; y < m_cpROI.y + szBump.cy; y++) for (long x = m_cpROI.x - szBump.cx; x < m_cpROI.x + szBump.cx; x++)
		{
			if (m_pDat->m_pBufH[y][x] >= m_midGV)
			{
				fSum += m_pDat->m_pBufH[y][x];
				fCount += 1;
			}
		}
		nGV = (int)((fSum / fCount) * ratioBump);
		fSum = fCount = 0;
		if (m_midGV > nGV) nGV = m_midGV;
	}
	for (long y = m_cpROI.y - szBump.cy; y < m_cpROI.y + szBump.cy; y++)  for (long x = m_cpROI.x - szBump.cx; x < m_cpROI.x + szBump.cx; x++)
	{
		if (m_pDat->m_pBufH[y][x] >= nGV)
		{
			fSum += (m_pDat->m_pBufH[y][x] * m_pDat->m_pBufB[y][x]);
			fCount += m_pDat->m_pBufB[y][x];
		}
	}
	if (fCount == 0) return 0; 
	return (fSum / fCount);
}

double ATI3D_Height::CalcHeight_SR(CSize szSR, double ratioSR)
{
	double fSum = 0, fCount = 0;
	int nGV = m_midGV;
	if (ratioSR < 1.0)
	{
		for (long y = m_cpROI.y - szSR.cy; y < m_cpROI.y + szSR.cy; y++) for (long x = m_cpROI.x - szSR.cx; x < m_cpROI.x + szSR.cx; x++)
		{
			if (m_pDat->m_pBufH[y][x] < m_midGV)
			{
				fSum += m_pDat->m_pBufH[y][x];
				fCount += 1;
			}
		}
		nGV = (int)((fSum / fCount) * ratioSR);
		fSum = fCount = 0;
	}
	for (long y = m_cpROI.y - szSR.cy; y < m_cpROI.y + szSR.cy; y++) for (long x = m_cpROI.x - szSR.cx; x < m_cpROI.x + szSR.cx; x++)
	{
		if (m_pDat->m_pBufH[y][x]<nGV) 
		{ 
			fSum += (m_pDat->m_pBufH[y][x] * m_pDat->m_pBufB[y][x]); 
			fCount += m_pDat->m_pBufB[y][x];
		}
	}
	if (fCount == 0) return 0;
	return (fSum / fCount);
}

double ATI3D_Height::CalcHeight(CPoint cpROI, CSize szROI, CSize szBump, double ratioBump, double ratioSR)
{
	CalcOverlap(cpROI, szROI); 
	if (CalcGV()) return 0;
	return CalcHeight_Bump(szBump, ratioBump) - CalcHeight_SR(szROI, ratioSR);
}

const long c_dGV = 8; //forget
const long c_n3DBright = 4;

double ATI3D_Height::CalcHeight(CPoint cpROI, CSize szROI, long nGVSR)
{
	CalcOverlap(cpROI, szROI);
	if (CalcGV()) return 0;
	long nCount[2] = { 0, 0 }, nGVB, nGVH, nGVMax = 0; 
	CPoint cpGVMax; 
	double fSum[2] = { 0, 0 }, fGV;
	for (long y = m_cpROI.y; y < m_cpROI.y + m_szROI.cy; y++) for (long x = m_cpROI.x; x<m_cpROI.x + m_szROI.cx; x++)
	{
		if (m_pDat->m_pBufB[y][x] > nGVMax)
		{
			nGVMax = m_pDat->m_pBufB[y][x];
			cpGVMax = CPoint(x, y);
		}
	}
	if (nGVMax == 0) return 0; 
	nGVMax = 0;
	for (long y = m_cpROI.y; y < m_cpROI.y + m_szROI.cy; y++) for (long x = m_cpROI.x; x<m_cpROI.x + m_szROI.cx; x++) nGVMax += m_pDat->m_pBufB[y][x];
	nGVB = nGVMax / 9 - nGVSR * m_pDat->m_szRaw.y / c_n3DBright;
	for (long y = m_cpROI.y; y < m_cpROI.y + m_szROI.cy; y++) for (long x = m_cpROI.x; x < m_cpROI.x + m_szROI.cx; x++)
	{
		if (m_pDat->m_pBufB[y][x] >= nGVB)
		{ 
			fSum[1] += (m_pDat->m_pBufH[y][x] * m_pDat->m_pBufB[y][x]);
			nCount[1] += m_pDat->m_pBufB[y][x];
		}
	}
	if (nCount[1] <= 0) return 0; 
	fGV = fSum[1] / nCount[1]; 
	nGVH = (long)fGV - c_dGV * m_pDat->m_szRaw.y;
	for (long y = m_cpROI.y; y < m_cpROI.y + m_szROI.cy; y++) for (long x = m_cpROI.x; x < m_cpROI.x + m_szROI.cx; x++)
	{
		if ((m_pDat->m_pBufB[y][x] < nGVB) && (m_pDat->m_pBufH[y][x] < nGVH))
		{
			fSum[0] += (m_pDat->m_pBufH[y][x] * m_pDat->m_pBufB[y][x]);
			nCount[0] += m_pDat->m_pBufB[y][x];
		}
	}
	if (nCount[0] <= 0) return 0; 
	else return fGV - fSum[0] / nCount[0];
}

double ATI3D_Height::CalcHeight2(CPoint cpROI, CSize szROI, long nGVSR)
{
	CalcOverlap(cpROI, szROI);
	if (CalcGV()) return 0;
	long n, nCount[2] = { 0, 0 }, nGVMid, nVal, nGap, nGV[2];
	double fSum[2] = { 0, 0 };
	nGV[0] = 50 * m_pDat->m_szRaw.y / c_n3DBright; 
	nGV[1] = 130 * m_pDat->m_szRaw.y / c_n3DBright;
	nGVMid = (nGV[1] + nGV[0]) / 2; nGap = (nGV[1] - nGV[0]) / 2;
	for (long y = m_cpROI.y; y<m_cpROI.y + m_szROI.cy; y++) for (long x = m_cpROI.x; x<m_cpROI.x + m_szROI.cx; x++)
	{
		if (m_pDat->m_pBufB[y][x] >= nGVMid) n = 1;
		else if ((m_pDat->m_pBufB[y - 1][x] < nGVMid) && (m_pDat->m_pBufB[y][x - 1] < nGVMid) && (m_pDat->m_pBufB[y + 1][x] < nGVMid) && (m_pDat->m_pBufB[y][x + 1] < nGVMid)) n = 0;
		else n = -1;
		if (n >= 0)
		{
			nVal = CalcWeight(m_pDat->m_pBufB[y][x] - nGV[n], nGap);
			fSum[n] += (m_pDat->m_pBufH[y][x] * nVal); nCount[n] += nVal;
		}
	}
	if ((nCount[0] == 0) || (nCount[1] == 0)) return 0; 
	return fSum[1] / nCount[1] - fSum[0] / nCount[0];
}

long ATI3D_Height::CalcWeight(long dGV, long nGap)
{
	dGV = nGap - abs(dGV); 
	if (dGV<0) return 0;
	return dGV;
}

double ATI3D_Height::CalcHeightBLT(CPoint cpROI, CSize szROI)
{
	CalcOverlap(cpROI, szROI);
	if (CalcGV()) return 0;
	double fSum = 0;
	long nCount = 0;
	for (long y = m_cpROI.y - szROI.cy; y<m_cpROI.y + szROI.cy; y++) for (long x = m_cpROI.x - szROI.cx; x<m_cpROI.x + szROI.cx; x++)
	{
		fSum += (m_pDat->m_pBufH[y][x] * m_pDat->m_pBufB[y][x]);
		nCount += m_pDat->m_pBufB[y][x];
	}
	if (nCount == 0) return 0;
	return (fSum / nCount);
}

double ATI3D_Height::CalcHeightBump(CPoint cpROI, CSize szROI, CSize szBump, double ratioBump)
{
	CalcOverlap(cpROI, szROI);
	if (CalcGV()) return 0;
	return CalcHeight_Bump(szBump, ratioBump);
}

double ATI3D_Height::CalcHeightSR(CPoint cpROI, CSize szROI, double ratioSR)
{
	CalcOverlap(cpROI, szROI);
	if (CalcGV()) return 0;
	return CalcHeight_Bump(szROI, ratioSR);
}