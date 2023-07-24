#pragma once
#include "ATI3D_Data.h"
#include "ATI3D_Calc.h"

class ATI3D
{
protected:
	void CheckDone();

public:
	ATI3D_Data m_dat;
	ATI3D_Calc *m_aCalc = nullptr;

	explicit ATI3D();
	virtual ~ATI3D();
	long Init(long nThread);
	void Close();
	bool IsDone(); 
	bool StartCalc_Ave(bool bInvDir, long lGrab, long msCalc, CPoint cpStart, long nGVMin);
	bool StartCalc_PeakDown(bool bInvDir, long lGrab, long msCalc, CPoint cpStart, long nGVMin1, long nGVMin2);
};

