#pragma once
#include "ATI3D_Data.h"

class ATI3D_Height
{
protected:
	ATI3D_Data *m_pDat = nullptr;
	long m_midGV = 0; 
	long m_nOverlap = 0; 
	long m_nStartOverlap = 0;
	long *m_aCountX = nullptr; 
	long *m_aCountY = nullptr; 
	CPoint m_cpROI; 
	CSize m_szROI; 
	CSize m_lszROI = CSize(0, 0);
	void CalcOverlap(CPoint cpROI, CSize szROI);
	void ReAllocate(CSize szROI);
	bool CalcGV();
	double CalcHeight_Bump(CSize szBump, double ratioBump); 
	double CalcHeight_SR(CSize szROI, double ratioSR); 
	long CalcWeight(long dGV, long nGap); 

public:
	ATI3D_Height();
	~ATI3D_Height();
	void Init(ATI3D_Data * pDat);
	void Close();
	void Set(long nOverlap, long nStartOverlap);
	double CalcHeight(CPoint cpROI, CSize szROI, CSize szBumpROI, double ratioBump, double ratioSR);
	double CalcHeight(CPoint cpROI, CSize szROI, long nGVSR); 
	double CalcHeight2(CPoint cpROI, CSize szROI, long nGVSR);
	double CalcHeightBLT(CPoint cpROI, CSize szROI); 
	double CalcHeightBump(CPoint cpROI, CSize szROI, CSize szBump, double ratioBump); 
	double CalcHeightSR(CPoint cpROI, CSize szROI, double ratioSR); 
};

