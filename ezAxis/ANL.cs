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
//========== �Է� ���� ���� ���� ����. =======================================================================
// lNodeNum   : CPU ����� ID ���� ���͸� ����ġ�� ���� �ǹ� �մϴ�.(0x00 �̻�,  0xF9 ����)
//==============================================================================================================

//========== ���̺귯�� �ʱ�ȭ =================================================================================

    // ���̺귯�� �ʱ�ȭ
    [DllImport("ANLNet.dll")] public static extern uint AnlOpen();
    // ��忡 ������ ��� �ʱ�ȭ  
    // ����ڰ� ���ϴ� �Ķ��Ÿ�� Flash�� ����Ǹ� �����ʱ�ȭ���� �װ����� �ʱ�ȭ �ǰ� ������� Axm�� �Ķ��Ÿ Default������ �ʱ�ȭ�ȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlInit(int lNodeNum);
    // ���̺귯�� ����� ����
    [DllImport("ANLNet.dll")] public static extern int AnlClose();
    // ���̺귯���� �ʱ�ȭ �Ǿ� �ִ� �� Ȯ��
    [DllImport("ANLNet.dll")] public static extern int AnlIsOpened();

//========== ���̺귯�� �� ���̽� ���� ���� =================================================================================

    // ��ϵ� ��� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeCount(ref int lpNodeCount);
    // ��� ����(��� ID, ��� ����) Ȯ��
    // lNodeNum : ����ȣ.
    // dwpModuleId : ��� ID �ݵ�� 17���迭�� �����ؾߵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeInfo(int lNodeNum, ref uint dwpModuleId);
    // ��ϵ� ��� ������ �����Ѵ�.
    // upNodeCount : ��尹��
    // upArrayNodeID : �迭 ��� ID �ݵ�� 250���迭�� �����ؾߵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNetInfo(ref uint upNodeCount,  ref uint upArrayNodeID);
    // ���̺귯�� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AxlGetLibVersion(byte[] szVersion);

//========== �α� ���� =================================================================================================

    // EzSpy�� ����� �޽��� ���� ����
    // uLevel : 0 - 3 ����
    // LEVEL_NONE(0)    : ��� �޽����� ������� �ʴ´�.
    // LEVEL_ERROR(1)   : ������ �߻��� �޽����� ����Ѵ�.
    // LEVEL_RUNSTOP(2) : ��ǿ��� Run / Stop ���� �޽����� ����Ѵ�.
    // LEVEL_FUNCTION(3): ��� �޽����� ����Ѵ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnlSetLogLevel(uint uLevel);
    // EzSpy�� ����� �޽��� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnlGetLogLevel(ref uint upLevel);

//========= ��Ʈ��ũ ���� ���� ����� �˸�  �Լ� =============================================================================
   
    // �ش� ����� ���� ��Ʈ��ũ ���¸� Ȯ�� �Ѵ�.
    // dwpFlag : '0' - connected ����
    //           '1' - disconnected ����
    [DllImport("ANLNet.dll")] public static extern uint ANLNetStatusRead(int lNodeNum, ref uint dwpFlag);
    // �ش� ����� Frimware ������ Ȯ���Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlGetNodeFirmVersion(int lNodeNum, ref char szVersion);

    // �ش� ��忡 �ִ� ������ �Ķ���͸� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlSaveParamToFlash(int lNodeNum);
    // �ش� ��忡 �ִ� ������ �������� �ε��Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnlLoadParamFromFlash(int lNodeNum);
    // �ش� ��忡 �ִ� ������ �������� ���� ������ �ʱ�ȭ �Ѵ�.
    // Axm�� �Ķ��Ÿ Default������ �ʱ�ȭ�ȴ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnlResetParamFlash(int lNodeNum);
    
    // Background process check function
    // dwpProcessState --> 255(Background process not execute)
    // dwpProcessState --> 0(Background process idle)
    // dwpProcessState --> 2(Background process run)
   [DllImport("ANLNet.dll")] public static extern uint AnlProcessStateCheck(ref uint dwpProcessState);  
}
