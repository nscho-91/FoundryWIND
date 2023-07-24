// ezCpp.cpp : �ش� DLL�� �ʱ�ȭ ��ƾ�� �����մϴ�.
//

#include "stdafx.h"
#include "ezCpp.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: �� DLL�� MFC DLL�� ���� �������� ��ũ�Ǿ� �ִ� ���
//		MFC�� ȣ��Ǵ� �� DLL���� ���������� ��� �Լ���
//		���� �κп� AFX_MANAGE_STATE ��ũ�ΰ�
//		��� �־�� �մϴ�.
//
//		��:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// �Ϲ����� �Լ� ������ ���⿡ �ɴϴ�.
//		}
//
//		�� ��ũ�δ� MFC�� ȣ���ϱ� ����
//		�� �Լ��� �ݵ�� ��� �־�� �մϴ�.
//		��, ��ũ�δ� �Լ��� ù ��° ���̾�� �ϸ� 
//		��ü ������ �����ڰ� MFC DLL��
//		ȣ���� �� �����Ƿ� ��ü ������ ����Ǳ� ����
//		���;� �մϴ�.
//
//		�ڼ��� ������
//		MFC Technical Note 33 �� 58�� �����Ͻʽÿ�.
//

// CezCppApp

BEGIN_MESSAGE_MAP(CezCppApp, CWinApp)
END_MESSAGE_MAP()


// CezCppApp ����

CezCppApp::CezCppApp()
{
	// TODO: ���⿡ ���� �ڵ带 �߰��մϴ�.
	// InitInstance�� ��� �߿��� �ʱ�ȭ �۾��� ��ġ�մϴ�.
}


// ������ CezCppApp ��ü�Դϴ�.

CezCppApp theApp;


// CezCppApp �ʱ�ȭ

BOOL CezCppApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

extern "C"
{
	__declspec(dllexport) void cpp_memcpy(unsigned char* dst, unsigned char* src, int nLength)
	{
		memcpy(dst, src, nLength);
	}

	__declspec(dllexport) void cpp_memset(unsigned char* dst, unsigned char val, int nLength)
	{
		memset(dst, val, nLength);
	}

	__declspec(dllexport) void cpp_StretchDIBits(int biWidth, int biHeight, int biBitCount, HDC hdc, int XDest, int YDest, int nDestWidth, int nDestHight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, const void *lpBits, UINT iUsage, DWORD dwRop)
	{
		BITMAPINFO* lpBitsInfo;
		BITMAPINFOHEADER lpBitsHeader;
		lpBitsInfo = (BITMAPINFO *) new BYTE[sizeof(BITMAPINFO)+256 * sizeof(RGBQUAD)];
		lpBitsHeader.biSize = sizeof(BITMAPINFOHEADER);
		lpBitsHeader.biWidth = biWidth;
		lpBitsHeader.biHeight = biHeight;
		lpBitsHeader.biPlanes = 1;
		lpBitsHeader.biBitCount = biBitCount;
		lpBitsHeader.biCompression = BI_RGB;
		lpBitsHeader.biSizeImage = (biWidth + 4 - (biWidth % 4)) * biHeight;
		lpBitsHeader.biXPelsPerMeter = 0;
		lpBitsHeader.biYPelsPerMeter = 0;
		lpBitsHeader.biClrUsed = 256;
		lpBitsHeader.biClrImportant = 256;
		lpBitsInfo->bmiHeader = lpBitsHeader;
		for (int i = 0; i < 256; i++)
		{
			lpBitsInfo->bmiColors[i].rgbBlue = (BYTE)i;
			lpBitsInfo->bmiColors[i].rgbRed = (BYTE)i;
			lpBitsInfo->bmiColors[i].rgbGreen = (BYTE)i;
			lpBitsInfo->bmiColors[i].rgbReserved = (BYTE)i;
		}
		StretchDIBits(hdc, XDest, YDest, nDestWidth, nDestHight, XSrc, YSrc, nSrcWidth, nSrcHeight, lpBits, lpBitsInfo, iUsage, dwRop);
	}
}