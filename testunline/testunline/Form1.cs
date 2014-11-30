using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using TestLog4Net;
using HappyCodingStudio;


namespace TestLog4net
{
    public partial class Form1 : Form
    {
        //UDP发送决策
        UdpClient SendClient;  //= new UdpClient(8000);
        IPAddress remoteIP = IPAddress.Parse("127.0.0.1"); //远程主机IP  
        int remotePort = 8050;//远程主机端口  
        IPEndPoint IpSend;
        List<string> list = new List<string>();
        List<RoadPoint> RoadList = new List<RoadPoint>();
        List<RoadPoint> RoadGps = new List<RoadPoint>();

        public VirtualSwitchBus vs;
        //转换坐标时使用
        int StartIndexForSend = 0;
        int StopIndexForSend = 0;
        Point2D XyFromLocalGps = new Point2D();
        Point2D XyFromMap = new Point2D();
        int SendPointCount = 0;
        List<SendMessage> ListSend;  //通过UDP发送给决策的数据

        //Gps接收数据
        int PointNum;
        Spmatrix.spmatrix sp = new Spmatrix.spmatrix();
        Spmatrix.tupletype tup;
        public int i = 0;
        public Form1()
        {
            InitializeComponent();
            vs = new VirtualSwitchBus(1);
            opentxt();
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //while (true)
            //{
          
             Convert_xy();
             i = i + 3;
            // if (i > 100) i = 0;
                //if (button1.Text == "开始发送")
                //{
                //    button1.Text = "暂停发送";
                //    opentxt();

                //    Convert_xy();
                //}
                //else
                //{
                //    button1.Text = "开始发送";
                //    SendClient.Close();
                //    //break;
                //}
            //}
        }
        public void opentxt()
        {
            StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "\\unlinetest.txt", Encoding.GetEncoding("GBK"));
            while (sr.Peek() != -1)
            {
                list.Add(sr.ReadLine());
            }

            foreach (string str in list)
            {
                int i = 0;
                string[] s = str.Split(new char[] { ',' });
                //double lat,lon,attribute;
                RoadPoint rp = new RoadPoint();
                rp.Lon = double.Parse(s[0]);
                rp.Lat = double.Parse(s[1]);
                rp.Mode = byte.Parse(s[2]);
                rp.Azimuth = double.Parse(s[3]);

                RoadList.Add(rp);  //将地图中的值加入到mp中
                RoadGps.Add(rp);
                i++;
            }
           

            IpSend = new IPEndPoint(remoteIP, remotePort);
            SendClient = new UdpClient();
        }
        public void Udp()
        {    //发点数
            //PointNum = StopIndexForSend - StartIndexForSend + 1;
            byte u = 0;
            sp = new Spmatrix.spmatrix();
            PointNum = ListSend.Count;
            sp.td = PointNum;
            sp.data = new Spmatrix.tupletype[PointNum];
            int m = 0;
            for (m = 0; m < ListSend.Count; m++)
            {

                tup.y = ListSend[m].Pointx;
                tup.x = ListSend[m].Pointy;
                tup.value = ListSend[m].Mode;
                tup.U = u;
                sp.data[m] = tup;
                u++;
            }
           // CountUdp = ListSend.Count;
            vs.PubMsg("Path_GPS",VirtualSwitchBus.GetBytes((object)sp));
            //byte[] mysendmessage = new byte[PointNum * 6 + 4];
            //mysendmessage = SerializeTool.Serialize(sp, 4, 6);
            //SendClient.Send(mysendmessage, PointNum * 6 + 4, IpSend);

        }
        //旋转地图，寻找合适的点进行发送
        public void Convert_xy()
        {
    //        for(int i=0;i<RoadGps.Count;i=i+5)
    //{
            double Pi = 3.1415926;
            ListSend = new List<SendMessage>();
            double distance; //当前车所在的点和所采集的地图上的点的距离
            double angel;//两点组成的向量与真北的夹角
            double angel1;//当前车的位置与地图中的点与车头的夹角
            //double angle2; //夹角减航向角的值
            double x, y;  //相对于车的坐标 以车为原点，单位是米
            short pointx, pointy;  //栅格图中的坐标
            int num = 0;
            bool flag = true; //确定起始索引的标志位

            XyFromLocalGps = CoordinateConverter.BLH2XYZ(RoadGps[i].Lat, RoadGps[i].Lon, 0);//将车当前位置的经纬度坐标转换为xy坐标

            #region 转换坐标
            for (int j = SendPointCount; j < RoadList.Count; j++)
            {
                XyFromMap = CoordinateConverter.BLH2XYZ(RoadList[j].Lat, RoadList[j].Lon, 0);//将事先采集好的地图中的经纬度坐标转换为xy坐标
                distance = CoordinateConverter.getDist2(XyFromLocalGps.x, XyFromLocalGps.y, XyFromMap.x, XyFromMap.y);//根据转换后的坐标公式求距离
                angel = CoordinateConverter.getAngle(RoadGps[i].Lon, RoadGps[i].Lat,
                   RoadList[j].Lon, RoadList[j].Lat); //根据经纬度求两点组成的向量与真北的夹角，0-360度
                //angel = CoordinateConverter.getAngle(rp.Lat, rp.Lon,
                //   mp.map1.RoadPointList[j].Lat, mp.map1.RoadPointList[j].Lon); //根据经纬度求两点组成的向量与真北的夹角，0-360度
                angel1 = angel - RoadGps[i].Azimuth;  //角度之差

                #region 满足条件的点
                if ((angel1 > -360 && angel1 <= -270) || (angel1 > 270 && angel1 <= 360) || (angel1 >= -90 && angel1 <= 0) || (angel1 >= 0 && angel1 <= 90))
                {
                    if (angel1 > -360 && angel1 <= -270) angel1 = angel1 + 360;
                    if (angel1 > 270 && angel1 <= 360) angel1 = angel1 - 360;
                    y = distance * Math.Cos(angel1 * Pi / 180);
                    if (angel1 < 0)
                        x = -distance * Math.Abs(Math.Sin(angel1 * Pi / 180));
                    else
                        x = distance * Math.Abs(Math.Sin(angel1 * Pi / 180));
                    if (x >= -10 && x <= 10 && y >= -20 && y <= 70)
                    {
                        SendPointCount = j;
                        if (flag)
                        {
                            StartIndexForSend = SendPointCount; //找到的第一个满足条件的点
                            flag = false;
                        }
                        pointx = (short)(50 + (int)(x / 0.2));
                        pointy = (short)(350 - (int)(y / 0.2));
                        SendMessage sendmessage = new SendMessage(1, 2, 0);
                        sendmessage.Pointx = pointx;
                        sendmessage.Pointy = pointy;
                        sendmessage.Mode = RoadList[j].Mode;

                        ListSend.Add(sendmessage);

                    }
                    else
                    {
                        if (flag == false && num >= 3)
                        {
                            //StopIndexForSend = count;
                            break;
                        }
                        num++;
                    }
                }
                #endregion
                else
                {
                    if (flag == false && num >= 3)
                    {
                        //StopIndexForSend = count;
                        break;
                    }
                    num++;
                }//说明曾经满足过条件，现在出现3次不满足条件了，跳出循环
                // if ((StartIndexForSend + ListSend.Count - 1) != j) break;  //作为跳出for循环的条件
            }
            #endregion

            //一次找点结束的位置
            if (ListSend.Count > 0)
            {
                Udp();
            }
            ListSend.Clear();  //清除上一次发送的内容
            SendPointCount = StartIndexForSend;  //为了下一次循环做准备
            StartIndexForSend = 0;
            StopIndexForSend = 0;
           // flag = true;

        
}

        private void timer1_Tick(object sender, EventArgs e)
        {
            //opentxt();
           // for (i = 0; i < RoadGps.Count;i++ )
                Convert_xy();
            i = i + 3;
            if (i > RoadGps.Count) i = 0;
            LogHelper.WriteLog(typeof(Form1), "    Timer1:" + Environment.TickCount.ToString());
        }
    }
}
