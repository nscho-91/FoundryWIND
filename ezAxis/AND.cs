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
//========== 노드 및 모듈 정보 
    // DIO 모듈이 있는지 확인
    [DllImport("ANLNet.dll")] public static extern uint AndInfoIsDIOModule(ref uint upStatus);
    // DIO 입출력 모듈의 개수 확인
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetModuleCount(ref int lpModuleCount);
    // 지정한 모듈의 입력 접점 개수 확인
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetInputCount(int lModuleNo, ref int lpCount);
    // 지정한 모듈의 출력 접점 개수 확인
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetOutputCount(int lModuleNo, ref int lpCount);
    // 지정한 모듈 번호로 노드 ID 번호, 모듈 위치, 모듈 ID 확인
    [DllImport("ANLNet.dll")] public static extern uint AndInfoGetModule(int lModuleNo, ref int lpNodeNum, ref int lpModulePos, ref uint upModuleID);

//========== 입출력 레벨 설정 확인 =================================================================================
//==입력 레벨 설정 확인
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelSetInport(int lOffset, uint uLevel);
    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiLevelGetInport(int lOffset, ref uint upLevel);
    
//==출력 레벨 설정 확인
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelSetOutport(int lOffset, uint uLevel);
    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoLevelGetOutport(int lOffset, ref uint upLevel);
    
//========== 입출력 포트 쓰기 읽기 =================================================================================
//==출력 포트 쓰기
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutport(int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportBit(int lModuleNo, int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uValue          : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportByte(int lModuleNo, int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uValue          : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoWriteOutportWord(int lModuleNo, int lOffset, uint uValue);
    
//==출력 포트 읽기    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutport(int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoReadOutportWord(int lModuleNo, int lOffset, ref uint upValue);
    
//==입력 포트 일기    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInport(int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo       : 모듈 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiReadInportWord(int lModuleNo, int lOffset, ref uint upValue);
        
//========== 입출력 레벨 설정 확인 (지정 노드에 대한 제어)
//==입력 레벨 설정 인
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportBit(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uValue          : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportByte(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportWord(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInportDword(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelSetInport(int lNodeNum, int lOffset, uint uLevel);

    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *uLevel         : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNLevelGetInport(int lNodeNum, int lOffset, ref uint upLevel);

//==입력 포트 읽기
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInport(int lNodeNum, int lOffset, ref uint upValue);

    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 입력 접점에 대한 Offset 위치
    // *upValue        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndiNReadInportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

//==출력 레벨 설정 확인
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportBit(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportByte(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportWord(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutportDword(int lNodeNum, int lModulePos, int lOffset, uint uLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upLevel);

    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelSetOutport(int lNodeNum, int lOffset, uint uLevel);

    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // *upLevel        : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNLevelGetOutport(int lNodeNum, int lOffset, ref uint upLevel);

//========== 입출력 포트 쓰기 읽기 
//==출력 포트 쓰기
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutport(int lNodeNum, int lOffset, uint uValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lNodeNum        : 노드 번호
    // lModulePos      : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset         : 출력 접점에 대한 Offset 위치
    // uLevel          : LOW(0)
    //                 : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportBit(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 출력
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 출력 접점에 대한 Offset 위치
    // uValue           : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportByte(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 출력
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 출력 접점에 대한 Offset 위치
    // uValue           : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportWord(int lNodeNum, int lModulePos, int lOffset, uint uValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 출력
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 출력 접점에 대한 Offset 위치
    // uValue           : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNWriteOutportDword(int lNodeNum, int lModulePos, int lOffset, uint uValue);

//==출력 포트 읽기
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lOffset          : 입력 접점에 대한 Offset 위치
    // *upValue         : LOW(0)
    //                  : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutport(int lNodeNum, int lOffset, ref uint upValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 입력 접점에 대한 Offset 위치
    // *upValue         : LOW(0)
    //                  : HIGH(1)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportBit(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 입력 접점에 대한 Offset 위치
    // *upValue         : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportByte(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 입력 접점에 대한 Offset 위치
    // *upValue         : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportWord(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);

    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 읽기
    //===============================================================================================//
    // lNodeNum         : 노드 번호
    // lModulePos       : 모듈 위치(사용자가 로터리 스위치로 정한 절대 위치)
    // lOffset          : 입력 접점에 대한 Offset 위치
    // *upValue         : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNReadOutportDword(int lNodeNum, int lModulePos, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈이 연결된 노드의 통신이 끊어 졌을때 현재 출력 접점 상태에 대한 예외처리 방법 설정.
    //===============================================================================================//
    // lModuleNo      : 모듈 번호
    // dwNetErrorAct  : 접점의 상태(0 - 1)
    //                  '0' - 현재 접점 상태 유지
    //                  '1' - AndoSetNetWorkErrorByteValue 함수에 의하여 Setting된 값으로 동작
    //===============================================================================================//
     [DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorSetAction(int lModuleNo, uint dwNetErrorAct);

    // 지정한 출력 접점 모듈이 연결된 노드의 통신이 끊어 졌을때 현재 출력 접점 상태에 대한 예외처리 방법 확인.
    //===============================================================================================//
    // lModuleNo      : 모듈 번호
    // *dwNetErrorAct : 접점의 상태(0 - 1)
    //                  '0' - 현재 접점 상태 유지
    //                  '1' - AndoSetNetWorkErrorByteValue 함수에 의하여 Setting된 값으로 동작하도록 설정
    //===============================================================================================//
    [DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorGetAction(int lModuleNo, ref uint dwpNetErrorAct);
 
    // 지정한 출력 접점 모듈이 연결된 노드의 통신이 끊어 졌을때 현재 출력 접점 상태에 대한 예외처리 상태.
    //===============================================================================================//
    // lModuleNo      : 모듈 번호
    // lOffset        : 출력 접점에 대한 Offset 위치
    // uValue         : 0x00 ~ 0xFF('1'네트웍 에러시 출력 High, '0'네트웍 에러시 출력 Low) 
    //===============================================================================================//
	[DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorSetByteValue(int lModuleNo, int lOffset, uint uValue);

	// 지정한 출력 접점 모듈이 연결된 노드의 통신이 끊어 졌을때 현재 출력 접점 상태에 대한 예외처리 상태 확인.
    //===============================================================================================//
    // lModuleNo      : 모듈 번호
    // lOffset        : 출력 접점에 대한 Offset 위치
    // *upValue       : 0x00 ~ 0xFF('1'네트웍 에러시 출력 High, '0'네트웍 에러시 출력 Low) 
    //===============================================================================================//
	[DllImport("ANLNet.dll")] public static extern uint AndoNetWorkErrorGetByteValue(int lModuleNo, int lOffset, ref uint upValue);
}
