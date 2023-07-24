#include "stdafx.h"
#include "StopWatch.h"

StopWatch::StopWatch()
{
	QueryPerformanceFrequency(&m_freq); 
	m_ms = 0; 
	m_msFreq = (long)(m_freq.QuadPart / 1000);
	m_bFlag = m_bPause = false; 
	Start();
}

StopWatch::~StopWatch()
{
}

void StopWatch::Start()
{
	if (m_bPause) Pause(false); 
	else QueryPerformanceCounter(&m_start);
	m_bPause = false; 
}

long StopWatch::Check()
{
	if (m_bPause) m_stop.QuadPart = m_pause.QuadPart; 
	else QueryPerformanceCounter(&m_stop);
	m_ms = (long)((m_stop.QuadPart - m_start.QuadPart) / m_msFreq);
	return m_ms;
}

void StopWatch::Pause(bool bPause)
{
	if (m_bPause == bPause) return; 
	m_bPause = bPause;
	if (m_bPause) QueryPerformanceCounter(&m_pause);
	else 
	{ 
		QueryPerformanceCounter(&m_stop); 
		m_start.QuadPart = m_start.QuadPart + m_stop.QuadPart - m_pause.QuadPart; 
	}
}

void StopWatch::Delay_us(long us)
{
	Start(); 
	us = (long)(us * m_freq.QuadPart / 1000000);
	QueryPerformanceCounter(&m_stop);
	while ((long)(m_stop.QuadPart - m_start.QuadPart) < us) QueryPerformanceCounter(&m_stop);
}
