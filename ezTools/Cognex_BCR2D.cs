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
using Cognex.VisionPro.TwoDSymbol;

namespace ezTools
{
    public class Cognex_BCR2D
    {
        string m_id, m_strMode = "LightOnDark";
        string[] m_strModes = new string[2] { "LightOnDark", "DarkOnLight" };
        public double m_fAngle;
        Log m_log;
        ezImg m_img;
        Cog2DSymbolTool m_cog2DTool = null;

        public Cognex_BCR2D()
        {
        }

        public void Init(string id, Log log)
        {
            m_id = id;
            m_log = log;
            m_img = new ezImg(id, log);
            //m_cog2DTool = new Cog2DSymbolTool();
        }

        public void RunGridTeach(string m_id, ezGrid rGrid, eGrid eMode, ezJob job = null)
        {
            rGrid.Set(ref m_strMode, m_strModes, m_id, "Polarity", "Polarity");
            //Teach();
        }

        public void Teach()
        {
            m_cog2DTool.Pattern.PerspectiveEnabled = true;
            m_cog2DTool.Pattern.NonConformantModulesEnabled = true;
            if (m_strMode == "LightOnDark")
            {
                m_cog2DTool.Pattern.Polarity = Cog2DSymbolPolarityConstants.LightOnDark;
            }
            else
            {
                m_cog2DTool.Pattern.Polarity = Cog2DSymbolPolarityConstants.DarkOnLight;
            }
            m_cog2DTool.RunParams.PerspectiveMode = Cog2DSymbolPerspectiveModeConstants.UsePattern;
            m_cog2DTool.RunParams.NonConformantModulesMode = Cog2DSymbolNonConformantModulesModeConstants.UsePattern;
        }

        public string Run(ezImg img, CPoint cpSrc, CPoint szSrc, int nTryTrain = 2)
        {
            try
            {
                if (nTryTrain == 0) return "UnTrained";
                szSrc.x = (szSrc.x / 4) * 4; 
                m_img.ReAllocate(szSrc, img.m_nByte);
                m_img.Copy(img, cpSrc, new CPoint(0, 0), m_img.m_szImg);               
                Bitmap bmpSrc = m_img.GetBitmap();
                bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Directory.CreateDirectory("D:\\Temp");
                bmpSrc.Save("D:\\Temp\\2DOrg.bmp", ImageFormat.Bmp); // ing for test
                if (bmpSrc == null) return "";
                CogImage8Grey imgSrc;
                imgSrc = new CogImage8Grey(bmpSrc);
                m_cog2DTool.InputImage = imgSrc;
                bmpSrc.RotateFlip(RotateFlipType.RotateNoneFlipY);
                CogRectangleAffine Roi = new CogRectangleAffine();
                Roi.SetOriginLengthsRotationSkew(0, 0, imgSrc.Width, imgSrc.Height, 0, 0);
                m_cog2DTool.SearchRegion = Roi;
                m_cog2DTool.Pattern.Untrain();
                m_cog2DTool.Pattern.TrainImage = imgSrc;
                m_cog2DTool.Pattern.TrainRegion = Roi;
                m_cog2DTool.Pattern.Train();
                if(m_cog2DTool.Pattern.Trained == false)
                {
                    cpSrc += new CPoint(4, 4);
                    szSrc -= new CPoint(8, 8);
                    nTryTrain -= 1;
                    return Run(img, cpSrc, szSrc, nTryTrain);
                }
                if (!m_cog2DTool.Pattern.Trained) return "UnTrained";
                m_cog2DTool.Run();               
                Cog2DSymbolResult result = m_cog2DTool.Result;
                if (result == null || result.DecodedString == null) return "Unknown";
                m_fAngle = result.Angle;
                return result.DecodedString;
            }
            catch (Exception ex) 
            {
                m_log.Add(ex.Message); m_log.Popup("2D Read Fail !!");
                return "Unknown";
            }
           
        }
    }
}
