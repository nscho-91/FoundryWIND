using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ezTools
{
    public enum IppStatus
    {
        ippStsNotSupportedModeErr = -9999,  /* The requested mode is currently not supported  */
        ippStsCpuNotSupportedErr = -9998,  /* The target cpu is not supported */

        ippStsRoundModeNotSupportedErr = -213, /* Unsupported round mode*/
        ippStsDecimateFractionErr = -212, /* Unsupported fraction in Decimate */
        ippStsWeightErr = -211, /* Wrong value of weight */

        ippStsQualityIndexErr = -210, /* Quality Index can't be calculated for image filled with constant */
        ippStsIIRPassbandRippleErr = -209, /* Ripple in passband for Chebyshev1 design is less zero, equal to zero or greater than 29*/
        ippStsFilterFrequencyErr = -208, /* Cut of frequency of filter is less zero, equal to zero or greater than 0.5 */
        ippStsFIRGenOrderErr = -207, /* Order of an FIR filter for design them is less than one                    */
        ippStsIIRGenOrderErr = -206, /* Order of an IIR filter for design them is less than one or greater than 12 */

        ippStsConvergeErr = -205, /* The algorithm does not converge*/
        ippStsSizeMatchMatrixErr = -204, /* Unsuitable sizes of the source matrices*/
        ippStsCountMatrixErr = -203, /* Count value is negative or equal to 0*/
        ippStsRoiShiftMatrixErr = -202, /* RoiShift value is negative or not dividend to size of data type*/

        ippStsResizeNoOperationErr = -201, /* One of the output image dimensions is less than 1 pixel */
        ippStsSrcDataErr = -200, /* The source buffer contains unsupported data */
        ippStsMaxLenHuffCodeErr = -199, /* Huff: Max length of Huffman code is more than expected one */
        ippStsCodeLenTableErr = -198, /* Huff: Invalid codeLenTable */
        ippStsFreqTableErr = -197, /* Huff: Invalid freqTable */

        ippStsIncompleteContextErr = -196, /* Crypto: set up of context is'n complete */

        ippStsSingularErr = -195, /* Matrix is singular */
        ippStsSparseErr = -194, /* Tap positions are not in ascending order, negative or repeated*/
        ippStsBitOffsetErr = -193, /* Incorrect bit offset value */
        ippStsQPErr = -192, /* Incorrect quantization parameter */
        ippStsVLCErr = -191, /* Illegal VLC or FLC during stream decoding */
        ippStsRegExpOptionsErr = -190, /* RegExp: Options for pattern are incorrect */
        ippStsRegExpErr = -189, /* RegExp: The structure pRegExpState contains wrong data */
        ippStsRegExpMatchLimitErr = -188, /* RegExp: The match limit has been exhausted */
        ippStsRegExpQuantifierErr = -187, /* RegExp: wrong quantifier */
        ippStsRegExpGroupingErr = -186, /* RegExp: wrong grouping */
        ippStsRegExpBackRefErr = -185, /* RegExp: wrong back reference */
        ippStsRegExpChClassErr = -184, /* RegExp: wrong character class */
        ippStsRegExpMetaChErr = -183, /* RegExp: wrong metacharacter */


        ippStsStrideMatrixErr = -182,  /* Stride value is not positive or not dividend to size of data type */

        ippStsCTRSizeErr = -181,  /* Wrong value for crypto CTR block size */

        ippStsJPEG2KCodeBlockIsNotAttached = -180, /* codeblock parameters are not attached to the state structure */
        ippStsNotPosDefErr = -179,  /* Not positive-definite matrix */

        ippStsEphemeralKeyErr = -178, /* ECC: Bad ephemeral key   */
        ippStsMessageErr = -177, /* ECC: Bad message digest  */
        ippStsShareKeyErr = -176, /* ECC: Invalid share key   */
        ippStsIvalidPublicKey = -175, /* ECC: Invalid public key  */
        ippStsIvalidPrivateKey = -174, /* ECC: Invalid private key */
        ippStsOutOfECErr = -173, /* ECC: Point out of EC     */
        ippStsECCInvalidFlagErr = -172, /* ECC: Invalid Flag        */

        ippStsMP3FrameHeaderErr = -171,  /* Error in fields IppMP3FrameHeader structure */
        ippStsMP3SideInfoErr = -170,  /* Error in fields IppMP3SideInfo structure */

        ippStsBlockStepErr = -169,  /* Step for Block less than 8 */
        ippStsMBStepErr = -168,  /* Step for MB less than 16 */

        ippStsAacPrgNumErr = -167,  /* AAC: Invalid number of elements for one program   */
        ippStsAacSectCbErr = -166,  /* AAC: Invalid section codebook                     */
        ippStsAacSfValErr = -164,  /* AAC: Invalid scalefactor value                    */
        ippStsAacCoefValErr = -163,  /* AAC: Invalid quantized coefficient value          */
        ippStsAacMaxSfbErr = -162,  /* AAC: Invalid coefficient index  */
        ippStsAacPredSfbErr = -161,  /* AAC: Invalid predicted coefficient index  */
        ippStsAacPlsDataErr = -160,  /* AAC: Invalid pulse data attributes  */
        ippStsAacGainCtrErr = -159,  /* AAC: Gain control not supported  */
        ippStsAacSectErr = -158,  /* AAC: Invalid number of sections  */
        ippStsAacTnsNumFiltErr = -157,  /* AAC: Invalid number of TNS filters  */
        ippStsAacTnsLenErr = -156,  /* AAC: Invalid TNS region length  */
        ippStsAacTnsOrderErr = -155,  /* AAC: Invalid order of TNS filter  */
        ippStsAacTnsCoefResErr = -154,  /* AAC: Invalid bit-resolution for TNS filter coefficients  */
        ippStsAacTnsCoefErr = -153,  /* AAC: Invalid TNS filter coefficients  */
        ippStsAacTnsDirectErr = -152,  /* AAC: Invalid TNS filter direction  */
        ippStsAacTnsProfileErr = -151,  /* AAC: Invalid TNS profile  */
        ippStsAacErr = -150,  /* AAC: Internal error  */
        ippStsAacBitOffsetErr = -149,  /* AAC: Invalid current bit offset in bitstream  */
        ippStsAacAdtsSyncWordErr = -148,  /* AAC: Invalid ADTS syncword  */
        ippStsAacSmplRateIdxErr = -147,  /* AAC: Invalid sample rate index  */
        ippStsAacWinLenErr = -146,  /* AAC: Invalid window length (not short or long)  */
        ippStsAacWinGrpErr = -145,  /* AAC: Invalid number of groups for current window length  */
        ippStsAacWinSeqErr = -144,  /* AAC: Invalid window sequence range  */
        ippStsAacComWinErr = -143,  /* AAC: Invalid common window flag  */
        ippStsAacStereoMaskErr = -142,  /* AAC: Invalid stereo mask  */
        ippStsAacChanErr = -141,  /* AAC: Invalid channel number  */
        ippStsAacMonoStereoErr = -140,  /* AAC: Invalid mono-stereo flag  */
        ippStsAacStereoLayerErr = -139,  /* AAC: Invalid this Stereo Layer flag  */
        ippStsAacMonoLayerErr = -138,  /* AAC: Invalid this Mono Layer flag  */
        ippStsAacScalableErr = -137,  /* AAC: Invalid scalable object flag  */
        ippStsAacObjTypeErr = -136,  /* AAC: Invalid audio object type  */
        ippStsAacWinShapeErr = -135,  /* AAC: Invalid window shape  */
        ippStsAacPcmModeErr = -134,  /* AAC: Invalid PCM output interleaving indicator  */
        ippStsVLCUsrTblHeaderErr = -133,  /* VLC: Invalid header inside table */
        ippStsVLCUsrTblUnsupportedFmtErr = -132,  /* VLC: Unsupported table format */
        ippStsVLCUsrTblEscAlgTypeErr = -131,  /* VLC: Unsupported Ecs-algorithm */
        ippStsVLCUsrTblEscCodeLengthErr = -130,  /* VLC: Incorrect Esc-code length inside table header */
        ippStsVLCUsrTblCodeLengthErr = -129,  /* VLC: Unsupported code length inside table */
        ippStsVLCInternalTblErr = -128,  /* VLC: Invalid internal table */
        ippStsVLCInputDataErr = -127,  /* VLC: Invalid input data */
        ippStsVLCAACEscCodeLengthErr = -126,  /* VLC: Invalid AAC-Esc code length */
        ippStsNoiseRangeErr = -125,  /* Noise value for Wiener Filter is out range. */
        ippStsUnderRunErr = -124,  /* Data under run error */
        ippStsPaddingErr = -123,  /* Detected padding error shows the possible data corruption */
        ippStsOFBSizeErr = -122,  /* Wrong value for crypto OFB block size */
        ippStsCFBSizeErr = -122,  /* Wrong value for crypto CFB block size */
        ippStsPaddingSchemeErr = -121,  /* Invalid padding scheme  */
        ippStsInvalidCryptoKeyErr = -120,  /* A compromised key causes suspansion of requested cryptographic operation  */
        ippStsLengthErr = -119,  /* Wrong value of string length */
        ippStsBadModulusErr = -118,  /* Bad modulus caused a module inversion failure */
        ippStsLPCCalcErr = -117,  /* Linear prediction could not be evaluated */
        ippStsRCCalcErr = -116,  /* Reflection coefficients could not be computed */
        ippStsIncorrectLSPErr = -115,  /* Incorrect Linear Spectral Pair values */
        ippStsNoRootFoundErr = -114,  /* No roots are found for equation */
        ippStsJPEG2KBadPassNumber = -113,  /* Pass number exceeds allowed limits [0,nOfPasses-1] */
        ippStsJPEG2KDamagedCodeBlock = -112,  /* Codeblock for decoding is damaged */
        ippStsH263CBPYCodeErr = -111,  /* Illegal Huffman code during CBPY stream processing */
        ippStsH263MCBPCInterCodeErr = -110,  /* Illegal Huffman code during MCBPC Inter stream processing */
        ippStsH263MCBPCIntraCodeErr = -109,  /* Illegal Huffman code during MCBPC Intra stream processing */
        ippStsNotEvenStepErr = -108,  /* Step value is not pixel multiple */
        ippStsHistoNofLevelsErr = -107,  /* Number of levels for histogram is less than 2 */
        ippStsLUTNofLevelsErr = -106,  /* Number of levels for LUT is less than 2 */
        ippStsMP4BitOffsetErr = -105,  /* Incorrect bit offset value */
        ippStsMP4QPErr = -104,  /* Incorrect quantization parameter */
        ippStsMP4BlockIdxErr = -103,  /* Incorrect block index */
        ippStsMP4BlockTypeErr = -102,  /* Incorrect block type */
        ippStsMP4MVCodeErr = -101,  /* Illegal Huffman code during MV stream processing */
        ippStsMP4VLCCodeErr = -100,  /* Illegal Huffman code during VLC stream processing */
        ippStsMP4DCCodeErr = -99,   /* Illegal code during DC stream processing */
        ippStsMP4FcodeErr = -98,   /* Incorrect fcode value */
        ippStsMP4AlignErr = -97,   /* Incorrect buffer alignment            */
        ippStsMP4TempDiffErr = -96,   /* Incorrect temporal difference         */
        ippStsMP4BlockSizeErr = -95,   /* Incorrect size of block or macroblock */
        ippStsMP4ZeroBABErr = -94,   /* All BAB values are zero             */
        ippStsMP4PredDirErr = -93,   /* Incorrect prediction direction        */
        ippStsMP4BitsPerPixelErr = -92,   /* Incorrect number of bits per pixel    */
        ippStsMP4VideoCompModeErr = -91,   /* Incorrect video component mode        */
        ippStsMP4LinearModeErr = -90,   /* Incorrect DC linear mode */
        ippStsH263PredModeErr = -83,   /* Prediction Mode value error                                       */
        ippStsH263BlockStepErr = -82,   /* Step value is less than 8                                         */
        ippStsH263MBStepErr = -81,   /* Step value is less than 16                                        */
        ippStsH263FrameWidthErr = -80,   /* Frame width is less then 8                                        */
        ippStsH263FrameHeightErr = -79,   /* Frame height is less than or equal to zero                        */
        ippStsH263ExpandPelsErr = -78,   /* Expand pixels number is less than 8                               */
        ippStsH263PlaneStepErr = -77,   /* Step value is less than the plane width                           */
        ippStsH263QuantErr = -76,   /* Quantizer value is less than or equal to zero, or greater than 31 */
        ippStsH263MVCodeErr = -75,   /* Illegal Huffman code during MV stream processing                  */
        ippStsH263VLCCodeErr = -74,   /* Illegal Huffman code during VLC stream processing                 */
        ippStsH263DCCodeErr = -73,   /* Illegal code during DC stream processing                          */
        ippStsH263ZigzagLenErr = -72,   /* Zigzag compact length is more than 64                             */
        ippStsFBankFreqErr = -71,   /* Incorrect value of the filter bank frequency parameter */
        ippStsFBankFlagErr = -70,   /* Incorrect value of the filter bank parameter           */
        ippStsFBankErr = -69,   /* Filter bank is not correctly initialized"              */
        ippStsNegOccErr = -67,   /* Negative occupation count                      */
        ippStsCdbkFlagErr = -66,   /* Incorrect value of the codebook flag parameter */
        ippStsSVDCnvgErr = -65,   /* No convergence of SVD algorithm"               */
        ippStsJPEGHuffTableErr = -64,   /* JPEG Huffman table is destroyed        */
        ippStsJPEGDCTRangeErr = -63,   /* JPEG DCT coefficient is out of the range */
        ippStsJPEGOutOfBufErr = -62,   /* Attempt to access out of the buffer    */
        ippStsDrawTextErr = -61,   /* System error in the draw text operation */
        ippStsChannelOrderErr = -60,   /* Wrong order of the destination channels */
        ippStsZeroMaskValuesErr = -59,   /* All values of the mask are zero */
        ippStsQuadErr = -58,   /* The quadrangle is nonconvex or degenerates into triangle, line or point */
        ippStsRectErr = -57,   /* Size of the rectangle region is less than or equal to 1 */
        ippStsCoeffErr = -56,   /* Unallowable values of the transformation coefficients   */
        ippStsNoiseValErr = -55,   /* Bad value of noise amplitude for dithering"             */
        ippStsDitherLevelsErr = -54,   /* Number of dithering levels is out of range"             */
        ippStsNumChannelsErr = -53,   /* Bad or unsupported number of channels                   */
        ippStsCOIErr = -52,   /* COI is out of range */
        ippStsDivisorErr = -51,   /* Divisor is equal to zero, function is aborted */
        ippStsAlphaTypeErr = -50,   /* Illegal type of image compositing operation                           */
        ippStsGammaRangeErr = -49,   /* Gamma range bounds is less than or equal to zero                      */
        ippStsGrayCoefSumErr = -48,   /* Sum of the conversion coefficients must be less than or equal to 1    */
        ippStsChannelErr = -47,   /* Illegal channel number                                                */
        ippStsToneMagnErr = -46,   /* Tone magnitude is less than or equal to zero                          */
        ippStsToneFreqErr = -45,   /* Tone frequency is negative, or greater than or equal to 0.5           */
        ippStsTonePhaseErr = -44,   /* Tone phase is negative, or greater than or equal to 2*PI              */
        ippStsTrnglMagnErr = -43,   /* Triangle magnitude is less than or equal to zero                      */
        ippStsTrnglFreqErr = -42,   /* Triangle frequency is negative, or greater than or equal to 0.5       */
        ippStsTrnglPhaseErr = -41,   /* Triangle phase is negative, or greater than or equal to 2*PI          */
        ippStsTrnglAsymErr = -40,   /* Triangle asymmetry is less than -PI, or greater than or equal to PI   */
        ippStsHugeWinErr = -39,   /* Kaiser window is too huge                                             */
        ippStsJaehneErr = -38,   /* Magnitude value is negative                                           */
        ippStsStrideErr = -37,   /* Stride value is less than the row length */
        ippStsEpsValErr = -36,   /* Negative epsilon value error"            */
        ippStsWtOffsetErr = -35,   /* Invalid offset value of wavelet filter                                       */
        ippStsAnchorErr = -34,   /* Anchor point is outside the mask                                             */
        ippStsMaskSizeErr = -33,   /* Invalid mask size                                                           */
        ippStsShiftErr = -32,   /* Shift value is less than zero                                                */
        ippStsSampleFactorErr = -31,   /* Sampling factor is less than or equal to zero                                */
        ippStsSamplePhaseErr = -30,   /* Phase value is out of range: 0 <= phase < factor                             */
        ippStsFIRMRFactorErr = -29,   /* MR FIR sampling factor is less than or equal to zero                         */
        ippStsFIRMRPhaseErr = -28,   /* MR FIR sampling phase is negative, or greater than or equal to the sampling factor */
        ippStsRelFreqErr = -27,   /* Relative frequency value is out of range                                     */
        ippStsFIRLenErr = -26,   /* Length of a FIR filter is less than or equal to zero                         */
        ippStsIIROrderErr = -25,   /* Order of an IIR filter is not valid */
        ippStsDlyLineIndexErr = -24,   /* Invalid value of the delay line sample index */
        ippStsResizeFactorErr = -23,   /* Resize factor(s) is less than or equal to zero */
        ippStsInterpolationErr = -22,   /* Invalid interpolation mode */
        ippStsMirrorFlipErr = -21,   /* Invalid flip mode                                         */
        ippStsMoment00ZeroErr = -20,   /* Moment value M(0,0) is too small to continue calculations */
        ippStsThreshNegLevelErr = -19,   /* Negative value of the level in the threshold operation    */
        ippStsThresholdErr = -18,   /* Invalid threshold bounds */
        ippStsContextMatchErr = -17,   /* Context parameter doesn't match the operation */
        ippStsFftFlagErr = -16,   /* Invalid value of the FFT flag parameter */
        ippStsFftOrderErr = -15,   /* Invalid value of the FFT order parameter */
        ippStsStepErr = -14,   /* Step value is not valid */
        ippStsScaleRangeErr = -13,   /* Scale bounds are out of the range */
        ippStsDataTypeErr = -12,   /* Bad or unsupported data type */
        ippStsOutOfRangeErr = -11,   /* Argument is out of range or point is outside the image */
        ippStsDivByZeroErr = -10,   /* An attempt to divide by zero */
        ippStsMemAllocErr = -9,    /* Not enough memory allocated for the operation */
        ippStsNullPtrErr = -8,    /* Null pointer error */
        ippStsRangeErr = -7,    /* Bad values of bounds: the lower bound is greater than the upper bound */
        ippStsSizeErr = -6,    /* Wrong value of data size */
        ippStsBadArgErr = -5,    /* Function arg/param is bad */
        ippStsNoMemErr = -4,    /* Not enough memory for the operation */
        ippStsSAReservedErr3 = -3,    /*  */
        ippStsErr = -2,    /* Unknown/unspecified error */
        ippStsSAReservedErr1 = -1,    /*  */
        /*  */
        /* no errors */
        /*  */
        ippStsNoErr = 0,   /* No error, it's OK */
        /*  */
        /* warnings */
        /*  */
        ippStsNoOperation = 1,       /* No operation has been executed */
        ippStsMisalignedBuf = 2,       /* Misaligned pointer in operation in which it must be aligned */
        ippStsSqrtNegArg = 3,       /* Negative value(s) of the argument in the function Sqrt */
        ippStsInvZero = 4,       /* INF result. Zero value was met by InvThresh with zero level */
        ippStsEvenMedianMaskSize = 5,       /* Even size of the Median Filter mask was replaced by the odd one */
        ippStsDivByZero = 6,       /* Zero value(s) of the divisor in the function Div */
        ippStsLnZeroArg = 7,       /* Zero value(s) of the argument in the function Ln     */
        ippStsLnNegArg = 8,       /* Negative value(s) of the argument in the function Ln */
        ippStsNanArg = 9,       /* Not a Number argument value warning                  */
        ippStsJPEGMarker = 10,      /* JPEG marker was met in the bitstream                 */
        ippStsResFloor = 11,      /* All result values are floored                        */
        ippStsOverflow = 12,      /* Overflow occurred in the operation                   */
        ippStsLSFLow = 13,      /* Quantized LP syntethis filter stability check is applied at the low boundary of [0,pi] */
        ippStsLSFHigh = 14,      /* Quantized LP syntethis filter stability check is applied at the high boundary of [0,pi] */
        ippStsLSFLowAndHigh = 15,      /* Quantized LP syntethis filter stability check is applied at both boundaries of [0,pi] */
        ippStsZeroOcc = 16,      /* Zero occupation count */
        ippStsUnderflow = 17,      /* Underflow occurred in the operation */
        ippStsSingularity = 18,      /* Singularity occurred in the operation                                       */
        ippStsDomain = 19,      /* Argument is out of the function domain                                      */
        ippStsNonIntelCpu = 20,      /* The target cpu is not Genuine Intel                                         */
        ippStsCpuMismatch = 21,      /* The library for given cpu cannot be set                                     */
        ippStsNoIppFunctionFound = 22,      /* Application does not contain IPP functions calls                            */
        ippStsDllNotFoundBestUsed = 23,      /* The newest version of IPP dll's not found by dispatcher                     */
        ippStsNoOperationInDll = 24,      /* The function does nothing in the dynamic version of the library             */
        ippStsInsufficientEntropy = 25,      /* Insufficient entropy in the random seed and stimulus bit string caused the prime/key generation to fail */
        ippStsOvermuchStrings = 26,      /* Number of destination strings is more than expected                         */
        ippStsOverlongString = 27,      /* Length of one of the destination strings is more than expected              */
        ippStsAffineQuadChanged = 28,      /* 4th vertex of destination quad is not equal to customer's one               */
        ippStsWrongIntersectROI = 29,      /* Wrong ROI that has no intersection with the source or destination ROI. No operation */
        ippStsWrongIntersectQuad = 30,      /* Wrong quadrangle that has no intersection with the source or destination ROI. No operation */
        ippStsSmallerCodebook = 31,      /* Size of created codebook is less than cdbkSize argument */
        ippStsSrcSizeLessExpected = 32,      /* DC: The size of source buffer is less than expected one */
        ippStsDstSizeLessExpected = 33,      /* DC: The size of destination buffer is less than expected one */
        ippStsStreamEnd = 34,      /* DC: The end of stream processed */
        ippStsDoubleSize = 35,      /* Sizes of image are not multiples of 2 */
        ippStsNotSupportedCpu = 36,      /* The cpu is not supported */
        ippStsUnknownCacheSize = 37,      /* The cpu is supported, but the size of the cache is unknown */
        ippStsSymKernelExpected = 38,      /* The Kernel is not symmetric*/
        ippStsEvenMedianWeight = 39       /* Even weight of the Weighted Median Filter was replaced by the odd one */
    }

    public struct IppiSize
    {
        public int width;
        public int height;
    }

    public struct IppiRect
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    public class ippTools
    {
        int m_nSrc;
        byte[] m_aSrc;
        float[] m_aResult;

        public ippTools()
        {

        }

        static readonly object m_csLock = new object();

        unsafe public double ippiCrossCorrSame_NormLevel(ezImg rImgTemplate, ezImg rImgSrc, CPoint cpSrc, CPoint szSrc, ref CPoint cpCenter)
        {
            IppStatus iState;
            IppiSize iszSrc, iszTpl;
            iszSrc.width = szSrc.x;
            iszSrc.height = szSrc.y;
            iszTpl.width = rImgTemplate.m_szImg.x;
            iszTpl.height = rImgTemplate.m_szImg.y;
            int dstStep = szSrc.x * sizeof(float);
            ReAllocate(szSrc);
            CopySrc(rImgSrc, m_aSrc, cpSrc, szSrc);

            if (m_aSrc == null) return 0.0; // BHJ 190722 add
            lock(m_csLock)
            {
                fixed (byte* pSrc = &m_aSrc[0]) fixed (float* aResult = &m_aResult[0])
                {
                    iState = ippiCrossCorrSame_NormLevel_8u32f_C1R((IntPtr)pSrc, szSrc.x, iszSrc, (IntPtr)rImgTemplate.GetIntPtr(0, 0), iszTpl.width, iszTpl, (IntPtr)aResult, dstStep);
                }
            }
            if (iState != IppStatus.ippStsNoErr) return 0;
            int[] xMax = new int[1] { 0 };
            int[] yMax = new int[1] { 0 };
            float[] fMax = new float[1] { 0 };
            lock (m_csLock)
            {
                fixed (float* aResult = &m_aResult[0]) fixed (float* pMax = &fMax[0]) fixed (int* pMaxX = &xMax[0]) fixed (int* pMaxY = &yMax[0])
                {
                    iState = ippiMaxIndx_32f_C1R((IntPtr)aResult, dstStep, iszSrc, (IntPtr)pMax, (IntPtr)pMaxX, (IntPtr)pMaxY);
                }
            }
            if (iState != IppStatus.ippStsNoErr) return 0;
            cpCenter.x = xMax[0] + cpSrc.x;
            cpCenter.y = yMax[0] + cpSrc.y;
            return 100 * fMax[0];
        }

        unsafe public double ippiCrossCorrSame_NormLevel(double[] aTemplate, double[] aSrc, CPoint szTemp, CPoint szSrc)
        {
            IppStatus iState;
            IppiSize iszSrc, iszTpl;
            iszSrc.width = szSrc.x;
            iszSrc.height = szSrc.y;
            iszTpl.width = szTemp.x;
            iszTpl.height = szTemp.y;
            int dstStep = szSrc.x * sizeof(float);
            ReAllocate(szSrc);

            lock (m_csLock)
            {
                fixed (double* pSrc = &aSrc[0]) fixed (double* pTemp = &aTemplate[0]) fixed (float* aResult = &m_aResult[0])
                {
                    iState = ippiCrossCorrSame_NormLevel_32f_C1R((IntPtr)pSrc, szSrc.x, iszSrc, (IntPtr)pTemp, iszTpl.width, iszTpl, (IntPtr)aResult, dstStep);
                    //iState = ippiCrossCorrSame_NormLevel_8u32f_C1R((IntPtr)pSrc, szSrc.x, iszSrc, (IntPtr)pTemp, iszTpl.width, iszTpl, (IntPtr)aResult, dstStep);
                }
            }
            if (iState != IppStatus.ippStsNoErr) return 0;
            int[] xMax = new int[1] { 0 };
            int[] yMax = new int[1] { 0 };
            float[] fMax = new float[1] { 0 };
            float dScore = 0;
            lock (m_csLock)
            {
                fixed (float* aResult = &m_aResult[0]) fixed (float* pMax = &fMax[0]) fixed (int* pMaxX = &xMax[0]) fixed (int* pMaxY = &yMax[0])
                {
                    iState = ippiMaxIndx_32f_C1R((IntPtr)aResult, dstStep, iszSrc, (IntPtr)pMax, (IntPtr)pMaxX, (IntPtr)pMaxY);
                }
            }
            dScore = fMax[0] * (m_aResult.Length / 2 - Math.Abs(m_aResult.Length / 2 - xMax[0])) / (m_aResult.Length / 2);
            if (iState != IppStatus.ippStsNoErr) return 0;
            return 100 * dScore;
        }

        unsafe public void ippiCrossCorrForRingmark(ezImg rImgTemplate, ezImg rImgSrc, CPoint cpSrc, CPoint szSrc)
        {
            IppStatus iState;
            IppiSize iszSrc, iszTpl;
            iszSrc.width = szSrc.x;
            iszSrc.height = szSrc.y;
            iszTpl.width = rImgTemplate.m_szImg.x;
            iszTpl.height = rImgTemplate.m_szImg.y;
            int dstStep = szSrc.x * sizeof(float);
            ReAllocate(szSrc);
            CopySrc(rImgSrc, m_aSrc, cpSrc, szSrc);

            lock (m_csLock)
            {
                fixed (byte* pSrc = &m_aSrc[0]) fixed (float* aResult = &m_aResult[0])
                {
                    iState = ippiCrossCorrSame_NormLevel_8u32f_C1R((IntPtr)pSrc, szSrc.x, iszSrc, (IntPtr)rImgTemplate.GetIntPtr(0, 0), iszTpl.width, iszTpl, (IntPtr)aResult, dstStep);
                }
            }
            if (iState != IppStatus.ippStsNoErr) return;
            int[] xMax = new int[1] { 0 };
            int[] yMax = new int[1] { 0 };
            float[] fMax = new float[1] { 0 };
            lock (m_csLock)
            {
                for (int i = 0; i < szSrc.y; i++)
                {
                    for (int j = 0; j < szSrc.x; j++)
                    {
                        if ((m_aResult[i * szSrc.x + j]) < 0)
                            rImgSrc.m_aBuf[i, j] = 0;
                        else
                            rImgSrc.m_aBuf[i, j] = (byte)((m_aResult[i * szSrc.x + j]) * 256);
                    }

                }
            }
        }

        void ReAllocate(CPoint szSrc)
        {
	        int nSrc = szSrc.x * szSrc.y; 
	        if ((nSrc == 0) || (nSrc > m_nSrc)) 
	        { 
		        if (m_nSrc > 0) 
		        { 
			        Array.Clear(m_aSrc, 0, m_aSrc.Length);
                    Array.Clear(m_aResult, 0, m_aResult.Length);
		        }
                m_nSrc = 0; 
	        }
	        if (nSrc > m_nSrc) 
	        { 
		        m_aSrc = new byte[nSrc];
		        m_aResult = new float[nSrc]; 
		        m_nSrc = nSrc; 
	        }
        }

        void CopySrc(ezImg rImgSrc, byte[]aSrc, CPoint cpSrc, CPoint szSrc)
        {
            unsafe
            {
                byte* pSrc;
                for (int iy = 0, y = cpSrc.y; iy < szSrc.y; iy++, y++)
                {
                    fixed (byte* pDst = &aSrc[szSrc.x * iy])
                    {
                        pSrc = (byte*)rImgSrc.GetIntPtr(y, cpSrc.x);
                        cpp_memcpy(pDst, pSrc, szSrc.x);
                    }
                }
            }
        }

        unsafe public bool ippiRotate(ezImg rDst, CPoint szDst, Rectangle rtDst, ezImg rSrc, CPoint szSrc, Rectangle rtSrc, double dAngle, double xShift = 0, double yShift = 0, int nInterpolation = 1)
        {
            /*
            <nInterpolation>
            IPPI_INTER_NN     = 1,
            IPPI_INTER_LINEAR = 2,
            IPPI_INTER_CUBIC  = 4,
            IPPI_INTER_SUPER  = 8
            */
            IppStatus iState;
            IntPtr pDst, pSrc;
            IppiSize iszDst, iszSrc;
            IppiRect irtDst, irtSrc;
            int nSrcStep, nDstStep;
            iszDst.width = szDst.x; iszDst.height = szDst.y;
            iszSrc.width = szSrc.x; iszSrc.height = szSrc.y;
            irtDst.x = rtDst.X; irtDst.y = rtDst.Y; irtDst.width = rtDst.Width; irtDst.height = rtDst.Height;
            irtSrc.x = rtSrc.X; irtSrc.y = rtSrc.Y; irtSrc.width = rtSrc.Width; irtSrc.height = rtSrc.Height;
            nSrcStep = iszSrc.width * sizeof(byte);
            nDstStep = iszDst.width * sizeof(byte);
            pDst = rDst.GetIntPtr(rtDst.Y, rtDst.X);
            pSrc = rSrc.GetIntPtr(rtSrc.Y, rtSrc.X);
            iState = ippiRotateCenter_8u_C1R(pSrc, iszSrc, nSrcStep, irtSrc, pDst, nDstStep, irtDst, dAngle, xShift, yShift, nInterpolation);
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiAdd(int[] arrSrc, int[] arrSrcDst, int nScaleFactor = 0)
        {
            //arrSrcDst += arrSrc
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if (arrSrc == null || arrSrcDst == null) return true;
            iszROI = new IppiSize();
            iszROI.width = arrSrc.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int)) * sizeof(int);
            fixed (int* pSrc = &arrSrc[0]) fixed (int *pSrcDst = &arrSrcDst[0])
            {
                iState = ippiAdd_32sc_C1IRSfs((IntPtr)pSrc, nStep, (IntPtr)pSrcDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiSub(int[] arrSrc, int[] arrSrcDst, int nScaleFactor = 0)
        {
            //arrSrcDst -= arrSrc
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if (arrSrc == null || arrSrcDst == null) return true;
            iszROI = new IppiSize();
            iszROI.width = arrSrc.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (int* pSrc = &arrSrc[0]) fixed (int* pSrcDst = &arrSrcDst[0])
            {
                iState = ippiSub_32sc_C1IRSfs((IntPtr)pSrc, nStep, (IntPtr)pSrcDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiSubC(byte[] arrSrcDst, int nConstant, int nScaleFactor = 0)
        {
            //arrSrcDst -= nConstant
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if (arrSrcDst == null) return true;
            iszROI = new IppiSize();
            iszROI.width = arrSrcDst.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (byte* pSrcDst = &arrSrcDst[0])
            {
                iState = ippiSubC_8u_C1IRSfs(nConstant, (IntPtr)pSrcDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiSubC(byte[] arrSrc, byte[] arrDst, int nConstant, int nScaleFactor = 0)
        {
            //arrSrcDst -= nConstant
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if ((arrSrc == null) || (arrDst == null)) return true;
            iszROI = new IppiSize();
            iszROI.width = arrSrc.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (byte* pSrc = &arrSrc[0]) fixed (byte* pDst = &arrDst[0])
            {
                iState = ippiSubC_8u_C1RSfs((IntPtr)pSrc, nStep, nConstant, (IntPtr)pDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiMulC(int nConstance, int[] aSrc, int[] aDst, int nScaleFactor = 0)
        {
            //arrSrcDst *= dConstance
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if (aSrc == null) return true;
            if (aDst == null) return true;
            iszROI = new IppiSize();
            iszROI.width = aSrc.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (int* pDst = &aDst[0], pSrc = &aSrc[0])
            {
                iState = ippiMulC_32sc_C1RSfs((IntPtr)pSrc, nStep, nConstance, (IntPtr)pDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiMulC(int nConstance, int[] arrSrcDst, int nScaleFactor = 0)
        {
            //arrSrcDst *= dConstance
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if (arrSrcDst == null) return true;
            iszROI = new IppiSize();
            iszROI.width = arrSrcDst.Length;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (int* pSrcDst = &arrSrcDst[0])
            {
                iState = ippiMulC_32sc_C1RSfs((IntPtr)pSrcDst, nStep, nConstance, (IntPtr)pSrcDst, nStep, iszROI, nScaleFactor);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiCopy(byte[] aSrc, int nSrcStep, byte[] aDst, int nDstStep, int nCount)
        {
            //arrSrcDst *= dConstance
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if ((aSrc == null) || (aDst == null)) return true;
            iszROI = new IppiSize();
            iszROI.width = nCount;
            iszROI.height = 1;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (byte* pSrc = &aSrc[0]) fixed (byte* pDst = &aDst[0])
            {
                iState = ippiCopy_8u_C1R((IntPtr)pSrc, nSrcStep, (IntPtr)pDst, nDstStep, iszROI);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        unsafe public bool ippiTranspose(byte[] aSrc, byte[] aDst, CPoint szSrc)
        {
            //arrSrcDst *= dConstance
            int nStep;
            IppStatus iState;
            IppiSize iszROI;
            if ((aSrc == null) || (aDst == null)) return true;
            iszROI = new IppiSize();
            iszROI.width = szSrc.x;
            iszROI.height = szSrc.y;
            nStep = Marshal.SizeOf(typeof(int));
            fixed (byte* pSrc = &aSrc[0]) fixed (byte* pDst = &aDst[0])
            {
                iState = ippiTranspose_8u_C1R((IntPtr)pSrc, szSrc.x, (IntPtr)pDst, szSrc.y, iszROI);
            }
            if (iState != IppStatus.ippStsNoErr) return true;
            return false;
        }

        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiCrossCorrSame_NormLevel_8u32f_C1R(IntPtr pSrc, int srcStep, IppiSize srcRoiSize, IntPtr pTpl, int tplStep, IppiSize tpRoiSize, IntPtr pDst, int dstStep);
        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiCrossCorrSame_NormLevel_32f_C1R(IntPtr pSrc, int srcStep, IppiSize srcRoiSize, IntPtr pTpl, int tplStep, IppiSize tpRoiSize, IntPtr pDst, int dstStep);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiMaxIndx_32f_C1R(IntPtr pSrc, int srcStep, IppiSize roiSize, IntPtr pMax, IntPtr pIndexX, IntPtr pIndexY);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiRotateCenter_8u_C1R(IntPtr pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi, IntPtr pDst, int dstStep, IppiRect dstRoi, double angle, double xShift, double yShift, int interpolation);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiAdd_32sc_C1IRSfs(IntPtr pSrc, int srcStep, IntPtr pSrcDst, int srcDstStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiMulC_32sc_C1RSfs(IntPtr pSrc, int srcStep, int constance, IntPtr pSrcDst, int srcDstStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiSub_32sc_C1IRSfs(IntPtr pSrc, int srcStep, IntPtr pDst, int srcDstStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiSubC_32sc_C1IRSfs(int constance, IntPtr pSrc, int srcStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiSubC_8u_C1IRSfs(int constance, IntPtr pSrc, int srcStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiSubC_8u_C1RSfs(IntPtr pSrc, int srcStep, int constance, IntPtr pDst, int dstStep, IppiSize roiSize, int scaleFactor);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiCopy_8u_C1R(IntPtr pSrc, int srcStep, IntPtr pSrcDst, int srcDstStep, IppiSize roiSize);

        [DllImport("ippim7-5.3.dll")]
        unsafe public static extern IppStatus ippiTranspose_8u_C1R(IntPtr pSrc, int srcStep, IntPtr pSrcDst, int srcDstStep, IppiSize roiSrcSize);

    }
}
