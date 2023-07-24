using System;
using System.IO; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ezTools
{
    public class ezImg
    {
        public string m_id;
        public bool m_bNew = false;
        public CPoint m_szImg = new CPoint(0, 0);
        public int m_nByte = 1;
        public byte[,] m_aBuf;
        public double m_fAngle = 0f;

        Bitmap m_bitmap = null;
        Log m_log;

        private readonly object hLock = new object();

        public ezImg(string id, Log log)
        {
            m_id = id; m_log = log;
        }

        public void ReAllocate(ezImg rImg)
        {
            ReAllocate(rImg.m_szImg, rImg.m_nByte); 
        }
        
        public void ReAllocate(CPoint szImg, int nByte)
        {
            if ((m_szImg == szImg) && (m_nByte == nByte)) return;
            if (szImg.x < 1 || szImg.y < 1) return;
            m_szImg = szImg; m_nByte = nByte; m_szImg.x = (m_szImg.x / 4) * 4; 
            m_aBuf = new byte[m_szImg.y, nByte * m_szImg.x];
            if (m_bitmap == null) return;
            if (m_bitmap.Size == (Size)m_szImg.ToPoint()) return;
            ReAllocateBitmap(m_szImg, m_nByte); 
        }

        void ReAllocateBitmap(CPoint szImg, int nByte)
        {
            PixelFormat pixelFormat; 
            if (szImg.x < 1 || szImg.y < 1) { m_log.Popup("Image Size Is Wrong !!"); return; }
            if (nByte == 3) pixelFormat = PixelFormat.Format24bppRgb;
            else pixelFormat = PixelFormat.Format8bppIndexed; 
            m_bitmap = new Bitmap(szImg.x, szImg.y, szImg.x, pixelFormat, GetIntPtr(0, 0)); //forget
            if (nByte != 1) return;
            SetPalette();
        }

        void SetPalette()
        {
            int n;
            ColorPalette palette = m_bitmap.Palette;
            for (n = 0; n < 256; n++) palette.Entries[n] = Color.FromArgb(n, n, n);
            m_bitmap.Palette = palette;             
        }

        public Bitmap GetBitmap()
        {
            lock (hLock)
            {
                if (m_bitmap == null)
                {
                    ReAllocateBitmap(m_szImg, m_nByte);
                }
                else
                {
                    BitmapData data = m_bitmap.LockBits(new Rectangle(0, 0, m_szImg.x, m_szImg.y), ImageLockMode.ReadWrite, m_bitmap.PixelFormat);
                    unsafe
                    {
                        byte* pDst = (byte*)data.Scan0;
                        byte* pSrc = (byte*)GetIntPtr(0, 0);
                        cpp_memcpy(pDst, pSrc, m_szImg.x * m_szImg.y * m_nByte);
                    }
                    m_bitmap.UnlockBits(data);
                }
                return m_bitmap;
            }
        }

        public bool IsInside(ref int x, ref int y)
        {
            bool bInside = true;
            if (x < 0) { m_log.Popup(x.ToString("GetIntPtr x = 0")); x = 0; bInside = false; }
            if (y < 0) { m_log.Popup(y.ToString("GetIntPtr y = 0")); y = 0; bInside = false; }
            if (x >= m_szImg.x) { m_log.Popup(x.ToString("GetIntPtr x = 0")); x = m_szImg.x - 1; bInside = false; }
            if (y >= m_szImg.y) { m_log.Popup(y.ToString("GetIntPtr y = 0")); y = m_szImg.y - 1; bInside = false; }
            return bInside;
        }

        public bool IsInside(ref CPoint cp)
        {
            return IsInside(ref cp.x, ref cp.y); 
        }

        public IntPtr GetIntPtr(int y, int x)
        {
            IntPtr ip;
            IsInside(ref x, ref y);
            if (m_aBuf == null) return (IntPtr)0; 
            unsafe 
            { 
                fixed (byte *p = &m_aBuf[y, x]) { ip = (IntPtr)(p); }
            }
            return ip;
        }

        //Draw

        public void Clone(ezImg img)
        {
            ReAllocate(img); 
            img.m_aBuf.CopyTo(m_aBuf, 0); //forget
        }

        public bool FileOpen(string strFile, bool bGray = false)
        {
            string[] strFiles = strFile.Split(new char[] {'.'});
            int l = strFiles.Length;
            if (l < 2) { m_log.Popup("Invalid File Name !!"); return true; }
            string strExt = strFiles[l-1].ToLower();
            if (strExt == "bmp")
            {
                if (bGray) return FileOpenBMPGray(strFile);
                return FileOpenBMP(strFile);
            }
            if (strExt == "jpg")
            {
                if (bGray) return FileOpenJPGGray(strFile); 
                return FileOpenJPG(strFile);
            }
            return false;
        }

        public void FileSave(string strFile)
        {
            string[] strFiles = strFile.Split(new char[] { '.' });
            int l = strFiles.Length;
            if (l < 2) { m_log.Popup("Invalid File Name !!"); return; }
            //if (m_bitmap == null) { m_log.Popup("Image is null !!"); return; }
            string strExt = strFiles[l - 1].ToLower();
            Cursor.Current = Cursors.WaitCursor;
            Bitmap bitmap = GetBitmap();
            if (bitmap == null) { m_log.Popup("No Image !!"); return; }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            try
            {
                if (strExt == "bmp") bitmap.Save(strFile, ImageFormat.Bmp);
                if (strExt == "jpg") bitmap.Save(strFile, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
                Cursor.Current = Cursors.Default;
            }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY); // ing
            Cursor.Current = Cursors.Default;
        }

        bool FileOpenBMP(string strFile)
        {
            int nByte; 
            CPoint szImg = new CPoint(0, 0);
            FileStream fs = null;
            try
            {
                fs = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read, 32768, true);
            }
            catch(Exception ex)
            {
                m_log.Popup(ex.Message); return true;
            }
            BinaryReader br = new BinaryReader(fs);
            ushort bfType = br.ReadUInt16(); 
            uint bfSize = br.ReadUInt32();
            br.ReadUInt16(); br.ReadUInt16();
            uint bfOffBits = br.ReadUInt32();
            if (bfType != 0x4D42) { m_log.Popup("File is not BMP !!"); return true; }
            uint biSize = br.ReadUInt32();
            szImg.x = br.ReadInt32(); 
            szImg.y = br.ReadInt32();
            br.ReadUInt16();
            nByte = br.ReadUInt16() / 8;
            br.ReadUInt32();
            br.ReadUInt32();
            br.ReadInt32(); br.ReadInt32(); 
            br.ReadUInt32(); br.ReadUInt32();
            if (m_nByte != 1) { m_log.Popup("BMP is not Gray !!"); return true; }
            byte[] hRGB = br.ReadBytes(256 * 4);
            ReAllocate(szImg, nByte);
            byte[] pBuf; int x, y;
            for (y = 0; y < m_szImg.y; y++)
            {
                pBuf = br.ReadBytes(m_szImg.x);
                Buffer.BlockCopy(pBuf, 0, m_aBuf, y * m_szImg.x, m_szImg.x); //forget
            }
            br.Close(); fs.Close();
            /*byte[] pGV = new byte[256];
            bool bTable = false; 
            for (int n = 0; n < 256; n++) 
            {
                pGV[n] = (byte)(0.299 * hRGB[4 * n] + 0.587 * hRGB[4 * n + 1] + 0.114 * hRGB[4 * n + 2]);  //forget B, G, R
                if (pGV[n] != n) bTable = true; 
            }
            if (bTable)
            {
                for (y = 0; y < m_szImg.y; y++) for (x = 0; x < m_szImg.x; x++)
                {
                    m_aBuf[y, x] = pGV[m_aBuf[y, x]]; 
                }
            }*/
            m_bNew = true; 
            return false; 
        }

        bool FileOpenBMPGray(string strFile)
        {
            int x, y;
            Bitmap lbmpBitmap = new Bitmap(strFile);
            m_bitmap = new Bitmap(lbmpBitmap.Width, lbmpBitmap.Height, PixelFormat.Format8bppIndexed);
            ReAllocate(new CPoint(m_bitmap.Width, m_bitmap.Height), 1);
            Rectangle lrRect = new Rectangle(0, 0, lbmpBitmap.Width, lbmpBitmap.Height);
            BitmapData lbdData = lbmpBitmap.LockBits(lrRect, ImageLockMode.WriteOnly, lbmpBitmap.PixelFormat);
            BitmapData data = m_bitmap.LockBits(lrRect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            unsafe
            {
                byte* pSrc = (byte*)lbdData.Scan0;
                byte* pDst = (byte*)GetIntPtr(0, 0);
                if ((lbmpBitmap.PixelFormat == PixelFormat.Format32bppArgb) || (lbmpBitmap.PixelFormat == PixelFormat.Format32bppRgb))
                {
                    for (y = m_bitmap.Height - 1; y >= 0; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        for (x = 0; x < m_bitmap.Width; x++)
                        {
                            byte r = (byte)(*pSrc * 0.299); pSrc++;
                            byte g = (byte)(*pSrc * 0.587); pSrc++;
                            byte b = (byte)(*pSrc * 0.114); pSrc++;
                            pSrc++;
                            *pDst = (byte)(r + g + b);
                            pDst++;
                        }
                    }
                }
                else
                {
                    for (y = m_bitmap.Height - 1; y >= 0; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        for (x = 0; x < m_bitmap.Width; x++)
                        {
                            byte r = (byte)(*pSrc * 0.299); pSrc++;
                            byte g = (byte)(*pSrc * 0.587); pSrc++;
                            byte b = (byte)(*pSrc * 0.114); pSrc++;
                            *pDst = (byte)(r + g + b);
                            pDst++;
                        }
                    }
                }
            }
            lbmpBitmap.UnlockBits(lbdData);
            lbmpBitmap.Dispose(); 
            m_bitmap.UnlockBits(data);
            SetPalette();
            return false; //forget
        }

        bool FileOpenJPG(string strFile) 
        {
            int x, y;
            Stream imageStreamSource = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder ldDecoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapFrame lfFrame = ldDecoder.Frames[0];
            Bitmap lbmpBitmap = new Bitmap(lfFrame.PixelWidth, lfFrame.PixelHeight);
            m_bitmap = new Bitmap(lfFrame.PixelWidth, lfFrame.PixelHeight, (lfFrame.Format.BitsPerPixel == 24 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb));
            ReAllocate(new CPoint(m_bitmap.Width, m_bitmap.Height), 3);
            Rectangle lrRect = new Rectangle(0, 0, lbmpBitmap.Width, lbmpBitmap.Height);
            BitmapData lbdData = lbmpBitmap.LockBits(lrRect, ImageLockMode.WriteOnly, (lfFrame.Format.BitsPerPixel == 24 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb));
            lfFrame.CopyPixels(System.Windows.Int32Rect.Empty, lbdData.Scan0, lbdData.Height * lbdData.Stride, lbdData.Stride);
            BitmapData data = m_bitmap.LockBits(lrRect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            unsafe
            {
                byte* pSrc = (byte*)lbdData.Scan0;
                byte* pDst = (byte*)GetIntPtr(0, 0);
                if (lfFrame.Format.BitsPerPixel == 32)
                {
                    for (y = m_bitmap.Height - 1; y >= 0 ; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        for (x = 0; x < m_bitmap.Width; x++)
                        {
                            *pDst = *pSrc; pSrc++; pDst++;
                            *pDst = *pSrc; pSrc++; pDst++;
                            *pDst = *pSrc; pSrc++; pDst++; 
                            pSrc++;
                        }
                    }
                }
                else
                {
                    for (y = m_bitmap.Height - 1; y >= 0; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        cpp_memcpy(pDst, pSrc, 3 * m_bitmap.Width);
                        pSrc += 3 * m_bitmap.Width; 
                    }
                }
            }
            lbmpBitmap.UnlockBits(lbdData);
            lbmpBitmap.Dispose(); 
            m_bitmap.UnlockBits(data);
            return false; 
        }

        bool FileOpenJPGGray(string strFile)
        {
            int x, y;
            Stream imageStreamSource = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder ldDecoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapFrame lfFrame = ldDecoder.Frames[0];
            Bitmap lbmpBitmap = new Bitmap(lfFrame.PixelWidth, lfFrame.PixelHeight);
            m_bitmap = new Bitmap(lfFrame.PixelWidth, lfFrame.PixelHeight, PixelFormat.Format8bppIndexed);
            ReAllocate(new CPoint(m_bitmap.Width, m_bitmap.Height), 1);
            Rectangle lrRect = new Rectangle(0, 0, lbmpBitmap.Width, lbmpBitmap.Height);
            BitmapData lbdData = lbmpBitmap.LockBits(lrRect, ImageLockMode.WriteOnly, (lfFrame.Format.BitsPerPixel == 24 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb));
            lfFrame.CopyPixels(System.Windows.Int32Rect.Empty, lbdData.Scan0, lbdData.Height * lbdData.Stride, lbdData.Stride);
            BitmapData data = m_bitmap.LockBits(lrRect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            unsafe
            {
                byte* pSrc = (byte*)lbdData.Scan0;
                byte* pDst = (byte*)GetIntPtr(0, 0);
                if (lfFrame.Format.BitsPerPixel == 32)
                {
                    for (y = m_bitmap.Height - 1; y >= 0; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        for (x = 0; x < m_bitmap.Width; x++)
                        {
                            byte r = (byte)(*pSrc * 0.299); pSrc++;
                            byte g = (byte)(*pSrc * 0.587); pSrc++;
                            byte b = (byte)(*pSrc * 0.114); pSrc++;
                            pSrc++;
                            *pDst = (byte)(r + g + b);
                            pDst++;
                        }
                    }
                }
                else
                {
                    for (y = m_bitmap.Height - 1; y >= 0; y--)
                    {
                        pDst = (byte*)GetIntPtr(y, 0);
                        for (x = 0; x < m_bitmap.Width; x++)
                        {
                            byte r = (byte)(*pSrc * 0.299); pSrc++;
                            byte g = (byte)(*pSrc * 0.587); pSrc++;
                            byte b = (byte)(*pSrc * 0.114); pSrc++;
                            *pDst = (byte)(r + g + b);
                            pDst++;
                        }
                    }
                }
            }
            lbmpBitmap.UnlockBits(lbdData);
            lbmpBitmap.Dispose(); 
            m_bitmap.UnlockBits(data);
            SetPalette();
            return false; //forget
        }

        public void Clear()
        {
            //m_aBuf.Initialize();
            Array.Clear(m_aBuf, 0, m_aBuf.Length);
            m_bNew = true;
        }

        public void Fill(byte val)
        {
            unsafe
            {
                cpp_memset((byte*)GetIntPtr(0, 0), val, m_szImg.x * m_szImg.y * m_nByte);
            }
        }

        public bool CheckRange(ref CPoint cpImg, ref CPoint szImg)
        {
            if (szImg.x < 0) { cpImg.x += szImg.x; szImg.x = -szImg.x; }
            if (szImg.y < 0) { cpImg.y += szImg.y; szImg.y = -szImg.y; }
            if ((cpImg.x < 0) || ((cpImg.x + szImg.x) > m_szImg.x)) return true;
            if ((cpImg.y < 0) || ((cpImg.y + szImg.y) > m_szImg.y)) return true;
            return false; 
        }

        public bool Copy(ezImg rImgSrc, CPoint cpSrc, CPoint cpDst, CPoint szSrc)
        {
            unsafe
            {
                int[] y = new int[2]; byte* pSrc; byte* pDst;
                for (y[0] = cpSrc.y, y[1] = cpDst.y; y[0] < cpSrc.y + szSrc.y; y[0]++, y[1]++)
                {
                    pSrc = (byte*)rImgSrc.GetIntPtr(y[0], cpSrc.x);
                    pDst = (byte*)GetIntPtr(y[1], cpDst.x);
                    cpp_memcpy(pDst, pSrc, szSrc.x);
                }
            }
            return false; 
        }

        public unsafe bool Copy(byte *pSrc)
        {
            byte* pDst = (byte*)GetIntPtr(0, 0);
            cpp_memcpy(pDst, pSrc, m_szImg.x * m_szImg.y);
            return false; 
        }

        public Rectangle GetRect()
        {
            return new Rectangle(0, 0, m_szImg.x, m_szImg.y); 
        }

        public bool HasImage()
        {
            if (m_szImg.x == 0) return false;
            if (m_szImg.y == 0) return false;
            return true; 
        }

        public Bitmap GetBitmapROI(CPoint cp1, CPoint cp0)
        {
            int nWidth = cp1.x - cp0.x;
            int nHeight = cp1.y - cp0.y;
            if (nWidth < 4 || nHeight < 1)
            {
                m_log.Popup("ROI Save Fail. Image Size Is Wrong !!");
                return null;
            }
            Bitmap bmpROI = new Bitmap(nWidth, nHeight, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmpROI.Palette;
            Color[] entries = palette.Entries;
            for (int n = 0; n < 256; n++) entries[n] = Color.FromArgb(n, n, n);
            bmpROI.Palette = palette;
            BitmapData btData = bmpROI.LockBits(new Rectangle(0, 0, bmpROI.Width, bmpROI.Height), ImageLockMode.ReadWrite, bmpROI.PixelFormat);
            unsafe
            {
                byte* pSrc = (byte*)btData.Scan0;
                for (int y = 0; y < bmpROI.Height; y++)
                {
                    byte* pOrg = (byte*)GetIntPtr((cp1.y - y) % m_szImg.y, cp0.x);
                    for (int x = 0; x < bmpROI.Width; x++)
                    {
                        *pSrc = *pOrg; pSrc++; pOrg++;
                    }
                }
            }
            bmpROI.UnlockBits(btData);
            return bmpROI;
        }

        public bool SaveROIGrayBMP(string strFile, CPoint cp1, CPoint cp0)
        {
            int nWidth = cp1.x - cp0.x;
            int nHeight = cp1.y - cp0.y;
            if (nWidth < 4 || nHeight < 1)
            {
                m_log.Popup("ROI Save Fail. Image Size Is Wrong !!");
                return true;
            }
            Bitmap bmpROI = new Bitmap(nWidth, nHeight, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmpROI.Palette;
            Color[] entries = palette.Entries;
            for (int n = 0; n < 256; n++) entries[n] = Color.FromArgb(n, n, n);
            bmpROI.Palette = palette;
            BitmapData btData = bmpROI.LockBits(new Rectangle(0, 0, bmpROI.Width, bmpROI.Height), ImageLockMode.ReadWrite, bmpROI.PixelFormat);
            unsafe
            {
                byte* pSrc = (byte*)btData.Scan0;
                for (int y = 0; y < bmpROI.Height; y++)
                {
                    byte* pOrg = (byte*)GetIntPtr((cp1.y - y) % m_szImg.y, cp0.x);
                    for (int x = 0; x < bmpROI.Width; x++)
                    {
                        *pSrc = *pOrg; pSrc++; pOrg++;
                    }
                }
            }
            bmpROI.UnlockBits(btData);
            bmpROI.Save(strFile + ".bmp", ImageFormat.Bmp);
            return false;
        }
        
        public bool AddTIF(FileStream fs, TiffBitmapEncoder encoder, CPoint cpLT, CPoint cpRT, CPoint cpLB, CPoint cpRB, double fAngle = 0, bool bGray = true, string strDraw = "", string strBmp = "") // ing 170208
        {
            int nWidth, nHeight, stride;
            double fDistance, fA;
            CPoint cpGV;
            if (cpLT.y > m_szImg.y - 1) cpLT.y -= m_szImg.y;
            if (cpRT.y > m_szImg.y - 1) cpRT.y -= m_szImg.y;
            if (cpLB.y > m_szImg.y - 1) cpLB.y -= m_szImg.y;
            if (cpRB.y > m_szImg.y - 1) cpRB.y -= m_szImg.y;
            if (!IsInside(ref cpLT)) return true;
            if (!IsInside(ref cpRT)) return true;
            if (!IsInside(ref cpLB)) return true;
            if (!IsInside(ref cpRB)) return true;
            nWidth = (int)cpLT.GetL(cpRT);
            nHeight = (int)cpLT.GetL(cpLB);
            if (nWidth < 4) return true;
            if (nWidth % 4 == 0) stride = nWidth;
            else stride = nWidth - nWidth % 4 + 4;
            byte[] aPixel = new byte[nHeight * stride];
            for (int n = 0; n < nHeight; n++)
            {
                unsafe
                {
                    if (fAngle == 0)
                    {
                        if (cpLT.y - n < 0) return true;
                        byte* pSrc = (byte*)GetIntPtr(cpLT.y - n, cpLT.x);
                        fixed (byte* pDst = &(aPixel[n * stride]))
                        {
                            cpp_memcpy(pDst, pSrc, stride);
                        }
                    }
                    else if (fAngle == 90)
                    {
                        if (cpLB.x + n > m_szImg.x - 1) return true;
                        byte* pSrc = (byte*)GetIntPtr(cpLB.y, cpLB.x + n);
                        for (int m = 0; m < stride; m++)
                        {
                            aPixel[n * stride + m] = *pSrc;
                            pSrc += m_szImg.x;
                        }
                    }
                    else if (fAngle == 180)
                    {
                        if (cpLT.y + n > m_szImg.y - 1) return true;
                        byte* pSrc = (byte*)GetIntPtr(cpLT.y + n, cpLT.x);
                        for (int m = 0; m < stride; m++)
                        {
                            aPixel[n * stride + m] = *pSrc;
                            pSrc--;
                        }
                    }
                    else if (fAngle == 270)
                    {
                        if (cpLT.x - n < 0) return true;
                        byte* pSrc = (byte*)GetIntPtr(cpLT.y, cpLT.x - n);
                        for (int m = 0; m < stride; m++)
                        {
                            aPixel[n * stride + m] = *pSrc;
                            pSrc -= m_szImg.x;
                        }
                    }
                    else
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
                }
            }
            try
            {
                BitmapPalette myPalette;
                BitmapSource image;
                if (bGray)
                {
                    myPalette = BitmapPalettes.Gray256;
                    image = BitmapSource.Create(stride, nHeight, 96, 96, System.Windows.Media.PixelFormats.Indexed8, myPalette, aPixel, stride);
                    if (strBmp != "")
                    {
                        try
                        {
                            FileStream fileStream = new FileStream(strBmp + ".bmp", FileMode.Create);
                            BmpBitmapEncoder bmpEncoder = new BmpBitmapEncoder();
                            bmpEncoder.Frames.Add(BitmapFrame.Create(image));
                            bmpEncoder.Save(fileStream);
                            fileStream.Close();
                        }
                        catch
                        {
                            m_log.Add(strBmp + ".bmp Save Fail");
                        }
                    }
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
                    bmpColor.Save("d:\\TestColor.bmp", ImageFormat.Bmp);
                    image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpColor.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    g.Dispose(); 
                }
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(image));
            }
            catch (Exception ex)
            {
                m_log.Popup(" Can not Save " + fs.Name + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        public bool SaveTIF(string strFile, ArrayList aIsland)
        {
            if (aIsland.Count < 1) return false;
            FileStream stream;
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            try
            {
                stream = new FileStream(strFile + ".tif", FileMode.Create);
            }
            catch (Exception ex)
            {
                m_log.Popup("File Create Fail : " + strFile + ".tif");
                m_log.Add(ex.Message);
                return true;
            }
            foreach (azBlob.Island island in aIsland)   
            {
                int x, y, nWidth, nHeight, stride;
                int[] nMargin = new int[2] { 0, 0 };
                CPoint cp0 = island.m_cp0;
                CPoint cp1 = island.m_cp1;
                nWidth = cp1.x - cp0.x;
                nHeight = cp1.y - cp0.y;
                if (nWidth < 100) nMargin[0] = 100;
                if (nHeight < 100) nMargin[1] = 100;
                nWidth += nMargin[0] * 2;
                nHeight += nMargin[1] * 2;
                x = cp0.x - nMargin[0]; 
                y = cp0.y - nMargin[1];
                if (x < 1 || y < 1 || nWidth < 1 || nWidth < 1 || x > m_szImg.x - 1 || y > m_szImg.y - 1 || x + nWidth > m_szImg.x - 1 || y + nHeight > m_szImg.y - 1 || nWidth < 4)
                {
                    stream.Close(); 
                    return true;
                }
                if (nWidth % 4 == 0) stride = nWidth;
                else stride = nWidth - nWidth % 4 + 4;
                byte[] aPixel = new byte[nHeight * stride];
                for (int n = 0; n < nHeight; n++)
                {
                    unsafe
                    {
                        byte* pSrc = (byte*)GetIntPtr(y + n, x);
                        for (int m = 0; m < stride; m++)
                        {
                            aPixel[n * stride + m] = *pSrc;
                            pSrc++;
                        }
                    }
                }
                try
                {
                    BitmapPalette myPalette = BitmapPalettes.Gray256;
                    BitmapSource image = BitmapSource.Create(stride, nHeight, 96, 96, System.Windows.Media.PixelFormats.Indexed8, myPalette, aPixel, stride);
                    encoder.Compression = TiffCompressOption.None;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                }
                catch(Exception ex)
                {
                    m_log.Popup(" Can not Save " + strFile + ".tif");
                    m_log.Add(ex.Message);
                    stream.Close();
                    return true;
                }
            }

            try
            {
                encoder.Save(stream);
            }
            catch (Exception ex)
            {
                m_log.Popup(" Can not Save " + strFile + ".tif");
                m_log.Add(ex.Message);
                stream.Close();
            }
            stream.Close();
            return false;
        }

        /***************************************************/
        //Thresholding image.Start
        /***************************************************/

        public void Binarization(CPoint cp, CPoint szSrc, int nGV)
        {
            unsafe
            {
                int y, x; byte* pSrc;
                if (!CheckPoint(cp, szSrc)) return;
                for (y = cp.y; y < cp.y + szSrc.y; y++)
                {
                    pSrc = (byte*)GetIntPtr(y - cp.y, cp.x);
                    for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { if (*pSrc > nGV) *pSrc = 255; else *pSrc = 0; }
                }

            }
        }

        public void AdaptiveThrehold(CPoint cp, CPoint szSrc, int nSize, int nOffset)
        {
            unsafe
            {
                int d_m, d_n, x, y, d_size, x1, x2, y1, y2;
                double[] pSum;
                double sum_pixel; byte* pSrc; 
                byte[] pBuf;
	            if (!CheckPoint(cp, szSrc)) return;
	            pSrc = (byte*)GetIntPtr(0, 0);
	            d_m = szSrc.y; d_n = szSrc.x;
	            pSum = new double[szSrc.x*szSrc.y];

	            pSum[0] = pSrc[0];
	            for (x = 1; x < d_n; x++) pSum[x] = pSum[x - 1] + pSrc[x];
	            for (y = 1; y < d_m; y++) pSum[y*szSrc.x] = pSum[(y - 1)*szSrc.x] + pSrc[y*szSrc.x];
	            for (y = 1; y < d_m; y++) for (x = 1; x < d_n; x++)
		            pSum[x + y*szSrc.x] = pSum[x - 1 + y*szSrc.x] + pSum[x + (y - 1)*szSrc.x] - pSum[x - 1 + (y - 1)*szSrc.x] + pSrc[x + y*szSrc.x];
	            d_size = nSize / 2;
	            pBuf = new byte[szSrc.x*szSrc.y];
	            for (y = 0; y<szSrc.y; y++) for (x = 0; x<szSrc.x; x++)
	            {
		            x1 = x - d_size; y1 = y - d_size; x2 = x + d_size; y2 = y + d_size;
		            if (x1 < 1) x1 = 1; if (x2 > szSrc.x - 1) x2 = szSrc.x - 1;
		            if (y1 < 1) y1 = 1; if (y2 > szSrc.y - 1) y2 = szSrc.y - 1;
		            sum_pixel = pSum[x2 + y2*szSrc.x] - pSum[x2 + (y1 - 1)*szSrc.x] - pSum[x1 - 1 + y2*szSrc.x] + pSum[(x1 - 1) + (y1 - 1)*szSrc.x];
		            if (sum_pixel*(100 - nOffset) / 100 >= pSrc[x + y*szSrc.x] * (x2 - x1)*(y2 - y1)) pBuf[x + y*szSrc.x] = 0;
		            else pBuf[x + y*szSrc.x] = 255;
	            }
	            for (y = 0; y < d_m; y++) for (x = 0; x < d_n; x++) pSrc[y*szSrc.x + x] = pBuf[y*szSrc.x + x];
            }
            
        }

        public void InteractiveThreshold(CPoint cp, CPoint szSrc)
        {
            unsafe
            {
                int y, x, n, nThres, nThresOld, a, b, c, d, nArea; byte *pSrc;
                int[] aHisto = new int[256];
	            if (!CheckPoint(cp, szSrc)) return;
	            nThres = 0;
	            for (n = 0; n<256; n++) aHisto[n] = 0;
	            for (y = cp.y; y < cp.y + szSrc.y; y++)
	            {
                    pSrc = (byte*)GetIntPtr(y - cp.y, cp.x);
		            for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { aHisto[*pSrc]++; nThres += *pSrc; }
	            }
	            nArea = szSrc.x * szSrc.y;
	            nThres = (int)(1.0*nThres / nArea);
	            do
	            {
		            nThresOld = nThres; a = b = 0; c = d = 0;
		            for (n = 0; n <= nThresOld; n++) { a += n*aHisto[n]; b += aHisto[n]; }
		            for (n = nThresOld + 1; n<256; n++) { c += n*aHisto[n]; d += aHisto[n]; }
		            b += b; d += d; if (b == 0) b = 1; if (d == 0) d = 1;
		            nThres = a / b + c / d;
	            } while (nThres != nThresOld);
	            for (y = cp.y; y < cp.y + szSrc.y; y++)
	            {
		            pSrc = (byte*)GetIntPtr(y - cp.y, cp.x);
		            for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { if (*pSrc<nThres) *pSrc = 0; else *pSrc = 255; }
	            }
            }
        }

        /*public void BandThreshold(CPoint cp, CPoint szSrc, long nBandMin, long nBandMax)
        {
            unsafe
            {
                int y, x; byte* pSrc;
                Mat img_src;
                Mat img_tg1, img_tg2, img_dst;
                pSrc = GetAdd(cp.x, cp.y);
                long m, n;
                m = szSrc.cy;
                n = szSrc.cx;
                img_src.create(m, n, CV_8UC1);

                for (y = cp.y; y < cp.y + szSrc.cy; y++)
                    for (x = cp.x; x < cp.x + szSrc.cx; x++)
                    {
                        img_src.at<uchar>(y, x) = pSrc[x + y * szSrc.cx];
                    }

                threshold(img_src, img_tg1, nBandMin, 255, CV_THRESH_BINARY);
                threshold(img_src, img_tg2, nBandMax, 255, CV_THRESH_BINARY_INV);
                bitwise_and(img_tg1, img_tg2, img_dst);
                for (y = cp.y; y < cp.y + szSrc.cy; y++)
                    for (x = cp.x; x < cp.x + szSrc.cx; x++)
                        pSrc[x + y * szSrc.cx] = img_dst.at<uchar>(y, x);
            }
        }*/ // ing using opencv

        /*public void KMeanThreshold(CPoint cp, CSize szSrc, long nSegment)
        {
	        long y, x; BYTE *pSrc;
	        Mat img_src;
	        Mat img_tg1, img_tg2, img_dst;
	        int i, j;
	        pSrc = GetAdd(cp.x, cp.y);
	        long m, n;
	        m = szSrc.cy;
	        n = szSrc.cx;
	        img_src.create(m, n, CV_8UC1);
	        img_dst.create(m, n, CV_8UC1);
	        for (y = cp.y; y < cp.y + szSrc.cy; y++)
	        for (x = cp.x; x < cp.x + szSrc.cx; x++)
	        {
		        img_src.at<uchar>(y, x) = pSrc[x + y*szSrc.cx];
	        }
	        Mat samples;
	        samples.create(m*n, 1, CV_32FC1);
	        for (i = 0; i < m; i++)
	        for (j = 0; j < n; j++)
	        {
		        samples.at<float>(i*n + j, 0) = img_src.at<uchar>(i, j);
	        }
	        int cluster_count = nSegment;
	        int attempts = 5;
	        Mat centers, labels;
	        kmeans(samples, cluster_count, labels, TermCriteria(CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 10000, 0.0001), attempts, KMEANS_PP_CENTERS, centers);
	        int free[100];
	        for (i = 0; i < 100; i++)
		        free[i] = 0;
	        int color[100];
	        if (cluster_count>1)
		        x = 255 / (cluster_count - 1);
	        else x = 0;

	        color[0] = 0;

	        for (i = 1; i < cluster_count; i++)
	        {
		        color[i] = color[i - 1] + x;
	        }

	        x = 0;
	        for (i = 0; i < m; i++)
	        for (j = 0; j < n; j++)
	        {
		        int p = labels.at<int>(i*n + j, 0);
		        if (free[p] == 0)
		        {
			        centers.at<float>(p, 0) = (float)color[x];
			        x++;
			        free[p] = 1;
		        }
	        }

	        for (i = 0; i < m; i++)
	        for (j = 0; j < n; j++)
	        {
		        int cluster_idx = labels.at<int>(i*n + j, 0);
		        img_dst.at<uchar>(i, j) = (uchar)centers.at<float>(cluster_idx, 0);
	        }

	        for (y = cp.y; y < cp.y + szSrc.cy; y++)
	        for (x = cp.x; x < cp.x + szSrc.cx; x++)
		        pSrc[x + y*szSrc.cx] = img_dst.at<uchar>(y, x);
        }*/ // ing using opencv

        public void OtsuThreshold(CPoint cp, CPoint szSrc)
        {
            unsafe
            {
                int n, x, y; byte* pSrc; double OtsuValue; double[] aHisto = new double[256];
	            if (!CheckPoint(cp, szSrc)) return;
	            for (n = 0; n<256; n++) aHisto[n] = 0;
	            for (y = cp.y; y < cp.y + szSrc.y; y++)
	            {
		            pSrc = (byte*)GetIntPtr(y, cp.x);
		            for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { aHisto[*pSrc] += 1.0; }
	            }
	            OtsuValue = GetOtsuThreshold(aHisto);
	            for (y = cp.y; y < cp.y + szSrc.y; y++)
	            {
                    pSrc = (byte*)GetIntPtr(y, cp.x);
		            for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { if (*pSrc<OtsuValue) *pSrc = 0; else *pSrc = 255; }
                }
            }   
        }

        double Px(int init, int end, double[] hist)
        {
            double sum = 0; int i;
            for (i = init; i <= end; i++) sum += hist[i];
            return (double)sum;
        }

        double Mx(int init, int end, double[] hist)
        {
            double sum = 0; int i;
            for (i = init; i <= end; i++) sum += i * hist[i];
            return (double)sum;
        }

        int GetMax(double[] vec, int n)
        {
            double maxVec = 0; int i, idx = 0;
            for (i = 1; i < n - 1; i++) { if (vec[i] > maxVec) { maxVec = vec[i]; idx = i; } }
            return idx;
        }

        int GetOtsuThreshold(double[] hist)
        {
            byte t = 0;
	        double[] vet = new double[256]; double p1, p2, p12, diff; int k;
	        for (k = 1; k <= 255; k++)
	        {
		        p1 = Px(0, k, hist); p2 = Px(k + 1, 255, hist); p12 = p1 * p2;
		        if (p12 == 0) p12 = 1;
		        diff = (Mx(0, k, hist) * p2) - (Mx(k + 1, 255, hist) * p1);
		        vet[k] = (double)diff * diff / p12;
	        }
	        t = (byte)GetMax(vet, 256);
	        return t;
        }

        /***************************************************/
        //Thresholding image.End
        /***************************************************/

        /***************************************************/
        //Morphology operation. Start
        /***************************************************/

        public void Erosion(CPoint cp, CPoint szSrc, byte nTarget)
        {
            ezStopWatch sw = new ezStopWatch();
            unsafe
            {
                int y, x; byte *pSrc;  byte[] aDst;
	            if (!CheckPoint(cp, szSrc)) return;
	            aDst = new byte[szSrc.x*szSrc.y]; 
               
                pSrc = (byte*)GetIntPtr(cp.y, cp.x);
                for (y = cp.y + 1; y < cp.y + szSrc.y - 1; y++)
                {
                    for (x = cp.x + 1; x < cp.x + szSrc.x - 1; x++)
                    {
                        if ((pSrc[x + y * szSrc.x] == nTarget) &&
                            (pSrc[x - 1 + (y - 1) * szSrc.x] == nTarget) &&
                            (pSrc[x + (y - 1) * szSrc.x] == nTarget) &&
                            (pSrc[x + 1 + (y - 1) * szSrc.x] == nTarget) &&
                            (pSrc[x - 1 + y * szSrc.x] == nTarget) &&
                            (pSrc[x + 1 + y * szSrc.x] == nTarget) &&
                            (pSrc[x - 1 + (y + 1) * szSrc.x] == nTarget) &&
                            (pSrc[x + (y + 1) * szSrc.x] == nTarget) &&
                            (pSrc[x + 1 + (y + 1) * szSrc.x] == nTarget))
                            aDst[x + y * szSrc.x] = nTarget;
                    }
                }
                fixed (byte* pDst = &aDst[0])
                {
                    cpp_memcpy(pSrc, pDst, szSrc.x * szSrc.y);
                }
            }
        }

        public void ErosionGray()
        {
            ezStopWatch sw = new ezStopWatch();
            unsafe
            {
                int[,] pSEGray = new int[3, 3] { { 1, 0, 1 }, { 0, 1, 1 }, { 1, 0, 1 } };

	            byte[,] TempImage;
	            byte[] pArray = new byte[9];
	            int Col, Row, i, j, count;
	            int width = m_szImg.x;
	            int height = m_szImg.y;
	            TempImage = new byte[height, width];

                fixed(byte* pTemp = &TempImage[0,0], pBuf = &m_aBuf[0,0])
                {
                    cpp_memcpy(pTemp, pBuf, height * width);

                    for (Row = 1; Row < height - 1; Row++)
                        for (Col = 1; Col < width - 1; Col++)
                        {
                            count = 0;
                            for (i = -1; i <= 1; i++)
                                for (j = -1; j <= 1; j++)
                                {
                                    if (pSEGray[i + 1, j + 1] == 1)
                                    {
                                        pArray[count] = m_aBuf[Row + i, Col + j];
                                        count++;
                                    }
                                }
                            TempImage[Row, Col] = Getmin(pArray, count - 1);
                        }
                    cpp_memcpy(pBuf, pTemp, height * width);
                }
               
            }
        }

        byte Getmax(byte[] pData, int size)
        {
            int i;
            int max = -1;
            for (i = 0; i < size; i++)
                if (max < pData[i])
                    max = pData[i];
            return (byte)max;
        }

        public void DilationGray()
        {
            int[,] pSEGray = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
	        byte[,] TempImage;
	        byte[] pArray = new byte[9];
	        int Col, Row, i, j, count;
	        int width = m_szImg.x;
	        int height = m_szImg.y;
            TempImage = new byte[height, width];
            unsafe
            {
                fixed (byte* pTemp = &TempImage[0, 0], pBuf = &m_aBuf[0, 0])
                {
                    cpp_memcpy(pTemp, pBuf, height * width);
                    for (Row = 1; Row < height - 1; Row++)
                        for (Col = 1; Col < width - 1; Col++)
                        {
                            count = 0;
                            for (i = -1; i <= 1; i++)
                                for (j = -1; j <= 1; j++)
                                {
                                    if (pSEGray[i + 1, j + 1] == 1)
                                    {
                                        pArray[count] = m_aBuf[Row + i, Col + j];
                                        count++;
                                    }
                                }
                            TempImage[Row, Col] = Getmax(pArray, count - 1);
                        }

                    /*for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            m_aBuf[y, x] = TempImage[y, x];
                        }
                    }*/
                    cpp_memcpy(pBuf, pTemp, height * width);
                }
            }           
        }

        byte Getmin(byte[] pData, int size)
        {
            int i;
            int min = 1000;
            for (i = 0; i < size; i++)
                if (min > pData[i])
                    min = pData[i];
            return (byte)min;
        }

        public void Dilation(CPoint cp, CPoint szSrc, byte nTarget)
        {
            unsafe
            {
                int y, x; byte* pSrc; byte[,] aDst;
	            if (!CheckPoint(cp, szSrc)) return;
                aDst = new byte[szSrc.y, szSrc.x]; 
                pSrc = (byte*)GetIntPtr(cp.y, cp.x);
                fixed(byte* pDst = &aDst[0,0])
                {
                    for (y = cp.y + 1; y < cp.y + szSrc.y - 1; y++)
                    {
                        for (x = cp.x + 1; x < cp.x + szSrc.x - 1; x++)
                        {
                            if (pSrc[x + y * szSrc.x] == nTarget)
                            {
                                aDst[y - 1, x - 1] = nTarget;
                                aDst[y - 1, x] = nTarget;
                                aDst[y - 1, x + 1] = nTarget;
                                aDst[y, x - 1] = nTarget;
                                aDst[y, x + 1] = nTarget;
                                aDst[y + 1, x - 1] = nTarget;
                                aDst[y + 1, x] = nTarget;
                                aDst[y + 1, x + 1] = nTarget;
                                aDst[y, x] = nTarget;

                            }
                        }
                    }
                    cpp_memcpy(pDst, pSrc, szSrc.x * szSrc.y);
                }
	           
	        }
        }

        /***************************************************/
        //Morphology operation. End
        /***************************************************/

        public void Resize(ezImg pImgOut, double dScale)
        {
            unsafe
            {
                byte[,] pIn; byte *pOut; byte *pTemp; CPoint sz; int x, y; CPoint cp; double dValue;
	            if (dScale == 1.0) return;
                ezImg ImgT = new ezImg(m_id + "TempImg", m_log);
                pImgOut.ReAllocate(m_szImg, 1);
	            sz.x = (int)(m_szImg.x * dScale); sz.y = (int)(m_szImg.y * dScale); ImgT.ReAllocate(sz, 1);
                pIn = m_aBuf;
                pOut = (byte*)pImgOut.GetIntPtr(0, 0);
	            pTemp = (byte*)ImgT.GetIntPtr(0, 0);
	            for (y = 0; y < sz.y; y++) for (x = 0; x < sz.x; x++)
	            {
		            dValue = Math.Abs(GetBilinearValue(x, y, m_szImg, pIn, dScale));
                    if (dValue > 255) dValue = 255;
		            pTemp[y*sz.x + x] = (byte)dValue;
	            }

	            if (dScale < 1)
	            {
		            cp.x = m_szImg.x / 2 - sz.x / 2; cp.y = m_szImg.y / 2 - sz.y / 2;
		            for (y = cp.y; y < cp.y + sz.y; y++)
		            {
                        pOut = (byte*)pImgOut.GetIntPtr(y, cp.x); pTemp = (byte*)ImgT.GetIntPtr(y - cp.y, 0);
			            for (x = cp.x; x < cp.x + sz.x; x++, pOut++, pTemp++) *pOut = *pTemp;
		            }
	            }
	            else
	            {
		            cp.x = -m_szImg.x / 2 + sz.x / 2; cp.y = -m_szImg.y / 2 + sz.y / 2;
		            for (y = 0; y < m_szImg.y; y++)
		            {
                        pOut = (byte*)pImgOut.GetIntPtr(y, 0); pTemp = (byte*)ImgT.GetIntPtr(y + cp.y, cp.x);
			            for (x = 0; x < m_szImg.x; x++, pOut++, pTemp++) *pOut = *pTemp;
		            }
	            }
            }
        }

        double GetBilinearValue(int Col, int Row, CPoint ImageSize, byte[,] Data, double factor)
        {
            double value;
	        double pixel_lt, pixel_rt, pixel_lb, pixel_rb;
	        double remain_x, remain_y;
	        int col, row;

	        col = (int)(Col / factor); row = (int)(Row / factor);
	        remain_x = Col - col*factor;
	        remain_y = Row - row*factor;

	        if (row >= 0 && row < ImageSize.y - 1 && col >= 0 && col < ImageSize.x - 1)
	        {
		        pixel_lt = Data[row, col];
		        pixel_rt = Data[row, col + 1];
		        pixel_lb = Data[row + 1, col];
		        pixel_rb = Data[row + 1, col + 1];
		        value = (1 - remain_y)*(remain_x*pixel_rt + (1 - remain_x)*pixel_lt)
			        + remain_y*(remain_x*pixel_rb + (1 - remain_x)*pixel_lb);
	        }
	        else value = 0.0;
        	return value;
        }

        public void Shifting(ezImg pImgSrc, ezImg pImgOut, int nPixel, string strDir)
        {
            unsafe
            {
                int x, y; byte* pSrc;
                pImgOut.ReAllocate(m_szImg, 1); Array.Clear(pImgOut.m_aBuf, 0, pImgOut.m_szImg.x * pImgOut.m_szImg.y);
                pSrc = (byte*)pImgSrc.GetIntPtr(0, 0);
                switch (strDir)
                {
                    case "ShiftUp":
                        for (y = nPixel; y < m_szImg.y; y++) for (x = 0; x < m_szImg.x; x++)
                                pImgOut.m_aBuf[y - nPixel, x] = pSrc[y * m_szImg.x + x];
                        break;
                    case "ShiftRight":
                        for (y = 0; y < m_szImg.y; y++) for (x = 0; x < m_szImg.x - nPixel; x++)
                                pImgOut.m_aBuf[y, x + nPixel] = pSrc[y * m_szImg.x + x];
                        break;
                    case "ShiftDown":
                        for (y = 0; y < m_szImg.y - nPixel; y++) for (x = 0; x < m_szImg.x; x++)
                                pImgOut.m_aBuf[y + nPixel, x] = pSrc[y * m_szImg.x + x];
                        break;                       
                    case "ShiftLeft":
                        for (y = 0; y < m_szImg.y; y++) for (x = nPixel; x < m_szImg.x; x++)
                                pImgOut.m_aBuf[y, x - nPixel] = pSrc[y * m_szImg.x + x];
                        break;
                }
            }
        }

        public void Shift(CPoint cpOffset)
        {
            int x = 0, y = 0;
            try
            {
                if (cpOffset.x < 0)
                {
                    for (y = 0; y < m_szImg.y; y++)
                    {
                        for (x = 0; x < m_szImg.x + cpOffset.x; x++)
                        {
                            m_aBuf[y, x] = m_aBuf[y, x - cpOffset.x];
                        }
                    }
                }
                else if (cpOffset.x > 0)
                {
                    for (y = 0; y < m_szImg.y; y++)
                    {
                        for (x = m_szImg.x - 1; x >= cpOffset.x; x--)
                        {
                            m_aBuf[y, x] = m_aBuf[y, x - cpOffset.x];
                        }
                    }
                }
                if (cpOffset.y < 0)
                {
                    for (y = 0; y < m_szImg.y + cpOffset.y; y++)
                    {
                        for (x = 0; x < m_szImg.x; x++)
                        {
                            m_aBuf[y, x] = m_aBuf[y - cpOffset.y, x];
                        }
                    }
                }
                else if (cpOffset.y > 0)
                {
                    for (y = m_szImg.y - 1; y >= cpOffset.y; y--)
                    {
                        for (x = 0; x < m_szImg.x; x++)
                        {
                            m_aBuf[y, x] = m_aBuf[y - cpOffset.y, x];
                        }
                    }
                }
            }
            catch
            {
                m_log.Add("Can not Access : X = " + x.ToString() + " Y = " + y.ToString());
            }
        }

        public void AddNoise(string sType, double dAmount)
        {
            unsafe
            {
                int x, y; byte *pData; double rnd;
                Random rand = new Random();
	            pData = (byte*)GetIntPtr(0, 0);
	            for (y = 0; y < m_szImg.y; y++) for (x = 0; x < m_szImg.x; x++, pData++)
	            {
		            if (sType == "Gaussian")
		            {
			            rnd = GaussianRand(0, dAmount);
                        if (*pData + rnd > 255) *pData = 255;
                        else if (*pData + rnd < 0) *pData = 0;
		            }
		            if (sType == "Salt&Pepper")
                    {
                        rnd = (int)(rand.NextDouble() * 100);
			            if (rnd < dAmount / 2)*pData = 0;
			            else if (rnd < dAmount)*pData = 255;
		            }
	            }
            }
        }

        double GaussianRand(double mean, double std)
        {
            double x1, x2, radius, factor, y1, y2;
            Random rand = new Random();
		    do 
            {
                x1 = rand.NextDouble() * 2.0 - 1.0;
                x2 = rand.NextDouble() * 2.0 - 1.0;
			    radius = x1 * x1 + x2 * x2;
            } while (radius < 0.00000001 || radius >= 1.0);
		    factor = Math.Sqrt((-2.0 * Math.Log(radius)) / radius);
		    y1 = x1 * factor; y2 = x2 * factor;
	        return (mean + y1*std);
        }

        public byte GetGV(CPoint cp)
        {
            if (cp.x < 0 || cp.y < 0 || cp.x >= m_szImg.x || cp.y >= m_szImg.y) return 0;
            return m_aBuf[cp.y, cp.x];
        }

        public byte GetGV(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_szImg.x || y >= m_szImg.y) return 0;
            return m_aBuf[y, x];
        }

        bool CheckPoint(CPoint cp, CPoint szSrc)
        {
            if (cp.x < 0 || cp.y < 0 || cp.x + szSrc.x > m_szImg.x || cp.y + szSrc.y > m_szImg.y) return false;
            return true;
        }

        public void RotateImage(ezImg pImgSrc, ezImg pImgDest, string sRotate)
        {
            unsafe
            {
                int x, y; byte* pSrc; byte* pDst; CPoint sz, sz1;
	            if (sRotate == "0")
	            {
		            sz = pImgSrc.m_szImg; pImgDest.ReAllocate(sz, 1);
		            pSrc = (byte*)pImgSrc.GetIntPtr(0, 0); pDst = (byte*)pImgDest.GetIntPtr(0, 0);
		            for (y = 0; y<sz.y; y++) for (x = 0; x<sz.x; x++) pDst[x + y*sz.x] = pSrc[x + y*sz.x];
	            }
	            if (sRotate == "90")
	            {
		            sz = pImgSrc.m_szImg; sz1.x = sz.y; sz1.y = sz.x; pImgDest.ReAllocate(sz1, 1);
		            pSrc = (byte*)pImgSrc.GetIntPtr(0, 0); pDst = (byte*)pImgDest.GetIntPtr(0, 0);
		            for (y = 0; y<sz.y; y++) for (x = 0; x<sz.x; x++) pDst[(sz.y - 1 - y) + x*sz.y] = pSrc[x + y*sz.x];
	            }
	            if (sRotate == "180")
	            {
		            sz = pImgSrc.m_szImg; pImgDest.ReAllocate(sz, 1);
		            pSrc = (byte*)pImgSrc.GetIntPtr(0, 0); pDst = (byte*)pImgDest.GetIntPtr(0, 0);
		            for (y = 0; y<sz.y; y++) for (x = 0; x<sz.x; x++)  pDst[x + y*sz.x] = pSrc[(sz.x - 1 - x) + (sz.y - 1 - y)*sz.x];
	            }

	            if (sRotate == "270")
	            {
		            sz = pImgSrc.m_szImg; sz1.x = sz.y; sz1.y = sz.x; pImgDest.ReAllocate(sz1, 1);
		            pSrc = (byte*)pImgSrc.GetIntPtr(0, 0); pDst = (byte*)pImgDest.GetIntPtr(0, 0);
		            for (y = 0; y<sz.y; y++) for (x = 0; x<sz.x; x++)   pDst[y + (sz.x - 1 - x)*sz.y] = pSrc[x + y*sz.x];
	            }
            }
        }

        public ezImg Rotate(string strAngle)
        {
            ezImg tempImg = new ezImg("Temp", m_log);
            RotateImage(this, tempImg, strAngle);
            return tempImg;
        }

        public bool RotateBilinear(double angle)
        {
            unsafe
            {
                byte *pSrcImage; byte[] pDstImage; byte nInt; CPoint sizeSrcImage;
	            double ang = -angle * Math.Acos(0.0f) / 90.0f;
	            float cos_angle = (float)Math.Cos(ang);
	            float sin_angle = (float)Math.Sin(ang);
	            pSrcImage = (byte*)GetIntPtr(0, 0);
	            sizeSrcImage = m_szImg;
	            pDstImage = new byte[sizeSrcImage.x*sizeSrcImage.y];
	            RPoint[] p = new RPoint[4];
	            p[0] = new RPoint(-0.5f, -0.5f);
	            p[1] = new RPoint(sizeSrcImage.x - 0.5f, -0.5f);
	            p[2] = new RPoint(-0.5f, sizeSrcImage.y - 0.5f);
	            p[3] = new RPoint(sizeSrcImage.x - 0.5f, sizeSrcImage.y - 0.5f);
	            RPoint[] newp = new RPoint[4];
	            for (int i = 0; i<4; i++)
	            {
		            newp[i].x = p[i].x;
		            newp[i].y = p[i].y;
	            }
	            float minx = (float)Math.Min(Math.Min(newp[0].x, newp[1].x), Math.Min(newp[2].x, newp[3].x));
                float miny = (float)Math.Min(Math.Min(newp[0].y, newp[1].y), Math.Min(newp[2].y, newp[3].y));
                float maxx = (float)Math.Max(Math.Max(newp[0].x, newp[1].x), Math.Max(newp[2].x, newp[3].x));
                float maxy = (float)Math.Max(Math.Max(newp[0].y, newp[1].y), Math.Max(newp[2].y, newp[3].y));
	            int newWidth = (int)Math.Floor(maxx - minx + 0.5f);
	            int newHeight = (int)Math.Floor(maxy - miny + 0.5f);
	            float ssx = ((maxx + minx) - ((float)newWidth - 1)) / 2.0f;
	            float ssy = ((maxy + miny) - ((float)newHeight - 1)) / 2.0f;
	            float newxcenteroffset = 0.5f * newWidth;
	            float newycenteroffset = 0.5f * newHeight;
	            ssx -= 0.5f * sizeSrcImage.x; ssy -= 0.5f * sizeSrcImage.y;
	            float x, y;
	            float origx, origy;
	            int destx, desty;
	            y = ssy;
	            for (desty = 0; desty<newHeight; desty++)
	            {
		            x = ssx;
		            for (destx = 0; destx<newWidth; destx++)
		            {
			            origx = cos_angle*x + sin_angle*y + newxcenteroffset;
			            origy = cos_angle*y - sin_angle*x + newycenteroffset;
			            nInt = GetPixelInterpolated(origx, origy, pSrcImage, sizeSrcImage);
			            if (nInt < 0) nInt = 0; if (nInt>255) nInt = 255;
			            pDstImage[destx + desty*sizeSrcImage.x] = nInt;
			            x++;
		            }
		            y++;
	            }
	            pSrcImage = (byte*)GetIntPtr(0, 0);
                fixed(byte* pDst = &pDstImage[0])
                {
                    cpp_memcpy(pSrcImage, pDst, sizeSrcImage.x * sizeSrcImage.y);
                }
	            return true;
            }
        }

        unsafe byte GetPixelInterpolated(float x, float y, byte* pSrcImage, CPoint sizeSrcImage)
        {
            int xi = (int)(x); if (x < 0) xi--;
            int yi = (int)(y); if (y < 0) yi--;

            if (xi < -1 || xi >= sizeSrcImage.x || yi < -1 || yi >= sizeSrcImage.y) return 0;
            if ((xi + 1) < sizeSrcImage.x && xi >= 0 && (yi + 1) < sizeSrcImage.y && yi >= 0)
            {
                ushort wt1 = (ushort)((x - xi) * 256.0f), wt2 = (ushort)((y - yi) * 256.0f);
                ushort wd = (ushort)(wt1 * wt2 >> 8);
                ushort wb = (ushort)(wt1 - wd);
                ushort wc = (ushort)(wt2 - wd);
                ushort wa = (ushort)(256 - wt1 - wc);
                ushort wData;
                wData = (ushort)(wa * pSrcImage[xi + yi * sizeSrcImage.x]);
                wData += (ushort)(wb * pSrcImage[(xi + 1) + yi * sizeSrcImage.x]);
                wData += (ushort)(wc * pSrcImage[xi + (yi + 1) * sizeSrcImage.x]);
                wData += (ushort)(wd * pSrcImage[(xi + 1) + (yi + 1) * sizeSrcImage.x]);
                return (byte)(wData >> 8);
            }
            else
            {
                return 0;
            }
        }

        public unsafe void SetROI(ezImg pImgData, CPoint cp)
        {
            int y; byte* pSrc; byte *pData;
	        if (pImgData.m_szImg.x + cp.x < m_szImg.x || pImgData.m_szImg.y + cp.y < m_szImg.y) return;
	        for (y = 0; y < m_szImg.y; y++)
	        {
                pData = (byte*)GetIntPtr(0, 0); pSrc = (byte*)pImgData.GetIntPtr(cp.y, cp.x);
                cpp_memcpy(pData, pSrc, pImgData.m_szImg.x);
	        }
        }

        public unsafe double GetBlackPercent(CPoint cp, CPoint szSrc, Byte nThres)
        {
            int y, x, count = 0; byte *pSrc; double dRatio;
	        if (!CheckPoint(cp, szSrc)) return 0.0;
	        for (y = cp.y; y < cp.y + szSrc.y; y++)
	        {
                pSrc = (byte*)GetIntPtr(y - cp.y, cp.x);
		        for (x = cp.x; x < cp.x + szSrc.x; x++, pSrc++) { if (*pSrc <= nThres) count++; }
	        }
	        dRatio = (double)count / (double)m_szImg.x*m_szImg.y;
	        return dRatio;
        }

        unsafe int ScanX(int nID, ezImg pImg, CPoint cp, CPoint sz, int y, byte nGV)
        {
            int x, nL = 0; byte* pSrc;
            if (cp.y + y >= pImg.m_szImg.y) return 0;
            pSrc = (byte*)GetIntPtr(cp.y + y, cp.x);
            for (x = cp.x; x < cp.x + sz.x; x++, pSrc++)
            {
                if (nID == 0 && (*pSrc <= nGV)) nL++;
                if (nID == 1 && (*pSrc >= nGV)) nL++;
            }
            return nL;
        }

        unsafe int ScanY(int nID, ezImg pImg, CPoint cp, CPoint sz, int x, byte nGV) //0=Black; 1=White)
        {
            int y, nL = 0; byte* pSrc;
            if (x + cp.x >= pImg.m_szImg.x) return 0;
            pSrc = (byte*)GetIntPtr(cp.y, cp.x + x);
            for (y = cp.y; y < cp.y + sz.y; y++, pSrc += pImg.m_szImg.x) //Check here
            {
                if (nID == 0 && (*pSrc <= nGV)) nL++;
                if (nID == 1 && (*pSrc >= nGV)) nL++;
            }
            return nL;
        }

        public unsafe void DrawRect(Rectangle rect, byte bValue)
        {
            int x, y; byte* pSrc; CPoint cp0, cp1;
            cp0.x = rect.Left; cp0.y = rect.Top; cp1.x = rect.Right; cp1.y = rect.Bottom;
            pSrc = (byte*)GetIntPtr(cp0.y, cp0.x); for (x = cp0.x; x < cp1.x; x++, pSrc++) *pSrc = bValue;
            pSrc = (byte*)GetIntPtr(cp1.y, cp0.x); for (x = cp0.x; x < cp1.x; x++, pSrc++) *pSrc = bValue;
            pSrc = (byte*)GetIntPtr(cp0.y, cp0.x); for (y = cp0.y; y < cp1.y; y++) { *pSrc = bValue; pSrc += m_szImg.x; }
            pSrc = (byte*)GetIntPtr(cp0.y, cp1.x); for (y = cp0.y; y < cp1.y; y++) { *pSrc = bValue; pSrc += m_szImg.x; }
        }

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);

        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memset(byte* pDst, byte val, int nLength);
    }
}
