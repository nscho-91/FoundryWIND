/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AND.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Digital Library Header File
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

using System;
using System.Runtime.InteropServices;

public class CAND
{
//========== ��� �� ��� ���� 
    // DIO ����� �ִ��� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AndInfoIsDIOModule(ref uint upStatus);
    // DIO ����� ����� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetModuleCount(ref int lpModuleCount);
    // ������ ����� �Է� ���� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetInputCount(int lModuleNo, ref int lpCount);
    // ������ ����� ��� ���� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetOutputCount(int lModuleNo, ref int lpCount);
    // ������ ��� ��ȣ�� ��� ID ��ȣ, ��� ��ġ, ��� ID Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetModule(int lModuleNo, ref int lpNodeNum, ref int lpModulePos, ref uint upModuleID);

//========== ����� ���� ���� Ȯ�� =================================================================================
//==�Է� ���� ���� Ȯ��
    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� word ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� double word ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ������ �Է� ���� ����� Offset ��ġ���� word ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInport(int lOffset, uint uLevel);
    
    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInport(int lOffset, ref uint upLevel);
    
//==��� ���� ���� Ȯ��
    // ������ ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� byte ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� word ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� double word ������ ������ ������ ����
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� byte ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ������ ��� ���� ����� Offset ��ġ���� word ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutport(int lOffset, uint uLevel);
    
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutport(int lOffset, ref uint upLevel);
    
//========== ����� ��Ʈ ���� �б� =================================================================================
//==��� ��Ʈ ����
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ �����͸� ���
    //===============================================================================================//
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutport(int lOffset, uint uValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� bit ������ �����͸� ���
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportBit(int lModuleNo, int lOffset, uint uValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� byte ������ �����͸� ���
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uValue          : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportByte(int lModuleNo, int lOffset, uint uValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� word ������ �����͸� ���
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uValue          : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportWord(int lModuleNo, int lOffset, uint uValue);
    
//==��� ��Ʈ �б�    
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutport(int lOffset, ref uint upValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� byte ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // ������ ��� ���� ����� Offset ��ġ���� word ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportWord(int lModuleNo, int lOffset, ref uint upValue);
    
//==�Է� ��Ʈ �ϱ�    
    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInport(int lOffset, ref uint upValue);
    
    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // ������ �Է� ���� ����� Offset ��ġ���� word ������ �����͸� �б�
    //===============================================================================================//
    // lModuleNo       : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportWord(int lModuleNo, int lOffset, ref uint upValue);
        
//========== ����� ���� ���� Ȯ�� (���� ��忡 ���� ����)
//==�Է� ���� ���� ��
    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportBit(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uValue          : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportByte(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� word ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportWord(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� double word ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportDword(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ������ �Է� ���� ����� Offset ��ġ���� word ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInport(int lNodeNum, int lOffset, uint uLevel);

    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *uLevel         : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInport(int lNodeNum, int lOffset, ref uint upLevel);

//==�Է� ��Ʈ �б�
    // ��ü �Է� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInport(int lNodeNum, int lOffset, ref uint upValue);

    // ������ �Է� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // ������ �Է� ���� ����� Offset ��ġ���� byte ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // ������ �Է� ���� ����� Offset ��ġ���� word ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : �Է� ������ ���� Offset ��ġ
    // *upValue        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

//==��� ���� ���� Ȯ��
    // ������ ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportBit(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ ��� ���� ����� Offset ��ġ���� byte ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportByte(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ ��� ���� ����� Offset ��ġ���� word ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportWord(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ ��� ���� ����� Offset ��ġ���� double word ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportDword(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // ������ ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ������ ��� ���� ����� Offset ��ġ���� byte ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ������ ��� ���� ����� Offset ��ġ���� word ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ ����
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutport(int lNodeNum, int lOffset, uint uLevel);

    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ ������ ������ Ȯ��
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutport(int lNodeNum, int lOffset, ref uint upLevel);

//========== ����� ��Ʈ ���� �б� 
//==��� ��Ʈ ����
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ �����͸� ���
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutport(int lNodeNum, int lOffset, uint uValue);

    // ������ ��� ���� ����� Offset ��ġ���� bit ������ �����͸� ���
    //===============================================================================================//
    // lNodeNum        : ��� ��ȣ
    // lModulePos      : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset         : ��� ������ ���� Offset ��ġ
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportBit(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // ������ ��� ���� ����� Offset ��ġ���� byte ������ �����͸� ���
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : ��� ������ ���� Offset ��ġ
    // uValue           : 0x00 ~ 0x0FF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportByte(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // ������ ��� ���� ����� Offset ��ġ���� word ������ �����͸� ���
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : ��� ������ ���� Offset ��ġ
    // uValue           : 0x00 ~ 0x0FFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportWord(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // ������ ��� ���� ����� Offset ��ġ���� double word ������ �����͸� ���
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : ��� ������ ���� Offset ��ġ
    // uValue           : 0x00 ~ 0x0FFFFFFFF('1'�� ���� �� ��Ʈ�� HIGH, '0'���� ���� �� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportDword(int lNodeNum, int lModulePos, int lOffset, uint uValue);

//==��� ��Ʈ �б�
    // ��ü ��� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lOffset          : �Է� ������ ���� Offset ��ġ
    // *upValue         : LOW(0)
    //                  : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutport(int lNodeNum, int lOffset, ref uint upValue);

    // ������ ��� ���� ����� Offset ��ġ���� bit ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : �Է� ������ ���� Offset ��ġ
    // *upValue         : LOW(0)
    //                  : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // ������ ��� ���� ����� Offset ��ġ���� byte ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : �Է� ������ ���� Offset ��ġ
    // *upValue         : 0x00 ~ 0x0FF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // ������ ��� ���� ����� Offset ��ġ���� word ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : �Է� ������ ���� Offset ��ġ
    // *upValue         : 0x00 ~ 0x0FFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // ������ ��� ���� ����� Offset ��ġ���� double word ������ �����͸� �б�
    //===============================================================================================//
    // lNodeNum         : ��� ��ȣ
    // lModulePos       : ��� ��ġ(����ڰ� ���͸� ����ġ�� ���� ���� ��ġ)
    // lOffset          : �Է� ������ ���� Offset ��ġ
    // *upValue         : 0x00 ~ 0x0FFFFFFFF('1'�� ���� ��Ʈ�� HIGH, '0'���� ���� ��Ʈ�� LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportDword(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);
    
    // ������ ��� ���� ����� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ��� ����.
    //===============================================================================================//
    // lModuleNo      : ��� ��ȣ
    // dwNetErrorAct  : ������ ����(0 - 1)
    //                  '0' - ���� ���� ���� ����
    //                  '1' - AndoSetNetWorkErrorByteValue �Լ��� ���Ͽ� Setting�� ������ ����
    //===============================================================================================//
     [DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorSetAction(int lModuleNo, uint dwNetErrorAct);

    // ������ ��� ���� ����� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ��� Ȯ��.
    //===============================================================================================//
    // lModuleNo      : ��� ��ȣ
    // *dwNetErrorAct : ������ ����(0 - 1)
    //                  '0' - ���� ���� ���� ����
    //                  '1' - AndoSetNetWorkErrorByteValue �Լ��� ���Ͽ� Setting�� ������ �����ϵ��� ����
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorGetAction(int lModuleNo, ref uint dwpNetErrorAct);
 
    // ������ ��� ���� ����� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ����.
    //===============================================================================================//
    // lModuleNo      : ��� ��ȣ
    // lOffset        : ��� ������ ���� Offset ��ġ
    // uValue         : 0x00 ~ 0xFF('1'��Ʈ�� ������ ��� High, '0'��Ʈ�� ������ ��� Low) 
    //===============================================================================================//
	[DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorSetByteValue(int lModuleNo, int lOffset, uint uValue);

	// ������ ��� ���� ����� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ���� Ȯ��.
    //===============================================================================================//
    // lModuleNo      : ��� ��ȣ
    // lOffset        : ��� ������ ���� Offset ��ġ
    // *upValue       : 0x00 ~ 0xFF('1'��Ʈ�� ������ ��� High, '0'��Ʈ�� ������ ��� Low) 
    //===============================================================================================//
	[DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorGetByteValue(int lModuleNo, int lOffset, ref uint upValue);
}
