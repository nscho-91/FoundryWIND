using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ezTools
{
    public class azMatch
    {
        const int c_nTemplate = 200;
       	int[,] m_nISum = new int[c_nTemplate, c_nTemplate];
        int[,] m_nISumSqr = new int[c_nTemplate, c_nTemplate];
        CPoint m_ptOri;
        
        public azMatch()
        {
            for (int i = 0; i < c_nTemplate; i++) 
                for (int j = 0; j < c_nTemplate; j++) m_nISum[i,j] = m_nISumSqr[i,j] = 0;
        }

        double Match(ezImg rImgSrc, ezImg rImgDst, CPoint cpDst, int nAveSrc)
        {
            int x, y, dSrc, dDst, nAveDst; double[] fSum = new double[3]; double fMatch = 0; 
            CPoint[] cp = new CPoint[2]; CPoint[] sz = new CPoint[2];
        	sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg;
        	cp[0] = cpDst; cp[1] = cp[0] + rImgSrc.m_szImg; fSum[0] = 0;
	        unsafe
            {
                byte *pDst; byte *pSrc;
                for (y = cp[0].y; y < cp[1].y; y++)
                {
                    pDst = (byte*)rImgDst.GetIntPtr(y, cp[0].x);
                    for (x = cp[0].x; x < cp[1].x; x++, pDst++) fSum[0] += *pDst;  		    
        	    }
        	    nAveDst = (int)Math.Round(fSum[0] / sz[0].x / sz[0].y);
        	    fSum[0] = fSum[1] = fSum[2] = 0;
            	for (y = cp[0].y; y < cp[1].y; y++)
        	    {
                    pSrc = (byte*)rImgSrc.GetIntPtr(y - cp[0].y, 0); pDst = (byte*)rImgDst.GetIntPtr(y, cp[0].x);
	            	for (x = cp[0].x; x < cp[1].x; x++, pSrc++, pDst++)
	            	{
        	    		dSrc = *pSrc - nAveSrc; dDst = *pDst - nAveDst;
	            		fSum[0] += dSrc * dSrc; fSum[1] += dDst * dDst; fSum[2] += dSrc * dDst;
	            	}
	            	if (fSum[0] * fSum[1] == 0) return 0;
                    fMatch = 100 * Math.Abs(fSum[2]) / Math.Sqrt(fSum[0]) / Math.Sqrt(fSum[1]);
	            }
            }
	        return fMatch;
        }

	    double Match2(ezImg rImgSrc, ezImg rImgDst, CPoint cpMove, double nAveSrc, int nVariance)
        {
            int x, y; double[] fSum = new double[3]; double fMatch = 0, sumSqr, sum; 
            CPoint[] cpM = new CPoint[2]; CPoint[] sz = new CPoint[2];
	        sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg;
	        cpM[0] = cpMove + m_ptOri; cpM[1] = cpM[0] + rImgSrc.m_szImg; fSum[0] = 0;

	        if (cpM[1].x > sz[1].x) cpM[1].x = sz[1].x - 1;
	        if (cpM[1].y > sz[1].y) cpM[1].y = sz[1].y - 1;

	        fSum[0] = nVariance;

	        if (cpMove.y == 0 && cpMove.x == 0)
        	{
	        	sum = m_nISum[sz[0].x - 1, sz[0].y - 1];
	        	sumSqr = m_nISumSqr[sz[0].x - 1, sz[0].y - 1];
        	}
        	else if (cpMove.y == 0)
        	{
        		sum = m_nISum[cpMove.x + sz[0].x - 1, sz[0].y - 1] - m_nISum[cpMove.x - 1, sz[0].y - 1];
        		sumSqr = m_nISumSqr[cpMove.x + sz[0].x - 1, sz[0].y - 1] - m_nISumSqr[cpMove.x - 1, sz[0].y - 1];
	        }
	         if (cpMove.x == 0)
	        {
	        	sum = m_nISum[sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISum[sz[0].x - 1, cpMove.y - 1];
	        	sumSqr = m_nISumSqr[sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[sz[0].x - 1, cpMove.y - 1];
	        }
	        else
	        {
	        	sumSqr = (m_nISumSqr[cpMove.x + sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[cpMove.x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[cpMove.x + sz[0].x - 1, cpMove.y - 1] + m_nISumSqr[cpMove.x - 1, cpMove.y - 1]);
	        	sum = (m_nISum[cpMove.x + sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISum[cpMove.x - 1, cpMove.y + sz[0].y - 1] - m_nISum[cpMove.x + sz[0].x - 1, cpMove.y - 1] + m_nISum[cpMove.x - 1, cpMove.y - 1]);
	        }
	        fSum[1] = sumSqr - sum*sum / (sz[0].x * sz[0].y);


	        fSum[2] = 0;
            unsafe
            {
                byte* pSrc; byte* pDst;
	            for (y = cpM[0].y; y < cpM[1].y; y++)
	            {
                    pSrc = (byte*)rImgSrc.GetIntPtr(y - cpM[0].y, 0); pDst = (byte*)rImgDst.GetIntPtr(y, cpM[0].x);
                    for (x = cpM[0].x; x < cpM[1].x; x++, pSrc++, pDst++)
                    {
                        fSum[2] += (*pSrc - nAveSrc) * (*pDst);
                    }
                }
	        }

	        if (fSum[0] * fSum[1] == 0) return 0;
	        fMatch = 100 * fSum[2] / Math.Sqrt(Math.Abs(fSum[0] * fSum[1]));   //fMatch can be less than 0
	        return fMatch;
        }

        double Match3(ezImg rImgSrc, ezImg rImgDst, CPoint cpMove, double score, double nAveSrc, int nVariance)
        {
            int x, y; double[] fSum = new double[3]; double fMatch, sumSqr, sum, thres;
            CPoint[] cpM = new CPoint[2]; CPoint[] sz = new CPoint[2]; bool bFlag = false;
	        sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg;
	        cpM[0] = cpMove + m_ptOri; cpM[1] = cpM[0] + rImgSrc.m_szImg;

        	if (cpM[1].x > sz[1].x) cpM[1].x = sz[1].x - 1;
        	if (cpM[1].y > sz[1].y) cpM[1].y = sz[1].y - 1;

        	fSum[0] = nVariance;

        	//Sum table caculation

        	if (cpMove.y == 0 && cpMove.x == 0)
        	{
	        	sum = m_nISum[sz[0].x - 1, sz[0].y - 1];
	        	sumSqr = m_nISumSqr[sz[0].x - 1, sz[0].y - 1];
        	}
        	else if (cpMove.y == 0)
        	{
        		sum = m_nISum[cpMove.x + sz[0].x - 1, sz[0].y - 1] - m_nISum[cpMove.x - 1, sz[0].y - 1];
	        	sumSqr = m_nISumSqr[cpMove.x + sz[0].x - 1, sz[0].y - 1] - m_nISumSqr[cpMove.x - 1, sz[0].y - 1];
	        }
	        else if (cpMove.x == 0)
	        {
	        	sum = m_nISum[sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISum[sz[0].x - 1, cpMove.y - 1];
	        	sumSqr = m_nISumSqr[sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[sz[0].x - 1, cpMove.y - 1];
	        }
	        else
	        {
	        	sumSqr = (m_nISumSqr[cpMove.x + sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[cpMove.x - 1, cpMove.y + sz[0].y - 1] - m_nISumSqr[cpMove.x + sz[0].x - 1, cpMove.y - 1] + m_nISumSqr[cpMove.x - 1, cpMove.y - 1]);
	        	sum = (m_nISum[cpMove.x + sz[0].x - 1, cpMove.y + sz[0].y - 1] - m_nISum[cpMove.x - 1, cpMove.y + sz[0].y - 1] - m_nISum[cpMove.x + sz[0].x - 1, cpMove.y - 1] + m_nISum[cpMove.x - 1, cpMove.y - 1]);
	        }
	        fSum[1] = sumSqr - sum*sum / (sz[0].x * sz[0].y);
	        fSum[2] = -sum*nAveSrc;

        	thres = score * Math.Sqrt(Math.Abs(fSum[0] * fSum[1])) / 100;   //modify 1412
        
        	//nominator of NCC calculation, the loop will break if nominator higher than thres-->better speed
            unsafe
            {
                byte* pSrc; byte* pDst; 
	            for (y = cpM[0].y; y < cpM[1].y; y++)
	            {
                    pSrc = (byte*)rImgSrc.GetIntPtr(y - cpM[0].y, 0); pDst = (byte*)rImgDst.GetIntPtr(y, cpM[0].x);
	        	    for (x = cpM[0].x; x < cpM[1].x; x++, pSrc++, pDst++)
	        	    {
	        		    fSum[2] += (*pSrc) * (*pDst);
		        	    if (fSum[2] > thres)
		        	    {
		        		    bFlag = true;
		        		    break;
		        	    }
		            }
		            if (bFlag) break;
	            }
            }
	        if (thres == 0) return 0;
	        fMatch = score * fSum[2] / thres;
	        return fMatch;
        }

        public double DoMatch(ezImg rImgSrc, ezImg rImgDst, CPoint cpDst, int nArea)
        {
            int x, y, nAve; CPoint[]cp = new CPoint[2];  CPoint[] sz = new CPoint[2]; double fSum = 0, fMatch, fMax;
        	sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg; fMax = 0;
	        unsafe
            {
                byte* pSrc = (byte*)rImgSrc.GetIntPtr(0,0);
                for (y = 0; y < sz[0].y; y++) for (x = 0; x < sz[0].x; x++, pSrc++) fSum += *pSrc;
            }

	        nAve = (int)Math.Round(fSum / sz[0].x / sz[0].y);
	        cp[0] = cpDst - new CPoint(nArea, nArea); if (cp[0].x < 0) cp[0].x = 0; if (cp[0].y < 0) cp[0].y = 0;
        	cp[1] = cpDst + new CPoint(nArea, nArea) + sz[0];
        	if (cp[1].x >= sz[1].x) cp[1].x = sz[1].x - 1; if (cp[1].y >= sz[1].y) cp[1].y = sz[1].y - 1;
        	cp[1] -= sz[0];
        	for (y = cp[0].y; y<cp[1].y; y++) for (x = cp[0].x; x<cp[1].x; x++)
        	{
		        fMatch = Match(rImgSrc, rImgDst, new CPoint(x, y), nAve);
		        if (fMax<fMatch) fMax = fMatch;
	        }
	        return fMax;
        }

	    double DoMatchBWTM(ezImg rImgSrc, Rectangle rect, ezImg rImgDst, int nThreshold, int offset)
        {
            int x, y, i, j, curX; int[] count = new int[3]; CPoint cp0, cp1; double ratio, dmax = 0;
	        cp0.x = rect.Left; cp0.y = rect.Top; cp1.x = rect.Right; cp1.y = rect.Bottom;

        	y = cp0.y - offset; x = x = cp0.x - offset;
            unsafe
            {
                byte* pSrc; byte* pText;
                while ((y <= cp0.y + offset))
        	    {
	        	    while ((x <= cp0.x + offset))
	        	    {
	        		    count[0] = 0; count[1] = 0; count[2] = 0;
	        		    for (i = 0; i<rImgDst.m_szImg.y; i++)
	        		    {
	        			    pText = (byte*)rImgDst.GetIntPtr(i, 0);
	        			    pSrc = (byte*)rImgSrc.GetIntPtr(y + i, x);
	        			    curX = 0;
	        			    for (j = 0; j<rImgDst.m_szImg.x; j++)
	        			    {
	        				    if ((*pText >= nThreshold) && (*pSrc >= nThreshold) && curX<rect.Width) count[0]++;
	        				    if (*pSrc >= nThreshold) count[1]++;
	        				    if (*pText >= nThreshold) count[2]++;
		        			    pText++; pSrc++; curX++;
			        	    }
	        		    }
	        		    if (count[0]>0)
	        		    {
		        		    ratio = (double)count[0] / (double)count[1] + 1;
		        		    if (ratio>(double)count[0] / (double)count[2] + 1)
		        			    ratio = (double)count[0] / (double)count[2] + 1;
		        		    if (dmax<ratio) dmax = ratio;
		        	    }
		        	    x++;
	        	    }
	        	    y++;
	            }
            }
        	
	        return (dmax * 100);
        }
	
	    double DoMatch2(ezImg rImgSrc, ezImg rImgDst, CPoint cpDst, int nArea)
        {
           	int x, y, nVariance, nTemp; double fSum = 0, fSumSqr = 0, fMatch, fMax = 0, nAve;
	        CPoint[] sz = new CPoint[2];

        	sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg;

	        if (sz[0].x > c_nTemplate || sz[0].y > c_nTemplate)  return 0.0;
        	if (sz[1].x < sz[0].x || sz[1].y < sz[0].y)  return 0.0;
        	if (sz[0].x <= 0 || sz[0].y <= 0) return 0.0;
        	
            unsafe
            {
                byte* pSrc;
                pSrc = (byte*)rImgSrc.GetIntPtr(0, 0);
        	    for (y = 0; y<sz[0].y; y++) for (x = 0; x<sz[0].x; x++, pSrc++)
        	    {
        		    fSum += *pSrc;
        		    fSumSqr += (*pSrc)*(*pSrc);
        	    }
        	    nAve = fSum / sz[0].x / sz[0].y;
        	    nVariance = (int)(fSumSqr - fSum*nAve);
	            m_ptOri = cpDst - new CPoint(nArea, nArea); if (m_ptOri.x < 0) m_ptOri.x = 0; if (m_ptOri.y < 0) m_ptOri.y = 0;
	            //sum table initialization
                m_nISum[0, 0] = (int)rImgDst.m_aBuf[m_ptOri.y, m_ptOri.x]; m_nISumSqr[0, 0] = m_nISum[0, 0] * m_nISum[0, 0];
	            for (y = 1; y < sz[0].y + 2 * nArea; y++)
        	    {
        		    nTemp = rImgDst.m_aBuf[ m_ptOri.y + y, m_ptOri.x];
	        	    if (y >= c_nTemplate) y = c_nTemplate - 1;
	        	    m_nISum[0, y] = m_nISum[0, y - 1] + nTemp;
	        	    m_nISumSqr[0, y] = m_nISumSqr[0, y - 1] + nTemp*nTemp;
	            }
	            for (x = 1; x < sz[0].x + 2 * nArea; x++)
	            {
	        	    nTemp = rImgDst.m_aBuf[m_ptOri.y, m_ptOri.x + x];
	        	    if (x >= c_nTemplate) x = c_nTemplate - 1;
	        	    m_nISum[x, 0] = m_nISum[x - 1, 0] + nTemp;
	        	    m_nISumSqr[x, 0] = m_nISumSqr[x - 1, 0] + nTemp*nTemp;
	            }

	            for (x = 1; x < sz[0].x + 2 * nArea; x++) for (y = 1; y < sz[0].y + 2 * nArea; y++)
	            {
	        	    nTemp = rImgDst.m_aBuf[m_ptOri.y + y, m_ptOri.x + x];
	        	    if (x >= c_nTemplate) x = c_nTemplate - 1;
	        	    if (y >= c_nTemplate) y = c_nTemplate - 1;
	        	    m_nISum[x, y] = nTemp + m_nISum[x - 1, y] + m_nISum[x, y - 1] - m_nISum[x - 1, y - 1];
	        	    m_nISumSqr[x, y] = nTemp*nTemp + m_nISumSqr[x - 1, y] + m_nISumSqr[x, y - 1] - m_nISumSqr[x - 1, y - 1];
	            }

            }
	        //calculate NCC depend on moving position
	        for (y = 0; y < nArea; y++) for (x = 0; x < nArea; x++)
	        {
	        	fMatch = Match2(rImgSrc, rImgDst, new CPoint(2 * x, 2 * y), nAve, nVariance);
	        	if (fMax < fMatch) fMax = fMatch;
	        }
	        return fMax;
        }
	    
        double DoMatch3(ezImg rImgSrc, ezImg rImgDst, CPoint cpDst, int nArea, double score, double nAveSrc, int nVariance)
        {
            //this function will stop when NCC reach desired score, so when NCC is lower than score it's return value is NCC and vice versa
	        // use this function when defect's inspection is more important than score for each character to get better speed
	        int x, y, nTemp; double fMatch, fMax = 0;
	        CPoint[] sz = new CPoint[2];

	        sz[0] = rImgSrc.m_szImg; sz[1] = rImgDst.m_szImg;

	        if (sz[0].x > c_nTemplate || sz[0].y > c_nTemplate)  return 0.0;
	        if (sz[1].x < sz[0].x || sz[1].y < sz[0].y)  return 0.0;
	        if (sz[0].x <= 0 || sz[0].y <= 0) return 0.0;
            unsafe
            {
                byte *pSrc;  
	            pSrc = (byte*)rImgSrc.GetIntPtr(0, 0);
	            m_ptOri = cpDst - new CPoint(nArea, nArea); if (m_ptOri.x < 0) m_ptOri.x = 0; if (m_ptOri.y < 0) m_ptOri.y = 0;


        	    //sum table initialization

        	    m_nISum[0, 0] = rImgDst.m_aBuf[m_ptOri.y, m_ptOri.x]; m_nISumSqr[0, 0] = m_nISum[0, 0] * m_nISum[0, 0];
	            for (y = 1; y < sz[0].y + 2 * nArea; y++)
	            {
	        	    nTemp = rImgDst.m_aBuf[m_ptOri.y + y, m_ptOri.x];
	        	    if (y >= c_nTemplate) y = c_nTemplate - 1;
	        	    m_nISum[0, y] = m_nISum[0, y - 1] + nTemp;
	        	    m_nISumSqr[0, y] = m_nISumSqr[0, y - 1] + nTemp*nTemp;
	            }
        	    for (x = 1; x < sz[0].x + 2 * nArea; x++)
	            {
	        	    nTemp = rImgDst.m_aBuf[m_ptOri.y, m_ptOri.x + x];
	        	    if (x >= c_nTemplate) x = c_nTemplate - 1;
	        	    m_nISum[x, 0] = m_nISum[x - 1, 0] + nTemp;
                m_nISumSqr[x, 0] = m_nISumSqr[x - 1, 0] + nTemp*nTemp;
        	    }

        	    for (x = 1; x < sz[0].x + 2 * nArea; x++) for (y = 1; y < sz[0].y + 2 * nArea; y++)
        	    {
	        	    nTemp = rImgDst.m_aBuf[m_ptOri.y + y, m_ptOri.x + x];
	        	    if (y >= c_nTemplate) y = c_nTemplate - 1;
	        	    if (x >= c_nTemplate) x = c_nTemplate - 1;
	        	    m_nISum[x, y] = nTemp + m_nISum[x - 1, y] + m_nISum[x, y - 1] - m_nISum[x - 1, y - 1];
	        	    m_nISumSqr[x, y] = nTemp*nTemp + m_nISumSqr[x - 1, y] + m_nISumSqr[x, y - 1] - m_nISumSqr[x - 1, y - 1];
	            }
            }
            

	        //calculate NCC depend on moving position, when NCC is high enough, the loop will break to decrease calculation's time

	        bool bFlag1 = false, bFlag2 = false;
	        int nHalf = (nArea + 1) / 2;              //modify 1412
	        for (y = nHalf; y < nHalf + 2; y++)
	        {
	        	for (x = nHalf + 1; x < nHalf + 3; x++)
	        	{
	        		fMatch = Match3(rImgSrc, rImgDst, new CPoint(2 * x, 2 * y), score, nAveSrc, nVariance);
	        		if (fMax < fMatch) fMax = fMatch;
	        		if (fMatch > score) { bFlag1 = true; break; }
	        	}
	        	if (bFlag1) break;
	        }


	        if (!bFlag1)
	        {
	        	for (y = 0; y < nArea; y++)
	        	{
	        		for (x = 0; x < nArea; x++)
		        	{
		        		fMatch = Match3(rImgSrc, rImgDst, new CPoint(2 * (nArea - x) - 1, 2 * (nArea - y) - 1), score, nAveSrc, nVariance);
		        		if (fMax < fMatch) fMax = fMatch;
		        		if (fMatch > score) { bFlag2 = true; break; }
		        	}
		        	if (bFlag2) break;
		        }
	        }
	        return fMax;
        }

    }
}
