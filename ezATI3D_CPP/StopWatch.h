#pragma once
#include <Windows.h>

class StopWatch
{
protected:
	bool m_bPause;
	long m_ms, m_msFreq;
	LARGE_INTEGER m_freq, m_start, m_stop, m_pause;
public:
	bool m_bFlag; 
	StopWatch();
	virtual ~StopWatch();
	void Start();
	long Check();
	void Pause(bool bPause);
	void Delay_us(long us);
};
