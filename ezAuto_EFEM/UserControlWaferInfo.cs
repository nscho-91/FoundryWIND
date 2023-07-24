using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ezAuto_EFEM
{
    public partial class UserControlWaferInfo : UserControl
    {
        public UserControlWaferInfo()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        bool m_bExist = false;
        int m_nLoadPort = 0;
        string m_strWaferSize = "";
        int m_nSlot = 0;
        string m_strWaferID = "";

        public void Set(bool bExist, int nLoadPort = 0, int nSlot = 0, string strWaferSize = "", string strWaferID = "")
        {
            m_bExist = bExist;
            m_nLoadPort = nLoadPort;
            m_strWaferSize = strWaferSize;
            m_nSlot = nSlot;
            m_strWaferID = strWaferID;
            Invalidate();
        }

        private void UserControlWaferInfo_Paint(object sender, PaintEventArgs e)
        {
            #region Info
            int nMargin = 1;
            int nH = this.Height - nMargin;
            int nW = this.Width - nMargin;
            int nFontSize = 11;

            using (BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                Graphics G = bufferedgraphic.Graphics;
                G.Clear(this.BackColor);
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                Font FontInfo = new Font(this.Font.Name, nFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                int nHNew = nH / 4;
                int nWidthSubject = 50;
                int nWidthValue = nW - nWidthSubject;

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                Rectangle rt0 = new Rectangle(0, 0, nWidthSubject + nWidthValue, nHNew);
                Rectangle rt00 = new Rectangle(0, 0, nWidthSubject, nHNew);
                Rectangle rt10 = new Rectangle(rt00.Left, rt00.Bottom, nWidthSubject, nHNew);
                Rectangle rt20 = new Rectangle(rt10.Left, rt10.Bottom, nWidthSubject, nHNew);
                Rectangle rt30 = new Rectangle(rt20.Left, rt20.Bottom, nWidthSubject, nHNew);

                Rectangle rt01 = new Rectangle(rt00.Right, rt00.Top, nWidthValue, nHNew);
                Rectangle rt11 = new Rectangle(rt01.Left, rt01.Bottom, nWidthValue, nHNew);
                Rectangle rt21 = new Rectangle(rt11.Left, rt11.Bottom, nWidthValue, nHNew);
                Rectangle rt31 = new Rectangle(rt21.Left, rt21.Bottom, nWidthValue, nHNew);

                DrawStringFillRect(G, "LoadPort", FontInfo, rt00, Brushes.LightGray);
                DrawStringFillRect(G, "Size", FontInfo, rt10, Brushes.FloralWhite);
                DrawStringFillRect(G, "Slot", FontInfo, rt20, Brushes.FloralWhite);
                DrawStringFillRect(G, "WaferID", FontInfo, rt30, Brushes.FloralWhite);

                if (m_bExist == true)
                {
                    DrawStringFillRect(G, (m_nLoadPort+1).ToString(), FontInfo, rt01, Brushes.FloralWhite);
                    DrawStringFillRect(G, m_strWaferSize.ToString(), FontInfo, rt11, Brushes.FloralWhite);
                    DrawStringFillRect(G, m_nSlot.ToString(), FontInfo, rt21, Brushes.FloralWhite);
                    DrawStringFillRect(G, m_strWaferID, FontInfo, rt31, Brushes.FloralWhite);
                }
                else
                {
                    DrawStringFillRect(G, "", FontInfo, rt01, Brushes.FloralWhite);
                    DrawStringFillRect(G, "", FontInfo, rt11, Brushes.FloralWhite);
                    DrawStringFillRect(G, "", FontInfo, rt21, Brushes.FloralWhite);
                    DrawStringFillRect(G, "", FontInfo, rt31, Brushes.FloralWhite);
                }

                bufferedgraphic.Render(e.Graphics);
            }
            #endregion
        }

        private int GetTxtWidth(Graphics G, string s, Font f)
        {
            SizeF AdjustedSizeNew = G.MeasureString(s, f);
            return (int)(AdjustedSizeNew.Width * 1.2);
        }
        private void DrawStringFillRect(Graphics G, string str, Font font, Rectangle rt, Brush fill = null)
        {
            Pen pen = new Pen(Color.LightGray);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            G.DrawRectangle(pen, rt);
            if (fill != null)
            {
                G.FillRectangle(fill, rt);
            }

            G.DrawString(str, font, Brushes.Black, rt, stringFormat);
        }
    }
}
