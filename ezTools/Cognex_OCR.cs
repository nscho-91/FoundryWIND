using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.OCRMax;

namespace ezTools
{
    public class Cognex_OCR
    {
        string m_id, m_strOldFont = "D:\\ATIOCR\\FontName.ocr", m_strFontName = "D:\\ATIOCR\\FontName.ocr", m_strMode = "LightOnDark";
        string[] m_strModes = new string[3] { "Unknown", "LightOnDark", "DarkOnLight" };
        int m_nThreshold = 50, m_nMinWidth = 5, m_nMinFragment = 15, m_nMinGab = 1, m_nAcceptScore = 70, m_nMinSize = 30;
        double m_nMinAspect = 1.0;
        ezImg m_img;
        Log m_log;
        CogOCRMaxFont m_cogFont;
        public CogOCRMaxTool m_cogOCRMax = null;

        public Cognex_OCR(string id, Log log)
        {
            m_id = id; m_log = log;
            m_img = new ezImg(id, m_log);
            m_cogOCRMax = new CogOCRMaxTool();
            m_cogFont = new CogOCRMaxFont();
        }

        public void RunGridTeach(string m_id, ezGrid rGrid, eGrid eMode, ezJob job = null)
        {
            rGrid.Set(ref m_strFontName, m_id, "FontPath", "Full Font Path");
            rGrid.Set(ref m_strMode, m_strModes, m_id, "Polarity", "Polarity");
            rGrid.Set(ref m_nMinSize, m_id, "MinSize", "Minimum Size (pixel)");
            rGrid.Set(ref m_nAcceptScore, m_id, "AcceptScore", "Accept Score");
            rGrid.Set(ref m_nMinWidth, m_id, "MinWidth", "Minimum Font Width");
            rGrid.Set(ref m_nThreshold, m_id, "GV", "Threshold value");
            rGrid.Set(ref m_nMinFragment, m_id, "MinFragment", "Minimum Foregournd Fragment (pixel)");
            rGrid.Set(ref m_nMinAspect, m_id, "MinAspect", "Minimum Allowable Aspect (0.0 ~ 1.0)");
            rGrid.Set(ref m_nMinGab, m_id, "MinGab", "Minimum Intercharacter Gab ");
            Teach();
            try 
            {
                //KJW if (eMode == eGrid.eRegRead || eMode == eGrid.eJobOpen) m_cogOCRMax.Classifier.Train();
                if (m_strFontName != m_strOldFont) { LoadFont(m_strFontName); m_strOldFont = m_strFontName; m_cogOCRMax.Classifier.Train(); } 
            }
            catch { m_log.Add("Can not Use Cognex License"); }
        }

        public bool LoadFont(string strFont)
        {
            try
            {
                if (m_cogOCRMax == null)
                {
                    m_cogOCRMax = new CogOCRMaxTool();
                }
                m_cogFont.Import(strFont);
                m_cogOCRMax.Classifier.Font = m_cogFont;
                return true;
            }
            catch (Exception ex) { m_log.Add(ex.Message); return false; }
        }

        public void Teach()
        {
            try
            {
                if (m_strMode == "Unknown") m_cogOCRMax.Segmenter.Polarity = CogOCRMaxPolarityConstants.Unknown;
                else if (m_strMode == "LightOnDark") m_cogOCRMax.Segmenter.Polarity = CogOCRMaxPolarityConstants.LightOnDark;
                else if (m_strMode == "DarkOnLight") m_cogOCRMax.Segmenter.Polarity = CogOCRMaxPolarityConstants.DarkOnLight;
                m_cogOCRMax.Segmenter.CharacterFragmentMinNumPels = m_nMinFragment;
                m_cogOCRMax.Segmenter.CharacterMinNumPels = m_nMinSize;
                m_cogOCRMax.Segmenter.CharacterMinWidth = m_nMinWidth;
                m_cogOCRMax.Segmenter.UseCharacterMinAspect = true;
                if (m_nMinAspect > 1 || m_nMinAspect < 0) m_nMinAspect = 0.5;
                m_cogOCRMax.Segmenter.CharacterMinAspect = m_nMinAspect;
                m_cogOCRMax.Segmenter.MinIntercharacterGap = m_nMinGab;
                m_cogOCRMax.Segmenter.ForegroundThresholdFrac = Convert.ToDouble(m_nThreshold) / 255.0;
                //Defalt
                m_cogOCRMax.Segmenter.MaxIntracharacterGap = 0;
                m_cogOCRMax.Segmenter.SkewHalfRange = Math.PI / 120; // ing 161213
                m_cogOCRMax.Segmenter.SpaceParams.SpaceMinWidth = 2;
                m_cogOCRMax.Segmenter.SpaceParams.SpaceMaxWidth = 100;
                m_cogOCRMax.Segmenter.SpaceParams.SpaceInsertMode = CogOCRMaxSegmenterSpaceInsertModeConstants.None;
                m_cogOCRMax.Segmenter.NormalizationMode = CogOCRMaxSegmenterNormalizationModeConstants.LocalAdvanced;
                m_cogOCRMax.Segmenter.CharacterFragmentMergeMode = CogOCRMaxSegmenterCharacterFragmentMergeModeConstants.RequireOverlap;
                m_cogOCRMax.Segmenter.AnalysisMode = CogOCRMaxSegmenterAnalysisModeConstants.Standard;
                m_cogOCRMax.Segmenter.PitchType = CogOCRMaxSegmenterFontPitchTypeConstants.Fixed;
                m_cogOCRMax.Segmenter.WidthType = CogOCRMaxSegmenterFontCharWidthTypeConstants.Fixed;
                m_cogOCRMax.Segmenter.UseCharacterMaxWidth = false;
                m_cogOCRMax.Segmenter.UseCharacterMaxHeight = false;
                m_cogOCRMax.Segmenter.UseStrokeWidthFilter = true;
                m_cogOCRMax.Segmenter.CharacterFragmentContrastThreshold = 30;
                m_cogOCRMax.Segmenter.CharacterFragmentMaxDistanceToMainLine = 0;
            }
            catch(Exception ex)
            {
                m_log.Popup(ex.Message);
            }
        }

        public bool Run(ezImg img, CPoint cpSrc, CPoint szSrc, ref string strOCR, int nTryRun = 2)
        {
            try
            {
                bool bLowScore = false;
                CPoint sz = new CPoint(szSrc.x, szSrc.y);
                if (m_cogOCRMax == null) return true;
                Teach();
                m_cogOCRMax.Classifier.Train();
                m_img.ReAllocate(sz, img.m_nByte);
                m_img.Copy(img, cpSrc, new CPoint(0, 0), m_img.m_szImg);
                Bitmap bmpSrc = m_img.GetBitmap();
                bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Directory.CreateDirectory("D:\\ATIOCR");
                bmpSrc.Save("D:\\ATIOCR\\Org.bmp", ImageFormat.Bmp); // ing for test
                if (bmpSrc == null) return true;
                CogImage8Grey imgSrc;
                imgSrc = new CogImage8Grey(bmpSrc);
                m_cogOCRMax.InputImage = imgSrc;
                bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);
                CogRectangleAffine Roi = new CogRectangleAffine();
                Roi.SetOriginLengthsRotationSkew(0, 0, imgSrc.Width, imgSrc.Height, 0, 0);
                m_cogOCRMax.Region = Roi;
                m_cogOCRMax.Segmenter.Execute(imgSrc);
                m_cogOCRMax.ClassifierRunParams.AcceptThreshold = (double)m_nAcceptScore / 100.0;
                m_cogOCRMax.Run();
                if (m_cogOCRMax.RunStatus.Result == CogToolResultConstants.Error)
                {
                    if(nTryRun == 0)
                    {
                        m_log.Popup("Cognex OCR Read Error !!");
                        return true;
                    }
                    cpSrc += new CPoint(4, 4);
                    szSrc -= new CPoint(8, 8);
                    nTryRun -= 1;
                    return Run(img, cpSrc, szSrc, ref strOCR, nTryRun);
                }
                string score = "";
                for (int n = 0; n < m_cogOCRMax.LineResult.Count; n++)
                {
                    score += Convert.ToInt32(m_cogOCRMax.LineResult[n].Score * 100).ToString() + " ";
                    if (m_cogOCRMax.LineResult[n].Score * 100 < m_nAcceptScore) bLowScore = true;
                }
                m_cogOCRMax.LineResult.BinarizedLineImage.ToBitmap().Save("D:\\ATIOCR\\Result.bmp");
                strOCR = m_cogOCRMax.LineResult.ResultString;
                m_log.Add("OCR Inspect Result : " + strOCR + " Score : " + score);
                if (bLowScore)
                {
                    m_log.Add(" Score Is Too Low !!");
                    return true;
                }
            }
            catch(Exception ex)
            {
                m_log.Popup(ex.Message); return true;
            }
            return false;
        }

        public int GetThreshold()
        {
            return m_nThreshold;
        }
    }
}
