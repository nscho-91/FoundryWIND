using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using ezTools;
using ezAutoMom;

namespace ezAuto_EFEM
{
    public class HW_Klarf
    {
        public string m_id;
        public bool m_bBusy = false;
        Auto_Mom m_auto;
        Log m_log;
        XGem300_Mom m_xGem;
        public ezImg[] m_img = new ezImg[2];
        public ezImg[] m_img90 = new ezImg[2];
        public ezImg[] m_imgDown = new ezImg[2];
        public ezImg[] m_imgDown90 = new ezImg[2];
        FileStream m_fs;
        TiffBitmapEncoder m_encoder;
        //Data
        char c_char34 = Convert.ToChar(34);
        DateTime m_timeResult;
        //CTime m_timeFile;
        public string m_strTiffSpec = "6.0 G R;";
        public string m_strInspectionStationVender = "ADVANCED TECHNOLOGY INC";
        public string m_strInspectionStationModel = "ATI004";
        public string m_strInspectionStationMachineID = "WIND";
        public string m_strSampleType = "WAFER";
        public string m_strTimeResult = "";
        public string m_strLotID = "";
        public string m_strCSTID = "";
        public string m_strDeviceID = "";
        public string m_strSetupID = "";
        public string m_strStepID = "";
        public string m_strWaferID = "";
        public string m_strRecipe = "";
        public string m_strSampleOrientationMarkTypeNotch = "NOTCH";
        public string m_strOrientationMarkLocation = "DOWN";
        public string m_strTiffFileName = "";
        public string m_strSampleTestPlan = "";
        public string m_strAreaPerTest = "";

        public int m_nSampleSize = 300;
        public int m_nFileVer1 = 1;
        public int m_nFileVer2 = 1;
        public int m_nKlarfRow = 0;
        public int m_nKlarfCol = 0;
        public int m_nSlot = 0;
        public int m_nInspectionTest = 1;
        public int m_nSampleTestCnt = 0;
        public int m_nDefectDieCnt = 0;
        public int m_nCenterX = 0;
        public int m_nCenterY = 0;

        public double m_fDiePitchX = 0.000000e+000;
        public double m_fDiePitchY = 0.000000e+000;
        public double m_fDieOriginX = 0.000000e+000;
        public double m_fDieOriginY = 0.000000e+000;
        public double m_fSampleCenterLocationX = 0.000000e+000;
        public double m_fSampleCenterLocationY = 0.000000e+000;
        public double m_fResolution = 20.0;
        public int m_nDown = 16;
        public bool m_bCoarseGrindMark = false;
        //bool m_bSaveADCBtimap = false;
        //string m_strPathBMP = "D:\\KlarfBMP";

        public HW_Klarf(string id)
        {
            m_id = id;
        }

        public void Init(Auto_Mom auto, Log log)
        {
            m_auto = auto;
            m_log = log;
            m_xGem = m_auto.ClassXGem();
            m_timeResult = DateTime.Now;
            m_img[0] = new ezImg(m_id + "0", m_log);
            m_img[1] = new ezImg(m_id + "1", m_log);
            m_imgDown[0] = new ezImg(m_id + "Down0", m_log);
            m_imgDown[1] = new ezImg(m_id + "Down1", m_log);
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_strTiffSpec, m_id, "TiffSpec", "Tiff Spec");
            rGrid.Set(ref m_strInspectionStationVender, m_id, "Vender", "Inspection Station ID");
            rGrid.Set(ref m_strInspectionStationModel, m_id, "Model", "Inspection Station Model");
            rGrid.Set(ref m_strInspectionStationMachineID, m_id, "MachineID", "Machine ID");
            rGrid.Set(ref m_strSampleType, m_id, "SampleType", "Sample Type");
            rGrid.Set(ref m_strDeviceID, m_id, "DeviceID", "Device ID");
            rGrid.Set(ref m_strSetupID, m_id, "SetupID", "Setup ID");
            rGrid.Set(ref m_strStepID, m_id, "StepID", "Step ID");
            rGrid.Set(ref m_strSampleOrientationMarkTypeNotch, m_id, "MarkType", "Sample Orientation Mark Type");
            rGrid.Set(ref m_strOrientationMarkLocation, m_id, "MarkLocation", "Orientation Mark Location");
            //rGrid.Set(ref m_bSaveADCBtimap, m_id, "SaveBmp", "Save Bitmap");
            //rGrid.Set(ref m_strPathBMP, m_id, "PathBitmap", "Path For Save Bitmap");
        }

        public bool SaveKlarf(string strFile, bool bSaveImg, Die[,] arrDie, double fTheta) // For Backside
        {
            m_bBusy = true;
            string str = "";
            bool bError = false;
            int x, y, nDefect = 0, nImg = 0, nDefectDie = 0;
            StreamWriter sw;
            m_nSampleTestCnt = 0;
            try
            {
                sw = new StreamWriter(new FileStream(strFile + ".001", FileMode.Create));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + ".001 Is Wrong Path !!");
                m_log.Add(ex.Message);
                m_bBusy = false;
                return true;
            }
            try
            {
                if (sw == null)
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("FileVersion " + m_nFileVer1.ToString() + " " + m_nFileVer2.ToString() + ";");
                if (Put_FileTimestamp(sw))
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("TiffSpec " + m_strTiffSpec + ";");
                if (bSaveImg) sw.WriteLine("InspectionStationID " + c_char34 + m_strInspectionStationVender +
                    c_char34 + " " + c_char34 + m_strInspectionStationModel +
                    c_char34 + " " + c_char34 + m_strInspectionStationMachineID + c_char34 + ";");
                sw.WriteLine("SampleType " + m_strSampleType + ";");
                Put_ResultTimestamp(sw);
                sw.WriteLine("LotID " + c_char34 + m_strLotID+"-"+ m_strCSTID + c_char34 + ";");
                WriteSampleSize(sw);
                sw.WriteLine("DeviceID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SetupID " + c_char34 + m_strRecipe + c_char34 + " " + m_strTimeResult + ";");
                sw.WriteLine("StepID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SampleOrientationMarkType " + m_strSampleOrientationMarkTypeNotch + ";");
                sw.WriteLine("OrientationMarkLocation " + m_strOrientationMarkLocation + ";");
                if (bSaveImg) sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                sw.WriteLine("DiePitch " + string.Format("{0:e}", m_fDiePitchX) + " " + string.Format("{0:e}", m_fDiePitchY) + ";");
                sw.WriteLine("DieOrigin " + m_fDieOriginX.ToString() + " " + m_fDieOriginY.ToString() + ";");
                sw.WriteLine("WaferID " + c_char34 + m_strWaferID + c_char34 + ";");
                sw.WriteLine("Slot " + m_nSlot.ToString("00") + ";");
                sw.WriteLine("SampleCenterLocation " + string.Format("{0:e}", m_fSampleCenterLocationX) + " " + string.Format("{0:e}", m_fSampleCenterLocationY) + ";");
                sw.WriteLine("InspectionTest " + m_nInspectionTest.ToString() + ";");
                for (y = 0; y < arrDie.GetLength(1); y++)
                {
                    for (x = 0; x < arrDie.GetLength(0); x++)
                    {
                        if (arrDie[x, y].m_bEnable)
                        {
                            m_nSampleTestCnt++;
                            str += arrDie[x, y].m_cpIndex.x.ToString() + " " + arrDie[x, y].m_cpIndex.y.ToString() + "\r\n";
                        }
                    }
                }
                sw.WriteLine("SampleTestPlan " + m_nSampleTestCnt.ToString() + ";");
                sw.Write(str);
                sw.WriteLine("AreaPerTest 1"); // HardCoding
                if (bSaveImg)
                    sw.WriteLine("DefectRecordSpec 17 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE IMAGECOUNT IMAGELIST;");
                else
                    sw.WriteLine("DefectRecordSpec 15 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE;");
                sw.Write("DefectList");
                for (int nTry = 0; nTry < 5; nTry++)
                {
                    try
                    {
                        m_fs = new FileStream(strFile + ".tif", FileMode.Create);
                        m_log.Add(strFile + ".tif Create.");
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        m_log.Add(strFile + ".tif Create Fail.");
                        m_log.Add(ex.Message);
                    }
                }
                m_encoder = new TiffBitmapEncoder();
                foreach (Die die in arrDie)
                {
                    if (!die.m_bEnable) continue;
                    if (die.m_aDefect.Count > 0) nDefectDie++;
                    bError = WriteDefect(sw, die, ref nDefect, ref nImg, bSaveImg, fTheta);
                    // RingMark, StainMark, CoarseGrindMark 
                }
                if (!bError && nImg > 0) m_encoder.Save(m_fs);
                else if (bError)
                {
                    m_log.Popup(strFile + " Save Error !!");
                    File.Delete(strFile + ".tif");
                }
                m_fs.Close();
                sw.WriteLine(";");
                sw.WriteLine("SummarySpec 5 TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
                sw.WriteLine("SummaryList");
                sw.WriteLine("1 " + nDefect.ToString() + " 0.0 " + m_nSampleTestCnt.ToString() + " " + nDefectDie.ToString());
                sw.WriteLine("EndOfFile;");
                sw.Close();
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + " Save Fail !!");
                m_log.Add(ex.Message);
                sw.Close();
                if (m_fs != null) m_fs.Close();
                m_bCoarseGrindMark = false;
                m_bBusy = false;
                return true;
            }
            m_bCoarseGrindMark = false;
            m_bBusy = false;
            return false;
        }

        public bool SaveKlarf(string strFile, bool bSaveImg, HW_Aligner_EBR_EBR data, Bitmap bmpResult) // For EBR // ing170215
        {
            m_bBusy = true;
            bool bError = false;
            int nDefect = 0, nImg = 0;
            StreamWriter sw;
            m_nSampleTestCnt = 1;
            ezStopWatch swTime = new ezStopWatch();
            try
            {
                sw = new StreamWriter(new FileStream(strFile + ".001", FileMode.Create));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + ".001 Is Wrong Path !!");
                m_log.Add(ex.Message);
                m_bBusy = false;
                return true;
            }
            try
            {
                if (sw == null)
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("FileVersion " + m_nFileVer1.ToString() + " " + m_nFileVer2.ToString() + ";");
                if (Put_FileTimestamp(sw))
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("TiffSpec " + m_strTiffSpec + ";");
                if (bSaveImg) sw.WriteLine("InspectionStationID " + c_char34 + m_strInspectionStationVender +
                    c_char34 + " " + c_char34 + m_strInspectionStationModel +
                    c_char34 + " " + c_char34 + m_strInspectionStationMachineID + c_char34 + ";");
                sw.WriteLine("SampleType " + m_strSampleType + ";");
                Put_ResultTimestamp(sw);
                //m_strLotID = m_strLotID.Substring(0, m_strLotID.IndexOf("_"));          //말도안되
                sw.WriteLine("LotID " + c_char34 + m_strLotID + "-" + m_strCSTID + c_char34 + ";");
                WriteSampleSize(sw);
                sw.WriteLine("DeviceID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SetupID " + c_char34 + m_strRecipe + c_char34 + " " + m_strTimeResult + ";");
                sw.WriteLine("StepID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SampleOrientationMarkType " + m_strSampleOrientationMarkTypeNotch + ";");
                sw.WriteLine("OrientationMarkLocation " + m_strOrientationMarkLocation + ";");
                if (bSaveImg) sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                sw.WriteLine("DiePitch " + string.Format("{0:e}", m_fDiePitchX) + " " + string.Format("{0:e}", m_fDiePitchY) + ";");
                sw.WriteLine("DieOrigin " + m_fDieOriginX.ToString() + " " + m_fDieOriginY.ToString() + ";");
                sw.WriteLine("WaferID " + c_char34 + m_strWaferID + c_char34 + ";");
                sw.WriteLine("Slot " + m_nSlot.ToString("00") + ";");
                sw.WriteLine("SampleCenterLocation " + string.Format("{0:e}", m_fSampleCenterLocationX) + " " + string.Format("{0:e}", m_fSampleCenterLocationY) + ";");
                sw.WriteLine("InspectionTest " + m_nInspectionTest.ToString() + ";");
                sw.WriteLine("SampleTestPlan " + m_nSampleTestCnt.ToString()); // ing 170307
                sw.WriteLine("0 0;");
                sw.WriteLine("AreaPerTest 1;"); // HardCoding // ing 170307

                if (bSaveImg)
                {
                    sw.WriteLine("TiffFilename " + m_strTiffFileName + ";"); // ing 170223
                    sw.WriteLine("DefectRecordSpec 17 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE IMAGECOUNT IMAGELIST;");
                }
                else
                    sw.WriteLine("DefectRecordSpec 15 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE;");
                sw.WriteLine("DefectList");
                m_fs = new FileStream(strFile + ".tif", FileMode.Create);
                m_encoder = new TiffBitmapEncoder();
                if (data.m_strEBRType == "Trim")
                {
                    foreach (HW_Aligner_EBR_Trim_Dat trim in data.m_aEBR)
                    {
                        bError = WriteDefect(sw, trim, ref nDefect, ref nImg, bSaveImg); // ing 170208
                    }
                }
                else
                {
                    if (data.USE_WRITE_DEFECT_DATA) // BHJ 190315 add
                    {
                        foreach (HW_Aligner_EBR_EBR_Dat ebr in data.m_aEBR)
                        {
                            bError = WriteDefect(sw, ebr, ref nDefect, ref nImg, bSaveImg); // ing 170208
                        }
                    }
                }
                if (!bError && data.USE_WRITE_DEFECT_DATA) // ing 170215
                {
                    BitmapSource bmpSorce;
                    nDefect++;
                    nImg++;
                    sw.WriteLine(nDefect.ToString() + " 0 0 0 0 0 0 0 0 0 1 0 100 0 0 1 1 ");
                    sw.Write(nImg.ToString() + " 0");
                    bmpSorce = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpResult.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    m_encoder.Compression = TiffCompressOption.None;
                    m_encoder.Frames.Add(BitmapFrame.Create(bmpSorce));
                    m_encoder.Save(m_fs);
                    m_fs.Close();
                }
                sw.WriteLine(";");
                sw.WriteLine("SummarySpec 5 TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
                sw.WriteLine("SummaryList");
                sw.WriteLine(m_nInspectionTest + " " + nDefect.ToString() + " " + m_nSampleTestCnt.ToString(" 0.00000 ") + m_nSampleTestCnt + " 0;"); // ing 170223
                sw.WriteLine("EndOfFile;");
                sw.Close();
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + " Save Fail !!");
                m_log.Add(ex.Message);
                sw.Close();
                m_fs.Close();
                m_bBusy = false;
                return true;
            }
            m_log.Add(strFile + " Save Time : " + swTime.Check());
            m_bBusy = false;
            return false;
        }

        // ing 170228
        public bool SaveKlarf(string strFile, bool bSaveImg, ArrayList arrDefect, int nID, int xOffset = 0, int yOffset = 0) // For Chipping
        {
            m_bBusy = true;
            bool bError = false;
            int nDefect = 0, nImg = 0;
            StreamWriter sw;
            m_nSampleTestCnt = 0;
            try
            {
                sw = new StreamWriter(new FileStream(strFile + ".001", FileMode.Create));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + ".001 Is Wrong Path !!");
                m_log.Add(ex.Message);
                m_bBusy = false;
                return true;
            }
            try
            {
                if (sw == null)
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("FileVersion " + m_nFileVer1.ToString() + " " + m_nFileVer2.ToString() + ";");
                if (Put_FileTimestamp(sw))
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("TiffSpec " + m_strTiffSpec + ";");
                if (bSaveImg) sw.WriteLine("InspectionStationID " + c_char34 + m_strInspectionStationVender +
                    c_char34 + " " + c_char34 + m_strInspectionStationModel +
                    c_char34 + " " + c_char34 + m_strInspectionStationMachineID + c_char34 + ";");
                sw.WriteLine("SampleType " + m_strSampleType + ";");
                Put_ResultTimestamp(sw);
                //m_strLotID = m_strLotID.Substring(0, m_strLotID.IndexOf("_"));          //말도안되
                sw.WriteLine("LotID " + c_char34 + m_strLotID + "-" + m_strCSTID + c_char34 + ";");
                WriteSampleSize(sw);
                sw.WriteLine("DeviceID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SetupID " + c_char34 + m_strRecipe + c_char34 + " " + m_strTimeResult + ";");
                sw.WriteLine("StepID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SampleOrientationMarkType " + m_strSampleOrientationMarkTypeNotch + ";");
                sw.WriteLine("OrientationMarkLocation " + m_strOrientationMarkLocation + ";");
                if (bSaveImg) sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                sw.WriteLine("DiePitch " + string.Format("{0:e}", m_fDiePitchX) + " " + string.Format("{0:e}", m_fDiePitchY) + ";");
                sw.WriteLine("DieOrigin " + m_fDieOriginX.ToString() + " " + m_fDieOriginY.ToString() + ";");
                sw.WriteLine("WaferID " + c_char34 + m_strWaferID + c_char34 + ";");
                sw.WriteLine("Slot " + m_nSlot.ToString("00") + ";");
                sw.WriteLine("SampleCenterLocation " + string.Format("{0:e}", m_fSampleCenterLocationX) + " " + string.Format("{0:e}", m_fSampleCenterLocationY) + ";");
                sw.WriteLine("InspectionTest " + m_nInspectionTest.ToString() + ";");
                m_nSampleTestCnt = 1;
                sw.WriteLine("SampleTestPlan " + m_nSampleTestCnt.ToString()); // ing 170307
                sw.Write("0 0;");
                sw.WriteLine("AreaPerTest 1;"); // HardCoding // ing 170307
                if (bSaveImg)
                {
                    sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                    sw.WriteLine("DefectRecordSpec 17 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE IMAGECOUNT IMAGELIST;");
                }
                else
                    sw.WriteLine("DefectRecordSpec 15 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE;");
                sw.Write("DefectList");
                m_fs = new FileStream(strFile + ".tif", FileMode.Create);
                m_encoder = new TiffBitmapEncoder();
                foreach (Defect defect in arrDefect)
                {
                    bError = WriteDefect(sw, defect, ref nDefect, ref nImg, bSaveImg, nID, xOffset, yOffset);
                }
                if (!bError && arrDefect.Count > 0) m_encoder.Save(m_fs);
                //else if (bError) File.Delete(strFile + ".tif");
                m_fs.Close();
                sw.WriteLine(";");
                sw.WriteLine("SummarySpec 5 TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
                sw.WriteLine("SummaryList");
                sw.WriteLine(m_nInspectionTest + " " + nDefect.ToString() + " " + m_nSampleTestCnt.ToString(" 0.00000 ") + m_nSampleTestCnt + " 0;"); // ing 170223
                sw.WriteLine("EndOfFile;");
                sw.Close();
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + " Save Fail !!");
                m_log.Add(ex.Message);
                sw.Close();
                m_fs.Close();
                m_bBusy = false;
                return true;
            }
            m_bBusy = false;
            return false;
        }

        // ing 170716
        public bool SaveKlarf(string strFile, bool bSaveImg, double dAngle) // For NotchAngle
        {
            string str;
            m_bBusy = true;
            StreamWriter sw;
            m_nSampleTestCnt = 0;
            try
            {
                sw = new StreamWriter(new FileStream(strFile + ".001", FileMode.Create));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + ".001 Is Wrong Path !!");
                m_log.Add(ex.Message);
                m_bBusy = false;
                return true;
            }
            try
            {
                if (sw == null)
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("FileVersion " + m_nFileVer1.ToString() + " " + m_nFileVer2.ToString() + ";");
                if (Put_FileTimestamp(sw))
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("TiffSpec " + m_strTiffSpec + ";");
                sw.WriteLine("InspectionStationID " + c_char34 + m_strInspectionStationVender +
                    c_char34 + " " + c_char34 + m_strInspectionStationModel +
                    c_char34 + " " + c_char34 + m_strInspectionStationMachineID + c_char34 + ";");
                sw.WriteLine("SampleType " + m_strSampleType + ";");
                Put_ResultTimestamp(sw);
                sw.WriteLine("LotID " + c_char34 + m_strLotID + "-" + m_strCSTID + c_char34 + ";");
                WriteSampleSize(sw);
                sw.WriteLine("DeviceID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SetupID " + c_char34 + m_strRecipe + c_char34 + " " + m_strTimeResult + ";");
                sw.WriteLine("StepID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SampleOrientationMarkType " + m_strSampleOrientationMarkTypeNotch + ";");
                sw.WriteLine("OrientationMarkLocation " + m_strOrientationMarkLocation + ";");
                if (bSaveImg) sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                sw.WriteLine("DiePitch " + string.Format("{0:e}", m_fDiePitchX) + " " + string.Format("{0:e}", m_fDiePitchY) + ";");
                sw.WriteLine("DieOrigin " + m_fDieOriginX.ToString() + " " + m_fDieOriginY.ToString() + ";");
                sw.WriteLine("WaferID " + c_char34 + m_strWaferID + c_char34 + ";");
                sw.WriteLine("Slot " + m_nSlot.ToString("00") + ";");
                sw.WriteLine("SampleCenterLocation " + string.Format("{0:e}", m_fSampleCenterLocationX) + " " + string.Format("{0:e}", m_fSampleCenterLocationY) + ";");
                sw.WriteLine("InspectionTest " + m_nInspectionTest.ToString() + ";");
                m_nSampleTestCnt = 1;
                sw.WriteLine("SampleTestPlan " + m_nSampleTestCnt.ToString()); // ing 170307
                sw.Write("0 0;");
                sw.WriteLine("AreaPerTest 1;"); // HardCoding // ing 170307
                if (bSaveImg)
                {
                    sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                    sw.WriteLine("DefectRecordSpec 17 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE IMAGECOUNT IMAGELIST;");
                }
                else
                    sw.WriteLine("DefectRecordSpec 15 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE;");
                sw.Write("DefectList");

                sw.WriteLine("");
                str = "1 ";
                str += string.Format("{0:e}", 0) + " " + string.Format("{0:e}", 0) + " ";
                str += "0 0 "; // Die Index
                str += "0.00 0.00 "; // X, Y Size
                str += "0 "; // Defect Area
                str += "0.00 "; // Defect Size
                str += "0 "; // DefectCode
                str += "1 "; // Test
                str += "0 "; // CLUSTERNUMBER
                str += dAngle.ToString("0.00") + " "; // ROUGHBINNUMBER 
                str += dAngle.ToString("0.00") + " "; // FINEBINNUMBER
                str += "0 "; // REVIEWSAMPLE  
                if (bSaveImg) str += "1 "; // IMAGECOUNT 
                if (bSaveImg) str += "1 "; // IMAGELIST; 
                sw.WriteLine(str);
                sw.Write("1 0"); // Only GrayColor
                if (bSaveImg)
                {
                    int nLength = 300;
                    Point[] pt = new Point[3];
                    Bitmap bmpNotch = new Bitmap(nLength, nLength, PixelFormat.Format24bppRgb);
                    pt[0] = new Point(nLength / 2 + (int)(Math.Sin(dAngle * Math.PI / 180) * (nLength / 2 - 10)), nLength / 2 + (int)(Math.Cos(dAngle * Math.PI / 180) * (nLength / 2 - 10)));
                    pt[1] = new Point(nLength / 2 + (int)(Math.Sin((dAngle + 3) * Math.PI / 180) * (nLength / 2)), nLength / 2 + (int)(Math.Cos((dAngle + 3) * Math.PI / 180) * (nLength / 2)));
                    pt[2] = new Point(nLength / 2 + (int)(Math.Sin((dAngle - 3) * Math.PI / 180) * (nLength / 2)), nLength / 2 + (int)(Math.Cos((dAngle - 3) * Math.PI / 180) * (nLength / 2)));
                    Graphics g = Graphics.FromImage(bmpNotch);
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), bmpNotch.Size));
                    g.FillPie(new SolidBrush(Color.LightGray), new Rectangle(new Point(0, 0), bmpNotch.Size), 0, 360);
                    g.FillPolygon(new SolidBrush(Color.White), pt);
                    g.DrawString(dAngle.ToString("0.0") + " Dgree", new Font("Arial", 10), new SolidBrush(Color.Red), 5, 5);
                    g.Dispose();
                    bmpNotch.Save("D:\\NotchAngle.bmp");
                    m_fs = new FileStream(strFile + ".tif", FileMode.Create);
                    m_encoder = new TiffBitmapEncoder();
                    MemoryStream streamBitmap = new MemoryStream();
                    bmpNotch.Save(streamBitmap, System.Drawing.Imaging.ImageFormat.Bmp);
                    m_encoder.Compression = TiffCompressOption.None;
                    m_encoder.Frames.Add(BitmapFrame.Create(streamBitmap));
                    m_encoder.Save(m_fs);
                    m_fs.Close();
                }
                sw.WriteLine(";");
                sw.WriteLine("SummarySpec 5 TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
                sw.WriteLine("SummaryList");
                sw.WriteLine(m_nInspectionTest + " 1 " + m_nSampleTestCnt.ToString(" 0.00000 ") + m_nSampleTestCnt + " 0;"); // ing 170223
                sw.WriteLine("EndOfFile;");
                sw.Close();
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + " Save Fail !!");
                m_log.Add(ex.Message);
                sw.Close();
                m_fs.Close();
                m_bBusy = false;
                return true;
            }
            m_bBusy = false;
            return false;
        }

        // ing 170814
        // For Chipping in 천안
        public bool SaveKlarf(string strFile, bool bSaveImg, object[] aAOI)
        {
            int nDefect = 0, nImg = 0;
            bool bError = false;
            m_bBusy = true;
            StreamWriter sw;
            m_nSampleTestCnt = 0;
            try
            {
                sw = new StreamWriter(new FileStream(strFile + ".001", FileMode.Create));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + ".001 Is Wrong Path !!");
                m_log.Add(ex.Message);
                m_bBusy = false;
                return true;
            }
            try
            {
                if (sw == null)
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("FileVersion " + m_nFileVer1.ToString() + " " + m_nFileVer2.ToString() + ";");
                if (Put_FileTimestamp(sw))
                {
                    m_bBusy = false;
                    return true;
                }
                sw.WriteLine("TiffSpec " + m_strTiffSpec + ";");
                sw.WriteLine("InspectionStationID " + c_char34 + m_strInspectionStationVender +
                    c_char34 + " " + c_char34 + m_strInspectionStationModel +
                    c_char34 + " " + c_char34 + m_strInspectionStationMachineID + c_char34 + ";");
                sw.WriteLine("SampleType " + m_strSampleType + ";");
                Put_ResultTimestamp(sw);
                sw.WriteLine("LotID " + c_char34 + m_strLotID + "-" + m_strCSTID + c_char34 + ";");
                WriteSampleSize(sw);
                sw.WriteLine("DeviceID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SetupID " + c_char34 + m_strRecipe + c_char34 + " " + m_strTimeResult + ";");
                sw.WriteLine("StepID " + c_char34 + m_strRecipe + c_char34 + ";");
                sw.WriteLine("SampleOrientationMarkType " + m_strSampleOrientationMarkTypeNotch + ";");
                sw.WriteLine("OrientationMarkLocation " + m_strOrientationMarkLocation + ";");
                if (bSaveImg) sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                sw.WriteLine("DiePitch " + string.Format("{0:e}", m_fDiePitchX) + " " + string.Format("{0:e}", m_fDiePitchY) + ";");
                sw.WriteLine("DieOrigin " + m_fDieOriginX.ToString() + " " + m_fDieOriginY.ToString() + ";");
                sw.WriteLine("WaferID " + c_char34 + m_strWaferID + c_char34 + ";");
                sw.WriteLine("Slot " + m_nSlot.ToString("00") + ";");
                sw.WriteLine("SampleCenterLocation " + string.Format("{0:e}", m_fSampleCenterLocationX) + " " + string.Format("{0:e}", m_fSampleCenterLocationY) + ";");
                sw.WriteLine("InspectionTest " + m_nInspectionTest.ToString() + ";");
                m_nSampleTestCnt = 1;
                sw.WriteLine("SampleTestPlan " + m_nSampleTestCnt.ToString());
                sw.Write("0 0;");
                sw.WriteLine("AreaPerTest 1;");
                if (bSaveImg)
                {
                    sw.WriteLine("TiffFilename " + m_strTiffFileName + ";");
                    sw.WriteLine("DefectRecordSpec 17 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE IMAGECOUNT IMAGELIST;");
                }
                else
                    sw.WriteLine("DefectRecordSpec 15 DEFECTID XREL YREL XINDEX YINDEX XSIZE YSIZE DEFECTAREA DSIZE CLASSNUMBER TEST CLUSTERNUMBER ROUGHBINNUMBER FINEBINNUMBER REVIEWSAMPLE;");
                sw.Write("DefectList");
                m_fs = new FileStream(strFile + ".tif", FileMode.Create);
                m_encoder = new TiffBitmapEncoder();
                foreach (HW_Aligner_ATI_AOI aoi in aAOI)
                {
                    for (int n = 0; n < aoi.m_aChippingInfo.Count; n++)
                    {
                        bError = WriteDefect(sw, (Defect)aoi.m_aChippingInfo[n], aoi.m_img, ref nDefect, ref nImg, bSaveImg);
                    }
                }
                if (!bError && nDefect > 0) m_encoder.Save(m_fs);
                m_fs.Close();
                sw.WriteLine(";");
                sw.WriteLine("SummarySpec 5 TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
                sw.WriteLine("SummaryList");
                sw.WriteLine(m_nInspectionTest + " " + nDefect.ToString() + " " + m_nSampleTestCnt.ToString(" 0.00000 ") + m_nSampleTestCnt + " 0;"); // ing 170223
                sw.WriteLine("EndOfFile;");
                sw.Close();
            }
            catch (System.Exception ex)
            {
                m_log.Popup(strFile + " Save Fail !!");
                m_log.Add(ex.Message);
                sw.Close();
                m_fs.Close();
                m_bBusy = false;
                return true;
            }
            m_bBusy = false;
            m_log.Add("Save Done : " + strFile);
            return false;
        }

        public bool Put_FileTimestamp(StreamWriter sw) // ing 170208
        {
            if (sw == null) return true;
            DateTime time = DateTime.Now;
            sw.WriteLine("FileTimestamp " + time.Month.ToString("00") + "-" + time.Day.ToString("00") + "-" + (time.Year % 100).ToString("00") + " " +
                time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" + time.Second.ToString("00") + ";");
            return false;
        }

        public void LotStart() // ing 170208
        {
            m_timeResult = DateTime.Now;
        }

        public bool Put_ResultTimestamp(StreamWriter sw) // ing 170208
        {
            if (sw == null) return true;
            m_strTimeResult = m_timeResult.Month.ToString("00") + "-" + m_timeResult.Day.ToString("00") + "-" + (m_timeResult.Year % 100).ToString("00") + " " +
                m_timeResult.Hour.ToString("00") + ":" + m_timeResult.Minute.ToString("00") + ":" + m_timeResult.Second.ToString("00");
            sw.WriteLine("ResultTimestamp " + m_strTimeResult + ";");
            return false;
        }

        bool WriteSampleSize(StreamWriter sw)
        {
            string strTemp;
            if (sw == null) return true;
            if (m_nSampleSize > 170 && m_nSampleSize < 230)
                strTemp = "1 200;";
            else if (m_nSampleSize > 110 && m_nSampleSize <= 170)
                strTemp = "1 150;";
            else
                strTemp = "1 " + m_nSampleSize.ToString();

            sw.WriteLine("SampleSize " + strTemp + ";");
            return false;
        }

        bool WriteDefect(StreamWriter sw, Die die, ref int nIndex, ref int nImg, bool bSaveImg, double fTheta) // For Backside
        {
            if (sw == null) return true;
            string str;
            CPoint cpDef;
            RPoint rpDef;
            CPoint cp;
            foreach (Defect def in die.m_aDefect)
            {
                nIndex++;
                nImg++;
                //sw.WriteLine("");
                cp = def.m_cp0 - def.m_cp1;
                cpDef = def.GetCenter() * m_fResolution;
                if (def.m_eDefType != eDefType.None)
                {
                    cpDef = new CPoint(0, 0);
                }
                rpDef = new RPoint(cpDef);
                str = nIndex.ToString() + " ";
                str += string.Format("{0:e}", rpDef.x) + " " + string.Format("{0:e}", rpDef.y) + " ";
                str += def.m_cpDieIndex.x.ToString() + " " + def.m_cpDieIndex.y.ToString() + " "; // Die Index
                str += Math.Abs(cp.x * m_fResolution).ToString("0.000") + " " + Math.Abs(cp.y * m_fResolution).ToString("0.000") + " ";
                str += def.m_nSize.ToString() + " "; // Defect Area
                str += (def.m_nSize * Math.Sqrt(2) * m_fResolution).ToString("0.000") + " "; // Defect Size
                if (def.m_strCode == "") def.m_strCode = "000";
                str += def.m_strCode.Trim() + " "; // DefectCode
                //str += "0 "; // DefectCode
                str += "1 "; // Test
                str += "0 "; // CLUSTERNUMBER
                string sRoughBin = "1 ";
                string sFineBin = "0 ";
                string sReviewSample = "0";
                //str += "100 "; // ROUGHBINNUMBER 
                //str += "0 "; // FINEBINNUMBER 
                //str += "0"; // REVIEWSAMPLE  
                if (!m_bCoarseGrindMark)
                {
                    if (def.m_eDefType != eDefType.None)
                    {
                        if (HW_BackSide_ATI_ADC.Instance.UseAdc == true)
                        {
                            if (def.m_eDefType == eDefType.Edge)
                            {
                                str += HW_BackSide_ATI_ADC.Instance.GetRoughbin("edge chipping");
                            }
                            else if (def.m_eDefType == eDefType.StainMark)
                            {
                                str += HW_BackSide_ATI_ADC.Instance.GetRoughbin("stain mark");
                            }
                            else if (def.m_eDefType == eDefType.RingMark)
                            {
                                str += HW_BackSide_ATI_ADC.Instance.GetRoughbin("ring mark");
                            }

                            str += sFineBin;
                            str += sReviewSample;
                        }
                        else
                        {
                            str += sRoughBin;
                            str += sFineBin;
                            str += sReviewSample;
                        }
                        if (bSaveImg) // IMAGECOUNT  
                        {
                            if (def.m_nRotateMode % 2 == 0) str += " 2 2 ";
                            else str += " 1 1 ";
                        }
                        sw.WriteLine("\r\n" + str);
                        sw.Write(nImg.ToString() + " 0"); // Only GrayColor

                        if (def.m_eDefType == eDefType.Edge)
                        {
                            if (def.m_nRotateMode % 2 == 0)
                            {

                                if (AddDefectImage(m_img[0], def.m_dieChipping, 0)) return true;
                                if (AddDefectImage(m_img[1], def.m_dieChipping, 0)) return true;
                                nImg++;
                                sw.WriteLine();
                                sw.Write(nImg.ToString() + " 0"); // Only GrayColor
                            }
                            else
                            {
                                if (AddDefectImage(m_img90[0], def.m_dieChipping, 0)) return true;
                                //if (AddDefectImage(m_img90[1], def.m_dieChipping, 0)) return true;
                            }
                        }
                        else
                        {
                            if (def.m_nRotateMode % 2 == 0)
                            {
                                if (AddBigDefectImage(m_imgDown[1], die, fTheta, def.m_eDefType.ToString())) return true;
                                if (AddBigDefectImage(m_imgDown[0], die, fTheta, def.m_eDefType.ToString())) return true;
                                nImg++;
                                sw.WriteLine();
                                sw.Write(nImg.ToString() + " 0"); // Only GrayColor
                            }
                            else
                            {
                                if (AddBigDefectImage(m_imgDown90[0], die, fTheta, def.m_eDefType.ToString())) return true;
                                //if (AddBigDefectImage(m_imgDown90[1], die, fTheta, def.m_eDefType.ToString())) return true;
                            }
                        }
                    }
                    else
                    {
                        /*if (HW_BackSide_ATI_ADC.Instance.UseAdc == true)
                        {
                            string name1 = string.Empty;
                            string name2 = string.Empty;
                            double score1 = 0.0f;
                            double score2 = 0.0f;                          

                            Rectangle rtDie = new Rectangle((die.m_cpRB.X + die.m_cpLT.X) / 2, (die.m_cpRB.Y + die.m_cpLT.Y) / 2, Math.Abs(die.m_cpRB.X - die.m_cpLT.X), Math.Abs(die.m_cpRB.Y - die.m_cpLT.Y));                           
                            Bitmap corpped = null;
                            ezStopWatch sw1 = new ezStopWatch();
                            if (def.m_nRotateMode % 2 == 0)
                            {
                                corpped = m_img[0].GetBitmap().Clone(rtDie, m_img[0].GetBitmap().PixelFormat);
                                ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out name1, out score1, m_log);
                                m_log.Add("Clssify Time : " + sw1.Check().ToString());

                                //corpped = m_img[1].GetBitmap().Clone(rtDie, m_img[1].GetBitmap().PixelFormat);
                                //ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out name2, out score2, m_log);

                                if (score1 > HW_BackSide_ATI_ADC.Instance.Score || score2 > HW_BackSide_ATI_ADC.Instance.Score)
                                {
                                    if (score1 > score2)
                                    {
                                        str += HW_BackSide_ATI_ADC.Instance.GetRoughbin(name1);
                                    }
                                    else
                                    {
                                        str += HW_BackSide_ATI_ADC.Instance.GetRoughbin(name2);
                                    }
                                }
                                else
                                {
                                    str += sRoughBin;
                                }
                            }
                            else
                            {
                                corpped = m_img[0].GetBitmap().Clone(rtDie, m_img[0].GetBitmap().PixelFormat);
                                ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out name1, out score1, m_log);
                                m_log.Add("Clssify Time : " + sw1.Check().ToString());

                                if (score1 > HW_BackSide_ATI_ADC.Instance.Score)
                                {
                                    str += HW_BackSide_ATI_ADC.Instance.GetRoughbin(name1);
                                }
                                else
                                {
                                    str += sRoughBin;
                                }
                            }
                            str += sFineBin;
                            str += sReviewSample;
                        }
                        else
                        {
                            str += sRoughBin;
                            str += sFineBin;
                            str += sReviewSample;
                        }
                        */
                        str += def.m_strCode + " ";
                        str += sFineBin;
                        str += sReviewSample;
                        if (bSaveImg) // IMAGECOUNT  
                        {
                            if (def.m_nRotateMode % 2 == 0) str += " 2 2 ";
                            else str += " 1 1 ";
                        }
                        sw.WriteLine("\r\n" + str);
                        sw.Write(nImg.ToString() + " 0"); // Only GrayColor

                        if (def.m_nRotateMode % 2 == 0)
                        {
                            ezStopWatch sw1 = new ezStopWatch();
                            if (AddDefectImage(m_img[0], die, fTheta, "0")) return true;
                            if (AddDefectImage(m_img[1], die, fTheta, "1")) return true;
                            m_log.Add("Tiff Add Time : " + sw1.Check().ToString());
                            nImg++;
                            sw.WriteLine();
                            sw.Write(nImg.ToString() + " 0"); // Only GrayColor
                        }
                        else
                        {
                            ezStopWatch sw1 = new ezStopWatch();
                            if (AddDefectImage(m_img90[0], die, fTheta, "90")) return true;
                            //if (AddDefectImage(m_img90[1], die, fTheta)) return true;
                            m_log.Add("Tiff Add Time : " + sw1.Check().ToString());
                        }
                    }
                }
                else
                {
                    str += HW_BackSide_ATI_ADC.Instance.GetRoughbin("CoarseGrindMark");
                    str += sFineBin;
                    str += sReviewSample;
                    sw.Write("\r\n" + str);
                }
                if (def.m_eDefType == eDefType.CoarseGrindMark) m_bCoarseGrindMark = true;
            }
            return false;
        }

        // ing 170208
        bool WriteDefect(StreamWriter sw, HW_Aligner_EBR_EBR_Dat ebr, ref int nIndex, ref int nImg, bool bSaveImg) // For EBR // ing 170208
        {
            if (sw == null) return true;
            string str;
            CPoint cpDef;
            RPoint rpDef;
            CPoint cp;
            Defect def = ebr.m_defect;
            nIndex++;
            if (bSaveImg && ebr.m_bImg) nImg++;
            //sw.WriteLine("");
            cp = def.m_cp0 - def.m_cp1;
            cpDef = def.GetCenter() * m_fResolution;
            rpDef = new RPoint(cpDef);
            str = nIndex.ToString() + " ";
            str += string.Format("{0:e}", def.m_cpOffset.x) + " " + string.Format("{0:e}", def.m_cpOffset.y) + " ";
            str += def.m_cpDieIndex.x.ToString() + " " + def.m_cpDieIndex.y.ToString() + " "; // Die Index
            str += ebr.m_fEBR.ToString("0.00") + " 0 ";
            str += ebr.m_fEBR.ToString("0.00") + " "; // Defect Area
            str += ebr.m_fEBR.ToString("0.00") + " "; // Defect Size
            str += "0 "; // DefectCode
            str += "1 "; // Test
            str += "0 "; // CLUSTERNUMBER
            //str += "100 "; // ROUGHBINNUMBER 
            str += ebr.m_fEBR.ToString("0.00") + " "; // ROUGHBINNUMBER // ing 170223
            str += ebr.m_fAngle.ToString("0.00") + " "; // FINEBINNUMBER // ing 170223
            str += "0 "; // REVIEWSAMPLE  
            if (bSaveImg && ebr.m_bImg) str += "1 "; // IMAGECOUNT 
            else str += "0 ";
            if (bSaveImg && ebr.m_bImg) str += "1 "; // IMAGELIST; 
            else str += "0 ";
            sw.WriteLine(str);
            if (ebr.m_bImg) sw.WriteLine(nImg.ToString() + " 0"); // Only GrayColor
            //else sw.Write("0 0");
            if (ebr.m_bImg && AddTIF(m_img[0], m_fs, m_encoder, ebr, "Angle : " + ebr.m_fAngle.ToString("0.00") + "\r\nEBR : " + ebr.m_fEBR.ToString("0.00 um") + "\r\nBevel : " + ebr.m_fBevel.ToString("0.00 um"), true))
            {
                m_encoder.Save(m_fs);
                m_fs.Close();
                return true; // ing ROI 조절
            }
            return false;
        }

        // ing 170829
        // For Trim
        bool WriteDefect(StreamWriter sw, HW_Aligner_EBR_Trim_Dat trim, ref int nIndex, ref int nImg, bool bSaveImg)
        {
            if (sw == null) return true;
            string str;
            CPoint cpDef;
            RPoint rpDef;
            CPoint cp;
            Defect def = trim.m_defect;
            nIndex++;
            nImg++;
            sw.WriteLine("");
            cp = def.m_cp0 - def.m_cp1;
            cpDef = def.GetCenter() * m_fResolution;
            rpDef = new RPoint(cpDef);
            str = nIndex.ToString() + " ";
            str += string.Format("{0:e}", def.m_cpOffset.x) + " " + string.Format("{0:e}", def.m_cpOffset.y) + " ";
            str += def.m_cpDieIndex.x.ToString() + " " + def.m_cpDieIndex.y.ToString() + " "; // Die Index
            str += trim.m_fTrim.ToString("0.00") + " 0 ";
            str += trim.m_fTrim.ToString("0.00") + " "; // Defect Area
            str += trim.m_fTrim.ToString("0.00") + " "; // Defect Size
            str += "0 "; // DefectCode
            str += "1 "; // Test
            str += "0 "; // CLUSTERNUMBER
            //str += "100 "; // ROUGHBINNUMBER 
            str += trim.m_fTrim.ToString("0.00") + " "; // FINEBINNUMBER // ing 170223
            str += trim.m_fAngle.ToString("0.00") + " "; // FINEBINNUMBER // ing 170223
            str += "0 "; // REVIEWSAMPLE  
            if (bSaveImg) str += "1 "; // IMAGECOUNT 
            if (bSaveImg) str += "1 "; // IMAGELIST; 
            sw.WriteLine(str);
            sw.Write(nImg.ToString() + " 0"); // Only GrayColor
            if (AddTIF(m_img[0], m_fs, m_encoder, trim, "Angle : " + trim.m_fAngle.ToString("0.00") + "\r\nTrim : " + trim.m_fTrim.ToString("0.00 um")))
            {
                m_encoder.Save(m_fs);
                m_fs.Close();
                return true; // ing ROI 조절
            }
            return false;
        }

        //ing 170228
        //For Chipping
        bool WriteDefect(StreamWriter sw, Defect defect, ref int nDefect, ref int nImg, bool bSaveImg, int nID, int xOffset = 0, int yOffset = 0)
        {
            if (sw == null) return true;
            double nY;
            double nX;
            string str;
            nDefect++;
            nImg++;
            sw.WriteLine("");
            str = nDefect.ToString() + " ";
            defect.m_cpOffset.x = (int)(Math.Cos((defect.m_fAngle - 90) * Math.PI / 180) * (m_fDiePitchX / 2) + (m_fDiePitchX / 2)); // ing 170302
            defect.m_cpOffset.y = (int)(Math.Sin((defect.m_fAngle - 90) * Math.PI / 180) * (m_fDiePitchY / 2) + (m_fDiePitchY / 2)); // ing 170302
            str += string.Format("{0:e}", defect.m_cpOffset.x) + " " + string.Format("{0:e}", defect.m_cpOffset.y) + " ";
            str += defect.m_cpDieIndex.x.ToString() + " " + defect.m_cpDieIndex.y.ToString() + " "; // Die Index
            nX = Math.Abs(defect.m_cp1.x - defect.m_cp0.x) * m_fResolution;
            nY = Math.Abs(defect.m_cp1.y - defect.m_cp0.y) * m_fResolution;
            str += nX.ToString("0.00") + " " + nY.ToString("0.00") + " ";
            str += defect.m_nSize.ToString() + " "; // Defect Area
            str += (defect.m_nSize * Math.Sqrt(2) * m_fResolution).ToString("0.00") + " "; // Defect Size
            str += "0 "; // DefectCode
            str += "1 "; // Test
            str += "0 "; // CLUSTERNUMBER
            str += defect.m_nSize.ToString() + " "; // ROUGHBINNUMBER 
            str += defect.m_fAngle.ToString("0.00") + " "; // FINEBINNUMBER // ing 170223
            str += "0 "; // REVIEWSAMPLE  
            if (bSaveImg) str += "1 "; // IMAGECOUNT 
            if (bSaveImg) str += "1 "; // IMAGELIST; 
            sw.WriteLine(str);
            sw.Write(nImg.ToString() + " 0"); // Only GrayColor
            if (AddTIF(m_img[0], m_fs, m_encoder, defect, ""))
            {
                m_encoder.Save(m_fs);
                m_fs.Close();
                return true; // ing ROI 조절
            }
            if (nID != 0) // ing 170419
            {
                if (AddTIF(m_img[1], m_fs, m_encoder, defect, "", xOffset, yOffset))
                {
                    m_encoder.Save(m_fs);
                    m_fs.Close();
                    return true;
                }
            }
            return false;
        }

        //ing 170814
        // For Chipping in 천안
        bool WriteDefect(StreamWriter sw, Defect defect, ezImg img, ref int nDefect, ref int nImg, bool bSaveImg)
        {
            if (sw == null) return true;
            double nY;
            double nX;
            string str;
            nDefect++;
            nImg++;
            sw.WriteLine("");
            str = nDefect.ToString() + " ";
            defect.m_cpOffset.x = (int)(Math.Cos((defect.m_fAngle - 90) * Math.PI / 180) * (m_fDiePitchX / 2) + (m_fDiePitchX / 2));
            defect.m_cpOffset.y = (int)(Math.Sin((defect.m_fAngle - 90) * Math.PI / 180) * (m_fDiePitchY / 2) + (m_fDiePitchY / 2));
            str += string.Format("{0:e}", defect.m_cpOffset.x) + " " + string.Format("{0:e}", defect.m_cpOffset.y) + " ";
            str += defect.m_cpDieIndex.x.ToString() + " " + defect.m_cpDieIndex.y.ToString() + " "; // Die Index
            nX = (defect.m_cp1.x - defect.m_cp0.x) * m_fResolution;
            nY = defect.m_nSize * m_fResolution;
            str += nX.ToString("0.00") + " " + nY.ToString("0.00") + " ";
            str += ((int)(nX * nY / 2)).ToString() + " "; // Defect Area
            str += nX.ToString("0.00") + " "; // Defect Size
            str += "0 "; // DefectCode
            str += "1 "; // Test
            str += "0 "; // CLUSTERNUMBER
            str += "0 "; // ROUGHBINNUMBER 
            str += defect.m_fAngle.ToString("0.00") + " "; // FINEBINNUMBER // ing 170223
            str += "0 "; // REVIEWSAMPLE  
            if (bSaveImg) str += "1 "; // IMAGECOUNT 
            if (bSaveImg) str += "1 "; // IMAGELIST; 
            sw.WriteLine(str);
            sw.Write(nImg.ToString() + " 0"); // Only GrayColor
            if (AddTIFArea(img, m_fs, m_encoder, defect, ""))
            {
                m_encoder.Save(m_fs);
                m_fs.Close();
                return true; // ing ROI 조절
            }
            return false;
        }

        bool AddDefectImage(ezImg img, Die die, double fTheta, string prefix = "")
        {
            string strBMP = "";
            bool bError = false;
            //if (m_bSaveADCBtimap)
            //{
            //    Directory.CreateDirectory(m_strPathBMP);
            //    Directory.CreateDirectory(m_strPathBMP + "\\" + m_auto.ClassRecipe().m_sRecipe);
            //    Directory.CreateDirectory(m_strPathBMP + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + m_strWaferID + "_" + m_strLotID + "_" + m_nSlot.ToString());
            //    strBMP = m_strPathBMP + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + m_strWaferID + "_" + m_strLotID + "_" + m_nSlot.ToString() + "\\" + prefix + "_" + die.m_cpIndex.ToString();
            //}
            if (img.AddTIF(m_fs, m_encoder, die.m_cpLT, die.m_cpRT, die.m_cpLB, die.m_cpRB, fTheta, true, "", strBMP))
            {
                bError = true;
                m_log.Popup("Klarf File Error !!");
            }
            return bError;
        }

        bool AddBigDefectImage(ezImg img, Die die, double fTheta = 0, string strDraw = "")
        {
            bool bError = false;
            if (AddTIF(img, m_fs, m_encoder, die, m_nDown, fTheta, false, true, strDraw))
            {
                bError = true;
                m_log.Popup("Klarf File Error !!");
            }
            return bError;
        }

        // For EBR
        // ing 170208
        public bool AddTIF(ezImg img, FileStream fs, TiffBitmapEncoder encoder, HW_Aligner_EBR_EBR_Dat ebr, string strDraw = "", bool bImgFullSize = false)
        {
            int nWidth, nHeight, stride;
            CPoint cpLT, cpRB;
            if (bImgFullSize)
            {
                cpLT.x = 0;
                cpLT.y = ebr.m_defect.m_cp0.y - 200;
                cpRB.x = img.m_szImg.x - 1;
                cpRB.y = ebr.m_defect.m_cp0.y + 200;
            }
            else
            {
                cpLT = ebr.m_defect.m_cp0 - new CPoint(150, 200);
                cpRB = ebr.m_defect.m_cp0 + new CPoint(50 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
            }
            if (cpLT.x > img.m_szImg.x - 1) cpLT.x -= img.m_szImg.x;
            if (cpLT.x < 0) cpLT.x = 0;
            if (cpRB.x > img.m_szImg.x - 1) cpRB.x = img.m_szImg.x - 1;
            nWidth = Math.Abs(cpRB.x - cpLT.x);
            nHeight = Math.Abs(cpLT.y - cpRB.y);
            if (nHeight == 0) nHeight = 1;
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    if (cpLT.y + n > img.m_szImg.y - 1) cpLT.y -= img.m_szImg.y;
                    byte* pSrc = (byte*)img.GetIntPtr(cpLT.y + n, cpLT.x);
                    for (int m = 0; m < stride; m++)
                    {
                        aPixel[n * stride + m] = *pSrc;
                        pSrc++;
                    }
                }
            }
            try
            {
                Bitmap bmpColor;
                Graphics g;
                MemoryStream streamBitmap = new MemoryStream();
                bmpColor = new Bitmap(stride, nHeight, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(bmpColor);
                BitmapData btData = bmpColor.LockBits(new Rectangle(0, 0, stride, nHeight), ImageLockMode.ReadWrite, bmpColor.PixelFormat);
                unsafe
                {
                    byte* pSrc = (byte*)btData.Scan0;
                    for (int n = 0; n < stride * nHeight; n++)
                    {
                        *pSrc = (byte)aPixel[n]; pSrc++;
                        *pSrc = (byte)aPixel[n]; pSrc++;
                        *pSrc = (byte)aPixel[n]; pSrc++;
                    }
                }
                bmpColor.UnlockBits(btData);
                bmpColor.RotateFlip(RotateFlipType.RotateNoneFlipY); // ing 170215
                g.DrawString(strDraw, new Font("Arial", 10), new SolidBrush(Color.Red), 5, 5);
                if (bImgFullSize)
                {
                    g.DrawLine(new Pen(Color.Green, 3), ebr.m_defect.m_cp0.x, 200, ebr.m_defect.m_cp1.x, 200);
                    g.DrawLine(new Pen(Color.Yellow, 3), ebr.m_defect.m_cp1.x, 200, ebr.m_defect.m_cp1.x - (int)(ebr.m_fBevel / ebr.m_fResolution), 200);
                }
                else
                {
                    g.DrawLine(new Pen(Color.Green, 3), 150, 200, 150 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
                    g.DrawLine(new Pen(Color.Yellow, 3), 150 + (int)((ebr.m_fEBR - ebr.m_fBevel) / ebr.m_fResolution), 200, 150 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
                }
                bmpColor.Save(streamBitmap, System.Drawing.Imaging.ImageFormat.Bmp);
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(streamBitmap));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        // For Trim
        // ing 170829
        public bool AddTIF(ezImg img, FileStream fs, TiffBitmapEncoder encoder, HW_Aligner_EBR_Trim_Dat ebr, string strDraw = "")
        {
            int nWidth, nHeight, stride;
            CPoint cpLT, cpRB;
            cpLT = ebr.m_defect.m_cp0 - new CPoint(150, 200);
            cpRB = ebr.m_defect.m_cp0 + new CPoint(50 + (int)(ebr.m_fTrim / ebr.m_fResolution), 200);
            if (cpLT.x > img.m_szImg.x - 1) cpLT.x -= img.m_szImg.x;
            if (cpLT.x < 0) cpLT.x = 0;
            if (cpRB.x > img.m_szImg.x - 1) cpRB.x = img.m_szImg.x - 1;
            nWidth = Math.Abs(cpRB.x - cpLT.x);
            nHeight = Math.Abs(cpLT.y - cpRB.y);
            if (nHeight == 0) nHeight = 1;
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    if (cpLT.y + n > img.m_szImg.y - 1) cpLT.y -= img.m_szImg.y;
                    byte* pSrc = (byte*)img.GetIntPtr(cpLT.y + n, cpLT.x);
                    for (int m = 0; m < stride; m++)
                    {
                        aPixel[n * stride + m] = *pSrc;
                        pSrc++;
                    }
                }
            }
            try
            {
                Bitmap bmpColor;
                Graphics g;
                MemoryStream streamBitmap = new MemoryStream();
                bmpColor = new Bitmap(stride, nHeight, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(bmpColor);
                BitmapData btData = bmpColor.LockBits(new Rectangle(0, 0, stride, nHeight), ImageLockMode.ReadWrite, bmpColor.PixelFormat);
                unsafe
                {
                    byte* pSrc = (byte*)btData.Scan0;
                    for (int n = 0; n < stride * nHeight; n++)
                    {
                        *pSrc = (byte)aPixel[n]; pSrc++;
                        *pSrc = (byte)aPixel[n]; pSrc++;
                        *pSrc = (byte)aPixel[n]; pSrc++;
                    }
                }
                bmpColor.UnlockBits(btData);
                bmpColor.RotateFlip(RotateFlipType.RotateNoneFlipY); // ing 170215
                g.DrawString(strDraw, new Font("Arial", 10), new SolidBrush(Color.Red), 5, 5);
                g.DrawLine(new Pen(Color.Green, 3), 150, 200, 150 + (int)(ebr.m_fTrim / ebr.m_fResolution), 200);
                //g.DrawLine(new Pen(Color.Yellow, 3), 150 + (int)((ebr.m_fEBR - ebr.m_fBevel) / ebr.m_fResolution), 200, 150 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
                bmpColor.Save(streamBitmap, System.Drawing.Imaging.ImageFormat.Bmp);
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(streamBitmap));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        // For Backside
        // ing 170329

        public bool AddTIF(ezImg img, FileStream fs, TiffBitmapEncoder encoder, Die die, int nDown = 1, double fTheta = 0, bool bRotate = false, bool bGray = true, string strDraw = "")
        {
            int nWidth, nHeight, stride;
            CPoint cpLB, cpLT, cpRB, cpRT;
            cpLB = die.m_cpLB / nDown;
            cpLT = die.m_cpLT / nDown;
            cpRB = die.m_cpRB / nDown;
            cpRT = die.m_cpRT / nDown;
            nWidth = img.m_szImg.x;
            nHeight = img.m_szImg.y;
            fTheta += 360; fTheta %= 360;
            if (fTheta > 270 || fTheta < 90) fTheta = 0;
            else if (fTheta >= 90 && fTheta < 180) fTheta = 90;
            else if (fTheta >= 180 && fTheta < 270) fTheta = 180;
            else fTheta = 270;
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            /*for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    if (bRotate)
                    {
                        for (int m = 0; m < stride; m++)
                        {
                            fA = Math.Atan2(n, m);
                            fDistance = Math.Sqrt((n * n) + (m * m));
                            cpGV.x = cpLT.x + (int)(Math.Cos(fAngle / 180 * Math.PI - fA) * fDistance);
                            cpGV.y = cpLT.y + (int)(Math.Sin(fAngle / 180 * Math.PI - fA) * fDistance);
                            aPixel[n * stride + m] = GetGV(cpGV);
                        }
                    }
                    else
                    {
                        byte* pSrc = (byte*)img.GetIntPtr(0, 0);
                        fixed (byte* pDst = &(aPixel[n * stride]))
                        {
                            cpp_memcpy(pDst, pSrc, stride);
                        }
                    }
                }
            }*/
            try
            {
                BitmapSource image;
                if (bGray)
                {
                    Bitmap bmpGray;
                    Graphics g;
                    FormatConvertedBitmap convert;
                    ImageAttributes attributes = new ImageAttributes();
                    ColorMatrix colorMatrix = new ColorMatrix(new float[][] 
                    { 
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1} 
                    });
                    attributes.SetColorMatrix(colorMatrix);

                    bmpGray = new Bitmap(img.m_szImg.x / 2, img.m_szImg.y / 2);
                    g = Graphics.FromImage(bmpGray);
                    g.DrawImage(img.GetBitmap(), new Rectangle(0, 0, img.m_szImg.x / 2, img.m_szImg.y / 2), 0, 0, img.m_szImg.x, img.m_szImg.y, GraphicsUnit.Pixel, attributes);
                    g.DrawLine(new Pen(new SolidBrush(Color.White)), (cpLB / 2).ToPoint(), (cpLT / 2).ToPoint());
                    g.DrawLine(new Pen(new SolidBrush(Color.White)), (cpLT / 2).ToPoint(), (cpRT / 2).ToPoint());
                    g.DrawLine(new Pen(new SolidBrush(Color.White)), (cpRT / 2).ToPoint(), (cpRB / 2).ToPoint());
                    g.DrawLine(new Pen(new SolidBrush(Color.White)), (cpRB / 2).ToPoint(), (cpLB / 2).ToPoint());
                    bmpGray.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    g.DrawString(strDraw, new Font("Arial", 8), new SolidBrush(Color.White), new Point(0, 0));
                    g.RotateTransform((float)fTheta);
                    image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpGray.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    convert = new FormatConvertedBitmap();
                    convert.BeginInit();
                    convert.Source = image;
                    convert.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
                    convert.EndInit();
                    encoder.Compression = TiffCompressOption.None;
                    encoder.Frames.Add(BitmapFrame.Create(convert));
                }
                else // ing 170208
                {
                    Bitmap bmpColor;
                    Graphics g;
                    bmpColor = new Bitmap(stride, nHeight, PixelFormat.Format24bppRgb);
                    g = Graphics.FromImage(bmpColor);
                    for (int y = 0; y < nHeight; y++)
                    {
                        for (int x = 0; x < stride; x++)
                        {
                            bmpColor.SetPixel(x, y, Color.FromArgb(aPixel[x + y * stride], aPixel[x + y * stride], aPixel[x + y * stride]));
                        }
                    }
                    g.DrawString(strDraw, new Font("Arial", 10), new SolidBrush(Color.Red), 5, 5);
                    image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpColor.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    encoder.Compression = TiffCompressOption.None;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                }
            }
            catch (System.Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        // For Chipping
        // ing 170228
        public bool AddTIF(ezImg img, FileStream fs, TiffBitmapEncoder encoder, Defect defect, string strDraw = "", int xOffset = 0, int yOffset = 0)
        {
            int nWidth, nHeight, stride;
            CPoint cpLT, cpRB;
            cpLT.y = defect.m_cp1.y + 100 + yOffset;
            cpRB.y = defect.m_cp0.y - 100 + yOffset;
            cpLT.x = xOffset;
            cpRB.x = img.m_szImg.x;
            nWidth = Math.Abs(cpRB.x - cpLT.x);
            nHeight = Math.Abs(cpLT.y - cpRB.y);
            if (nHeight == 0) nHeight = 1;
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    if (cpLT.y > img.m_szImg.y - 1) cpLT.y -= img.m_szImg.y;
                    if (cpLT.y < 0) cpLT.y += img.m_szImg.y;
                    byte* pSrc = (byte*)img.GetIntPtr(cpLT.y, cpLT.x);
                    for (int m = 0; m < stride; m++)
                    {
                        aPixel[n * stride + m] = *pSrc;
                        pSrc++;
                    }
                    cpLT.y--;
                }
            }
            try
            {
                BitmapPalette myPalette;
                BitmapSource image;
                myPalette = BitmapPalettes.Gray256;
                image = BitmapSource.Create(stride, nHeight, 96, 96, System.Windows.Media.PixelFormats.Indexed8, myPalette, aPixel, stride);
                /*BitmapSource image;
                Bitmap bmpColor;
                //Graphics g;
                bmpColor = new Bitmap(stride, nHeight, PixelFormat.Format16bppGrayScale);
                g = Graphics.FromImage(bmpColor);
                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < stride; x++)
                    {
                        bmpColor.SetPixel(x, y, Color.FromArgb(aPixel[x + y * stride], aPixel[x + y * stride], aPixel[x + y * stride]));
                    }
                }
                bmpColor.RotateFlip(RotateFlipType.RotateNoneFlipY); // ing 170215
                g.DrawString(strDraw, new Font("Arial", 10), new SolidBrush(Color.Red), 5, 5);
//                g.DrawLine(new Pen(Color.Green, 3), 150, 200, 150 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
//                g.DrawLine(new Pen(Color.Yellow, 3), 150 + (int)((ebr.m_fEBR - ebr.m_fBevel) / ebr.m_fResolution), 200, 150 + (int)(ebr.m_fEBR / ebr.m_fResolution), 200);
                //bmpColor.Save("d:\\TestColor.bmp", ImageFormat.Bmp);
            image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpColor.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            */
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(image));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        // For Chipping in 천안
        // ing 170814
        public bool AddTIFArea(ezImg img, FileStream fs, TiffBitmapEncoder encoder, Defect defect, string strDraw = "")
        {
            int nWidth, nHeight, stride;
            CPoint cpLT, cpRB;
            cpLT.y = defect.m_cp0.y + 100;
            cpRB.y = defect.m_cp1.y - 100;
            cpLT.x = defect.m_cp0.x - 100;
            cpRB.x = defect.m_cp1.x + 100;
            if (cpLT.y > img.m_szImg.y - 1) cpLT.y = img.m_szImg.y - 1;
            if (cpRB.y < 0) cpRB.y = 0;
            if (cpLT.x < 0) cpLT.x = 0;
            if (cpRB.x > img.m_szImg.x - 1) cpRB.x = img.m_szImg.x - 1;
            nWidth = Math.Abs(cpRB.x - cpLT.x);
            nHeight = Math.Abs(cpLT.y - cpRB.y);
            if (nHeight == 0) nHeight = 1;
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    byte* pSrc = (byte*)img.GetIntPtr(cpLT.y, cpLT.x);
                    for (int m = 0; m < stride; m++)
                    {
                        aPixel[n * stride + m] = *pSrc;
                        pSrc++;
                    }
                    cpLT.y--;
                }
            }
            try
            {
                BitmapPalette myPalette;
                BitmapSource image;
                myPalette = BitmapPalettes.Gray256;
                image = BitmapSource.Create(stride, nHeight, 96, 96, System.Windows.Media.PixelFormats.Indexed8, myPalette, aPixel, stride);
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(image));
            }
            catch (System.Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }


        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);
    }
}
