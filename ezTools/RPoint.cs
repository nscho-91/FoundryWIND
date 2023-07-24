using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ezTools
{
    public struct RPoint
    {
        public double x, y;

        public RPoint(double xp, double yp)
        {
            x = xp; y = yp;
        }

        public RPoint(Point cp)
        {
            x = cp.X; y = cp.Y;
        }

        public RPoint(CPoint cp)
        {
            x = cp.x; y = cp.y;
        }

        public RPoint(RPoint rp)
        {
            x = rp.x; y = rp.y;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double GetL(RPoint rp)
        {
            double dx, dy;
            dx = x - rp.x; dy = y - rp.y;
            return Math.Sqrt(dx * dx + dy * dy); 
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (x != ((RPoint)obj).x) return false;
            if (y != ((RPoint)obj).y) return false;
            return true;
        }

        public static bool operator ==(RPoint rp0, RPoint rp1)
        {
            if (rp0.x != rp1.x) return false;
            if (rp0.y != rp1.y) return false;
            return true;
        }

        public static bool operator !=(RPoint rp0, RPoint rp1)
        {
            if (rp0.x == rp1.x) return false;
            if (rp0.y == rp1.y) return false;
            return true;
        }

        public static RPoint operator +(RPoint rp0, RPoint rp1)
        {
            return new RPoint(rp0.x + rp1.x, rp0.y + rp1.y);
        }

        public static RPoint operator -(RPoint rp0, RPoint rp1)
        {
            return new RPoint(rp0.x - rp1.x, rp0.y - rp1.y);
        }

        public static RPoint operator *(RPoint rp0, double f)
        {
            return new RPoint(rp0.x * f, rp0.y * f); 
        }

        public static RPoint operator /(RPoint rp0, double f)
        {
            return new RPoint(rp0.x / f, rp0.y / f);
        }

        public CPoint ToCPoint()
        {
            return new CPoint((int)Math.Round(x), (int)Math.Round(y));
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }
}
