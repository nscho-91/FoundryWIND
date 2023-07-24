/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXHS.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Resource Define Header File
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

using System.Runtime.InteropServices;

// ���̽����� ����
public enum ANT_BASE_NODE:uint
{
        ANT_UNKNOWN               = 0x00,            // Unknown Baseboard
        ANT_EIP                   = 0x20,            // EtherNet/IP interface Node
        ANT_ECAT                  = 0x21,            // EtherCat interface Node
        ANT_CAN                   = 0x22,            // CAN interface Node
        ANT_DNET                  = 0x23,            // DEVICENET interface Node
        ANT_PBUS                  = 0x24             // ProfiBus interface Node
}

// ��� ����
public enum ANT_MODULE:uint
{
        ANT_NMC_2V04              = 0xB1,            // CAMC-QI, 2 Axis
        ANT_NMC_4V04              = 0xB2,            // CAMC-QI, 4 Axis
        ANT_NIO_DC16              = 0xB3,            // Digital 16 �� Configurable
        ANT_NIO_AI4               = 0xB4,            // AI 4Ch, 16 bit
        ANT_NIO_DI32              = 0xB5,            // Digital IN  32��    (reserved)
        ANT_NIO_DO32              = 0xB6,            // Digital OUT 32��    (reserved)
        ANT_NIO_DB32              = 0xB7,            // Digital IN  16�� / OUT 16��    (reserved)
        ANT_NIO_AO4               = 0xB8,            // AO 4Ch, 16 bit    (reserved)
        ANT_FN_EMPTY              = 0xFE,            // Empty module area.
        ANT_FN_UNKNOWN            = 0xFF             // Unkown module.
}

public enum ANT_FUNC_RESULT:uint
{
        ANT_RT_SUCCESS                                = 0000,
        ANT_RT_OPEN_ERROR                             = 1001,
        ANT_RT_OPEN_ALREADY                           = 1002,
        ANT_RT_NOT_OPEN                               = 1053,
        ANT_RT_NOT_SUPPORT_VERSION                    = 1054,
        ANT_RT_INVALID_NODE_NO                        = 1101,
        ANT_RT_INVALID_MODULE_POS                     = 1102,
        ANT_RT_INVALID_LEVEL                          = 1103,
        ANT_RT_FLASH_BUSY                             = 1150,
        ANT_RT_ERROR_VERSION_READ                     = 1151,
        ANT_RT_ERROR_NETWORK                          = 1152,
        ANT_RT_ERROR_HW_ACCESS                        = 1153,            
        ANT_RT_ERROR_NETWORK_CHEKSUM                  = 1154,

        ANT_RT_1ST_BELOW_MIN_VALUE                    = 1160,
        ANT_RT_1ST_ABOVE_MAX_VALUE                    = 1161,
        ANT_RT_2ND_BELOW_MIN_VALUE                    = 1170,
        ANT_RT_2ND_ABOVE_MAX_VALUE                    = 1171,
        ANT_RT_3RD_BELOW_MIN_VALUE                    = 1180,
        ANT_RT_3RD_ABOVE_MAX_VALUE                    = 1181,
        ANT_RT_4TH_BELOW_MIN_VALUE                    = 1190,
        ANT_RT_4TH_ABOVE_MAX_VALUE                    = 1191,
        ANT_RT_5TH_BELOW_MIN_VALUE                    = 1200,
        ANT_RT_5TH_ABOVE_MAX_VALUE                    = 1201,
        ANT_RT_6TH_BELOW_MIN_VALUE                    = 1210,
        ANT_RT_6TH_ABOVE_MAX_VALUE                    = 1211,
        ANT_RT_7TH_BELOW_MIN_VALUE                    = 1220,
        ANT_RT_7TH_ABOVE_MAX_VALUE                    = 1221,
        ANT_RT_8TH_BELOW_MIN_VALUE                    = 1230,
        ANT_RT_8TH_ABOVE_MAX_VALUE                    = 1231,
        ANT_RT_9TH_BELOW_MIN_VALUE                    = 1240,
        ANT_RT_9TH_ABOVE_MAX_VALUE                    = 1241,
        ANT_RT_10TH_BELOW_MIN_VALUE                   = 1250,
        ANT_RT_10TH_ABOVE_MAX_VALUE                   = 1251,

        ANT_RT_AIO_OPEN_ERROR                         = 2001,
        ANT_RT_AIO_NOT_MODULE                         = 2051,
        ANT_RT_AIO_NOT_EVENT                          = 2052,
        ANT_RT_AIO_INVALID_MODULE_NO                  = 2101,
        ANT_RT_AIO_INVALID_CHANNEL_NO                 = 2102,
        ANT_RT_AIO_INVALID_USE                        = 2106,
        ANT_RT_AIO_INVALID_TRIGGER_MODE               = 2107,

        ANT_RT_DIO_OPEN_ERROR                         = 3001,
        ANT_RT_DIO_NOT_MODULE                         = 3051,
        ANT_RT_DIO_NOT_INTERRUPT                      = 3052,
        ANT_RT_DIO_INVALID_MODULE_NO                  = 3101,
        ANT_RT_DIO_INVALID_OFFSET_NO                  = 3102,
        ANT_RT_DIO_INVALID_LEVEL                      = 3103,
        ANT_RT_DIO_INVALID_MODE                       = 3104,
        ANT_RT_DIO_INVALID_VALUE                      = 3105,
        ANT_RT_DIO_INVALID_USE                        = 3106,

        ANT_RT_MOTION_OPEN_ERROR                      = 4001,
        ANT_RT_MOTION_NOT_MODULE                      = 4051,
        ANT_RT_MOTION_NOT_INTERRUPT                   = 4052,
        ANT_RT_MOTION_NOT_INITIAL_AXIS_NO             = 4053,
        ANT_RT_MOTION_NOT_IN_CONT_INTERPOL            = 4054,
        ANT_RT_MOTION_NOT_PARA_READ                   = 4055,
        ANT_RT_MOTION_INVALID_AXIS_NO                 = 4101,
        ANT_RT_MOTION_INVALID_METHOD                  = 4102,
        ANT_RT_MOTION_INVALID_USE                     = 4103,
        ANT_RT_MOTION_INVALID_LEVEL                   = 4104,
        ANT_RT_MOTION_INVALID_BIT_NO                  = 4105,
        ANT_RT_MOTION_INVALID_STOP_MODE               = 4106,
        ANT_RT_MOTION_INVALID_TRIGGER_MODE            = 4107,
        ANT_RT_MOTION_INVALID_TRIGGER_LEVEL           = 4108,
        ANT_RT_MOTION_INVALID_SELECTION               = 4109,
        ANT_RT_MOTION_INVALID_TIME                    = 4110,
        ANT_RT_MOTION_INVALID_FILE_LOAD               = 4111,
        ANT_RT_MOTION_INVALID_FILE_SAVE               = 4112,
        ANT_RT_MOTION_INVALID_VELOCITY                = 4113,
        ANT_RT_MOTION_INVALID_ACCELTIME               = 4114,
        ANT_RT_MOTION_INVALID_PULSE_VALUE             = 4115,
        ANT_RT_MOTION_INVALID_NODE_NUMBER             = 4116,
        ANT_RT_MOTION_INVALID_TARGET                  = 4117,
        ANT_RT_MOTION_ERROR_IN_NONMOTION              = 4151,
        ANT_RT_MOTION_ERROR_IN_MOTION                 = 4152,
        ANT_RT_MOTION_ERROR                           = 4153,
       
        ANT_RT_MOTION_ERROR_GANTRY_ENABLE             = 4154,
        ANT_RT_MOTION_ERROR_GANTRY_AXIS               = 4155,
        ANT_RT_MOTION_ERROR_MASTER_SERVOON            = 4156,
        ANT_RT_MOTION_ERROR_SLAVE_SERVOON             = 4157,
        ANT_RT_MOTION_INVALID_POSITION                = 4158,

        ANT_RT_ERROR_NOT_SAME_MODULE                  = 4159,
        ANT_RT_ERROR_NOT_SAME_PRODUCT                 = 4161,
        ANT_RT_NOT_CAPTURED                           = 4162,
        ANT_RT_ERROR_NOT_SAME_IC                      = 4163,
        ANT_RT_ERROR_NOT_GEARMODE                     = 4164,
        ANT_ERROR_CONTI_INVALID_AXIS_NO               = 4165,
        ANT_ERROR_CONTI_INVALID_MAP_NO                = 4166,
        ANT_ERROR_CONTI_EMPTY_MAP_NO                  = 4167,
        ANT_RT_MOTION_ERROR_CACULATION                = 4168,
        ANT_RT_ERROR_NOT_SAME_NODE                    = 4169,

        ANT_ERROR_HELICAL_INVALID_AXIS_NO             = 4170,
        ANT_ERROR_HELICAL_INVALID_MAP_NO              = 4171,
        ANT_ERROR_HELICAL_EMPTY_MAP_NO                = 4172,

        ANT_ERROR_SPLINE_INVALID_AXIS_NO              = 4180,
        ANT_ERROR_SPLINE_INVALID_MAP_NO               = 4181,
        ANT_ERROR_SPLINE_EMPTY_MAP_NO                 = 4182,
        ANT_ERROR_SPLINE_NUM_ERROR                    = 4183,
        ANT_RT_MOTION_INTERPOL_VALUE                  = 4184,
        ANT_RT_ERROR_NOT_CONTIBEGIN                   = 4185,
        ANT_RT_ERROR_NOT_CONTIEND                     = 4186,

        ANT_RT_MOTION_HOME_SEARCHING                  = 4201,
        ANT_RT_MOTION_HOME_ERROR_SEARCHING            = 4202,
        ANT_RT_MOTION_HOME_ERROR_START                = 4203,
        ANT_RT_MOTION_HOME_ERROR_GANTRY               = 4204,
        ANT_RT_MOTION_POSITION_OUTOFBOUND             = 4251,
        ANT_RT_MOTION_PROFILE_INVALID                 = 4252,
        ANT_RT_MOTION_VELOCITY_OUTOFBOUND             = 4253,
        ANT_RT_MOTION_MOVE_UNIT_IS_ZERO               = 4254,
        ANT_RT_MOTION_SETTING_ERROR                   = 4255,
        ANT_RT_MOTION_IN_CONT_INTERPOL                = 4256,
        ANT_RT_MOTION_DISABLE_TRIGGER                 = 4257,
        ANT_RT_MOTION_INVALID_CONT_INDEX              = 4258,
        ANT_RT_MOTION_CONT_QUEUE_FULL                 = 4259,

        ANT_RT_INIT_DOSE_NOT_EXIST_LAN_CARD           = 4301,
        ANT_RT_INIT_DOES_NOT_RESPONSE_SLAVE           = 4302,
        ANT_RT_INIT_INVALID_HOST_IP_ADDRESS           = 4303,
        ANT_RT_INIT_INVALID_HOST_COUNT                = 4304,
        ANT_RT_INIT_EIPSTART_FAIL                     = 4305,
        ANT_RT_INIT_ALREADY_INITIALIZED               = 4306,
        ANT_RT_INIT_INVALID_NET_TYPE                  = 4307,
        ANT_RT_INIT_PRODUCTID                         = 4308,
        ANT_RT_INIT_NOT_ENOUGH_MEMORY                 = 4309,
        ANT_RT_INIT_DOES_NOT_RESPONSE_SLAVE_AT_RING   = 4310,
        ANT_RT_INIT_NETWORK_ERROR                     = 4311,
        ANT_RT_INIT_BACKGROUND_NOT_STARTED            = 4312,
        ANT_RT_INIT_BACKGROUND_START_FAIL             = 4313
}

public enum ANT_BOOLEAN:uint
{
    N_FALSE,
    N_TRUE
}

public enum ANT_EVENT:uint
{
    N_WM_USER                      = 0x0400
    //WM_AXL_INTERRUPT           = (WM_USER + 1001)
    //WM_AXL_NETSTAT             = (WM_USER + 1002)
}

public enum ANT_LOG_LEVEL:uint
{
    N_LEVEL_NONE,
    N_LEVEL_ERROR,
    N_LEVEL_RUNSTOP,
    N_LEVEL_FUNCTION
}

public enum ANT_EXISTENCE:uint
{
    N_STATUS_NOTEXIST,
    N_STATUS_EXIST
}

public enum ANT_USE:uint
{
    N_DISABLE,
    N_ENABLE
}

public enum ANT_AIO_TRIGGER_MODE:uint
{
    N_DISABLE_MODE               = 0,
    N_NORMAL_MODE                = 1,
    N_TIMER_MODE, 
    N_EXTERNAL_MODE
}

public enum ANT_AIO_FULL_MODE:uint
{
    N_NEW_DATA_KEEP,
    N_CURR_DATA_KEEP
}

public enum ANT_AIO_EVENT_MASK:uint 
{
    N_DATA_EMPTY                  = 0x01,
    N_DATA_MANY                   = 0x02,
    N_DATA_SMAL                   = 0x04,
    N_DATA_FULL                   = 0x08
}

public enum ANT_AIO_INTERRUPT_MASK:uint 
{
    N_ADC_DONE                    = 0x00,
    N_SCAN_END                    = 0x01,
    N_FIFO_HALF_FULL              = 0x02,
    N_NO_SIGNAL                   = 0x03
}

public enum AIO_EVENT_MODE:uint 
{
    N_AIO_EVENT_DATA_RESET         = 0x00, 
    N_AIO_EVENT_DATA_UPPER, 
    N_AIO_EVENT_DATA_LOWER, 
    N_AIO_EVENT_DATA_FULL, 
    N_AIO_EVENT_DATA_EMPTY         = 0x03
}
    
public enum ANT_DIO_EDGE:uint
{
    N_DOWN_EDGE,
    N_UP_EDGE
}

public enum ANT_DIO_STATE:uint
{
    N_OFF_STATE,
    N_ON_STATE
}

public enum ANT_MOTION_STOPMODE:uint
{
    N_EMERGENCY_STOP,
    N_SLOWDOWN_STOP
}

public enum ANT_MOTION_EDGE:uint
{
    N_SIGNAL_UP_EDGE,
    N_SIGNAL_DOWN_EDGE
}

public enum ANT_MOTION_SELECTION:uint
{
    N_COMMAND,
    N_ACTUAL
}

public enum ANT_MOTION_TRIGGER_MODE:uint
{
    N_PERIOD_MODE,
    N_ABS_POS_MODE
}

public enum ANT_MOTION_LEVEL_MODE:uint
{
    N_LOW,
    N_HIGH,
    N_UNUSED,
    N_USED
}

public enum ANT_MOTION_ABSREL_MODE:uint
{
    N_POS_ABS_MODE,
    N_POS_REL_MODE
}

public enum ANT_MOTION_PROFILE_MODE:uint
{
    N_SYM_TRAPEZOIDE_MODE,
    N_ASYM_TRAPEZOIDE_MODE,
    N_QUASI_S_CURVE_MODE,
    N_SYM_S_CURVE_MODE,
    N_ASYM_S_CURVE_MODE
}

public enum ANT_MOTION_SIGNAL_LEVEL:uint
{
    N_INACTIVE,
    N_ACTIVE
}

public enum ANT_MOTION_HOME_RESULT:uint
{
    N_HOME_SUCCESS                      = 0x01,
    N_HOME_SEARCHING                    = 0x02,
    N_HOME_ERR_GNT_RANGE                = 0x10,
    N_HOME_ERR_USER_BREAK               = 0x11,
    N_HOME_ERR_VELOCITY                 = 0x12,
    N_HOME_ERR_AMP_FAULT                = 0x13,
    N_HOME_ERR_NEG_LIMIT                = 0x14,
    N_HOME_ERR_POS_LIMIT                = 0x15,
    N_HOME_ERR_NOT_DETECT               = 0x16,
    N_HOME_ERR_UNKNOWN                  = 0xFF
}

public enum ANT_MOTION_UNIV_INPUT:uint
{
    N_UIO_INP0,
    N_UIO_INP1,
    N_UIO_INP2,
    N_UIO_INP3,
    N_UIO_INP4,
    N_UIO_INP5
}

public enum ANT_MOTION_UNIV_OUTPUT:uint
{
    N_UIO_OUT0,
    N_UIO_OUT1,
    N_UIO_OUT2,
    N_UIO_OUT3,
    N_UIO_OUT4,
    N_UIO_OUT5
}

public enum ANT_MOTION_DETECT_DOWN_START_POINT:uint
{
    N_AutoDetect,
    N_RestPulse
}

public enum ANT_MOTION_PULSE_OUTPUT:uint
{
    N_OneHighLowHighm,                     // 1�޽� ���, PULSE(Active High), ������(DIR=Low)  / ������(DIR=High)
    N_OneHighHighLow,                      // 1�޽� ���, PULSE(Active High), ������(DIR=High) / ������(DIR=Low)
    N_OneLowLowHigh,                       // 1�޽� ���, PULSE(Active Low),  ������(DIR=Low)  / ������(DIR=High)
    N_OneLowHighLow,                       // 1�޽� ���, PULSE(Active Low),  ������(DIR=High) / ������(DIR=Low)
    N_TwoCcwCwHigh,                        // 2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active High     
    N_TwoCcwCwLow,                         // 2�޽� ���, PULSE(CCW:������),  DIR(CW:������),  Active Low     
    N_TwoCwCcwHigh,                        // 2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active High
    N_TwoCwCcwLow,                         // 2�޽� ���, PULSE(CW:������),   DIR(CCW:������), Active Low
    N_TwoPhase                             // 2��(90' ������),  PULSE lead DIR(CW: ������), PULSE lag DIR(CCW:������)
}

public enum ANT_MOTION_EXTERNAL_COUNTER_INPUT:uint
{
    N_ObverseUpDownMode,                   // ������ Up/Down
    N_ObverseSqr1Mode,                     // ������ 1ü��
    N_ObverseSqr2Mode,                     // ������ 2ü��
    N_ObverseSqr4Mode,                     // ������ 4ü��
    N_ReverseUpDownMode,                   // ������ Up/Down
    N_ReverseSqr1Mode,                     // ������ 1ü��
    N_ReverseSqr2Mode,                     // ������ 2ü��
    N_ReverseSqr4Mode                      // ������ 4ü��
}

public enum ANT_MOTION_ACC_UNIT:uint
{
    N_UNIT_SEC2              = 0x0,    // unit/sec2
    N_SEC                    = 0x1     // sec
}

public enum ANT_MOTION_MOVE_DIR:uint
{
    N_DIR_CCW                = 0x0,    // �ݽð����
    N_DIR_CW                 = 0x1     // �ð����
}

public enum ANT_MOTION_RADIUS_DISTANCE:uint
{
    N_SHORT_DISTANCE          = 0x0,    // ª�� �Ÿ��� ��ȣ �̵� 
    N_LONG_DISTANCE           = 0x1     // �� �Ÿ��� ��ȣ �̵� 
}

public enum ANT_MOTION_INTERPOLATION_AXIS:uint
{
    N_INTERPOLATION_AXIS2     = 0x0,         // 2���� �������� ����� ��
    N_INTERPOLATION_AXIS3     = 0x1,         // 3���� �������� ����� ��
    N_INTERPOLATION_AXIS4     = 0x2          // 4���� �������� ����� ��
}

public enum ANT_MOTION_CONTISTART_NODE:uint
{
    N_CONTI_NODE_VELOCITY               = 0x0,    // �ӵ� ���� ���� ���
    N_CONTI_NODE_MANUAL                 = 0x1,    // ��� ������ ���� ���
    N_CONTI_NODE_AUTO                   = 0x2     // �ڵ� ������ ���� ���
}

public enum ANT_MOTION_HOME_DETECT_SIGNAL:uint
{
    N_PosEndLimit                       = 0x0,    // +Elm(End limit) +���� ����Ʈ ���� ��ȣ
    N_NegEndLimit                       = 0x1,    // -Elm(End limit) -���� ����Ʈ ���� ��ȣ
    N_PosSloLimit                       = 0x2,    // +Slm(Slow Down limit) ��ȣ - ������� ����
    N_NegSloLimit                       = 0x3,    // -Slm(Slow Down limit) ��ȣ - ������� ����
    N_HomeSensor                        = 0x4,    // IN0(ORG)  ���� ���� ��ȣ
    N_EncodZPhase                       = 0x5,    // IN1(Z��)  Encoder Z�� ��ȣ
    N_UniInput02                        = 0x6,    // IN2(����) ���� �Է� 2�� ��ȣ
    N_UniInput03                        = 0x7,    // IN3(����) ���� �Է� 3�� ��ȣ
}

public enum ANT_MOTION_MPG_INPUT_METHOD:uint
{
    N_MPG_DIFF_ONE_PHASE                = 0x0,    // MPG �Է� ��� One Phase
    N_MPG_DIFF_TWO_PHASE_1X             = 0x1,    // MPG �Է� ��� TwoPhase1
    N_MPG_DIFF_TWO_PHASE_2X             = 0x2,    // MPG �Է� ��� TwoPhase2
    N_MPG_DIFF_TWO_PHASE_4X             = 0x3,    // MPG �Է� ��� TwoPhase4
    N_MPG_LEVEL_ONE_PHASE               = 0x4,    // MPG �Է� ��� Level One Phase
    N_MPG_LEVEL_TWO_PHASE_1X            = 0x5,    // MPG �Է� ��� Level Two Phase1
    N_MPG_LEVEL_TWO_PHASE_2X            = 0x6,    // MPG �Է� ��� Level Two Phase2
    N_MPG_LEVEL_TWO_PHASE_4X            = 0x7,    // MPG �Է� ��� Level Two Phase4
}

public enum ANT_MOTION_HOME_CRC_SELECT:uint
{
    N_CRC_SELECT1                       = 0x0,    // ��ġŬ���� ������, �ܿ��޽� Ŭ���� ��� ����
    N_CRC_SELECT2                       = 0x1,    // ��ġŬ���� �����, �ܿ��޽� Ŭ���� ��� ����
    N_CRC_SELECT3                       = 0x2,    // ��ġŬ���� ������, �ܿ��޽� Ŭ���� �����
    N_CRC_SELECT4                       = 0x3     // ��ġŬ���� �����, �ܿ��޽� Ŭ���� �����
}

public enum ANT_MOTION_QIDETECT_DESTINATION_SIGNAL:uint
{
    N_Signal_PosEndLimit                = 0x0,    // +Elm(End limit) +���� ����Ʈ ���� ��ȣ
    N_Signal_NegEndLimit                = 0x1,    // -Elm(End limit) -���� ����Ʈ ���� ��ȣ
    N_Signal_PosSloLimit                = 0x2,    // +Slm(Slow Down limit) ��ȣ - ������� ����
    N_Signal_NegSloLimit                = 0x3,    // -Slm(Slow Down limit) ��ȣ - ������� ����
    N_Signal_HomeSensor                 = 0x4,    // IN0(ORG)  ���� ���� ��ȣ
    N_Signal_EncodZPhase                = 0x5,    // IN1(Z��)  Encoder Z�� ��ȣ
    N_Signal_UniInput02                 = 0x6,    // IN2(����) ���� �Է� 2�� ��ȣ
    N_Signal_UniInput03                 = 0x7     // IN3(����) ���� �Է� 3�� ��ȣ
}

public enum ANT_MOTION_QIMECHANICAL_SIGNAL:uint
{
    N_QIMECHANICAL_PELM_LEVEL           = 0x00001,       // Bit 0, +Limit ������ ��ȣ ���� ����
    N_QIMECHANICAL_NELM_LEVEL           = 0x00002,       // Bit 1, -Limit ������ ��ȣ ���� ����
    N_QIMECHANICAL_PSLM_LEVEL           = 0x00004,       // Bit 2, +limit �������� ���� ����.
    N_QIMECHANICAL_NSLM_LEVEL           = 0x00008,       // Bit 3, -limit �������� ���� ����
    N_QIMECHANICAL_ALARM_LEVEL          = 0x00010,       // Bit 4, Alarm ��ȣ ���� ����
    N_QIMECHANICAL_INP_LEVEL            = 0x00020,       // Bit 5, Inposition ��ȣ ���� ����
    N_QIMECHANICAL_ESTOP_LEVEL          = 0x00040,       // Bit 6, ��� ���� ��ȣ(ESTOP) ���� ����.
    N_QIMECHANICAL_ORG_LEVEL            = 0x00080,       // Bit 7, ���� ��ȣ ���� ����
    N_QIMECHANICAL_ZPHASE_LEVEL         = 0x00100,       // Bit 8, Z �� �Է� ��ȣ ���� ����
    N_QIMECHANICAL_ECUP_LEVEL           = 0x00200,       // Bit 9, ECUP �͹̳� ��ȣ ����.
    N_QIMECHANICAL_ECDN_LEVEL           = 0x00400,       // Bit 10, ECDN �͹̳� ��ȣ ����.
    N_QIMECHANICAL_EXPP_LEVEL           = 0x00800,       // Bit 11, EXPP �͹̳� ��ȣ ����
    N_QIMECHANICAL_EXMP_LEVEL           = 0x01000,       // Bit 12, EXMP �͹̳� ��ȣ ����
    N_QIMECHANICAL_SQSTR1_LEVEL         = 0x02000,       // Bit 13, SQSTR1 �͹̳� ��ȣ ����
    N_QIMECHANICAL_SQSTR2_LEVEL         = 0x04000,       // Bit 14, SQSTR2 �͹̳� ��ȣ ����
    N_QIMECHANICAL_SQSTP1_LEVEL         = 0x08000,       // Bit 15, SQSTP1 �͹̳� ��ȣ ����
    N_QIMECHANICAL_SQSTP2_LEVEL         = 0x10000,       // Bit 16, SQSTP2 �͹̳� ��ȣ ����
    N_QIMECHANICAL_MODE_LEVEL           = 0x20000        // Bit 17, MODE �͹̳� ��ȣ ����.
}

public enum ANT_MOTION_QIEND_STATUS:uint
{
    N_QIEND_STATUS_0                    = 0x00000001,    // Bit 0, ������ ����Ʈ ��ȣ(PELM)�� ���� ����
    N_QIEND_STATUS_1                    = 0x00000002,    // Bit 1, ������ ����Ʈ ��ȣ(NELM)�� ���� ����
    N_QIEND_STATUS_2                    = 0x00000004,    // Bit 2, ������ �ΰ� ����Ʈ ��ȣ(PSLM)�� ���� ���� ����
    N_QIEND_STATUS_3                    = 0x00000008,    // Bit 3, ������ �ΰ� ����Ʈ ��ȣ(NSLM)�� ���� ���� ����
    N_QIEND_STATUS_4                    = 0x00000010,    // Bit 4, ������ ����Ʈ ����Ʈ ������ ��ɿ� ���� ���� ����
    N_QIEND_STATUS_5                    = 0x00000020,    // Bit 5, ������ ����Ʈ ����Ʈ ������ ��ɿ� ���� ���� ����
    N_QIEND_STATUS_6                    = 0x00000040,    // Bit 6, ������ ����Ʈ ����Ʈ �������� ��ɿ� ���� ���� ����
    N_QIEND_STATUS_7                    = 0x00000080,    // Bit 7, ������ ����Ʈ ����Ʈ �������� ��ɿ� ���� ���� ����
    N_QIEND_STATUS_8                    = 0x00000100,    // Bit 8, ���� �˶� ��ɿ� ���� ���� ����.
    N_QIEND_STATUS_9                    = 0x00000200,    // Bit 9, ��� ���� ��ȣ �Է¿� ���� ���� ����.
    N_QIEND_STATUS_10                   = 0x00000400,    // Bit 10, �� ���� ��ɿ� ���� ���� ����.
    N_QIEND_STATUS_11                   = 0x00000800,    // Bit 11, ���� ���� ��ɿ� ���� ���� ����.
    N_QIEND_STATUS_12                   = 0x00001000,    // Bit 12, ���� ������ ��ɿ� ���� ���� ����
    N_QIEND_STATUS_13                   = 0x00002000,    // Bit 13, ���� ���� ��� #1(SQSTP1)�� ���� ���� ����.
    N_QIEND_STATUS_14                   = 0x00004000,    // Bit 14, ���� ���� ��� #2(SQSTP2)�� ���� ���� ����.
    N_QIEND_STATUS_15                   = 0x00008000,    // Bit 15, ���ڴ� �Է�(ECUP,ECDN) ���� �߻�
    N_QIEND_STATUS_16                   = 0x00010000,    // Bit 16, MPG �Է�(EXPP,EXMP) ���� �߻�
    N_QIEND_STATUS_17                   = 0x00020000,    // Bit 17, ���� �˻� ���� ����.
    N_QIEND_STATUS_18                   = 0x00040000,    // Bit 18, ��ȣ �˻� ���� ����.
    N_QIEND_STATUS_19                   = 0x00080000,    // Bit 19, ���� ������ �̻����� ���� ����.
    N_QIEND_STATUS_20                   = 0x00100000,    // Bit 20, ������ ���� �����߻�.
    N_QIEND_STATUS_21                   = 0x00200000,    // Bit 21, MPG ��� ��� �޽� ���� �����÷ο� �߻�
    N_QIEND_STATUS_22                   = 0x00400000,    // Bit 22, DON'CARE
    N_QIEND_STATUS_23                   = 0x00800000,    // Bit 23, DON'CARE
    N_QIEND_STATUS_24                   = 0x01000000,    // Bit 24, DON'CARE
    N_QIEND_STATUS_25                   = 0x02000000,    // Bit 25, DON'CARE
    N_QIEND_STATUS_26                   = 0x04000000,    // Bit 26, DON'CARE
    N_QIEND_STATUS_27                   = 0x08000000,    // Bit 27, DON'CARE
    N_QIEND_STATUS_28                   = 0x10000000,    // Bit 28, ����/������ ���� ����̺� ����
    N_QIEND_STATUS_29                   = 0x20000000,    // Bit 29, �ܿ� �޽� ���� ��ȣ ��� ��.
    N_QIEND_STATUS_30                   = 0x40000000,    // Bit 30, ������ ���� ���� ���� ����
    N_QIEND_STATUS_31                   = 0x80000000     // Bit 31, ���� ����̺� ����Ÿ ���� ����.
}

public enum ANT_MOTION_QIDRIVE_STATUS:uint
{
    N_QIDRIVE_STATUS_0                  = 0x0000001,     // Bit 0, BUSY(����̺� ���� ��)
    N_QIDRIVE_STATUS_1                  = 0x0000002,     // Bit 1, DOWN(���� ��)
    N_QIDRIVE_STATUS_2                  = 0x0000004,     // Bit 2, CONST(��� ��)
    N_QIDRIVE_STATUS_3                  = 0x0000008,     // Bit 3, UP(���� ��)
    N_QIDRIVE_STATUS_4                  = 0x0000010,     // Bit 4, ���� ����̺� ���� ��
    N_QIDRIVE_STATUS_5                  = 0x0000020,     // Bit 5, ���� �Ÿ� ����̺� ���� ��
    N_QIDRIVE_STATUS_6                  = 0x0000040,     // Bit 6, MPG ����̺� ���� ��
    N_QIDRIVE_STATUS_7                  = 0x0000080,     // Bit 7, �����˻� ����̺� ������
//���ǻ��� : (���� ���̺귯�� ���� ������ HOME�Լ��� ������� ���۵ȴ�.)
    N_QIDRIVE_STATUS_8                  = 0x0000100,     // Bit 8, ��ȣ �˻� ����̺� ���� ��
//���ǻ��� : (���� ���̺귯�� ���� ������ HOME�Լ��� ���� ���۵ȴ�. Ȩ�Լ��� ��ȣ��ġ�Լ��� ����������Ͽ� ������ �Ǿ����� Ȩ �˻��� BIT: 1 �ȴ�.)
    N_QIDRIVE_STATUS_9                  = 0x0000200,     // Bit 9, ���� ����̺� ���� ��
    N_QIDRIVE_STATUS_10                 = 0x0000400,     // Bit 10, Slave ����̺� ������
    N_QIDRIVE_STATUS_11                 = 0x0000800,     // Bit 11, ���� ���� ����̺� ����(���� ����̺꿡���� ǥ�� ���� �ٸ�)
    N_QIDRIVE_STATUS_12                 = 0x0001000,     // Bit 12, �޽� ����� ������ġ �Ϸ� ��ȣ �����.
    N_QIDRIVE_STATUS_13                 = 0x0002000,     // Bit 13, ���� ���� ����̺� ������.
    N_QIDRIVE_STATUS_14                 = 0x0004000,     // Bit 14, ��ȣ ���� ����̺� ������.
    N_QIDRIVE_STATUS_15                 = 0x0008000,     // Bit 15, �޽� ��� ��.
    N_QIDRIVE_STATUS_16                 = 0x0010000,     // Bit 16, ���� ���� ������ ����(ó��)(0-7)
    N_QIDRIVE_STATUS_17                 = 0x0020000,     // Bit 17, ���� ���� ������ ����(�߰�)(0-7)
    N_QIDRIVE_STATUS_18                 = 0x0040000,     // Bit 18, ���� ���� ������ ����(��)(0-7)
    N_QIDRIVE_STATUS_19                 = 0x0100000,     // Bit 19, ���� ���� Queue ��� ����.
    N_QIDRIVE_STATUS_20                 = 0x0200000,     // Bit 20, ���� ���� Queue ���� �H
    N_QIDRIVE_STATUS_21                 = 0x0400000,     // Bit 21, ���� ���� ����̺��� �ӵ� ���(ó��)
    N_QIDRIVE_STATUS_22                 = 0x0800000,     // Bit 22, ���� ���� ����̺��� �ӵ� ���(��)
    N_QIDRIVE_STATUS_23                 = 0x1000000,     // Bit 23, MPG ���� #1 Full
    N_QIDRIVE_STATUS_24                 = 0x2000000,     // Bit 24, MPG ���� #2 Full
    N_QIDRIVE_STATUS_25                 = 0x4000000,     // Bit 25, MPG ���� #3 Full
    N_QIDRIVE_STATUS_26                 = 0x8000000      // Bit 26, MPG ���� ������ OverFlow
}

public enum ANT_MOTION_QIINTERRUPT_BANK1:uint
{
    N_QIINTBANK1_DISABLE                = 0x00000000,    // INTERRUT DISABLED.
    N_QIINTBANK1_0                      = 0x00000001,    // Bit 0,  ���ͷ�Ʈ �߻� ��� ������ ���� �����.
    N_QIINTBANK1_1                      = 0x00000002,    // Bit 1,  ���� �����
    N_QIINTBANK1_2                      = 0x00000004,    // Bit 2,  ���� ���۽�.
    N_QIINTBANK1_3                      = 0x00000008,    // Bit 3,  ī���� #1 < �񱳱� #1 �̺�Ʈ �߻�
    N_QIINTBANK1_4                      = 0x00000010,    // Bit 4,  ī���� #1 = �񱳱� #1 �̺�Ʈ �߻�
    N_QIINTBANK1_5                      = 0x00000020,    // Bit 5,  ī���� #1 > �񱳱� #1 �̺�Ʈ �߻�
    N_QIINTBANK1_6                      = 0x00000040,    // Bit 6,  ī���� #2 < �񱳱� #2 �̺�Ʈ �߻�
    N_QIINTBANK1_7                      = 0x00000080,    // Bit 7,  ī���� #2 = �񱳱� #2 �̺�Ʈ �߻�
    N_QIINTBANK1_8                      = 0x00000100,    // Bit 8,  ī���� #2 > �񱳱� #2 �̺�Ʈ �߻�
    N_QIINTBANK1_9                      = 0x00000200,    // Bit 9,  ī���� #3 < �񱳱� #3 �̺�Ʈ �߻�
    N_QIINTBANK1_10                     = 0x00000400,    // Bit 10, ī���� #3 = �񱳱� #3 �̺�Ʈ �߻�
    N_QIINTBANK1_11                     = 0x00000800,    // Bit 11, ī���� #3 > �񱳱� #3 �̺�Ʈ �߻�
    N_QIINTBANK1_12                     = 0x00001000,    // Bit 12, ī���� #4 < �񱳱� #4 �̺�Ʈ �߻�
    N_QIINTBANK1_13                     = 0x00002000,    // Bit 13, ī���� #4 = �񱳱� #4 �̺�Ʈ �߻�
    N_QIINTBANK1_14                     = 0x00004000,    // Bit 14, ī���� #4 < �񱳱� #4 �̺�Ʈ �߻�
    N_QIINTBANK1_15                     = 0x00008000,    // Bit 15, ī���� #5 < �񱳱� #5 �̺�Ʈ �߻�
    N_QIINTBANK1_16                     = 0x00010000,    // Bit 16, ī���� #5 = �񱳱� #5 �̺�Ʈ �߻�
    N_QIINTBANK1_17                     = 0x00020000,    // Bit 17, ī���� #5 > �񱳱� #5 �̺�Ʈ �߻�
    N_QIINTBANK1_18                     = 0x00040000,    // Bit 18, Ÿ�̸� #1 �̺�Ʈ �߻�.
    N_QIINTBANK1_19                     = 0x00080000,    // Bit 19, Ÿ�̸� #2 �̺�Ʈ �߻�.
    N_QIINTBANK1_20                     = 0x00100000,    // Bit 20, ���� ���� ���� Queue �����.
    N_QIINTBANK1_21                     = 0x00200000,    // Bit 21, ���� ���� ���� Queue ����H
    N_QIINTBANK1_22                     = 0x00400000,    // Bit 22, Ʈ���� �߻��Ÿ� �ֱ�/������ġ Queue �����.
    N_QIINTBANK1_23                     = 0x00800000,    // Bit 23, Ʈ���� �߻��Ÿ� �ֱ�/������ġ Queue ����H
    N_QIINTBANK1_24                     = 0x01000000,    // Bit 24, Ʈ���� ��ȣ �߻� �̺�Ʈ
    N_QIINTBANK1_25                     = 0x02000000,    // Bit 25, ��ũ��Ʈ #1 ��ɾ� ���� ���� Queue �����.
    N_QIINTBANK1_26                     = 0x04000000,    // Bit 26, ��ũ��Ʈ #2 ��ɾ� ���� ���� Queue �����.
    N_QIINTBANK1_27                     = 0x08000000,    // Bit 27, ��ũ��Ʈ #3 ��ɾ� ���� ���� �������� ����Ǿ� �ʱ�ȭ ��.
    N_QIINTBANK1_28                     = 0x10000000,    // Bit 28, ��ũ��Ʈ #4 ��ɾ� ���� ���� �������� ����Ǿ� �ʱ�ȭ ��.
    N_QIINTBANK1_29                     = 0x20000000,    // Bit 29, ���� �˶���ȣ �ΰ���.
    N_QIINTBANK1_30                     = 0x40000000,    // Bit 30, |CNT1| - |CNT2| >= |CNT4| �̺�Ʈ �߻�.
    N_QIINTBANK1_31                     = 0x80000000     // Bit 31, ���ͷ�Ʈ �߻� ��ɾ�|INTGEN| ����.
}

public enum ANT_MOTION_QIINTERRUPT_BANK2:uint
{
    N_QIINTBANK2_DISABLE                = 0x00000000,    // INTERRUT DISABLED.
    N_QIINTBANK2_0                      = 0x00000001,    // Bit 0,  ��ũ��Ʈ #1 �б� ��� ��� Queue �� ����H.
    N_QIINTBANK2_1                      = 0x00000002,    // Bit 1,  ��ũ��Ʈ #2 �б� ��� ��� Queue �� ����H.
    N_QIINTBANK2_2                      = 0x00000004,    // Bit 2,  ��ũ��Ʈ #3 �б� ��� ��� �������Ͱ� ���ο� �����ͷ� ���ŵ�.
    N_QIINTBANK2_3                      = 0x00000008,    // Bit 3,  ��ũ��Ʈ #4 �б� ��� ��� �������Ͱ� ���ο� �����ͷ� ���ŵ�.
    N_QIINTBANK2_4                      = 0x00000010,    // Bit 4,  ��ũ��Ʈ #1 �� ���� ��ɾ� �� ���� �� ���ͷ�Ʈ �߻����� ������ ��ɾ� �����.
    N_QIINTBANK2_5                      = 0x00000020,    // Bit 5,  ��ũ��Ʈ #2 �� ���� ��ɾ� �� ���� �� ���ͷ�Ʈ �߻����� ������ ��ɾ� �����.
    N_QIINTBANK2_6                      = 0x00000040,    // Bit 6,  ��ũ��Ʈ #3 �� ���� ��ɾ� ���� �� ���ͷ�Ʈ �߻����� ������ ��ɾ� �����.
    N_QIINTBANK2_7                      = 0x00000080,    // Bit 7,  ��ũ��Ʈ #4 �� ���� ��ɾ� ���� �� ���ͷ�Ʈ �߻����� ������ ��ɾ� �����.
    N_QIINTBANK2_8                      = 0x00000100,    // Bit 8,  ���� ����
    N_QIINTBANK2_9                      = 0x00000200,    // Bit 9,  ���� ��ġ ���� �Ϸ�(Inposition)����� ����� ����,���� ���� �߻�.
    N_QIINTBANK2_10                     = 0x00000400,    // Bit 10, �̺�Ʈ ī���ͷ� ���� �� ����� �̺�Ʈ ���� #1 ���� �߻�.
    N_QIINTBANK2_11                     = 0x00000800,    // Bit 11, �̺�Ʈ ī���ͷ� ���� �� ����� �̺�Ʈ ���� #2 ���� �߻�.
    N_QIINTBANK2_12                     = 0x00001000,    // Bit 12, SQSTR1 ��ȣ �ΰ� ��.
    N_QIINTBANK2_13                     = 0x00002000,    // Bit 13, SQSTR2 ��ȣ �ΰ� ��.
    N_QIINTBANK2_14                     = 0x00004000,    // Bit 14, UIO0 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_15                     = 0x00008000,    // Bit 15, UIO1 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_16                     = 0x00010000,    // Bit 16, UIO2 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_17                     = 0x00020000,    // Bit 17, UIO3 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_18                     = 0x00040000,    // Bit 18, UIO4 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_19                     = 0x00080000,    // Bit 19, UIO5 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_20                     = 0x00100000,    // Bit 20, UIO6 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_21                     = 0x00200000,    // Bit 21, UIO7 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_22                     = 0x00400000,    // Bit 22, UIO8 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_23                     = 0x00800000,    // Bit 23, UIO9 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_24                     = 0x01000000,    // Bit 24, UIO10 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_25                     = 0x02000000,    // Bit 25, UIO11 �͹̳� ��ȣ�� '1'�� ����.
    N_QIINTBANK2_26                     = 0x04000000,    // Bit 26, ���� ���� ����(LMT, ESTOP, STOP, ESTOP, CMD, ALARM) �߻�.
    N_QIINTBANK2_27                     = 0x08000000,    // Bit 27, ���� �� ������ ���� ���� �߻�.
    N_QIINTBANK2_28                     = 0x10000000,    // Bit 28, Don't Care
    N_QIINTBANK2_29                     = 0x20000000,    // Bit 29, ����Ʈ ��ȣ(PELM, NELM)��ȣ�� �Է� ��.
    N_QIINTBANK2_30                     = 0x40000000,    // Bit 30, �ΰ� ����Ʈ ��ȣ(PSLM, NSLM)��ȣ�� �Է� ��.
    N_QIINTBANK2_31                     = 0x80000000     // Bit 31, ��� ���� ��ȣ(ESTOP)��ȣ�� �Էµ�.
}

public class CANHS
{
    public delegate void ANT_EVENT_PROC(int nActiveNo, uint uFlag);
    public delegate void ANT_INTERRUPT_PROC(int nActiveNo, uint uFlag);
    public delegate void ANT_NETMON_PROC(int nActiveNo, uint uFlag);

    public readonly static int MAX_NODE_COUNT            = 250;
    public readonly static int MAX_AXIS_PER_NODE         = 20;

    public readonly static uint WM_USER                  = 0x0400;
    public readonly static uint WM_AXL_INTERRUPT         = (WM_USER + 1001);

    public static int  N_AXIS_EVN(int nAxisNo) 
    {
        nAxisNo = (nAxisNo - (nAxisNo % 2));        // ���� �̷�� ���� ¦������ ã��

        return nAxisNo;
    }

    public static int  N_AXIS_ODD(int nAxisNo) 
    {
        nAxisNo = (nAxisNo + ((nAxisNo + 1) % 2));   // ���� �̷�� ���� Ȧ������ ã��

        return nAxisNo;

    }

    public static int  N_AXIS_QUR(int nAxisNo) 
    {
        nAxisNo = (nAxisNo % 4);                     // ���� �̷�� ���� Ȧ������ ã��

        return nAxisNo;
    }

    public static int  N_AXIS_N04(int nAxisNo, int nPos) 
    {
        nAxisNo = (((nAxisNo / 4) * 4) + nPos);       // �� Ĩ�� �� ��ġ�� ����(0~3)

        return nAxisNo;
    }

    public static int  N_AXIS_N01(int nAxisNo) 
    {
        nAxisNo = ((nAxisNo % 4) >> 2);               // 0, 1���� 0���� 2, 3���� 1�� ����

        return nAxisNo;
    }

    public static int  N_AXIS_N02(int nAxisNo) 
    {
        nAxisNo = ((nAxisNo % 4)  % 2);               // 0, 2���� 0���� 1, 3���� 1�� ����

        return nAxisNo;
    }

    public static int    m_SendAxis            =0;    // ���� ���ȣ
}
