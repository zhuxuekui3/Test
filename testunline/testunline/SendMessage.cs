using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLog4net
{
    class SendMessage
    {
        
            //public int Pointx;
            //public int  Pointy;
            //public int  Mode;
            //public SendMessage()
            //{
            //    Pointx = 0;
            //    Pointy = 0;
            //    Mode = 0;
            //}
            //public SendMessage(int  p,int  q,int m)
            //{
            //    Pointx = p;
            //    Pointy = q;
            //    Mode = m;
            //}
        public short Pointx;
        public short Pointy;
        public byte Mode;
        //public SendMessage()
        //{
        //    Pointx = 0;
        //    Pointy = 0;
        //    Mode = 0;
        //}
        public SendMessage(short p, short q, byte m)
        {
            Pointx = p;
            Pointy = q;
            Mode = m;
        }
    }
}
