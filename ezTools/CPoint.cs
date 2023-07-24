using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ezTools
{
    public class CPoint_Ref
    {
        public CPoint m_cp;

        public CPoint_Ref()
        {
            m_cp = new CPoint(0, 0);
        }

        public CPoint_Ref(CPoint cp)
        {
            m_cp = cp;
        }
    }

    public struct CPoint
    {
        public int x, y;

        public CPoint(int xp, int yp)
        {
            x = xp; y = yp;
        }

        public CPoint(Point cp)
        {
            x = cp.X; y = cp.Y;
        }

        public CPoint(CPoint cp)
        {
            x = cp.x; y = cp.y;
        }

        public void Set(int xp, int yp)
        {
            x = xp;
            y = yp; 
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public double GetL(CPoint cp)
        {
            int dx, dy;
            dx = x - cp.x; dy = y - cp.y;
            return Math.Sqrt(dx*dx+dy*dy); 
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (x != ((CPoint)obj).x) return false;
            if (y != ((CPoint)obj).y) return false;
            return true;
        }

        public static bool operator ==(CPoint cp0, CPoint cp1)
        {
            if (cp0.x != cp1.x) return false;
            if (cp0.y != cp1.y) return false;
            return true;
        }

        public static bool operator !=(CPoint cp0, CPoint cp1)
        {
            if (cp0.x != cp1.x) return true;
            if (cp0.y != cp1.y) return true;
            return false;
        }

        public static CPoint operator +(CPoint cp0, CPoint cp1)
        {
            return new CPoint(cp0.x + cp1.x, cp0.y + cp1.y);
        }

        public static CPoint operator -(CPoint cp0, CPoint cp1)
        {
            return new CPoint(cp0.x - cp1.x, cp0.y - cp1.y);
        }

        public static CPoint operator *(CPoint cp0, double f)
        {
            return new CPoint((int)Math.Round(cp0.x * f), (int)Math.Round(cp0.y * f)); 
        }

        public static CPoint operator /(CPoint cp0, double f)
        {
            return new CPoint((int)Math.Round(cp0.x / f), (int)Math.Round(cp0.y / f));
        }

        public bool IsInside(CPoint cp0, CPoint cp1)
        {
            if ((x < cp0.x) || (y < cp0.y)) return false;
            if ((x > cp1.x) || (y > cp1.y)) return false;
            return true; 
        }

        public void FixSize(ref CPoint sz)
        {
            if (sz.x < 0) { x += sz.x; sz.x = -sz.x; }
            if (sz.y < 0) { y += sz.y; sz.y = -sz.y; }
        }

        public void Square(CPoint cp)
        {
            x = cp.x * cp.x; y = cp.y * cp.y; 
        }

        public Point ToPoint()
        {
            Point cp = new Point((int)x, (int)y);
            return cp; 
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(0, 0, x, y);
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")"; 
        }
    }
}
