#include "stdafx.h"
#include "ATI3D.h"

ATI3D::ATI3D()
{
}

ATI3D::~ATI3D()
{
}

long ATI3D::Init(long lThread)
{
	m_dat.Init(lThread); 
	m_aCalc = new ATI3D_Calc[lThread];
	for (int n = 0; n < lThread; n++) m_aCalc[n].Init(n, &m_dat);
	return 0;
}

void ATI3D::Close()
{
	for (int n = 0; n < m_dat.m_lThread; n++) m_aCalc[n].Close();
	delete[] m_aCalc;
	m_dat.Close();
}

void ATI3D::CheckDone()
{
	for (long n = 0; n < m_dat.m_lThread; n++)
	{
		if (m_aCalc[n].m_bRun) return; 
	}
	m_dat.m_eStat = e3D_Stat_Done; 
}

bool ATI3D::IsDone()
{
	CheckDone(); 
	return m_dat.m_eStat == e3D_Stat_Done;
}

bool ATI3D::StartCalc_Ave(bool bInvDir, long lGrab, long msCalc, CPoint cpStart, long nGVMin)
{
	if (m_dat.Start(bInvDir, lGrab, msCalc, cpStart)) return true; 
	for (long n = 0; n < m_dat.m_lThread; n++) m_aCalc[n].Start_Ave(nGVMin);
	return false;
}

bool ATI3D::StartCalc_PeakDown(bool bInvDir, long lGrab, long msCalc, CPoint cpStart, long nGVMin1, long nGVMin2)
{
	if (m_dat.Start(bInvDir, lGrab, msCalc, cpStart)) return true;
	for (long n = 0; n < m_dat.m_lThread; n++) m_aCalc[n].Start_PeakDown(nGVMin1, nGVMin2);
	return false;
}

