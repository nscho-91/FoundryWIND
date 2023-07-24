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

// 베이스보드 정의
public enum ANT_BASE_NODE:uint
{
        ANT_UNKNOWN               = 0x00,            // Unknown Baseboard
        ANT_EIP                   = 0x20,            // EtherNet/IP interface Node
        ANT_ECAT                  = 0x21,            // EtherCat interface Node
        ANT_CAN                   = 0x22,            // CAN interface Node
        ANT_DNET                  = 0x23,            // DEVICENET interface Node
        ANT_PBUS                  = 0x24             // ProfiBus interface Node
}

// 모듈 정의
public enum ANT_MODULE:uint
{
        ANT_NMC_2V04              = 0xB1,            // CAMC-QI, 2 Axis
        ANT_NMC_4V04              = 0xB2,            // CAMC-QI, 4 Axis
        ANT_NIO_DC16              = 0xB3,            // Digital 16 점 Configurable
        ANT_NIO_AI4               = 0xB4,            // AI 4Ch, 16 bit
        ANT_NIO_DI32              = 0xB5,            // Digital IN  32점    (reserved)
        ANT_NIO_DO32              = 0xB6,            // Digital OUT 32점    (reserved)
        ANT_NIO_DB32              = 0xB7,            // Digital IN  16점 / OUT 16점    (reserved)
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
    N_OneHighLowHighm,                     // 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
    N_OneHighHighLow,                      // 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
    N_OneLowLowHigh,                       // 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
    N_OneLowHighLow,                       // 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
    N_TwoCcwCwHigh,                        // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
    N_TwoCcwCwLow,                         // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
    N_TwoCwCcwHigh,                        // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
    N_TwoCwCcwLow,                         // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
    N_TwoPhase                             // 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
}

public enum ANT_MOTION_EXTERNAL_COUNTER_INPUT:uint
{
    N_ObverseUpDownMode,                   // 정방향 Up/Down
    N_ObverseSqr1Mode,                     // 정방향 1체배
    N_ObverseSqr2Mode,                     // 정방향 2체배
    N_ObverseSqr4Mode,                     // 정방향 4체배
    N_ReverseUpDownMode,                   // 역방향 Up/Down
    N_ReverseSqr1Mode,                     // 역방향 1체배
    N_ReverseSqr2Mode,                     // 역방향 2체배
    N_ReverseSqr4Mode                      // 역방향 4체배
}

public enum ANT_MOTION_ACC_UNIT:uint
{
    N_UNIT_SEC2              = 0x0,    // unit/sec2
    N_SEC                    = 0x1     // sec
}

public enum ANT_MOTION_MOVE_DIR:uint
{
    N_DIR_CCW                = 0x0,    // 반시계방향
    N_DIR_CW                 = 0x1     // 시계방향
}

public enum ANT_MOTION_RADIUS_DISTANCE:uint
{
    N_SHORT_DISTANCE          = 0x0,    // 짧은 거리의 원호 이동 
    N_LONG_DISTANCE           = 0x1     // 긴 거리의 원호 이동 
}

public enum ANT_MOTION_INTERPOLATION_AXIS:uint
{
    N_INTERPOLATION_AXIS2     = 0x0,         // 2축을 보간으로 사용할 때
    N_INTERPOLATION_AXIS3     = 0x1,         // 3축을 보간으로 사용할 때
    N_INTERPOLATION_AXIS4     = 0x2          // 4축을 보간으로 사용할 때
}

public enum ANT_MOTION_CONTISTART_NODE:uint
{
    N_CONTI_NODE_VELOCITY               = 0x0,    // 속도 지정 보간 모드
    N_CONTI_NODE_MANUAL                 = 0x1,    // 노드 가감속 보간 모드
    N_CONTI_NODE_AUTO                   = 0x2     // 자동 가감속 보간 모드
}

public enum ANT_MOTION_HOME_DETECT_SIGNAL:uint
{
    N_PosEndLimit                       = 0x0,    // +Elm(End limit) +방향 리미트 센서 신호
    N_NegEndLimit                       = 0x1,    // -Elm(End limit) -방향 리미트 센서 신호
    N_PosSloLimit                       = 0x2,    // +Slm(Slow Down limit) 신호 - 사용하지 않음
    N_NegSloLimit                       = 0x3,    // -Slm(Slow Down limit) 신호 - 사용하지 않음
    N_HomeSensor                        = 0x4,    // IN0(ORG)  원점 센서 신호
    N_EncodZPhase                       = 0x5,    // IN1(Z상)  Encoder Z상 신호
    N_UniInput02                        = 0x6,    // IN2(범용) 범용 입력 2번 신호
    N_UniInput03                        = 0x7,    // IN3(범용) 범용 입력 3번 신호
}

public enum ANT_MOTION_MPG_INPUT_METHOD:uint
{
    N_MPG_DIFF_ONE_PHASE                = 0x0,    // MPG 입력 방식 One Phase
    N_MPG_DIFF_TWO_PHASE_1X             = 0x1,    // MPG 입력 방식 TwoPhase1
    N_MPG_DIFF_TWO_PHASE_2X             = 0x2,    // MPG 입력 방식 TwoPhase2
    N_MPG_DIFF_TWO_PHASE_4X             = 0x3,    // MPG 입력 방식 TwoPhase4
    N_MPG_LEVEL_ONE_PHASE               = 0x4,    // MPG 입력 방식 Level One Phase
    N_MPG_LEVEL_TWO_PHASE_1X            = 0x5,    // MPG 입력 방식 Level Two Phase1
    N_MPG_LEVEL_TWO_PHASE_2X            = 0x6,    // MPG 입력 방식 Level Two Phase2
    N_MPG_LEVEL_TWO_PHASE_4X            = 0x7,    // MPG 입력 방식 Level Two Phase4
}

public enum ANT_MOTION_HOME_CRC_SELECT:uint
{
    N_CRC_SELECT1                       = 0x0,    // 위치클리어 사용않함, 잔여펄스 클리어 사용 안함
    N_CRC_SELECT2                       = 0x1,    // 위치클리어 사용함, 잔여펄스 클리어 사용 안함
    N_CRC_SELECT3                       = 0x2,    // 위치클리어 사용안함, 잔여펄스 클리어 사용함
    N_CRC_SELECT4                       = 0x3     // 위치클리어 사용함, 잔여펄스 클리어 사용함
}

public enum ANT_MOTION_QIDETECT_DESTINATION_SIGNAL:uint
{
    N_Signal_PosEndLimit                = 0x0,    // +Elm(End limit) +방향 리미트 센서 신호
    N_Signal_NegEndLimit                = 0x1,    // -Elm(End limit) -방향 리미트 센서 신호
    N_Signal_PosSloLimit                = 0x2,    // +Slm(Slow Down limit) 신호 - 사용하지 않음
    N_Signal_NegSloLimit                = 0x3,    // -Slm(Slow Down limit) 신호 - 사용하지 않음
    N_Signal_HomeSensor                 = 0x4,    // IN0(ORG)  원점 센서 신호
    N_Signal_EncodZPhase                = 0x5,    // IN1(Z상)  Encoder Z상 신호
    N_Signal_UniInput02                 = 0x6,    // IN2(범용) 범용 입력 2번 신호
    N_Signal_UniInput03                 = 0x7     // IN3(범용) 범용 입력 3번 신호
}

public enum ANT_MOTION_QIMECHANICAL_SIGNAL:uint
{
    N_QIMECHANICAL_PELM_LEVEL           = 0x00001,       // Bit 0, +Limit 급정지 신호 현재 상태
    N_QIMECHANICAL_NELM_LEVEL           = 0x00002,       // Bit 1, -Limit 급정지 신호 현재 상태
    N_QIMECHANICAL_PSLM_LEVEL           = 0x00004,       // Bit 2, +limit 감속정지 현재 상태.
    N_QIMECHANICAL_NSLM_LEVEL           = 0x00008,       // Bit 3, -limit 감속정지 현재 상태
    N_QIMECHANICAL_ALARM_LEVEL          = 0x00010,       // Bit 4, Alarm 신호 현재 상태
    N_QIMECHANICAL_INP_LEVEL            = 0x00020,       // Bit 5, Inposition 신호 현재 상태
    N_QIMECHANICAL_ESTOP_LEVEL          = 0x00040,       // Bit 6, 비상 정지 신호(ESTOP) 현재 상태.
    N_QIMECHANICAL_ORG_LEVEL            = 0x00080,       // Bit 7, 원점 신호 헌재 상태
    N_QIMECHANICAL_ZPHASE_LEVEL         = 0x00100,       // Bit 8, Z 상 입력 신호 현재 상태
    N_QIMECHANICAL_ECUP_LEVEL           = 0x00200,       // Bit 9, ECUP 터미널 신호 상태.
    N_QIMECHANICAL_ECDN_LEVEL           = 0x00400,       // Bit 10, ECDN 터미널 신호 상태.
    N_QIMECHANICAL_EXPP_LEVEL           = 0x00800,       // Bit 11, EXPP 터미널 신호 상태
    N_QIMECHANICAL_EXMP_LEVEL           = 0x01000,       // Bit 12, EXMP 터미널 신호 상태
    N_QIMECHANICAL_SQSTR1_LEVEL         = 0x02000,       // Bit 13, SQSTR1 터미널 신호 상태
    N_QIMECHANICAL_SQSTR2_LEVEL         = 0x04000,       // Bit 14, SQSTR2 터미널 신호 상태
    N_QIMECHANICAL_SQSTP1_LEVEL         = 0x08000,       // Bit 15, SQSTP1 터미널 신호 상태
    N_QIMECHANICAL_SQSTP2_LEVEL         = 0x10000,       // Bit 16, SQSTP2 터미널 신호 상태
    N_QIMECHANICAL_MODE_LEVEL           = 0x20000        // Bit 17, MODE 터미널 신호 상태.
}

public enum ANT_MOTION_QIEND_STATUS:uint
{
    N_QIEND_STATUS_0                    = 0x00000001,    // Bit 0, 정방향 리미트 신호(PELM)에 의한 종료
    N_QIEND_STATUS_1                    = 0x00000002,    // Bit 1, 역방향 리미트 신호(NELM)에 의한 종료
    N_QIEND_STATUS_2                    = 0x00000004,    // Bit 2, 정방향 부가 리미트 신호(PSLM)에 의한 구동 종료
    N_QIEND_STATUS_3                    = 0x00000008,    // Bit 3, 역방향 부가 리미트 신호(NSLM)에 의한 구동 종료
    N_QIEND_STATUS_4                    = 0x00000010,    // Bit 4, 정방향 소프트 리미트 급정지 기능에 의한 구동 종료
    N_QIEND_STATUS_5                    = 0x00000020,    // Bit 5, 역방향 소프트 리미트 급정지 기능에 의한 구동 종료
    N_QIEND_STATUS_6                    = 0x00000040,    // Bit 6, 정방향 소프트 리미트 감속정지 기능에 의한 구동 종료
    N_QIEND_STATUS_7                    = 0x00000080,    // Bit 7, 역방향 소프트 리미트 감속정지 기능에 의한 구동 종료
    N_QIEND_STATUS_8                    = 0x00000100,    // Bit 8, 서보 알람 기능에 의한 구동 종료.
    N_QIEND_STATUS_9                    = 0x00000200,    // Bit 9, 비상 정지 신호 입력에 의한 구동 종료.
    N_QIEND_STATUS_10                   = 0x00000400,    // Bit 10, 급 정지 명령에 의한 구동 종료.
    N_QIEND_STATUS_11                   = 0x00000800,    // Bit 11, 감속 정지 명령에 의한 구동 종료.
    N_QIEND_STATUS_12                   = 0x00001000,    // Bit 12, 전축 급정지 명령에 의한 구동 종료
    N_QIEND_STATUS_13                   = 0x00002000,    // Bit 13, 동기 정지 기능 #1(SQSTP1)에 의한 구동 종료.
    N_QIEND_STATUS_14                   = 0x00004000,    // Bit 14, 동기 정지 기능 #2(SQSTP2)에 의한 구동 종료.
    N_QIEND_STATUS_15                   = 0x00008000,    // Bit 15, 인코더 입력(ECUP,ECDN) 오류 발생
    N_QIEND_STATUS_16                   = 0x00010000,    // Bit 16, MPG 입력(EXPP,EXMP) 오류 발생
    N_QIEND_STATUS_17                   = 0x00020000,    // Bit 17, 원점 검색 성공 종료.
    N_QIEND_STATUS_18                   = 0x00040000,    // Bit 18, 신호 검색 성공 종료.
    N_QIEND_STATUS_19                   = 0x00080000,    // Bit 19, 보간 데이터 이상으로 구동 종료.
    N_QIEND_STATUS_20                   = 0x00100000,    // Bit 20, 비정상 구동 정지발생.
    N_QIEND_STATUS_21                   = 0x00200000,    // Bit 21, MPG 기능 블록 펄스 버퍼 오버플로우 발생
    N_QIEND_STATUS_22                   = 0x00400000,    // Bit 22, DON'CARE
    N_QIEND_STATUS_23                   = 0x00800000,    // Bit 23, DON'CARE
    N_QIEND_STATUS_24                   = 0x01000000,    // Bit 24, DON'CARE
    N_QIEND_STATUS_25                   = 0x02000000,    // Bit 25, DON'CARE
    N_QIEND_STATUS_26                   = 0x04000000,    // Bit 26, DON'CARE
    N_QIEND_STATUS_27                   = 0x08000000,    // Bit 27, DON'CARE
    N_QIEND_STATUS_28                   = 0x10000000,    // Bit 28, 현재/마지막 구동 드라이브 방향
    N_QIEND_STATUS_29                   = 0x20000000,    // Bit 29, 잔여 펄스 제거 신호 출력 중.
    N_QIEND_STATUS_30                   = 0x40000000,    // Bit 30, 비정상 구동 정지 원인 상태
    N_QIEND_STATUS_31                   = 0x80000000     // Bit 31, 보간 드라이브 데이타 오류 상태.
}

public enum ANT_MOTION_QIDRIVE_STATUS:uint
{
    N_QIDRIVE_STATUS_0                  = 0x0000001,     // Bit 0, BUSY(드라이브 구동 중)
    N_QIDRIVE_STATUS_1                  = 0x0000002,     // Bit 1, DOWN(감속 중)
    N_QIDRIVE_STATUS_2                  = 0x0000004,     // Bit 2, CONST(등속 중)
    N_QIDRIVE_STATUS_3                  = 0x0000008,     // Bit 3, UP(가속 중)
    N_QIDRIVE_STATUS_4                  = 0x0000010,     // Bit 4, 연속 드라이브 구동 중
    N_QIDRIVE_STATUS_5                  = 0x0000020,     // Bit 5, 지정 거리 드라이브 구동 중
    N_QIDRIVE_STATUS_6                  = 0x0000040,     // Bit 6, MPG 드라이브 구동 중
    N_QIDRIVE_STATUS_7                  = 0x0000080,     // Bit 7, 원점검색 드라이브 구동중
//주의사항 : (현재 라이브러리 에서 구현된 HOME함수와 상관없이 동작된다.)
    N_QIDRIVE_STATUS_8                  = 0x0000100,     // Bit 8, 신호 검색 드라이브 구동 중
//주의사항 : (현재 라이브러리 에서 구현된 HOME함수와 같이 동작된다. 홈함수가 신호서치함수를 여러번사용하여 구현이 되었으며 홈 검색시 BIT: 1 된다.)
    N_QIDRIVE_STATUS_9                  = 0x0000200,     // Bit 9, 보간 드라이브 구동 중
    N_QIDRIVE_STATUS_10                 = 0x0000400,     // Bit 10, Slave 드라이브 구동중
    N_QIDRIVE_STATUS_11                 = 0x0000800,     // Bit 11, 현재 구동 드라이브 방향(보간 드라이브에서는 표시 정보 다름)
    N_QIDRIVE_STATUS_12                 = 0x0001000,     // Bit 12, 펄스 출력후 서보위치 완료 신호 대기중.
    N_QIDRIVE_STATUS_13                 = 0x0002000,     // Bit 13, 직선 보간 드라이브 구동중.
    N_QIDRIVE_STATUS_14                 = 0x0004000,     // Bit 14, 원호 보간 드라이브 구동중.
    N_QIDRIVE_STATUS_15                 = 0x0008000,     // Bit 15, 펄스 출력 중.
    N_QIDRIVE_STATUS_16                 = 0x0010000,     // Bit 16, 구동 예약 데이터 개수(처음)(0-7)
    N_QIDRIVE_STATUS_17                 = 0x0020000,     // Bit 17, 구동 예약 데이터 개수(중간)(0-7)
    N_QIDRIVE_STATUS_18                 = 0x0040000,     // Bit 18, 구동 예약 데이터 갯수(끝)(0-7)
    N_QIDRIVE_STATUS_19                 = 0x0100000,     // Bit 19, 구동 예약 Queue 비어 있음.
    N_QIDRIVE_STATUS_20                 = 0x0200000,     // Bit 20, 구동 예약 Queue 가득 찲
    N_QIDRIVE_STATUS_21                 = 0x0400000,     // Bit 21, 현재 구동 드라이브의 속도 모드(처음)
    N_QIDRIVE_STATUS_22                 = 0x0800000,     // Bit 22, 현재 구동 드라이브의 속도 모드(끝)
    N_QIDRIVE_STATUS_23                 = 0x1000000,     // Bit 23, MPG 버퍼 #1 Full
    N_QIDRIVE_STATUS_24                 = 0x2000000,     // Bit 24, MPG 버퍼 #2 Full
    N_QIDRIVE_STATUS_25                 = 0x4000000,     // Bit 25, MPG 버퍼 #3 Full
    N_QIDRIVE_STATUS_26                 = 0x8000000      // Bit 26, MPG 버퍼 데이터 OverFlow
}

public enum ANT_MOTION_QIINTERRUPT_BANK1:uint
{
    N_QIINTBANK1_DISABLE                = 0x00000000,    // INTERRUT DISABLED.
    N_QIINTBANK1_0                      = 0x00000001,    // Bit 0,  인터럽트 발생 사용 설정된 구동 종료시.
    N_QIINTBANK1_1                      = 0x00000002,    // Bit 1,  구동 종료시
    N_QIINTBANK1_2                      = 0x00000004,    // Bit 2,  구동 시작시.
    N_QIINTBANK1_3                      = 0x00000008,    // Bit 3,  카운터 #1 < 비교기 #1 이벤트 발생
    N_QIINTBANK1_4                      = 0x00000010,    // Bit 4,  카운터 #1 = 비교기 #1 이벤트 발생
    N_QIINTBANK1_5                      = 0x00000020,    // Bit 5,  카운터 #1 > 비교기 #1 이벤트 발생
    N_QIINTBANK1_6                      = 0x00000040,    // Bit 6,  카운터 #2 < 비교기 #2 이벤트 발생
    N_QIINTBANK1_7                      = 0x00000080,    // Bit 7,  카운터 #2 = 비교기 #2 이벤트 발생
    N_QIINTBANK1_8                      = 0x00000100,    // Bit 8,  카운터 #2 > 비교기 #2 이벤트 발생
    N_QIINTBANK1_9                      = 0x00000200,    // Bit 9,  카운터 #3 < 비교기 #3 이벤트 발생
    N_QIINTBANK1_10                     = 0x00000400,    // Bit 10, 카운터 #3 = 비교기 #3 이벤트 발생
    N_QIINTBANK1_11                     = 0x00000800,    // Bit 11, 카운터 #3 > 비교기 #3 이벤트 발생
    N_QIINTBANK1_12                     = 0x00001000,    // Bit 12, 카운터 #4 < 비교기 #4 이벤트 발생
    N_QIINTBANK1_13                     = 0x00002000,    // Bit 13, 카운터 #4 = 비교기 #4 이벤트 발생
    N_QIINTBANK1_14                     = 0x00004000,    // Bit 14, 카운터 #4 < 비교기 #4 이벤트 발생
    N_QIINTBANK1_15                     = 0x00008000,    // Bit 15, 카운터 #5 < 비교기 #5 이벤트 발생
    N_QIINTBANK1_16                     = 0x00010000,    // Bit 16, 카운터 #5 = 비교기 #5 이벤트 발생
    N_QIINTBANK1_17                     = 0x00020000,    // Bit 17, 카운터 #5 > 비교기 #5 이벤트 발생
    N_QIINTBANK1_18                     = 0x00040000,    // Bit 18, 타이머 #1 이벤트 발생.
    N_QIINTBANK1_19                     = 0x00080000,    // Bit 19, 타이머 #2 이벤트 발생.
    N_QIINTBANK1_20                     = 0x00100000,    // Bit 20, 구동 예약 설정 Queue 비워짐.
    N_QIINTBANK1_21                     = 0x00200000,    // Bit 21, 구동 예약 설정 Queue 가득찲
    N_QIINTBANK1_22                     = 0x00400000,    // Bit 22, 트리거 발생거리 주기/절대위치 Queue 비워짐.
    N_QIINTBANK1_23                     = 0x00800000,    // Bit 23, 트리거 발생거리 주기/절대위치 Queue 가득찲
    N_QIINTBANK1_24                     = 0x01000000,    // Bit 24, 트리거 신호 발생 이벤트
    N_QIINTBANK1_25                     = 0x02000000,    // Bit 25, 스크립트 #1 명령어 예약 설정 Queue 비워짐.
    N_QIINTBANK1_26                     = 0x04000000,    // Bit 26, 스크립트 #2 명령어 예약 설정 Queue 비워짐.
    N_QIINTBANK1_27                     = 0x08000000,    // Bit 27, 스크립트 #3 명령어 예약 설정 레지스터 실행되어 초기화 됨.
    N_QIINTBANK1_28                     = 0x10000000,    // Bit 28, 스크립트 #4 명령어 예약 설정 레지스터 실행되어 초기화 됨.
    N_QIINTBANK1_29                     = 0x20000000,    // Bit 29, 서보 알람신호 인가됨.
    N_QIINTBANK1_30                     = 0x40000000,    // Bit 30, |CNT1| - |CNT2| >= |CNT4| 이벤트 발생.
    N_QIINTBANK1_31                     = 0x80000000     // Bit 31, 인터럽트 발생 명령어|INTGEN| 실행.
}

public enum ANT_MOTION_QIINTERRUPT_BANK2:uint
{
    N_QIINTBANK2_DISABLE                = 0x00000000,    // INTERRUT DISABLED.
    N_QIINTBANK2_0                      = 0x00000001,    // Bit 0,  스크립트 #1 읽기 명령 결과 Queue 가 가득찲.
    N_QIINTBANK2_1                      = 0x00000002,    // Bit 1,  스크립트 #2 읽기 명령 결과 Queue 가 가득찲.
    N_QIINTBANK2_2                      = 0x00000004,    // Bit 2,  스크립트 #3 읽기 명령 결과 레지스터가 새로운 데이터로 갱신됨.
    N_QIINTBANK2_3                      = 0x00000008,    // Bit 3,  스크립트 #4 읽기 명령 결과 레지스터가 새로운 데이터로 갱신됨.
    N_QIINTBANK2_4                      = 0x00000010,    // Bit 4,  스크립트 #1 의 예약 명령어 중 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
    N_QIINTBANK2_5                      = 0x00000020,    // Bit 5,  스크립트 #2 의 예약 명령어 중 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
    N_QIINTBANK2_6                      = 0x00000040,    // Bit 6,  스크립트 #3 의 예약 명령어 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
    N_QIINTBANK2_7                      = 0x00000080,    // Bit 7,  스크립트 #4 의 예약 명령어 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
    N_QIINTBANK2_8                      = 0x00000100,    // Bit 8,  구동 시작
    N_QIINTBANK2_9                      = 0x00000200,    // Bit 9,  서보 위치 결정 완료(Inposition)기능을 사용한 구동,종료 조건 발생.
    N_QIINTBANK2_10                     = 0x00000400,    // Bit 10, 이벤트 카운터로 동작 시 사용할 이벤트 선택 #1 조건 발생.
    N_QIINTBANK2_11                     = 0x00000800,    // Bit 11, 이벤트 카운터로 동작 시 사용할 이벤트 선택 #2 조건 발생.
    N_QIINTBANK2_12                     = 0x00001000,    // Bit 12, SQSTR1 신호 인가 됨.
    N_QIINTBANK2_13                     = 0x00002000,    // Bit 13, SQSTR2 신호 인가 됨.
    N_QIINTBANK2_14                     = 0x00004000,    // Bit 14, UIO0 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_15                     = 0x00008000,    // Bit 15, UIO1 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_16                     = 0x00010000,    // Bit 16, UIO2 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_17                     = 0x00020000,    // Bit 17, UIO3 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_18                     = 0x00040000,    // Bit 18, UIO4 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_19                     = 0x00080000,    // Bit 19, UIO5 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_20                     = 0x00100000,    // Bit 20, UIO6 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_21                     = 0x00200000,    // Bit 21, UIO7 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_22                     = 0x00400000,    // Bit 22, UIO8 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_23                     = 0x00800000,    // Bit 23, UIO9 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_24                     = 0x01000000,    // Bit 24, UIO10 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_25                     = 0x02000000,    // Bit 25, UIO11 터미널 신호가 '1'로 변함.
    N_QIINTBANK2_26                     = 0x04000000,    // Bit 26, 오류 정지 조건(LMT, ESTOP, STOP, ESTOP, CMD, ALARM) 발생.
    N_QIINTBANK2_27                     = 0x08000000,    // Bit 27, 보간 중 데이터 설정 오류 발생.
    N_QIINTBANK2_28                     = 0x10000000,    // Bit 28, Don't Care
    N_QIINTBANK2_29                     = 0x20000000,    // Bit 29, 리미트 신호(PELM, NELM)신호가 입력 됨.
    N_QIINTBANK2_30                     = 0x40000000,    // Bit 30, 부가 리미트 신호(PSLM, NSLM)신호가 입력 됨.
    N_QIINTBANK2_31                     = 0x80000000     // Bit 31, 비상 정지 신호(ESTOP)신호가 입력됨.
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
        nAxisNo = (nAxisNo - (nAxisNo % 2));        // 쌍을 이루는 축의 짝수축을 찾음

        return nAxisNo;
    }

    public static int  N_AXIS_ODD(int nAxisNo) 
    {
        nAxisNo = (nAxisNo + ((nAxisNo + 1) % 2));   // 쌍을 이루는 축의 홀수축을 찾음

        return nAxisNo;

    }

    public static int  N_AXIS_QUR(int nAxisNo) 
    {
        nAxisNo = (nAxisNo % 4);                     // 쌍을 이루는 축의 홀수축을 찾음

        return nAxisNo;
    }

    public static int  N_AXIS_N04(int nAxisNo, int nPos) 
    {
        nAxisNo = (((nAxisNo / 4) * 4) + nPos);       // 한 칩의 축 위치로 변경(0~3)

        return nAxisNo;
    }

    public static int  N_AXIS_N01(int nAxisNo) 
    {
        nAxisNo = ((nAxisNo % 4) >> 2);               // 0, 1축을 0으로 2, 3축을 1로 변경

        return nAxisNo;
    }

    public static int  N_AXIS_N02(int nAxisNo) 
    {
        nAxisNo = ((nAxisNo % 4)  % 2);               // 0, 2축을 0으로 1, 3축을 1로 변경

        return nAxisNo;
    }

    public static int    m_SendAxis            =0;    // 현재 축번호
}
