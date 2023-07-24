// ezCpp.cpp : 해당 DLL의 초기화 루틴을 정의합니다.
//

#include "stdafx.h"
#include "ezCpp.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: 이 DLL이 MFC DLL에 대해 동적으로 링크되어 있는 경우
//		MFC로 호출되는 이 DLL에서 내보내지는 모든 함수의
//		시작 부분에 AFX_MANAGE_STATE 매크로가
//		들어 있어야 합니다.
//
//		예:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// 일반적인 함수 본문은 여기에 옵니다.
//		}
//
//		이 매크로는 MFC로 호출하기 전에
//		각 함수에 반드시 들어 있어야 합니다.
//		즉, 매크로는 함수의 첫 번째 문이어야 하며 
//		개체 변수의 생성자가 MFC DLL로
//		호출할 수 있으므로 개체 변수가 선언되기 전에
//		나와야 합니다.
//
//		자세한 내용은
//		MFC Technical Note 33 및 58을 참조하십시오.
//

// CezCppApp

BEGIN_MESSAGE_MAP(CezCppApp, CWinApp)
END_MESSAGE_MAP()


// CezCppApp 생성

CezCppApp::CezCppApp()
{
	// TODO: 여기에 생성 코드를 추가합니다.
	// InitInstance에 모든 중요한 초기화 작업을 배치합니다.
}


// 유일한 CezCppApp 개체입니다.

CezCppApp theApp;


// CezCppApp 초기화

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