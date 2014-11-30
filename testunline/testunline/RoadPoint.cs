using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLog4net
{
    [Serializable]
    public class RoadPoint
    {
        public double Lat;// 纬度
        public double Lon;// 经度
        public byte Mode;//模式
        public double Azimuth; //航向角
        public byte Reserved1;//
        public byte Reserved2;//保留字节
        //构造函数
        public RoadPoint()
        {
            Lat = 0;
            Lon=0;
            Mode = 0;
            Azimuth = 0;
            Reserved1 = 0;
            Reserved2 = 0;
        }
        public RoadPoint(double Lon_Value, double Lat_Value, double Azimuth_Value)
        {
            Lat = Lat_Value;
            Lon = Lon_Value;
            Azimuth = Azimuth_Value;
            Mode = 0;
            Reserved1 = 0;
            Reserved2 = 0;
        }        
    }
}
