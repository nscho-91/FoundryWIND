// ezCpp.h : ezCpp DLL�� �⺻ ��� �����Դϴ�.
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH�� ���� �� ������ �����ϱ� ���� 'stdafx.h'�� �����մϴ�."
#endif

#include "resource.h"		// �� ��ȣ�Դϴ�.


// CezCppApp
// �� Ŭ������ ������ ������ ezCpp.cpp�� �����Ͻʽÿ�.
//

class CezCppApp : public CWinApp
{
public:
	CezCppApp();

// �������Դϴ�.
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
