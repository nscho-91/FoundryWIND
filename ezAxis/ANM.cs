/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** ANM.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Motion Library Header File
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

public class CANM
{

//========== ��� �� ��� Ȯ���Լ�(Info) - Infomation =================================================================================

    // �ش� ���� ����ȣ, ��� ��ġ, ��� ���̵� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmInfoGetAxis(int nAxisNo, ref int lpNodeNum, ref int npModulePos, ref uint upModuleID);
    // ��� ����� �����ϴ��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmInfoIsMotionModule(ref uint upStatus);
    // �ش� ���� ��ȿ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmInfoIsInvalidAxisNo(int lAxisNo);
    // CAMC-QI �� ����, �ý��ۿ� ������ ��ȿ�� ��� ����� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmInfoGetAxisCount(ref int lpAxisCount);
    // �ش� ���/����� ù��° ���ȣ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmInfoGetFirstAxisNo(int lNodeNum, int lModulePos, ref int lpAxisNo);
    
//========= ���� �� �Լ� ============================================================================================    
    // �ʱ� ���¿��� Anm ��� �Լ��� ���ȣ ������ 0 ~ (���� �ý��ۿ� ������ ��� - 1) �������� ��ȿ������
    // �� �Լ��� ����Ͽ� ���� ������ ���ȣ ��� ������ ���ȣ�� �ٲ� �� �ִ�.
    // �� �Լ��� ���� �ý����� H/W ������� �߻��� ���� ���α׷��� �Ҵ�� ���ȣ�� �״�� �����ϰ� ���� ���� ���� 
    // �������� ��ġ�� �����Ͽ� ����� ���� ������� �Լ��̴�.
    // ���ǻ��� : ���� ���� ���� ���ȣ�� ���Ͽ� ���� ��ȣ�� ���� ���� �ߺ��ؼ� ������ ��� 
    //            ���� ���ȣ�� ���� �ุ ���� ���ȣ�� ���� �� �� ������, 
    //            ������ ���� ������ ��ȣ�� ���ε� ���� ��� �Ұ����� ��찡 �߻� �� �� �ִ�.

    // �������� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmVirtualSetAxisNoMap(int nRealAxisNo, int nVirtualAxisNo);
    // ������ ������ ��ȣ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmVirtualGetAxisNoMap(int nRealAxisNo, ref int npVirtualAxisNo);
    // ��Ƽ �������� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmVirtualSetMultiAxisNoMap(int nSize, int[] npRealAxesNo, int[] npVirtualAxesNo);
    // ������ ��Ƽ ������ ��ȣ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmVirtualGetMultiAxisNoMap(int nSize, ref int npRealAxesNo, ref int npVirtualAxesNo);
    // ������ ������ �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmVirtualResetAxisMap();

//======== ��� �Ķ��Ÿ ���� ===========================================================================================================================================================
    // AnmMotLoadParaAll�� ������ Load ��Ű�� ������ �ʱ� �Ķ��Ÿ ������ �⺻ �Ķ��Ÿ ����. 
    // ���� PC�� ���Ǵ� ����࿡ �Ȱ��� ����ȴ�. �⺻�Ķ��Ÿ�� �Ʒ��� ����. 
    // 00:AXIS_NO.             =0       01:PULSE_OUT_METHOD.    =4      02:ENC_INPUT_METHOD.    =3     03:INPOSITION.          =2
    // 04:ALARM.               =0       05:NEG_END_LIMIT.       =0      06:POS_END_LIMIT.       =0     07:MIN_VELOCITY.        =1
    // 08:MAX_VELOCITY.        =700000  09:HOME_SIGNAL.         =4      10:HOME_LEVEL.          =1     11:HOME_DIR.            =0
    // 12:ZPHASE_LEVEL.        =1       13:ZPHASE_USE.          =0      14:STOP_SIGNAL_MODE.    =0     15:STOP_SIGNAL_LEVEL.   =0
    // 16:HOME_FIRST_VELOCITY. =10000   17:HOME_SECOND_VELOCITY.=10000  18:HOME_THIRD_VELOCITY. =2000  19:HOME_LAST_VELOCITY.  =100
    // 20:HOME_FIRST_ACCEL.    =40000   21:HOME_SECOND_ACCEL.   =40000  22:HOME_END_CLEAR_TIME. =1000  23:HOME_END_OFFSET.     =0
    // 24:NEG_SOFT_LIMIT.      =0.000   25:POS_SOFT_LIMIT.      =0      26:MOVE_PULSE.          =1     27:MOVE_UNIT.           =1
    // 28:INIT_POSITION.       =1000    29:INIT_VELOCITY.       =200    30:INIT_ACCEL.          =400   31:INIT_DECEL.          =400
    // 32:INIT_ABSRELMODE.     =0       33:INIT_PROFILEMODE.    =4

    // 00=[AXIS_NO             ]: �� (0�� ���� ������)
    // 01=[PULSE_OUT_METHOD    ]: Pulse out method TwocwccwHigh = 6
    // 02=[ENC_INPUT_METHOD    ]: disable = 0   1ü�� = 1  2ü�� = 2  4ü�� = 3, �ἱ ���ù��� ��ü��(-).1ü�� = 11  2ü�� = 12  4ü�� = 13
    // 03=[INPOSITION          ], 04=[ALARM     ], 05,06 =[END_LIMIT   ]  : 0 = A���� 1= B���� 2 = ������. 3 = �������� ����
    // 07=[MIN_VELOCITY        ]: ���� �ӵ�(START VELOCITY)
    // 08=[MAX_VELOCITY        ]: ����̹��� ������ �޾Ƶ��ϼ� �ִ� ���� �ӵ�. ���� �Ϲ� Servo�� 700k
    // Ex> screw : 20mm pitch drive: 10000 pulse ����: 400w
    // 09=[HOME_SIGNAL         ]: 4 - Home in0 , 0 :PosEndLimit , 1 : NegEndLimit // _HOME_SIGNAL����.
    // 10=[HOME_LEVEL          ]: 0 = A���� 1= B���� 2 = ������. 3 = �������� ����
    // 11=[HOME_DIR            ]: Ȩ ����(HOME DIRECTION) 1:+����, 0:-����
    // 12=[ZPHASE_LEVEL        ]: 0 = A���� 1= B���� 2 = ������. 3 = �������� ����
    // 13=[ZPHASE_USE          ]: Z���뿩��. 0: ������ , 1: +����, 2: -���� 
    // 14=[STOP_SIGNAL_MODE    ]: ESTOP, SSTOP ���� ��� 0:��������, 1:������ 
    // 15=[STOP_SIGNAL_LEVEL   ]: ESTOP, SSTOP ��� ����.  0 = A���� 1= B���� 2 = ������. 3 = �������� ���� 
    // 16=[HOME_FIRST_VELOCITY ]: 1�������ӵ� 
    // 17=[HOME_SECOND_VELOCITY]: �����ļӵ� 
    // 18=[HOME_THIRD_VELOCITY ]: ������ �ӵ� 
    // 19=[HOME_LAST_VELOCITY  ]: index�˻��� �����ϰ� �˻��ϱ����� �ӵ�. 
    // 20=[HOME_FIRST_ACCEL    ]: 1�� ���ӵ� , 21=[HOME_SECOND_ACCEL   ] : 2�� ���ӵ� 
    // 22=[HOME_END_CLEAR_TIME ]: ���� �˻� Enc �� Set�ϱ� ���� ���ð�,  23=[HOME_END_OFFSET] : ���������� Offset��ŭ �̵�.
    // 24=[NEG_SOFT_LIMIT      ]: - SoftWare Limit ���� �����ϸ� ������, 25=[POS_SOFT_LIMIT ]: + SoftWare Limit ���� �����ϸ� ������.
    // 26=[MOVE_PULSE          ]: ����̹��� 1ȸ���� �޽���              , 27=[MOVE_UNIT  ]: ����̹� 1ȸ���� �̵��� ��:��ũ�� Pitch
    // 28=[INIT_POSITION       ]: ������Ʈ ���� �ʱ���ġ  , ����ڰ� ���Ƿ� ��밡��
    // 29=[INIT_VELOCITY       ]: ������Ʈ ���� �ʱ�ӵ�  , ����ڰ� ���Ƿ� ��밡��
    // 30=[INIT_ACCEL          ]: ������Ʈ ���� �ʱⰡ�ӵ�, ����ڰ� ���Ƿ� ��밡��
    // 31=[INIT_DECEL          ]: ������Ʈ ���� �ʱⰨ�ӵ�, ����ڰ� ���Ƿ� ��밡��
    // 32=[INIT_ABSRELMODE     ]: ����(0)/���(1) ��ġ ����
    // 33=[INIT_PROFILEMODE    ]: �������ϸ��(0 - 4) ���� ����
    //                            '0': ��Ī Trapezode, '1': ���Ī Trapezode, '2': ��Ī Quasi-S Curve, '3':��Ī S Curve, '4':���Ī S Curve
   
    // AnmMotSaveParaAll�� ���� �Ǿ��� .mot������ �ҷ��´�. �ش� ������ ����ڰ� Edit �Ͽ� ��� �����ϴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotLoadParaAll(string szFilePath);
    // ����࿡ ���� ��� �Ķ��Ÿ�� �ະ�� �����Ѵ�. .mot���Ϸ� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSaveParaAll(string szFilePath);
    
    // �Ķ��Ÿ 28 - 31������ ����ڰ� ���α׷�������  �� �Լ��� �̿��� ���� �Ѵ�
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetParaLoad(int nAxisNo, double InitPos, double InitVel, double InitAccel, double InitDecel);    
    // �Ķ��Ÿ 28 - 31������ ����ڰ� ���α׷�������  �� �Լ��� �̿��� Ȯ�� �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetParaLoad(int nAxisNo, ref double InitPos, ref double InitVel, ref double InitAccel, ref double InitDecel);    

    // ���� ���� �޽� ��� ����� �����Ѵ�.
    // uMethod  0 :OneHighLowHigh, 1 :OneHighHighLow, 2 :OneLowLowHigh, 3 :OneLowHighLow, 4 :TwoCcwCwHigh
    //          5 :TwoCcwCwLow, 6 :TwoCwCcwHigh, 7 :TwoCwCcwLow, 8 :TwoPhase, 9 :TwoPhaseReverse
    // OneHighLowHigh   = 0x0      // 1�޽� ���, PULSE(Active High), ������(DIR=Low)  / ������(DIR=High)
    // OneHighHighLow   = 0x1      // 1�޽� ���, PULSE(Active High), ������(DIR=High) / ������(DIR=Low)
    // OneLowLowHigh    = 0x2      // 1�޽� ���, PULSE(Active Low),  ������(DIR=Low)  / ������(DIR=High)
    // OneLowHighLow    = 0x3      // 1�޽� ���, PULSE(Active Low),  ������(DIR=High) / ������(DIR=Low)
    // TwoCcwCwHigh     = 0x4      // 2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active High     
    // TwoCcwCwLow      = 0x5      // 2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active Low     
    // TwoCwCcwHigh     = 0x6      // 2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active High
    // TwoCwCcwLow      = 0x7      // 2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active Low
    // TwoPhase         = 0x8      // 2��(90' ������),  PULSE lead DIR(CW: ������), PULSE lag DIR(CCW:������)
    // TwoPhaseReverse  = 0x9      // 2��(90' ������),  PULSE lead DIR(CCW: ������), PULSE lag DIR(CW:������)

    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetPulseOutMethod(int nAxisNo, uint uMethod);
    // ���� ���� �޽� ��� ��� ������ ��ȯ�Ѵ�,
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetPulseOutMethod(int nAxisNo, ref uint upMethod);

    // ���� ���� �ܺ�(Actual) ī��Ʈ�� ���� ���� ������ �����Ͽ� ���� ���� Encoder �Է� ����� �����Ѵ�.
    // uMethod : 0 - 7 ����
    // ObverseUpDownMode    = 0x0      // ������ Up/Down
    // ObverseSqr1Mode      = 0x1      // ������ 1ü��
    // ObverseSqr2Mode      = 0x2      // ������ 2ü��
    // ObverseSqr4Mode      = 0x3      // ������ 4ü��
    // ReverseUpDownMode    = 0x4      // ������ Up/Down
    // ReverseSqr1Mode      = 0x5      // ������ 1ü��
    // ReverseSqr2Mode      = 0x6      // ������ 2ü��
    // ReverseSqr4Mode      = 0x7      // ������ 4ü��
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetEncInputMethod(int nAxisNo, uint uMethod);
    // ���� ���� �ܺ�(Actual) ī��Ʈ�� ���� ���� ������ �����Ͽ� ���� ���� Encoder �Է� ����� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetEncInputMethod(int nAxisNo, ref uint upMethod);

    // ���� �ӵ� ������ RPM(Revolution Per Minute)���� ���߰� �ʹٸ�.
    // ex>    rpm ���:
    // 4500 rpm ?
    // unit/ pulse = 1 : 1�̸�      pulse/ sec �ʴ� �޽����� �Ǵµ�
    // 4500 rpm�� ���߰� �ʹٸ�     4500 / 60 �� : 75ȸ��/ 1��
    // ���Ͱ� 1ȸ���� �� �޽����� �˾ƾ� �ȴ�. �̰��� Encoder�� Z���� �˻��غ��� �˼��ִ�.
    // 1ȸ��:1800 �޽���� 75 x 1800 = 135000 �޽��� �ʿ��ϰ� �ȴ�.
    // AnmMotSetMoveUnitPerPulse�� Unit = 1, Pulse = 1800 �־� ���۽�Ų��.
    // �������� : rpm���� �����ϰ� �ȴٸ� �ӵ��� ���ӵ� �� rpm������ �ٲ�� �ȴ�.

    // ���� ���� �޽� �� �����̴� �Ÿ��� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetMoveUnitPerPulse(int nAxisNo, double dUnit, int nPulse);
    // ���� ���� �޽� �� �����̴� �Ÿ��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetMoveUnitPerPulse(int nAxisNo, ref double dpUnit, ref int npPulse);
    
    // ���� �࿡ ���� ���� ����Ʈ ���� ����� �����Ѵ�.
    // uMethod : 0 -1 ����
    // AutoDetect = 0x0 : �ڵ� ������.
    // RestPulse  = 0x1 : ���� ������."
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetDecelMode(int nAxisNo, uint uMethod);
    // ���� ���� ���� ���� ����Ʈ ���� ����� ��ȯ�Ѵ�    
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetDecelMode(int nAxisNo, ref uint upMethod);
    
    // ���� �࿡ ���� ���� ��忡�� �ܷ� �޽��� �����Ѵ�.
    // �����: ���� AnmMotSetRemainPulse�� 500 �޽��� ����
    //           AnmMoveStartPos�� ��ġ 10000�� ��������쿡 9500�޽����� 
    //           ���� �޽� 500��  AnmMotSetMinVel�� ������ �ӵ��� �����ϸ鼭 ���� �ȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetRemainPulse(int nAxisNo, uint uData);
    // ���� ���� ���� ���� ��忡�� �ܷ� �޽��� ��ȯ�Ѵ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetRemainPulse(int nAxisNo, ref uint upData);

    // ���� �࿡ ��ӵ� ���� �Լ������� �ְ� �ӵ��� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetMaxVel(int nAxisNo, double dVel);
    // ���� ���� ��ӵ� ���� �Լ������� �ְ� �ӵ��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetMaxVel(int nAxisNo, ref double dpVel);

    // ���� ���� �̵� �Ÿ� ��� ��带 �����Ѵ�.
    // uAbsRelMode  : POS_ABS_MODE '0' - ���� ��ǥ��
    //                POS_REL_MODE '1' - ��� ��ǥ��
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetAbsRelMode(int nAxisNo, uint uAbsRelMode);
    // ���� ���� ������ �̵� �Ÿ� ��� ��带 ��ȯ�Ѵ�
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetAbsRelMode(int nAxisNo, ref uint upAbsRelMode);

    // ���� ���� ���� �ӵ� �������� ��带 �����Ѵ�.
    // ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - ��Ī Trapezode
    //               ASYM_TRAPEZOIDE_MODE   '1' - ���Ī Trapezode
    //               QUASI_S_CURVE_MODE     '2' - ��������
    //               SYM_S_CURVE_MODE       '3' - ��Ī S Curve
    //               ASYM_S_CURVE_MODE      '4' - ���Ī S Curve
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetProfileMode(int nAxisNo, uint uProfileMode);
    // ���� ���� ������ ���� �ӵ� �������� ��带 ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetProfileMode(int nAxisNo, ref uint upProfileMode);

    // ���� ���� ���ӵ� ������ �����Ѵ�.
    // AccelUnit : UNIT_SEC2   '0' - ������ ������ unit/sec2 ���
    //             SEC         '1' - ������ ������ sec ���
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetAccelUnit(int nAxisNo, uint uAccelUnit);
    // ���� ���� ������ ���ӵ������� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetAccelUnit(int nAxisNo, ref uint upAccelUnit);

    // ���� �࿡ �ʱ� �ӵ��� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetMinVel(int nAxisNo, double dMinVelocity);
    // ���� ���� �ʱ� �ӵ��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetMinVel(int nAxisNo, ref double dpMinVelocity);

    // ���� ���� ���� ��ũ���� �����Ѵ�.[%].
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetAccelJerk(int nAxisNo, double dAccelJerk);
    // ���� ���� ������ ���� ��ũ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetAccelJerk(int nAxisNo, ref double dpAccelJerk);

    // ���� ���� ���� ��ũ���� �����Ѵ�.[%].
    [DllImport("ANLNet.dll")] public static extern uint AnmMotSetDecelJerk(int nAxisNo, double dDecelJerk);
    // ���� ���� ������ ���� ��ũ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMotGetDecelJerk(int nAxisNo, ref double dpDecelJerk);

//=========== ����� ��ȣ ���� �����Լ� ================================================================================

    // ���� ���� Z �� Level�� �����Ѵ�.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetZphaseLevel(int nAxisNo, uint uLevel);
    // ���� ���� Z �� Level�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetZphaseLevel(int nAxisNo, ref uint upLevel);

    // ���� ���� Servo-On��ȣ�� ��� ������ �����Ѵ�.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetServoOnLevel(int nAxisNo, uint uLevel);
    // ���� ���� Servo-On��ȣ�� ��� ���� ������ ��ȯ�Ѵ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetServoOnLevel(int nAxisNo, ref uint upLevel);

    // ���� ���� Servo-Alarm Reset ��ȣ�� ��� ������ �����Ѵ�.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetServoAlarmResetLevel(int nAxisNo, uint uLevel);
    // ���� ���� Servo-Alarm Reset ��ȣ�� ��� ������ ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetServoAlarmResetLevel(int nAxisNo, ref uint upLevel);

    // ���� ���� Inpositon ��ȣ ��� ���� �� ��ȣ �Է� ������ �����Ѵ�
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)    
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetInpos(int nAxisNo, uint uUse);
    // ���� ���� Inpositon ��ȣ ��� ���� �� ��ȣ �Է� ������ ��ȯ�Ѵ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetInpos(int nAxisNo, ref uint upUse);
    // ���� ���� Inpositon ��ȣ �Է� ���¸� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadInpos(int nAxisNo, ref uint upStatus);

    // ���� ���� �˶� ��ȣ �Է� �� ��� ������ ��� ���� �� ��ȣ �Է� ������ �����Ѵ�.
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetServoAlarm(int nAxisNo, uint uUse);
    // ���� ���� �˶� ��ȣ �Է� �� ��� ������ ��� ���� �� ��ȣ �Է� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetServoAlarm(int nAxisNo, ref uint upUse);
    // ���� ���� �˶� ��ȣ�� �Է� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadServoAlarm(int nAxisNo, ref uint upStatus);
    
    // ���� ���� end limit sensor�� ��� ���� �� ��ȣ�� �Է� ������ �����Ѵ�. 
    // end limit sensor ��ȣ �Է� �� �������� �Ǵ� �������� ���� ������ �����ϴ�.
       // uStopMode: EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uPositiveLevel, uNegativeLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetLimit(int nAxisNo, uint uStopMode, uint uPositiveLevel, uint uNegativeLevel);
    // ���� ���� end limit sensor�� ��� ���� �� ��ȣ�� �Է� ����, ��ȣ �Է� �� ������带 ��ȯ�Ѵ�
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetLimit(int nAxisNo, ref uint upStopMode, ref uint upPositiveLevel, ref uint upNegativeLevel);
    // �������� end limit sensor�� �Է� ���¸� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadLimit(int nAxisNo, ref uint upPositiveStatus, ref uint upNegativeStatus);
    
    // ���� ���� Software limit�� ��� ����, ����� ī��Ʈ, �׸��� ���� ����� �����Ѵ�
    // uUse       : DISABLE(0), ENABLE(1)
    // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uSelection : COMMAND(0), ACTUAL(1)
    // ���ǻ���: �����˻��� ���Լ��� �̿��Ͽ� ����Ʈ���� ������ �̸� �����ؼ� ������ �����˻��� �����˻��� ���߿� ���߾�������쿡��  Enable�ȴ�. 
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetSoftLimit(int nAxisNo, uint uUse, uint uStopMode, uint uSelection, double dPositivePos, double dNegativePos);
    // ���� ���� Software limit�� ��� ����, ����� ī��Ʈ, �׸��� ���� ����� ��ȯ�Ѵ�
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetSoftLimit(int nAxisNo, ref uint upUse, ref uint upStopMode, ref uint upSelection, ref double dpPositivePos, ref double dpNegativePos);

    // ��� ���� ��ȣ�� ���� ��� (������/��������) �Ǵ� ��� ������ �����Ѵ�.
    // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalSetStop(int nAxisNo, uint uStopMode, uint uLevel);
    // ��� ���� ��ȣ�� ���� ��� (������/��������) �Ǵ� ��� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalGetStop(int nAxisNo, ref uint upStopMode, ref uint upLevel);
    // ��� ���� ��ȣ�� �Է� ���¸� ��ȯ�Ѵ�.    
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadStop(int nAxisNo, ref uint upStatus);
    
    // ���� ���� Servo-On ��ȣ�� ����Ѵ�.
    // uOnOff : FALSE(0), TRUE(1) ( ���� 0��¿� �ش��)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalServoOn(int nAxisNo, uint uOnOff);
    // ���� ���� Servo-On ��ȣ�� ��� ���¸� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalIsServoOn(int nAxisNo, ref uint upOnOff);

    // ���� ���� Servo-Alarm Reset ��ȣ�� ����Ѵ�.
    // uOnOff : FALSE(0), TRUE(1) ( ���� 1��¿� �ش��)
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalServoAlarmReset(int nAxisNo, uint uOnOff);
    
    // ���� ��°��� �����Ѵ�.
    // uValue : Hex Value 0x00
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalWriteOutput(int nAxisNo, uint uValue);
    // ���� ��°��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadOutput(int nAxisNo, ref uint upValue);
    
    // lBitNo : Bit Number(0 - 4)
       // uOnOff : FALSE(0), TRUE(1)
    // ���� ��°��� ��Ʈ���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalWriteOutputBit(int nAxisNo, int nBitNo, uint uOn);
    // ���� ��°��� ��Ʈ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadOutputBit(int nAxisNo, int nBitNo, ref uint upOn);

    // ���� �Է°��� Hex������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadInput(int nAxisNo, ref uint upValue);
    
    // lBitNo : Bit Number(0 - 4)
    // ���� �Է°��� ��Ʈ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmSignalReadInputBit(int nAxisNo, int nBitNo, ref uint upOn);

//========== ��� ������ �� �����Ŀ� ���� Ȯ���ϴ� �Լ�============================================================

    // ���� ���� �޽� ��� ���¸� ��ȯ�Ѵ�.
    // (��������)"
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadInMotion(int nAxisNo, ref uint upStatus);

    // �������� ���� ���� ���� ���� �޽� ī���� ���� ��ȯ�Ѵ�.
    // ���ǻ���: �����߿��� ī���Ͱ��� ǥ���ϰ� ���������Ŀ��� ī���Ͱ��� CLEAR�ȴ�.    
    //  (�޽� ī��Ʈ ��)"
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadDrivePulseCount(int nAxisNo, ref int npPulse);
    
    // DriveStatus �������͸� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadMotion(int nAxisNo, ref uint upStatus);
    
    // EndStatus �������͸� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadStop(int nAxisNo, ref uint upStatus);
    
    // ���� ���� Mechanical Signal Data(���� ������� ��ȣ����) �� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadMechanical(int nAxisNo, ref uint upStatus);
    
    // ���� ���� ���� ���� �ӵ��� �о�´�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadVel(int nAxisNo, ref double dpVelocity);
    
    // Command Pos�� Actual Pos�� ���� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadPosError(int nAxisNo, ref double dpError);
    
    // ���� ����̺��� �̵� �Ÿ��� Ȯ��
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusReadDriveDistance(int nAxisNo, ref double dpUnit);

    // ���� ���� Actual ��ġ�� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusSetActPos(int nAxisNo, double dPos);
    // ���� ���� Actual ��ġ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusGetActPos(int nAxisNo, ref double dpPos);

    // ���� ���� Command ��ġ�� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusSetCmdPos(int nAxisNo, double dPos);
    // ���� ���� Command ��ġ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmStatusGetCmdPos(int nAxisNo, ref double dpPos);

//======== Ȩ���� �Լ�=============================================================================================================================================================================================    
    
    // ���� ���� Home ���� Level �� �����Ѵ�.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeSetSignalLevel(int nAxisNo, uint uLevel);
    // ���� ���� Home ���� Level �� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeGetSignalLevel(int nAxisNo, ref uint upLevel);
    // ���� Ȩ ��ȣ �Է»��¸� Ȯ���Ѵ�. Ȩ��ȣ�� ����ڰ� ���Ƿ� AnmHomeSetMethod �Լ��� �̿��Ͽ� �����Ҽ��ִ�.
    // upStatus : OFF(0), ON(1)
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeReadSignal(int nAxisNo, ref uint upStatus);
    
    // �ش� ���� �����˻��� �����ϱ� ���ؼ��� �ݵ�� ���� �˻����� �Ķ��Ÿ���� �����Ǿ� �־�� �˴ϴ�. 
    // ���� MotionPara���� ������ �̿��� �ʱ�ȭ�� ���������� ����ƴٸ� ������ ������ �ʿ����� �ʴ�. 
    // �����˻� ��� �������� �˻� �������, �������� ����� ��ȣ, �������� Active Level, ���ڴ� Z�� ���� ���� ���� ���� �Ѵ�.
    // (�ڼ��� ������ AnmMotSaveParaAll ���� �κ� ����)
    // Ȩ������ AnmSignalSetHomeLevel ����Ѵ�.
    // HClrTim : HomeClear Time : ���� �˻� Encoder �� Set�ϱ� ���� ���ð� 
    // HmDir(Ȩ ����): DIR_CCW (0) -���� , DIR_CW(1) +����
    // HOffset - ���������� �̵��Ÿ�.
    // uZphas: 1�� �����˻� �Ϸ� �� ���ڴ� Z�� ���� ���� ����  0: ������ , 1: +����, 2: -���� 
    // HmSig : PosEndLimit(0) -> +Limit
    //         NegEndLimit(1) -> -Limit
    //         HomeSensor (4) -> ��������(���� �Է� 0)

    [DllImport("ANLNet.dll")] public static extern uint AnmHomeSetMethod(int nAxisNo,int nHmDir, uint uHomeSignal, uint uZphas, double dHomeClrTime, double dHomeOffset);
    // �����Ǿ��ִ� Ȩ ���� �Ķ��Ÿ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeGetMethod(int nAxisNo,ref int nHmDir, ref uint uHomeSignal, ref uint uZphas, ref double dHomeClrTime, ref double dHomeOffset);

    // ������ ������ �����ϰ� �˻��ϱ� ���� ���� �ܰ��� �������� �����Ѵ�. �̶� �� ���ǿ� ��� �� �ӵ��� �����Ѵ�. 
    // �� �ӵ����� �������� ���� �����˻� �ð���, �����˻� ���е��� �����ȴ�. 
    // �� ���Ǻ� �ӵ����� ������ �ٲ㰡�鼭 �� ���� �����˻� �ӵ��� �����ϸ� �ȴ�. 
    // (�ڼ��� ������ AnmMotSaveParaAll ���� �κ� ����)
    // �����˻��� ���� �ӵ��� �����ϴ� �Լ�
    // [dVelFirst]- 1�������ӵ�   [dVelSecond]-�����ļӵ�   [dVelThird]- ������ �ӵ�  [dvelLast]- index�˻��� �����ϰ� �˻��ϱ�����. 
    // [dAccFirst]- 1���������ӵ� [dAccSecond]-�����İ��ӵ� 
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeSetVel(int nAxisNo,double dVelFirst, double dVelSecond, double dVelThird, double dvelLast, double dAccFirst, double dAccSecond);
    // �����Ǿ��ִ� �����˻��� ���� �ӵ��� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeGetVel(int nAxisNo, ref double dVelFirst, ref double dVelSecond, ref double dVelThird, ref double dvelLast, ref double dAccFirst, ref double dAccSecond);

    // �����˻��� �����Ѵ�.
    // �����˻� �����Լ��� �����ϸ� ���̺귯�� ���ο��� �ش����� �����˻��� ���� �� �����尡 �ڵ� �����Ǿ� �����˻��� ���������� ������ �� �ڵ� ����ȴ�.
    // ���ǻ��� : �������� �ݴ������ ����Ʈ ������ ���͵� ��������� ������ ACTIVE���������� �����Ѵ�.
    //            ���� �˻��� ���۵Ǿ� ��������� ����Ʈ ������ ������ ����Ʈ ������ �����Ǿ��ٰ� �����ϰ� �����ܰ�� ����ȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeSetStart(int nAxisNo);

    // �����˻� ����� ����ڰ� ���Ƿ� �����Ѵ�.
    // �����˻� �Լ��� �̿��� ���������� �����˻��� ����ǰ��� �˻� ����� HOME_SUCCESS�� �����˴ϴ�.
    // �� �Լ��� ����ڰ� �����˻��� ���������ʰ� ����� ���Ƿ� ������ �� �ִ�. 
    // uHomeResult ����
    // HOME_SUCCESS              = 0x01      // Ȩ �Ϸ�
    // HOME_SEARCHING            = 0x02      // Ȩ�˻���
    // HOME_ERR_GNT_RANGE        = 0x10      // Ȩ �˻� ������ ��������
    // HOME_ERR_USER_BREAK       = 0x11      // �ӵ� ������ ���Ƿ� ��������� ���������
    // HOME_ERR_VELOCITY         = 0x12      // �ӵ� ���� �߸��������
    // HOME_ERR_AMP_FAULT        = 0x13      // ������ �˶� �߻� ����
    // HOME_ERR_NEG_LIMIT        = 0x14      // (-)���� ������ (+)����Ʈ ���� ���� ����
    // HOME_ERR_POS_LIMIT        = 0x15      // (+)���� ������ (-)����Ʈ ���� ���� ����
    // HOME_ERR_NOT_DETECT       = 0x16      // ������ ��ȣ �������� �� �� ��� ����
    // HOME_ERR_UNKNOWN          = 0xFF    
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeSetResult(int nAxisNo, uint uHomeResult);
    // �����˻� ����� ��ȯ�Ѵ�.
    // �����˻� �Լ��� �˻� ����� Ȯ���Ѵ�. �����˻��� ���۵Ǹ� HOME_SEARCHING���� �����Ǹ� �����˻��� �����ϸ� ���п����� �����ȴ�. ���� ������ ������ �� �ٽ� �����˻��� �����ϸ� �ȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeGetResult(int nAxisNo, ref uint upHomeResult);
    // �����˻� ������� ��ȯ�Ѵ�.
    // �����˻� ���۵Ǹ� �������� Ȯ���� �� �ִ�. �����˻��� �Ϸ�Ǹ� �������ο� ������� 100�� ��ȯ�ϰ� �ȴ�. �����˻� �������δ� GetHome Result�Լ��� �̿��� Ȯ���� �� �ִ�.
    // upHomeMainStepNumber : Main Step �������̴�. 
    // ��Ʈ�� FALSE�� ���upHomeMainStepNumber : 0 �϶��� ������ �ุ ��������̰� Ȩ �������� upHomeStepNumber ǥ���Ѵ�.
    // ��Ʈ�� TRUE�� ��� upHomeMainStepNumber : 0 �϶��� ������ Ȩ�� ��������̰� ������ Ȩ �������� upHomeStepNumber ǥ���Ѵ�.
    // ��Ʈ�� TRUE�� ��� upHomeMainStepNumber : 10 �϶��� �����̺� Ȩ�� ��������̰� ������ Ȩ �������� upHomeStepNumber ǥ���Ѵ�.
    // upHomeStepNumber     : ������ �࿡���� �������� ǥ���Ѵ�. 
    // ��Ʈ�� FALSE�� ���  : ������ �ุ �������� ǥ���Ѵ�.
    // ��Ʈ�� TRUE�� ��� ��������, �����̺��� ������ �������� ǥ�õȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmHomeGetRate(int nAxisNo, ref uint upHomeMainStepNumber, ref uint upHomeStepNumber);

//========= ��ġ �����Լ� ===============================================================================================================
    
    // ���ǻ���: ��ġ�� �����Ұ�� �ݵ�� UNIT/PULSE�� ���߾ �����Ѵ�.
    //           ��ġ�� UNIT/PULSE ���� �۰��� ��� �ּҴ����� UNIT/PULSE�� ���߾����⶧���� ����ġ���� ������ �ɼ�����.
    // ���� �ӵ� ������ RPM(Revolution Per Minute)���� ���߰� �ʹٸ�.
    // ex>    rpm ���:
    // 4500 rpm ?
    // unit/ pulse = 1 : 1�̸�      pulse/ sec �ʴ� �޽����� �Ǵµ�
    // 4500 rpm�� ���߰� �ʹٸ�     4500 / 60 �� : 75ȸ��/ 1��
    // ���Ͱ� 1ȸ���� �� �޽����� �˾ƾ� �ȴ�. �̰��� Encoder�� Z���� �˻��غ��� �˼��ִ�.
    // 1ȸ��:1800 �޽���� 75 x 1800 = 135000 �޽��� �ʿ��ϰ� �ȴ�.
    // AnmMotSetMoveUnitPerPulse�� Unit = 1, Pulse = 1800 �־� ���۽�Ų��. 

    // ������ �Ÿ���ŭ �Ǵ� ��ġ���� �̵��Ѵ�.
    // ���� ���� ���� ��ǥ/ �����ǥ �� ������ ��ġ���� ������ �ӵ��� �������� ������ �Ѵ�.
    // �ӵ� ���������� AnmMotSetProfileMode �Լ����� �����Ѵ�.
    // �޽��� ��µǴ� �������� �Լ��� �����.
    // Vel���� ����̸� CW, �����̸� CCW �������� ����.
    // AnmMotSetAccelUnit(lAxisNo, 1) �ϰ�� dAccel -> dAccelTime , dDecel -> dDecelTime ���� �ٲ��.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveStartPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

    // ������ �Ÿ���ŭ �Ǵ� ��ġ���� �̵��Ѵ�.
    // ���� ���� ���� ��ǥ/�����ǥ�� ������ ��ġ���� ������ �ӵ��� �������� ������ �Ѵ�.
    // �ӵ� ���������� AnmMotSetProfileMode �Լ����� �����Ѵ�. 
    // �޽� ����� ����Ǵ� �������� �Լ��� �����
    // Vel���� ����̸� CW, �����̸� CCW �������� ����.
    [DllImport("ANLNet.dll")] public static extern uint AnmMovePos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

    // ������ �ӵ��� �����Ѵ�.
    // ���� �࿡ ���Ͽ� ������ �ӵ��� �������� ���������� �ӵ� ��� ������ �Ѵ�. 
    // �޽� ����� ���۵Ǵ� �������� �Լ��� �����.
    // Vel���� ����̸� CW, �����̸� CCW �������� ����.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveVel(int nAxisNo, double dVel, double dAccel, double dDecel);

    // ������ ���࿡ ���Ͽ� ������ �ӵ��� �������� ���������� �ӵ� ��� ������ �Ѵ�.
    // �޽� ����� ���۵Ǵ� �������� �Լ��� �����.
    // Vel���� ����̸� CW, �����̸� CCW �������� ����.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveStartMultiVel(int lArraySize, int[] lpAxesNo, double[] dVel, double[] dAccel, double[] dDecel);

    // Ư�� Input ��ȣ�� Edge�� �����Ͽ� ������ �Ǵ� ���������ϴ� �Լ�.
    // lDetect Signal : edge ������ �Է� ��ȣ ����.
    // lDetectSignal  : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // Signal Edge    : ������ �Է� ��ȣ�� edge ���� ���� (rising or falling edge).
    //                    SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // ��������       : Vel���� ����̸� CW, �����̸� CCW.
    // SignalMethod   : ������ EMERGENCY_STOP(0), �������� SLOWDOWN_STOP(1)
    // ���ǻ��� : SignalMethod�� EMERGENCY_STOP(0)�� ����Ұ�� �������� ���õǸ� ������ �ӵ��� ���� �������ϰԵȴ�.
    //            PCI-Nx04�� ����� ��� lDetectSignal�� PosEndLimit , NegEndLimit(0,1) �� ã����� ��ȣ�Ƿ��� Active ���¸� �����ϰԵȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveSignalSearch(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nSignalMethod);
    
    // ���� �࿡�� ������ ��ȣ�� �����ϰ� �� ��ġ�� �����ϱ� ���� �̵��ϴ� �Լ��̴�.
    // ���ϴ� ��ȣ�� ��� ã�� �����̴� �Լ� ã�� ��� �� ��ġ�� ������ѳ��� AnmGetCapturePos����Ͽ� �װ��� �д´�.
    // Signal Edge   : ������ �Է� ��ȣ�� edge ���� ���� (rising or falling edge).
    //                 SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // ��������      : Vel���� ����̸� CW, �����̸� CCW.
    // SignalMethod  : ������ EMERGENCY_STOP(0), �������� SLOWDOWN_STOP(1)
    // lDetect Signal: edge ������ �Է� ��ȣ ����.SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // lDetectSignal : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // lTarget       : COMMAND(0), ACTUAL(1)
    // ���ǻ���: SignalMethod�� EMERGENCY_STOP(0)�� ����Ұ�� �������� ���õǸ� ������ �ӵ��� ���� �������ϰԵȴ�.
    //           lDetectSignal�� PosEndLimit , NegEndLimit(0,1) �� ã����� ��ȣ�Ƿ��� Active ���¸� �����ϰԵȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveSignalCapture(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nTarget, int nSignalMethod);
    
    // 'AnmMoveSignalCapture' �Լ����� ����� ��ġ���� Ȯ���ϴ� �Լ��̴�.
    // ���ǻ���: �Լ� ���� ����� "AXT_RT_SUCCESS"�϶� ����� ��ġ�� ��ȿ�ϸ�, �� �Լ��� �ѹ� �����ϸ� ���� ��ġ���� �ʱ�ȭ�ȴ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveGetCapturePos(int nAxisNo, ref double dpCapPos);

    // "������ �Ÿ���ŭ �Ǵ� ��ġ���� �̵��ϴ� �Լ�.
    // �Լ��� �����ϸ� �ش� Motion ������ ������ �� Motion �� �Ϸ�ɶ����� ��ٸ��� �ʰ� �ٷ� �Լ��� ����������."
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveStartMultiPos(int nArraySize, int[] nAxisNo, double[] dPos, double[] dVel, double[] dAccel, double[] dDecel);
    
    // ������ ������ �Ÿ���ŭ �Ǵ� ��ġ���� �̵��Ѵ�.
    // ���� ����� ���� ��ǥ�� ������ ��ġ���� ������ �ӵ��� �������� ������ �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveMultiPos(int nArraySize, int[] nAxisNo, double[] dPos, double[] dVel, double[] dAccel, double[] dDecel);
    
    // ���� ���� ������ ���ӵ��� ���� ���� �Ѵ�.
    // dDecel : ���� �� ��������
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveStop(int nAxisNo, double dDecel);
    // ���� ���� �� ���� �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveEStop(int nAxisNo);
    // ���� ���� ���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMoveSStop(int nAxisNo);

//========= �������̵� �Լ� ============================================================================

    // ��ġ �������̵� �Ѵ�.
    // ���� ���� ������ ����Ǳ� �� ������ ��� �޽� ���� �����Ѵ�.
    // PCI-Nx04 �������ǻ���: �������̵��� ��ġ�� �������� ���� ������ ��ġ�� ���������� Relative ������ ��ġ������ �־��ش�.
    //                          ���������� ���������� ��� �������̵带 ����Ҽ������� �ݴ�������� �������̵��Ұ�쿡�� �������̵带 ����Ҽ�����.
    [DllImport("ANLNet.dll")] public static extern uint AnmOverridePos(int nAxisNo, double dOverridePos);

    // ���� ���� �ӵ��������̵� �ϱ����� �������̵��� �ְ�ӵ��� �����Ѵ�.
       // ������ : �ӵ��������̵带 5���Ѵٸ� ���߿� �ְ� �ӵ��� �����ؾߵȴ�. 
    [DllImport("ANLNet.dll")] public static extern uint AnmOverrideSetMaxVel(int nAxisNo, double dOverrideMaxVel);

    // �ӵ� �������̵� �Ѵ�.
    // ���� ���� ���� �߿� �ӵ��� ���� �����Ѵ�. (�ݵ�� ��� �߿� ���� �����Ѵ�.)
    // ������: AnmOverrideVel �Լ��� ����ϱ�����. AnmOverrideMaxVel �ְ�� �����Ҽ��ִ� �ӵ��� �����س��´�.
    // EX> �ӵ��������̵带 �ι��Ѵٸ� 
    // 1. �ΰ��߿� ���� �ӵ��� AnmOverrideMaxVel ���� �ְ� �ӵ��� ����.
    // 2. AnmMoveStartPos ���� ���� ���� ���� ��(Move�Լ� ��� ����)�� �ӵ��� ù��° �ӵ��� AnmOverrideVel ���� �����Ѵ�.
    // 3. ���� ���� ���� ��(Move�Լ� ��� ����)�� �ӵ��� �ι�° �ӵ��� AnmOverrideVel ���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmOverrideVel(int nAxisNo, double dOverrideVelocity);

    // ���ӵ�, �ӵ�, ���ӵ���  �������̵� �Ѵ�.
    // ���� ���� ���� �߿� ���ӵ�, �ӵ�, ���ӵ��� ���� �����Ѵ�. (�ݵ�� ��� �߿� ���� �����Ѵ�.)
    // ������: AnmOverrideAccelVelDecel �Լ��� ����ϱ�����. AnmOverrideMaxVel �ְ�� �����Ҽ��ִ� �ӵ��� �����س��´�.
    // EX> �ӵ��������̵带 �ι��Ѵٸ� 
    // 1. �ΰ��߿� ���� �ӵ��� AnmOverrideMaxVel ���� �ְ� �ӵ��� ����.
    // 2. AnmMoveStartPos ���� ���� ���� ���� ��(Move�Լ� ��� ����)�� ���ӵ�, �ӵ�, ���ӵ��� ù��° �ӵ��� AnmOverrideAccelVelDecel ���� �����Ѵ�.
    // 3. ���� ���� ���� ��(Move�Լ� ��� ����)�� ���ӵ�, �ӵ�, ���ӵ��� �ι�° �ӵ��� AnmOverrideAccelVelDecel ���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmOverrideAccelVelDecel(int nAxisNo, double dOverrideVelocity, double dMaxAccel, double dMaxDecel);

    // ��� �������� �ӵ� �������̵� �Ѵ�.
    // ��� ��ġ ������ �������̵��� �ӵ��� �Է½��� ����ġ���� �ӵ��������̵� �Ǵ� �Լ�
    // lTarget : COMMAND(0), ACTUAL(1)
    // ������: AnmOverrideVelAtPos �Լ��� ����ϱ�����. AnmOverrideMaxVel �ְ�� �����Ҽ��ִ� �ӵ��� �����س��´�.
    [DllImport("ANLNet.dll")] public static extern uint AnmOverrideVelAtPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dOverridePos, double dOverrideVelocity, int nTarget);
    
//========= ������, �����̺�  ����� ���� �Լ� ===========================================================================

    // Electric Gear ��忡�� Master ��� Slave ����� ���� �����Ѵ�.
    // dSlaveRatio : �������࿡ ���� �����̺��� ����( 0 : 0% , 0.5 : 50%, 1 : 100%)
    [DllImport("ANLNet.dll")] public static extern uint AnmLinkSetMode(int nMasterAxisNo, int nSlaveAxisNo, double dSlaveRatio);
    // Electric Gear ��忡�� ������ Master ��� Slave ����� ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmLinkGetMode(int nMasterAxisNo, ref uint nSlaveAxisNo, ref double dpGearRatio);
    // Master ��� Slave�ణ�� ���ڱ��� ���� ���� �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmLinkResetMode(int nMasterAxisNo);

//======== ��Ʈ�� ���� �Լ�===========================================================================================================================================================

    // ��Ǹ���� �� ���� �ⱸ������ Link�Ǿ��ִ� ��Ʈ�� �����ý��� ��� �����Ѵ�. 
    // �� �Լ��� �̿��� Master���� ��Ʈ�� ����� �����ϸ� �ش� Slave���� Master��� ����Ǿ� �����˴ϴ�. 
    // ���� ��Ʈ�� ���� ���� Slave�࿡ ��������̳� ���� ��ɵ��� ������ ��� ���õ˴ϴ�.
    // uSlHomeUse     : �������� Ȩ��� ��� (0 - 2)
    //             (0 : �����̺��� Ȩ�� �����ϰ� ���������� Ȩ�� ã�´�.)
    //             (1 : �������� , �����̺��� Ȩ�� ã�´�. �����̺� dSlOffset �� �����ؼ� ������.)
    //             (2 : �������� , �����̺��� Ȩ�� ã�´�. �����̺� dSlOffset �� �����ؼ� ��������.)
    // dSlOffset      : �����̺��� �ɼ°�
    // dSlOffsetRange : �����̺��� �ɼ°� ������ ����
    // PCI-Nx04 �������ǻ���: ��Ʈ�� ENABLE�� �����̺����� ����� AnmStatusReadMotion �Լ��� Ȯ���ϸ� True(Motion ���� ��)�� Ȯ�εǾ� �������̴�. 
    //                   �����̺��࿡ AnmStatusReadMotion�� Ȯ�������� InMotion �� False�̸� Gantry Enable�� �ȵȰ��̹Ƿ� �˶� Ȥ�� ����Ʈ ���� ���� Ȯ���Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmGantrySetEnable(int nMasterAxisNo, int nSlaveAxisNo, uint uSlHomeUse, double dSlOffset, double dSlOffsetRange);

    // Slave���� Offset���� �˾Ƴ��¹��.
    // A. ������, �����̺긦 �ΰ��� �������� ��Ų��.         
    // B. AnmGantrySetEnable�Լ����� uSlHomeUse = 2�� ������ AnmHomeSetStart�Լ��� �̿��ؼ� Ȩ�� ã�´�. 
    // C. Ȩ�� ã�� ���� ���������� Command���� �о�� ��������� �����̺����� Ʋ���� Offset���� �����ִ�.
    // D. Offset���� �о AnmGantrySetEnable�Լ��� dSlOffset���ڿ� �־��ش�. 
    // E. dSlOffset���� �־��ٶ� �������࿡ ���� �����̺� �� ���̱⶧���� ��ȣ�� �ݴ�� -dSlOffset �־��ش�.
    // F. dSIOffsetRange �� Slave Offset�� Range ������ ���ϴµ� Range�� �Ѱ踦 �����Ͽ� �Ѱ踦 ����� ������ �߻���ų�� ����Ѵ�.        
    // G. AnmGantrySetEnable�Լ��� Offset���� �־�������  AnmGantrySetEnable�Լ����� uSlHomeUse = 1�� ������ AnmHomeSetStart�Լ��� �̿��ؼ� Ȩ�� ã�´�.         

    // ��Ʈ�� ������ �־� ����ڰ� ������ �Ķ��Ÿ�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmGantryGetEnable(int nMasterAxisNo, ref uint upSlHomeUse, ref double dpSlOffset, ref double dSlORange, ref uint uGatryOn);

    // ��� ����� �� ���� �ⱸ������ Link�Ǿ��ִ� ��Ʈ�� �����ý��� ��� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmGantrySetDisable(int nMasterAxisNo, int nSlaveAxisNo);

//====���� ���� �Լ� ============================================================================================================================================;

    // ������ ��ǥ�迡 ���Ӻ��� �� ������ �����Ѵ�.
    // (����� ��ȣ�� 0 ���� ����))
    // ������: ������Ҷ��� �ݵ�� ���� ���ȣ�� ���� ���ں��� ū���ڸ� �ִ´�.
    //         ������ ���� �Լ��� ����Ͽ��� �� �������ȣ�� ���� ���ȣ�� ���� �� ���� lpAxesNo�� ���� ���ؽ��� �Է��Ͽ��� �Ѵ�.
    //         ������ ���� �Լ��� ����Ͽ��� �� �������ȣ�� �ش��ϴ� ���� ���ȣ�� �ٸ� ���̶�� �Ѵ�.
    //         ���� ���� �ٸ� Coordinate�� �ߺ� �������� ���ƾ� �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmContiSetAxisMap(int lCoord, uint lSize, int[] lpRealAxesNo);
    //������ ��ǥ�迡 ���Ӻ��� �� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmContiGetAxisMap(int lCoord, ref uint lSize, ref int lpRealAxesNo);
    
    // ������ ��ǥ�迡 ���Ӻ��� �� ����/��� ��带 �����Ѵ�.
    // (������ : �ݵ�� ����� �ϰ� ��밡��)
    // ���� ���� �̵� �Ÿ� ��� ��带 �����Ѵ�.
    // uAbsRelMode : POS_ABS_MODE '0' - ���� ��ǥ��
    //               POS_REL_MODE '1' - ��� ��ǥ��
    [DllImport("ANLNet.dll")] public static extern uint AnmContiSetAbsRelMode(int lCoord, uint uAbsRelMode);
    // ������ ��ǥ�迡 ���Ӻ��� �� ����/��� ��带 ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmContiGetAbsRelMode(int lCoord, ref uint upAbsRelMode);

    // ������ ��ǥ�迡 ���� ���� ������ Ȯ���ϴ� �Լ��̴�.
    [DllImport("ANLNet.dll")] public static extern uint AnmContiIsMotion(int lCoord, ref uint upInMotion);

//====�Ϲ� �����Լ� ============================================================================================================================================;

    // ���ǻ���1: AnmContiSetAxisMap�Լ��� �̿��Ͽ� ������Ŀ� ������������� ������ �ϸ鼭 ����ؾߵȴ�.
    //           ��ȣ������ ��쿡�� �ݵ�� ������������� ��迭�� �־�� ���� �����ϴ�.

    // ���ǻ���2: ��ġ�� �����Ұ�� �ݵ�� ��������� �����̺� ���� UNIT/PULSE�� ���߾ �����Ѵ�.
    //           ��ġ�� UNIT/PULSE ���� �۰� ������ ��� �ּҴ����� UNIT/PULSE�� ���߾����⶧���� ����ġ���� ������ �ɼ�����.

    // ���ǻ���3: ��ȣ ������ �Ұ�� �ݵ�� ��Ĩ������ ������ �ɼ������Ƿ� 

    // ���ǻ���4: ���� ���� ����/�߿� ������ ���� ����(+- Limit��ȣ, ���� �˶�, ������� ��)�� �߻��ϸ� 
    //            ���� ���⿡ ������� ������ �������� �ʰų� ���� �ȴ�.

    // ���� ���� �Ѵ�.
    // �������� �������� �����Ͽ� ���� ���� ���� �����ϴ� �Լ��̴�. ���� ���� �� �Լ��� �����.
    // AnmContiBeginNode, AnmContiEndNode�� ���̻��� ������ ��ǥ�迡 �������� �������� �����Ͽ� ���� ���� �����ϴ� Queue�� �����Լ����ȴ�. 
    // ���� �������� ���� ���� ������ ���� ���� Queue�� �����Ͽ� AnmContiStart�Լ��� ����ؼ� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmLineMove(int lCoord, double[] dPos, double dVel, double dAccel, double dDecel);

    // 2�� ��ȣ���� �Ѵ�.
    // ������, �������� �߽����� �����Ͽ� ��ȣ ���� �����ϴ� �Լ��̴�. ���� ���� �� �Լ��� �����.
    // AnmContiBeginNode, AnmContiEndNode, �� ���̻��� ������ ��ǥ�迡 ������, �������� �߽����� �����Ͽ� �����ϴ� ��ȣ ���� Queue�� �����Լ����ȴ�.
    // �������� ��ȣ ���� ���� ������ ���� ���� Queue�� �����Ͽ� AnmContiStart�Լ��� ����ؼ� �����Ѵ�.
    // dCenterPos = �߽��� X,Y  , dEndPos = ������ X,Y .
    // uCWDir   DIR_CCW(0): �ݽð����, DIR_CW(1) �ð����
    [DllImport("ANLNet.dll")] public static extern uint AnmCircleCenterMove(int lCoord, int[] lAxisNo, double[] dCenterPos, double[] dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir);

    // �߰���, �������� �����Ͽ� ��ȣ ���� �����ϴ� �Լ��̴�. ���� ���� �� �Լ��� �����.
    // AnmContiBeginNode, AnmContiEndNode�� ���̻��� ������ ��ǥ�迡 �߰���, �������� �����Ͽ� �����ϴ� ��ȣ ���� Queue�� �����Լ����ȴ�.
    // �������� ��ȣ ���� ���� ������ ���� ���� Queue�� �����Ͽ� AnmContiStart�Լ��� ����ؼ� �����Ѵ�.
    // dMidPos = �߰��� X,Y  , dEndPos = ������ X,Y 
    // uCWDir   DIR_CCW(0): �ݽð����, DIR_CW(1) �ð����
    [DllImport("ANLNet.dll")] public static extern uint AnmCirclePointMove(int lCoord, int[] lAxisNo, double[] dMidPos, double[] dEndPos, double dVel, double dAccel, double dDecel, int lArcCircle);

    // ������, �������� �������� �����Ͽ� ��ȣ ���� �����ϴ� �Լ��̴�. ���� ���� �� �Լ��� �����.
    // AnmContiBeginNode, AnmContiEndNode�� ���̻��� ������ ��ǥ�迡 ������, �������� �������� �����Ͽ� ��ȣ ���� �����ϴ� Queue�� �����Լ����ȴ�.
    // �������� ��ȣ ���� ���� ������ ���� ���� Queue�� �����Ͽ� AnmContiStart�Լ��� ����ؼ� �����Ѵ�.
    // lAxisNo = ���� �迭 , dRadius = ������, dEndPos = ������ X,Y �迭 , uShortDistance = ������(0), ū��(1)
    // uCWDir   DIR_CCW(0): �ݽð����, DIR_CW(1) �ð����
    [DllImport("ANLNet.dll")] public static extern uint AnmCircleRadiusMove(int lCoord, int[] lAxisNo, double dRadius, double[] dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);

    // ������, ȸ�������� �������� �����Ͽ� ��ȣ ���� �����ϴ� �Լ��̴�. ���� ���� �� �Լ��� �����.
    // AnmContiBeginNode, AnmContiEndNode�� ���̻��� ������ ��ǥ�迡 ������, ȸ�������� �������� �����Ͽ� ��ȣ ���� �����ϴ� Queue�� �����Լ����ȴ�.
    // �������� ��ȣ ���� ���� ������ ���� ���� Queue�� �����Ͽ� AnmContiStart�Լ��� ����ؼ� �����Ѵ�.
    // dCenterPos = �߽��� X,Y  , dAngle = ����.
    // uCWDir   DIR_CCW(0): �ݽð����, DIR_CW(1) �ð����
    [DllImport("ANLNet.dll")] public static extern uint AnmCircleAngleMove(int lCoord, int[] lAxisNo, double[] dCenterPos, double dAngle, double dVel, double dAccel, double dDecel, uint uCWDir);
    
//====================Ʈ���� �Լ� ===============================================================================================================================

    // ���ǻ���: Ʈ���� ��ġ�� �����Ұ�� �ݵ�� UNIT/PULSE�� ���߾ �����Ѵ�.
    //           ��ġ�� UNIT/PULSE ���� �۰��� ��� �ּҴ����� UNIT/PULSE�� ���߾����⶧���� ����ġ�� ����Ҽ�����.

    // ���� �࿡ Ʈ���� ����� ��� ����, ��� ����, ��ġ �񱳱�, Ʈ���� ��ȣ ���� �ð� �� Ʈ���� ��� ��带 �����Ѵ�.
    // Ʈ���� ��� ����� ���ؼ��� ����  AnmTriggerSetTimeLevel �� ����Ͽ� ���� ��� ������ ���� �Ͽ��� �Ѵ�.
    // dTrigTime        : Ʈ���� ��� �ð� 
    //                    1usec - �ִ� 50msec ( 1 - 50000 ���� ����)
    // upTriggerLevel   : Ʈ���� ��� ���� ����   => LOW(0), HIGH(1)
    // uSelect          : ����� ���� ��ġ        => COMMAND(0), ACTUAL(1)
    // uInterrupt       : ���ͷ�Ʈ ����           => DISABLE(0), ENABLE(1)

    // ���� �࿡ Ʈ���� ��ȣ ���� �ð� �� Ʈ���� ��� ����, Ʈ���� ��¹���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerSetTimeLevel(int lAxisNo, double dTrigTime, uint uTriggerLevel, uint uSelect, uint uInterrupt);
    // ���� �࿡ Ʈ���� ��ȣ ���� �ð� �� Ʈ���� ��� ����, Ʈ���� ��¹���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerGetTimeLevel(int lAxisNo, ref double dTrigTime, ref uint uTriggerLevel, ref uint uSelect, ref uint uInterrupt);

    // ���� ���� Ʈ���� ��� ����� �����Ѵ�.
    // uMethod : PERIOD_MODE      0x0 : ���� ��ġ�� �������� dPos�� ��ġ �ֱ�� ����� �ֱ� Ʈ���� ���
    //           ABS_POS_MODE     0x1 : Ʈ���� ���� ��ġ���� Ʈ���� �߻�, ���� ��ġ ���

    // dPos    : �ֱ� ���ý� : ��ġ������ġ���� ����ϱ⶧���� �� ��ġ
    //           ���� ���ý� : ����� �� ��ġ, �� ��ġ�Ͱ����� ������ ����� ������. 
    // ���ǻ���: �ֱ��� Ʈ���� ���� AnmTriggerSetAbsPeriod ���� ���� ���� ��ġ�� Ʈ���� ���� �ȿ� ������ Ʈ���� ����� �ѹ� �߻��Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerSetAbsPeriod(int nAxisNo, uint uMethod, double dPos);
    
    // ���� �࿡ Ʈ���� ����� ��� ����, ��� ����, ��ġ �񱳱�, Ʈ���� ��ȣ ���� �ð� �� Ʈ���� ��� ��带 ��ȯ�Ѵ�.
    // ���ǻ���: IP������ AnmTriiggerSetBlock�Լ��� ȣ��� ���ζ��̺귯������ �������� ABS_POS_MODE�� ����ϱ� ������ 
    // ���Լ��� ��ȯ�ϴ°��� 1�� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerGetAbsPeriod(int nAxisNo, ref uint upMethod, ref double dpPos);

    //  ����ڰ� ������ ������ġ���� ������ġ���� ������������ Ʈ���Ÿ� ��� �Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerSetBlock(int nAxisNo, double dStartPos, double dEndPos, double dPeriodPos);
    // 'AnmTriggerSetBlock' �Լ��� Ʈ���� ������ ���� �д´�..
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerGetBlock(int nAxisNo, ref double dpStartPos, ref double dpEndPos, ref double dpPeriodPos);
    // ����ڰ� �� ���� Ʈ���� �޽��� ����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerOneShot(int nAxisNo);
    // ����ڰ� �� ���� Ʈ���� �޽��� �����Ŀ� ����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerSetTimerOneshot(int nAxisNo, int mSec);
    // ������ġ Ʈ���� ���Ѵ� ������ġ ����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerOnlyAbs(int nAxisNo,int nTrigNum, ref double dTrigPos);
    // Ʈ���� ������ �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmTriggerSetReset(int nAxisNo);

//======== CRC( �ܿ� �޽� Ŭ���� �Լ�)=====================================================================    

    //Level   : LOW(0), HIGH(1), UNUSED(2), USED(3)
    //uMethod : �ܿ��޽� ���� ��� ��ȣ �޽� �� 2 - 6���� ��������.
    //          0: Don't care , 1: Don't care, 2: 500 uSec, 3: 1 mSec, 4: 10 mSec, 5: 50 mSec, 6: 100 mSec
    
    //���� �࿡ CRC ��ȣ ��� ���� �� ��� ������ �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmCrcSetMaskLevel(int nAxisNo, uint uLevel, uint uMethod);
        // ���� ���� CRC ��ȣ ��� ���� �� ��� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmCrcGetMaskLevel(int nAxisNo, ref uint upLevel, ref uint upMethod);
    
    //uOnOff  : CRC ��ȣ�� Program���� �߻� ����  (FALSE(0),TRUE(1))

    // ���� �࿡ CRC ��ȣ�� ������ �߻� ��Ų��.
    [DllImport("ANLNet.dll")] public static extern uint AnmCrcSetOutput(int nAxisNo, uint uOnOff);
    // ���� ���� CRC ��ȣ�� ������ �߻� ���θ� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmCrcGetOutput(int nAxisNo, ref uint upOnOff);

//======MPG(Manual Pulse Generation) �Լ�===========================================================

    // lInputMethod  : 0-3 ���� ��������. 0:OnePhase, 1:TwoPhase1(��������) , 2:TwoPhase2, 3:TwoPhase4
    // lDriveMode    : 0�� ��������
    //                0 :MPG ���Ӹ�� 

    // MPGPos        : MPG �Է½�ȣ���� �̵��ϴ� �Ÿ�

    // MPGdenominator: MPG(���� �޽� �߻� ��ġ �Է�)���� �� ������ ��
    // dMPGnumerator : MPG(���� �޽� �߻� ��ġ �Է�)���� �� ���ϱ� ��
    // dwNumerator   : �ִ�(1 ����    64) ���� ���� ����
    // dwDenominator : �ִ�(1 ����  4096) ���� ���� ����
    // dMPGdenominator = 4096, MPGnumerator=1 �� �ǹ��ϴ� ���� 
    // MPG �ѹ����� 200�޽��� �״�� 1:1�� 1�޽��� ����� �ǹ��Ѵ�. 
    // ���� dMPGdenominator = 4096, MPGnumerator=2 �� �������� 1:2�� 2�޽��� ����� �������ٴ��ǹ��̴�. 
    // ���⿡ MPG PULSE = ((Numerator) * (Denominator)/ 4096 ) Ĩ���ο� ��³����� �����̴�.
    
    // ���� �࿡ MPG �Է¹��, ����̺� ���� ���, �̵� �Ÿ�, MPG �ӵ� ���� �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMPGSetEnable(int nAxisNo, int nInputMethod, int nDriveMode, double dMPGPos, double dVel, double dAccel);
    // ���� �࿡ MPG �Է¹��, ����̺� ���� ���, �̵� �Ÿ�, MPG �ӵ� ���� ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMPGGetEnable(int nAxisNo, ref int npInputMethod, ref int npDriveMode, ref double dpMPGPos, ref double dpVel);

    // IP ������, QI ���� �Լ�.
    // ���� �࿡ MPG ����̺� ���� ��忡�� ���޽��� �̵��� �޽� ������ �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMPGSetRatio(int nAxisNo, uint dMPGnumerator, uint dMPGdenominator);
    // ���� �࿡ MPG ����̺� ���� ��忡�� ���޽��� �̵��� �޽� ������ ��ȯ�Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMPGGetRatio(int nAxisNo, ref uint dMPGnumerator, ref uint dMPGdenominator);

    // ���� �࿡ MPG ����̺� ������ �����Ѵ�.
    [DllImport("ANLNet.dll")] public static extern uint AnmMPGReset(int nAxisNo);
    
//========= �ΰ���� �Լ� ============================================================================
    // ���� ���� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ��� ����.
    // dwNetErrorAct :    '0' - ���� ��� ��� ����
    //                    '1' - ���� ����
    //                    '2' - �� ����
    [DllImport("ANLNet.dll")] public static extern uint AnmNetWorkErrorSetAction(int lAxisNo, uint dwNetErrorAct);
    // ���� ���� ����� ����� ����� ���� ������ ���� ��� ���� ���¿� ���� ����ó�� ��� Ȯ��.
    [DllImport("ANLNet.dll")] public static extern uint AnmNetWorkErrorGetAction(int lAxisNo, ref uint dwpNetErrorAct);

}
