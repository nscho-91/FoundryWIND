using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ezTools;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.OCRMax;

namespace ezAutoMom
{
    public partial class OCRTool : Form
    {
        int m_nCount = 0;
        double m_fZoom = 0.3, m_fZoomExt = 0.3;
        string m_id, m_strFontLib = "FontName";
        Log m_log;
        ezGrid m_grid;
        Cognex_OCR m_cogOCR;
        Bitmap m_img;
        public OCRTool()
        {
            InitializeComponent();
        }

        public void Init(string id, Log log)
        {
            m_id = id; m_log = log;
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_cogOCR = new Cognex_OCR(m_id, m_log);
            RunGrid(eGrid.eInit);
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_fZoom, "Option", "Zoom", "Zoom ratio");
            m_grid.Set(ref m_fZoomExt, "Option", "ZoomExt", "Zoom Extract Image Ratio");
            m_cogOCR.RunGridTeach(m_id, m_grid, eMode, job);
            m_grid.Refresh();
        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            InitOCRTable();
        }

        private void buttonLoadImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files (*.bmp)|*.bmp";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_strFontLib = dlg.FileName;
            if (m_img != null) m_img.Dispose();
            try { m_img = new Bitmap(dlg.FileName); }
            catch (Exception ex) { m_log.Add(ex.Message); return; }
            Invalidate();
        }

        private void buttonAnalysis_Click(object sender, EventArgs e)
        {
            CogAnalysis();
        }

        private void buttonLoadFont_Click(object sender, EventArgs e)
        {
            int n;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Font Files (*.ocr)|*.ocr";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_strFontLib = dlg.FileName;
            if (!m_cogOCR.LoadFont(m_strFontLib)) { m_log.Add("Font File Load Fail !!"); return; }
            for( n = 0; n < m_cogOCR.m_cogOCRMax.Classifier.Font.Count; n++)
            {
                int ch = m_cogOCR.m_cogOCRMax.Classifier.Font[n].CharacterCode;
                string[] strItems = new string[2] { Convert.ToChar(ch).ToString(), m_nCount.ToString() };
                ListViewItem Item = new ListViewItem(strItems);
                listSegment.Items.Add(Item); m_nCount++;
            }
        }

        private void listSegment_DoubleClick(object sender, EventArgs e)
        {
            if (listSegment.SelectedItems.Count == 0) return;
            listSegment.SelectedItems[0].BeginEdit();
        }

        private void listSegment_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            try 
            {
                m_cogOCR.m_cogOCRMax.Classifier.Font.RemoveAt(listSegment.SelectedItems[0].Index);
                listSegment.SelectedItems[0].Remove();
            }
            catch (Exception ex) { m_log.Add(ex.Message); }            
        }
      
        private void buttonTrain_Click(object sender, EventArgs e)
        {
            int n;
            for (n = 0; n < listSegment.Items.Count; n++ )
            {
                m_cogOCR.m_cogOCRMax.Classifier.Font[n].CharacterCode = Convert.ToInt16(listSegment.Items[n].Text[0]);
            }
            try { m_cogOCR.m_cogOCRMax.Classifier.Train(); }
            catch (Exception ex) { m_log.Popup(ex.Message); return; }
            if (!m_cogOCR.m_cogOCRMax.Classifier.Trained) { m_log.Popup("Could not train classifier !!"); return; }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Font Files (*.ocr)|*.ocr";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_cogOCR.m_cogOCRMax.Classifier.Font.Name = m_strFontLib;
            m_cogOCR.m_cogOCRMax.Classifier.Font.Export(dlg.FileName);
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
            Invalidate();
        }
        private void listSegment_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OCRTool_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmpChar;
            if (m_img != null) e.Graphics.DrawImage(m_img, 10, listSegment.Location.Y + listSegment.Height + 10, (int)(this.Size.Width * m_fZoom) - 20, (int)(this.Size.Width * m_fZoom) - 20);
            try
            {
                bmpChar = m_cogOCR.m_cogOCRMax.Classifier.Font[listSegment.SelectedItems[0].Index].Image.ToBitmap();
                e.Graphics.DrawImage(bmpChar, 10, listSegment.Location.Y + listSegment.Height + (int)(this.Size.Width * m_fZoom) - 10,
                    (int)(this.Size.Width * m_fZoomExt) - 20, (int)(this.Size.Width * m_fZoomExt) - 20);
            }
            catch { return; }
        }

        private void InitOCRTable()
        {
            m_nCount = 0;
            this.listSegment.Items.Clear();
            m_cogOCR = new Cognex_OCR(m_id, m_log);
        }

        private void CogAnalysis()
        {
            if (m_img == null) { m_log.Add("Please Load a Image File"); return; }
            CogImage8Grey cogImg = new CogImage8Grey(m_img);
            CogRectangleAffine cogROI = new CogRectangleAffine();
            cogROI.SetOriginLengthsRotationSkew(0, 0, cogImg.Width, cogImg.Height, 0, 0);
            CogOCRMaxSegmenterParagraphResult cogSegParagraph = null;
            try { cogSegParagraph = m_cogOCR.m_cogOCRMax.Segmenter.Execute(cogImg, cogROI); }
            catch (Exception ex) { m_log.Popup(ex.Message); return; }
            CogOCRMaxSegmenterLineResult cogSegLine = cogSegParagraph[0];
            foreach (CogOCRMaxSegmenterPositionResult cogSegPosition in cogSegLine)
            {
                CogOCRMaxChar cogChar = cogSegPosition.Character;
                m_cogOCR.m_cogOCRMax.Classifier.Font.Add(cogChar);
                string[] strItems = new string[2] { Convert.ToChar(cogChar.CharacterCode).ToString(), m_nCount.ToString() };
                ListViewItem Item = new ListViewItem(strItems); m_nCount++;
                listSegment.Items.Add(Item);
            }
            listSegment.Invalidate(); return;
        }

        private void OCRTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void listSegment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (listSegment.SelectedItems.Count == 0 || listSegment.SelectedItems[0] == null) return;
            if (e.KeyChar == Convert.ToChar(Keys.Delete))
            {
                try
                {
                    m_cogOCR.m_cogOCRMax.Classifier.Font.RemoveAt(listSegment.SelectedItems[0].Index);
                    listSegment.SelectedItems[0].Remove();
                }
                catch (Exception ex) { m_log.Add(ex.Message); }
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.F2))
            {
                listSegment.SelectedItems[0].BeginEdit();
                return;
            }
        }

    }
}
