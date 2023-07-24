/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** ANL.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Library Header File
** 
**
*****************************************************************************
*****************************************************************************
**
** Source Change Indices
** ---------------------
**
** (None)
**
*****************************************************************************
*****************************************************************************
**
** Website
** ---------------------
**
** http://www.ajinextek.com
**
*****************************************************************************
*****************************************************************************
*/

using System.Runtime.InteropServices;

public class CANL
{
//========== 입력 인자 사용시 주의 사항. =======================================================================
// lNodeNum   : CPU 모듈의 ID 설정 로터리 스위치의 값을 의미 합니다.(0x00 이상,  0xF9 이하)
//==============================================================================================================

//========== 라이브러리 초기화 =================================================================================

    // 라이브러리 초기화
    [DllImport("ANLNet.dll")] public static extern uint AnlOpen();
    // 노드에 장착된 모듈 초기화  
    // 사용자가 원하는 파라메타로 Flash에 저장되면 다음초기화부터 그값으로 초기화 되고 없을경우 Axm에 파라메타 Default값으로 초기화된다.
    [DllImport("ANLNet.dll")] public static extern uint AnlInit(int lNodeNum);
    // 라이브러리 사용을 종료
    [DllImport("ANLNet.dll")] public static extern int AnlClose();
    // 라이브러리가 초기화 되어 있는 지 확인
    [DllImport("ANLNet.dll")] public static extern int AnlIsOpened();

//========== 라이브러리 및 베이스 보드 정보 =================================================================================

    // 등록된 노드 개수 확인
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeCount(ref int lpNodeCount);
    // 노드 정보(노드 ID, 모듈 정보) 확인
    // lNodeNum : 노드번호.
    // dwpModuleId : 모듈 ID 반드시 17개배열로 선언해야됨.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeInfo(int lNodeNum, ref uint dwpModuleId);
    // 등록된 노드 정보를 제공한다.
    // upNodeCount : 노드갯수
    // upArrayNodeID : 배열 노드 ID 반드시 250개배열로 선언해야됨.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNetInfo(ref uint upNodeCount,  ref uint upArrayNodeID);
    // 라이브러리 버전 확인
    [DllImport("ANLNet.dll")] public static extern uint AxlGetLibVersion(byte[] szVersion);

//========== 로그 레벨 =================================================================================================

    // EzSpy에 출력할 메시지 레벨 설정
    // uLevel : 0 - 3 설정
    // LEVEL_NONE(0)    : 모든 메시지를 출력하지 않는다.
    // LEVEL_ERROR(1)   : 에러가 발생한 메시지만 출력한다.
    // LEVEL_RUNSTOP(2) : 모션에서 Run / Stop 관련 메시지를 출력한다.
    // LEVEL_FUNCTION(3): 모든 메시지를 출력한다.    
    [DllImport("ANLNet.dll")] public static extern uint AnlSetLogLevel(uint uLevel);
    // EzSpy에 출력할 메시지 레벨 확인
    [DllImport("ANLNet.dll")] public static extern uint AnlGetLogLevel(ref uint upLevel);

//========= 네트워크 상태 관련 사용자 알림  함수 =============================================================================
   
    // 해당 노드의 현재 네트웍크 상태를 확인 한다.
    // dwpFlag : '0' - connected 상태
    //           '1' - disconnected 상태
    [DllImport("ANLNet.dll")] public static extern uint ANLNetStatusRead(int lNodeNum, ref uint dwpFlag);
    // 해당 노드의 Frimware 정보를 확인한다.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeFirmVersion(int lNodeNum, ref char szVersion);

    // 해당 노드에 있는 모듈들의 파라미터를 저장한다.
    [DllImport("ANLNet.dll")] public static extern uint AnlSaveParamToFlash(int lNodeNum);
    // 해당 노드에 있는 모듈들의 설정값을 로드한다.
    [DllImport("ANLNet.dll")] public static extern uint AnlLoadParamFromFlash(int lNodeNum);
    // 해당 노드에 있는 모듈들의 설정값의 저장 공간을 초기화 한다.
    // Axm에 파라메타 Default값으로 초기화된다.    
    [DllImport("ANLNet.dll")] public static extern uint AnlResetParamFlash(int lNodeNum);
    
    // Background process check function
    // dwpProcessState --> 255(Background process not execute)
    // dwpProcessState --> 0(Background process idle)
    // dwpProcessState --> 2(Background process run)
   [DllImport("ANLNet.dll")] public static extern uint AnlProcessStateCheck(ref uint dwpProcessState);  
}
