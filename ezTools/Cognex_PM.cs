using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.PMAlign; 
using Cognex.VisionPro.Exceptions;

namespace ezTools
{
    public class Cognex_PM
    {
        string m_id;
        ezImg m_img;
        Log m_log;
        private CogPMAlignTool m_cogPM = new CogPMAlignTool();

        public Cognex_PM()
        {
        }

        public void Init(string id, Log log)
        {
            m_id = id; 
            m_log = log;
            m_img = new ezImg(id, m_log);
        }

        public void ThreadStop()
        {
            m_cogPM.Dispose(); 
        }

        CogImage8Grey GetCogImage(ezImg img, CPoint cp, CPoint sz)
        {
            m_img.ReAllocate(sz, img.m_nByte);
            m_img.Copy(img, cp, new CPoint(0, 0), m_img.m_szImg);
            Bitmap bmpSrc = m_img.GetBitmap();
            if (bmpSrc == null) return null;
            CogImage8Grey imgSrc;
            imgSrc = new CogImage8Grey(bmpSrc);
            return imgSrc;
        }

        public bool Train(ezImg imgTeach, CPoint cpSrc, CPoint szSrc)
        {
            if (m_cogPM == null)
            {
                m_log.Popup("Cognex PM is null !!");
                return true;
            }

            szSrc.x = (szSrc.x / 4) * 4;

            CogRectangleAffine regionTrain = default(CogRectangleAffine);
            regionTrain = m_cogPM.Pattern.TrainRegion as CogRectangleAffine;
            if ((regionTrain != null))
            {
                regionTrain.SetCenterLengthsRotationSkew((cpSrc.x + szSrc.x) / 2, (cpSrc.y + szSrc.y) / 2, szSrc.x, szSrc.y, 0, 0);
                regionTrain.GraphicDOFEnable = CogRectangleAffineDOFConstants.Position | CogRectangleAffineDOFConstants.Rotation | CogRectangleAffineDOFConstants.Size;
            }
            m_cogPM.Pattern.TrainImage = GetCogImage(imgTeach, cpSrc, szSrc);
            try { m_cogPM.Pattern.Train(); }
            catch (CogException cogex) { m_log.Popup("Following Specific Cognex Error Occured :" + cogex.Message); }
            catch (Exception ex) { m_log.Popup("PatMax Setup Error : " + ex.Message); }

            return false; 
        }

        public bool Run(ezImg img, CPoint cpSrc, CPoint szSrc)
        {
            if (m_cogPM == null)
            {
                m_log.Popup("Cognex PM is null !!");
                return true;
            }

            szSrc.x = (szSrc.x / 4) * 4;

            CogRectangle regionSearch = new CogRectangle();
            m_cogPM.SearchRegion = regionSearch;
            regionSearch.SetCenterWidthHeight((cpSrc.x + szSrc.x) / 2, (cpSrc.y + szSrc.y) / 2, szSrc.x, szSrc.y);
            regionSearch.GraphicDOFEnable = CogRectangleDOFConstants.Position | CogRectangleDOFConstants.Size;
            regionSearch.Interactive = true;

            m_cogPM.InputImage = GetCogImage(img, cpSrc, szSrc);

//            CogRectangleAffine roi = new CogRectangleAffine();
//            roi.SetOriginLengthsRotationSkew(0, 0, imgSrc.Width, imgSrc.Height, 0, 0);
//            m_cogPM.Region = roi;
            m_cogPM.Run();
            if ((m_cogPM.RunStatus.Exception != null))
            {
                m_log.Popup("PM Align Run Error : " + m_cogPM.RunStatus.Exception.Message);
            }

            /*

                        foreach (CogBarcodeResult result in m_cogBarcode.Results)
                        {
                            if (result.OwnedDecoded.Valid == true && result.OwnedDecoded.String != null)
                                m_strBCR = result.OwnedDecoded.String;
                        }
                        return m_strBCR;
            */
            return false; 
        }

    }
}
