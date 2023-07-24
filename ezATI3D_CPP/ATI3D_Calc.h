#pragma once
#include "ATI3D_Data.h"
#include "ippi.h"

enum e3D_Calc
{
	e3D_Calc_Ave,
	e3D_Calc_PeakDown
};

class ATI3D_Calc
{
protected:
	long m_nID = 0; 
	e3D_Calc m_eCalc = e3D_Calc_PeakDown;

	BYTE *m_aBufCalcT = nullptr;
	BYTE *m_aBufSub1 = nullptr;
	BYTE *m_aBufSub2 = nullptr;

	WORD *m_aBufH = nullptr;
	BYTE *m_aBufB = nullptr;

	ATI3D_Data *m_pDat = nullptr;
	CPoint m_szRaw; 
	IppiSize m_iszRaw;
	IppiSize m_iszRawT;
	IppiSize m_iszRawY;

	long m_nGVMin1 = 0; 
	long m_nGVMin2 = 0; 

	long m_nHScale = 1; 

	bool m_bThread = false;
	bool m_bFinish = false;

	void ReAllocate(CPoint szRaw);

	bool Calc_Ave(long yp);
	bool Calc_Ave(BYTE *pBufSub, WORD *pBufH, BYTE *pBufB);

	bool Calc_PeakDown(long yp);
	bool Calc_PeakDown(BYTE *pBufSub, WORD *pBufH, BYTE *pBufB);

public:
	bool m_bRun = false;
	BYTE *m_aBufCalc = nullptr;

	ATI3D_Calc();
	~ATI3D_Calc();
	void Init(long nID, ATI3D_Data *pDat); 
	void Close(); 
	void RunThread(long nID); 
	bool Start_Ave(long nGVMin);
	bool Start_PeakDown(long nGVMin1, long nGVMin2);
};

