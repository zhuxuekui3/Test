using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLog4net
{
    static class CoordinateConverter
    {

        public static double angToRad(double angle_d)  ///度数转弧度
        {
            double Pi = 3.1415926535898;
            double rad1;
            rad1 = angle_d * Pi / 180;
            return rad1;
        }

        //根据平面坐标计算距离
        public static double getDist2(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static int nFlag = 2;



        public static double GetL0InDegree(double dLIn)
        {
            //3°带求带号及中央子午线经度(d的形式)
            //具体公式请参阅《常用大地坐标系及其变换》朱华统，解放军出版社138页
            double L = dLIn;//d.d
            double L_ddd_Style = L;
            double ZoneNumber = (int)((L_ddd_Style - 1.5) / 3.0) + 1;
            double L0 = ZoneNumber * 3.0;//degree
            return L0;
        }
        /************************************************************************/
        /* B: lat纬度
         * L: lon经度
         * H: height
        /************************************************************************/
        public static Point2D BLH2XYZ(double B, double L, double H)
        {
            double N, E, h;
            double L0 = GetL0InDegree(L);//根据经度求中央子午线经度
            Point2D pt2d = new Point2D(0, 0);
            double a = 6378245.0;            //地球半径  北京6378245
            double F = 298.257223563;        //地球扁率
            double iPI = 0.0174532925199433; //2pi除以360，用于角度转换

            double f = 1 / F;
            double b = a * (1 - f);
            double ee = (a * a - b * b) / (a * a);
            double e2 = (a * a - b * b) / (b * b);
            double n = (a - b) / (a + b), n2 = (n * n), n3 = (n2 * n), n4 = (n2 * n2), n5 = (n4 * n);
            double al = (a + b) * (1 + n2 / 4 + n4 / 64) / 2;
            double bt = -3 * n / 2 + 9 * n3 / 16 - 3 * n5 / 32;
            double gm = 15 * n2 / 16 - 15 * n4 / 32;
            double dt = -35 * n3 / 48 + 105 * n5 / 256;
            double ep = 315 * n4 / 512;

            B = B * iPI;
            L = L * iPI;
            L0 = L0 * iPI;

            double l = L - L0, cl = (Math.Cos(B) * l), cl2 = (cl * cl), cl3 = (cl2 * cl), cl4 = (cl2 * cl2), cl5 = (cl4 * cl), cl6 = (cl5 * cl), cl7 = (cl6 * cl), cl8 = (cl4 * cl4);
            double lB = al * (B + bt * Math.Sin(2 * B) + gm * Math.Sin(4 * B) + dt * Math.Sin(6 * B) + ep * Math.Sin(8 * B));
            double t = Math.Tan(B), t2 = (t * t), t4 = (t2 * t2), t6 = (t4 * t2);
            double Nn = a / Math.Sqrt(1 - ee * Math.Sin(B) * Math.Sin(B));
            double yt = e2 * Math.Cos(B) * Math.Cos(B);
            N = lB;
            N += t * Nn * cl2 / 2;
            N += t * Nn * cl4 * (5 - t2 + 9 * yt + 4 * yt * yt) / 24;
            N += t * Nn * cl6 * (61 - 58 * t2 + t4 + 270 * yt - 330 * t2 * yt) / 720;
            N += t * Nn * cl8 * (1385 - 3111 * t2 + 543 * t4 - t6) / 40320;

            E = Nn * cl;
            E += Nn * cl3 * (1 - t2 + yt) / 6;
            E += Nn * cl5 * (5 - 18 * t2 + t4 + 14 * yt - 58 * t2 * yt) / 120;
            E += Nn * cl7 * (61 - 479 * t2 + 179 * t4 - t6) / 5040;

            E += 500000;
            if (nFlag == 1)
            {
                //UTM投影
                N = 0.9996 * N;
                E = 0.9996 * (E - 500000.0) + 500000.0;//Get y
            }
            if (nFlag == 2)
            {
                //UTM投影
                N = 0.9999 * N;
                E = 0.9999 * (E - 500000.0) + 250000.0;//Get y
            }

            //原
            //pt2d.x = N;
            //pt2d.y = E;
            //
            pt2d.x = E;
            pt2d.y = N;
            h = H;

            return pt2d;
        }


        //返回向量12和真北的夹角
        public static double getAngle(double lon1, double lat1, double lon2, double lat2)
        {
            double angle;
            double averageLat = (lat1 + lat2) / 2;
            if (Math.Abs(lat1 - lat2) <= 1e-6)
            {
                angle = 90;
            }
            else
            {
                angle = Math.Atan((lon1 - lon2) * Math.Cos(angToRad(averageLat)) / (lat1 - lat2)) * 180 / Math.PI;
            }
            if (lat1 > lat2)
            {
                angle = angle + 180;
            }
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            return angle;
        }

    }

    

    public class Point2D
    {
        public double x;
        public double y;
        public double lat;
        public double lon;
        public Point2D()
        {
            x = 0;
            y = 0;
            lat = 0;
            lon = 0;
        }
        public Point2D(double X, double Y)
        {
            x = X;
            y = Y;
        }
    }
}
