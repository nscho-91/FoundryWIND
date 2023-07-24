#pragma once
#include "StopWatch.h"
#include <atltypes.h>

enum e3D_Stat
{
	e3D_Stat_Init,
	e3D_Stat_Ready,
	e3D_Stat_Grab,
	e3D_Stat_Calc,
	e3D_Stat_Done,
};

enum e3D_Raw
{
	e3D_Raw_Copy,
	e3D_Raw_Line,
};

class ATI3D_Data
{
protected:
	unsigned char **m_pRaw = nullptr; 
	StopWatch m_swCalc;
	bool m_bAllocBuffer = false; 

	void ReAllocate(long lGrab);
	bool IsBusy();

	void CalcRawCopy(int yp, BYTE *pBufCalc); 
	void CalcRawLine(int yp, BYTE *pBufCalc);

public:
	e3D_Stat m_eStat = e3D_Stat_Init;
	e3D_Raw m_eRaw = e3D_Raw_Line; 
	WORD **m_pBufH = nullptr; 
	BYTE **m_pBufB = nullptr;
	long m_lThread = 1; 
	long m_lGrab = 0; 
	long m_nGrab = 0; 
	long m_msCalc = 0;
	CPoint m_szRaw; 
	CPoint m_szBuf;
	CPoint m_cpStart; 
	bool m_bInvDir = false; 
	long m_nCalcOffset = 0;

	ATI3D_Data();
	~ATI3D_Data();
	void Init(long lThread);
	void Close(); 
	void SetMode(e3D_Raw eRaw); 
	void SetBuffer(e3D_Raw eRaw, CPoint szRaw, CPoint szBuf, WORD **pBufH, BYTE **pBufB);
	void SetBuffer(e3D_Raw eRaw, CPoint szRaw, CPoint szBuf);
	void SetBuffer(long yp, WORD *pBufH, BYTE *pBufB);
	bool Start(bool bInvDir, long lGrab, long msCalc, CPoint cpStart);
	void GrabDone(long nGrab, BYTE *pBufRaw); 
	long GetCalcIndex(); 
	bool IsDone();
	void CalcRaw(int yp, BYTE *pBufCalc); 
	bool IsCalcTimeout(); 
	void MemCopy(long yp, WORD * pSrcH, BYTE * pSrcB);
};

