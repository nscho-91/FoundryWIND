using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.Barcode;

namespace ezTools
{
    public class Cognex_BCR
    {
        public string m_strBCR;
        string m_id;
        ezImg m_img;
        Log m_log;
        private CogBarcodeTool m_cogBarcode = null;

        public Cognex_BCR()
        {
        }

        public void Init(string id, Log log)
        {
            m_id = id; m_log = log;
            m_img = new ezImg(id, m_log);
            m_cogBarcode = new CogBarcodeTool();
        }

        public void Teach()
        {
            m_cogBarcode = new CogBarcodeTool();
            m_cogBarcode.RunParams.Code39.Enabled = true;
            m_cogBarcode.RunParams.I2Of5.Enabled = true;
            m_cogBarcode.RunParams.UpcEan.Enabled = true;
            m_cogBarcode.RunParams.Code128.Enabled = false;
            m_cogBarcode.RunParams.FourState.Enabled = false;
            m_cogBarcode.RunParams.Postnet.Enabled = false;
            m_cogBarcode.RunParams.Planet.Enabled = false;
            m_cogBarcode.RunParams.Mirrored = CogBarcodeMirroredConstants.Unknown;
            m_cogBarcode.RunParams.Polarity = CogBarcodePolarityConstants.DarkOnLight;
            m_cogBarcode.RunParams.GradingMode = CogBarcodeGradingModeConstants.All;
            m_cogBarcode.RunParams.NumToFind = 4;
            m_cogBarcode.RunParams.RegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
            m_cogBarcode.Region = null;
            m_cogBarcode.CurrentRecordEnable = CogBarcodeCurrentRecordConstants.InputImage |
              CogBarcodeCurrentRecordConstants.Region;
            m_cogBarcode.LastRunRecordEnable = CogBarcodeLastRunRecordConstants.ResultsBounds |
              CogBarcodeLastRunRecordConstants.ResultsCenters;
            m_cogBarcode.LastRunRecordDiagEnable = CogBarcodeLastRunRecordDiagConstants.InputImageByReference |
              CogBarcodeLastRunRecordDiagConstants.Region;
        }

        public string Run(ezImg img, CPoint cpSrc, CPoint szSrc, int nTryRun = 2)
        {
            szSrc.x = (szSrc.x / 4) * 4; 
            CPoint sz = new CPoint(szSrc.x, szSrc.y);
            m_strBCR = "Unknown";
            if (m_cogBarcode == null) return m_strBCR;
            m_img.ReAllocate(sz, img.m_nByte);
            m_img.Copy(img, cpSrc, new CPoint(0, 0), m_img.m_szImg);
            Bitmap bmpSrc = m_img.GetBitmap();
            bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Directory.CreateDirectory("D:\\Temp");
            bmpSrc.Save("D:\\Temp\\BarcodeOrg.bmp", ImageFormat.Bmp); // ing for test
            if (bmpSrc == null) return m_strBCR;
            CogImage8Grey imgSrc;
            imgSrc = new CogImage8Grey(bmpSrc);
            m_cogBarcode.InputImage = imgSrc;
            bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);            
            CogRectangleAffine Roi = new CogRectangleAffine();
            Roi.SetOriginLengthsRotationSkew(0, 0, imgSrc.Width, imgSrc.Height, 0, 0);
            m_cogBarcode.Region = Roi;
            m_cogBarcode.Run();   // run barcode tool
            if ((m_cogBarcode.RunStatus.Result == CogToolResultConstants.Error) || (m_cogBarcode.Results == null))
            {
                if(nTryRun == 0)
                {
                    return m_strBCR;
                }
                cpSrc += new CPoint(4, 4);
                szSrc -= new CPoint(8, 8);
                nTryRun -= 1;
                return Run(img, cpSrc, szSrc, nTryRun);
            }
            foreach (CogBarcodeResult result in m_cogBarcode.Results)
            {
                if (result.OwnedDecoded.Valid == true && result.OwnedDecoded.String != null)
                    m_strBCR = result.OwnedDecoded.String;
            }
            return m_strBCR;
        }

    }
}
